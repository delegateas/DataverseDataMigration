using System;
using DataRestoration.Orchestrator;

namespace ConsoleJobs
{
    class Program
    {
        static void Main(string[] args)
        {
            var success = true;
            var connector = Connector.GetCRMProdService();
            var orchestratorNotes = new OrchestratorNotes();

            orchestratorNotes.GenerateNotesList(connector);
            orchestratorNotes.ExportData(connector);
            //orchestratorNotes.ImportData(connector);
            Console.WriteLine(success ? "Success" : "Failed");
            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }
    }
}
