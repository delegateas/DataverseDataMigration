using DataverseDataMigration.Emun;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;

namespace Helpers
{
    public class MyXrm : Xrm
    {
        public class MyXrmOptions
        {
            public bool? DryRun { get; set; }
            public LogLevel? LogLevel { get; set; }
            public bool? BypassBusinessLogicExecution { get; set; }
        }

        public Dictionary<string, int> summaryCreate = new Dictionary<string, int>();
        public Dictionary<string, int> summaryUpdate = new Dictionary<string, int>();
        public readonly bool dryRun;
        private readonly LogLevel logLevel;
        private readonly bool bypassBusinessLogicExecution;
        private IOrganizationService service;
        public MyXrm(IOrganizationService service, MyXrmOptions options = null) : base(service)
        {
            this.service = service;
            dryRun = options?.DryRun ?? true;
            logLevel = options?.LogLevel ?? LogLevel.Summary;
            bypassBusinessLogicExecution = options?.BypassBusinessLogicExecution ?? true;

            Console.WriteLine($"XRM initialized. DryRun: {dryRun}, LogLevel: {logLevel}, BypassBusinessLogicExecution: {bypassBusinessLogicExecution}");
            if (dryRun)
            {
                Console.WriteLine("DryRun is enabled, no changes will be made to the target system.");
            }
            else
            {
                Console.WriteLine("DryRun is DISABLED, changes WILL BE MADE to the target system.");
            }
        }

        public void Create(Entity entity)
        {
            if (!dryRun)
            {
                var createRequest = new CreateRequest()
                {
                    Parameters = new ParameterCollection(),
                };
                if (bypassBusinessLogicExecution) { 
                    createRequest.Parameters.Add("BypassBusinessLogicExecution", "CustomSync,CustomAsync");
                }
                createRequest.Target = entity;
                service.Execute(createRequest);
            }
            var type = entity.GetType().Name;
            if (!summaryCreate.ContainsKey(type))
            {
                summaryCreate[type] = 0;
            }
            summaryCreate[type]++;
        }

        public void Update(Entity entity)
        {
            if (!dryRun)
            {
                var updateRequest = new UpdateRequest()
                {
                    Parameters = new ParameterCollection()
                };
                if(bypassBusinessLogicExecution) { 
                    updateRequest.Parameters.Add("BypassBusinessLogicExecution", "CustomSync,CustomAsync");
                }
                updateRequest.Target = entity;
                service.Execute(updateRequest);
            }

            var type = entity.LogicalName;
            if (!summaryUpdate.ContainsKey(type))
            {
                summaryUpdate[type] = 0;
            }
            summaryUpdate[type]++;

        }
        public void WriteLog(string message, LogLevel logLevel)
        {
            if (logLevel >= this.logLevel)
            {
                Console.WriteLine(message);
            }
        }
    }
}
