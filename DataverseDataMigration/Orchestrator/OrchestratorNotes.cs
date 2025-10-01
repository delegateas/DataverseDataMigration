using System.Linq;
using Microsoft.Xrm.Sdk;
using Export.Fetcher;
using Export.Helpers;
using System;
using System.Text.Json;
using Import.Creator;
using System.Collections.Generic;
using System.IO;
using Helpers;
using DataverseDataMigration.Emun;

namespace DataRestoration.Orchestrator
{
    public class OrchestratorNotes
    {
        public readonly string notesToExportFileName = "ExportedData/NotesToExport.json";
        public readonly string notesExportedFileName = "ExportedData/NotesExported.json";
        public readonly string notesImportedFileName = "ExportedData/NotesImported.json";
        public bool ExportData(IOrganizationService orgService)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            using (var context = new MyXrm(orgService))
            {
                var notesToProcess = JsonSerializer.Deserialize<List<string>>(ReadDataFromDisk.Read(notesToExportFileName));
                var notesProcessed = new List<string>();
                try
                {
                    notesProcessed = JsonSerializer.Deserialize<List<string>>(ReadDataFromDisk.Read(notesExportedFileName));
                }
                catch (FileNotFoundException)
                {
                    // Ignore if the file does not exist, we will create it later.
                }
                var preFilterCount = notesToProcess.Count;
                notesToProcess = notesToProcess.Where(a => !notesProcessed.Contains(a)).ToList();


                context.WriteLog($"Filtered {preFilterCount - notesToProcess.Count} notes that have already been processed skipping those.",LogLevel.Summary);

                var annotationFetcher = new AnnotationFetcher(context);
                context.WriteLog($"Found {notesToProcess.Count} notes to process.", LogLevel.Summary);
                var count = 0;
                foreach (var noteId in notesToProcess)
                {
                    var note = context.AnnotationSet.FirstOrDefault(n => n.Id == Guid.Parse(noteId));

                    if (note == null)
                    {
                        context.WriteLog($"Note with id {noteId} not found, skipping.", LogLevel.Detailed);
                        continue;
                    }
                    if (notesProcessed.Contains(note.Id.ToString()))
                    {
                        count++;
                        context.WriteLog("Skipping note since it's allready processed", LogLevel.Detailed);
                        continue;
                    }
                    var noteTimer = System.Diagnostics.Stopwatch.StartNew();
                    //context.WriteLog($"Starting processing of note: {note.Id}");
                    var noteRelation = annotationFetcher.FetchRelated(note);

                    // Serialize the note relation to JSON
                    var json = Serializor.SerializeEntityToJson(noteRelation);

                    SaveToDisk.Save(json, $"Note_{noteId}.json");
                    //context.WriteLog($"Finished processing of note: {note.Id}");
                    count++;
                    noteTimer.Stop();
                    if (count % 250 == 0)
                    {
                        context.WriteLog("", LogLevel.Summary);
                        context.WriteLog($"Processed {count} of {notesToProcess.Count} notes in {noteTimer.Elapsed.TotalSeconds} seconds.", LogLevel.Summary);
                        context.WriteLog($"Current total time spend: {timer.Elapsed.TotalSeconds} seconds.", LogLevel.Summary);
                        context.WriteLog($"Estimated time to completion: {(timer.Elapsed.TotalSeconds / count) * (notesToProcess.Count - count)} seconds.", LogLevel.Summary);
                        context.WriteLog($"Estimated total time: {(timer.Elapsed.TotalSeconds / count) * (notesToProcess.Count)} seconds.", LogLevel.Summary);
                    }
                    notesProcessed.Add(note.Id.ToString());
                    SaveToDisk.Save(JsonSerializer.Serialize(notesProcessed), notesExportedFileName);
                }
                timer.Stop();
                context.WriteLog($"Export completed in {timer.Elapsed.TotalSeconds} seconds.", LogLevel.Summary);
            }
            return true;
        }

        public bool ImportData(IOrganizationService orgService)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            using (var context = new MyXrm(orgService, new MyXrm.MyXrmOptions { DryRun = true, BypassBusinessLogicExecution = true, LogLevel = LogLevel.Verbose }))
            {
                var progress = new List<string>();
                try
                {
                    progress = JsonSerializer.Deserialize<List<string>>(ReadDataFromDisk.Read(notesImportedFileName));
                }
                catch (FileNotFoundException)
                {
                    // Ignore if the file does not exist, we will create it later.
                }
                var filesToProcess = ReadDataFromDisk.ListFiles();
                var countPreFilter = filesToProcess.Count;
                filesToProcess = filesToProcess.Where(f => f.EndsWith(".json") && !progress.Contains(f)).ToList();

                context.WriteLog($"Filtered {countPreFilter - filesToProcess.Count} files that have already been processed, skipping those.", LogLevel.Summary);
                context.WriteLog($"Found {filesToProcess.Count} files to process.", LogLevel.Summary);
                var count = 0;
                foreach (var file in filesToProcess) {
                    var fileTimer = System.Diagnostics.Stopwatch.StartNew();
                    context.WriteLog($"Starting processing of file: {file}", LogLevel.Detailed);
                    var json = ReadDataFromDisk.Read(file);
                    var entity = Serializor.DeserializeJsonToEntity(json);
                    var creator = new AnnotationCreator(context);
                    creator.Create(entity);
                    context.WriteLog($"Finished processing of file: {file}", LogLevel.Detailed);
                    count++;
                    if (!context.dryRun) { 
                        progress.Add(file);
                        SaveToDisk.Save(JsonSerializer.Serialize(progress), notesImportedFileName);
                    }
                    fileTimer.Stop();
                    if (count % 10 == 0)
                    {
                        context.WriteLog($"Processed {count} of {filesToProcess.Count} files in {fileTimer.Elapsed.TotalSeconds} seconds.", LogLevel.Summary);
                        context.WriteLog($"Current total time spend: {timer.Elapsed.TotalSeconds} seconds.", LogLevel.Summary);
                        context.WriteLog($"Estimated time to completion: {(timer.Elapsed.TotalSeconds / count) * (filesToProcess.Count - count)} seconds." , LogLevel.Summary);
                    }
                }
                context.WriteLog("",LogLevel.Summary);
                context.WriteLog(context.dryRun ? "Summary of entities that would be created" : "Summary of created entities", LogLevel.Summary);
                foreach(var name in context.summaryCreate.Keys)
                {
                    context.WriteLog($"{name}: {context.summaryCreate[name]}", LogLevel.Summary);
                }
                context.WriteLog("", LogLevel.Summary);
                context.WriteLog(context.dryRun ? "Summary of entities that would be updated" : "Summary of updated entities", LogLevel.Summary);
                foreach (var name in context.summaryUpdate.Keys)
                {
                    context.WriteLog($"{name}: {context.summaryUpdate[name]}", LogLevel.Summary);
                }
                timer.Stop();
                context.WriteLog($"Import completed in {timer.Elapsed.TotalSeconds} seconds.", LogLevel.Summary);
            }
            return true;
        }

        public void GenerateNotesList(IOrganizationService context)
        {
            var batchSize = 100;
            var existingNotesOnList = new List<string>();
            try { 
                existingNotesOnList = JsonSerializer.Deserialize<List<string>>(ReadDataFromDisk.Read(notesToExportFileName));
            }catch(FileNotFoundException)
            {
                // Ignore if the file does not exist, just means it's the first run.
            }
            var xrmContext = new MyXrm(context);
            while (true)
            {
                var part = xrmContext.AnnotationSet.OrderBy(x => x.Id).Skip(existingNotesOnList.Count).Take(batchSize).ToList().Select(x => x.Id).ToList();
                if (part.Count == 0)
                {
                    break;
                }
                xrmContext.WriteLog($"Fetched {part.Count} notes from CRM, total notes fetched: {existingNotesOnList.Count + part.Count}",LogLevel.Summary);
                existingNotesOnList.AddRange(part.Select(x => x.ToString()));
            }

            xrmContext.WriteLog($"Found {existingNotesOnList.Count} notes to move.", LogLevel.Summary);
            SaveToDisk.Save(JsonSerializer.Serialize(existingNotesOnList), notesToExportFileName);
        }
    }
}
