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
    public class OrchestratorAccounts
    {
        public readonly string accountsToExportFileName = "ExportedData/AccountsToExport.json";
        public readonly string accountsExportedFileName = "ExportedData/AccountsExported.json";
        public readonly string accountsImportedFileName = "ExportedData/AccountsImported.json";
        public bool ExportData(IOrganizationService orgService)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            using (var context = new MyXrm(orgService))
            {
                var accountsToProcess = JsonSerializer.Deserialize<List<string>>(ReadDataFromDisk.Read(accountsToExportFileName));
                var accountsProcessed = new List<string>();
                try
                {
                    accountsProcessed = JsonSerializer.Deserialize<List<string>>(ReadDataFromDisk.Read(accountsExportedFileName));
                }
                catch (FileNotFoundException)
                {
                    // Ignore if the file does not exist, we will create it later.
                }
                var preFilterCount = accountsToProcess.Count;
                accountsToProcess = accountsToProcess.Where(a => !accountsProcessed.Contains(a)).ToList();


                context.WriteLog($"Filtered {preFilterCount - accountsToProcess.Count} accounts that have already been processed skipping those.", LogLevel.Summary);

                var accountFetcher = new AccountFetcher(context);
                context.WriteLog($"Found {accountsToProcess.Count} accounts to process.", LogLevel.Summary);
                var count = 0;
                foreach (var accountNumber in accountsToProcess)
                {
                    var account = context.AccountSet
                    .Where(a => a.AccountNumber == accountNumber)
                    .FirstOrDefault();

                    if (account == null)
                    {
                        context.WriteLog($"Account with AccountNumber {accountNumber} not found, skipping.",LogLevel.Detailed);
                        continue;
                    }
                    if (accountsProcessed.Contains(account.Id.ToString()))
                    {
                        count++;
                        context.WriteLog("Skipping account since it's allready processed", LogLevel.Detailed);
                        continue;
                    }
                    var accountTimer = System.Diagnostics.Stopwatch.StartNew();
                    context.WriteLog($"Starting processing of account: {account.Name} ({account.Id})", LogLevel.Verbose);
                    var accountRelation = accountFetcher.FetchRelated(account);

                    // Serialize the account relation to JSON
                    var json = Serializor.SerializeEntityToJson(accountRelation);

                    SaveToDisk.Save(json, $"Account_{accountNumber}.json");
                    context.WriteLog($"Finished processing of account: {account.Name} ({account.Id})", LogLevel.Verbose);
                    count++;
                    accountTimer.Stop();
                    context.WriteLog($"Processed {count} of {accountsToProcess.Count} accounts in {accountTimer.Elapsed.TotalSeconds} seconds.", LogLevel.Detailed);
                    context.WriteLog($"Current total time spend: {timer.Elapsed.TotalSeconds} seconds.", LogLevel.Detailed);
                    accountsProcessed.Add(account.AccountNumber.ToString());
                    SaveToDisk.Save(JsonSerializer.Serialize(accountsProcessed), accountsExportedFileName);
                }
                timer.Stop();
                context.WriteLog($"Export completed in {timer.Elapsed.TotalSeconds} seconds.", LogLevel.Summary);
            }
            return true;
        }

        public bool ImportData(IOrganizationService orgService)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            using (var context = new MyXrm(orgService))
            {
                var progress = new List<string>();
                try
                {
                    progress = JsonSerializer.Deserialize<List<string>>(ReadDataFromDisk.Read(accountsImportedFileName));
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
                //7807560
                //ExportedData/Account_7807560.json
                foreach (var file in filesToProcess) {
                    var fileTimer = System.Diagnostics.Stopwatch.StartNew();
                    context.WriteLog($"Starting processing of file: {file}", LogLevel.Verbose);
                    var json = ReadDataFromDisk.Read(file);
                    var entity = Serializor.DeserializeJsonToEntity(json);
                    var creator = new AccountCreator(context);
                    creator.Create(entity);
                    context.WriteLog($"Finished processing of file: {file}", LogLevel.Verbose);
                    count++;
                    if (!context.dryRun) { 
                        progress.Add(file);
                        SaveToDisk.Save(JsonSerializer.Serialize(progress), accountsImportedFileName);
                    }
                    fileTimer.Stop();
                    context.WriteLog($"Processed {count} of {filesToProcess.Count} files in {fileTimer.Elapsed.TotalSeconds} seconds.", LogLevel.Detailed);
                    context.WriteLog($"Current total time spend: {timer.Elapsed.TotalSeconds} seconds.", LogLevel.Detailed);
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

        public void GenerateDeltaListOfAccounts(IOrganizationService prodBackup, IOrganizationService prodClone)
        {
            var accountsInProdBackup = new MyXrm(prodBackup).AccountSet.ToList();
            var accountsInProdClone = new MyXrm(prodClone).AccountSet.ToList();
            var accountsToCreate = new List<string>();
            foreach (var account in accountsInProdBackup)
            {
                if (!accountsInProdClone.Any(a => a.Id == account.Id))
                {
                    accountsToCreate.Add(account.AccountNumber);
                }
            }

            foreach( var account in accountsInProdClone)
            {
                if (account.CreatedOn > DateTime.Parse("2025-06-22"))
                {
                    accountsToCreate.Add(account.AccountNumber);
                }
            }

            Console.WriteLine($"Found {accountsToCreate.Count} accounts to create in prod clone.");
            SaveToDisk.Save(JsonSerializer.Serialize(accountsToCreate), accountsToExportFileName);
        }
    }
}
