using System;
using System.Collections.Generic;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Helpers;
using static DataRestoration.Mapper.IncidentMapper;
using DataRestoration.Mapper;
using ConsoleJobs.DataRestoration.Emun;
using DataverseDataMigration.Emun;

namespace Import.Creator
{
    public class IncidentCreator : BaseCreator
    {
        private MyXrm context = null;
        public IncidentCreator(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public override void Create(DTO entity, EntityType ParentType = EntityType.None, string ParentId = "")
        {
            if (entity.Type != EntityType.Incident)
            {
                throw new Exception("Expected type of Incident");
            }
            var incident = IncidentMapper.MapFromDTO((IncidentDTO)entity, context);
            // Validate if incident has already been created. If it has, skip and validate children
            var existingIncident = context.IncidentSet.FirstOrDefault(i => i.Id == incident.Id);
            if (existingIncident == null)
            {
                var relatedTable = "";
                switch (ParentType)
                {
                    case EntityType.Account:
                        relatedTable = "account";
                        break;
                    case EntityType.Contact:
                        relatedTable = "contact";
                        break;
                    default:
                        throw new NotImplementedException($"Parent type {ParentType} is not implemented for Incident creation.");
                }
                incident.CustomerId = new EntityReference(relatedTable, Guid.Parse(ParentId));
                context.Create(incident);
                context.WriteLog($"Incident created with ID: {incident.Id}", LogLevel.Verbose);
            }
            else
            {
                context.WriteLog($"Incident with ID: {incident.Id} already exists, skipping creation.", LogLevel.Verbose);
            }
            HandleStatus((IncidentDTO)entity, context);

            HandleBrokenLinks(incident.Id, entity.Relations, context);
            // Validate children
            HandleChildren(entity, context);
        }

        private void HandleStatus(IncidentDTO entity, MyXrm context)
        {
            if (context.dryRun) return;
            var incident = context.IncidentSet.Where(e => e.Id == Guid.Parse(entity.Id)).FirstOrDefault();
            if (entity.StateCode != null && incident.StateCode.ToString() != entity.StateCode || entity.StatusCode != null && incident.StatusCode.ToString() != entity.StatusCode)
            {
                if(entity.StateCode == IncidentState.Resolved.ToString())
                {
                    var closeRequest = new CloseIncidentRequest
                    {
                        Parameters = new ParameterCollection(),
                    };
                    closeRequest.Parameters.Add("BypassBusinessLogicExecution", "CustomSync,CustomAsync");
                    closeRequest.IncidentResolution = new IncidentResolution
                    {
                        StatusCode = IncidentResolution_StatusCode.Closed,
                        ResolutionTypeCode =IncidentResolution_ResolutionTypeCode.ProblemSolved,
                        IncidentId = incident.ToEntityReference(),
                        Subject = "",
                    };
                    closeRequest.Status = new OptionSetValue((int)Enum.Parse(typeof(Incident_StatusCode), entity.StatusCode));
                    if (!context.dryRun) { 
                        context.Execute(closeRequest);
                    }
                    else { 
                        context.WriteLog($"Dry run: Would close incident with ID: {incident.Id} with status {closeRequest.Status}", LogLevel.Verbose); 
                    }
                }

                if (entity.StateCode == IncidentState.Cancelled.ToString())
                {
                    var cancleRequest = new SetStateRequest();
                    cancleRequest.EntityMoniker = incident.ToEntityReference();
                    cancleRequest.State = new OptionSetValue((int)IncidentState.Cancelled);

                    cancleRequest.Status = new OptionSetValue((int)Enum.Parse(typeof(Incident_StatusCode), entity.StatusCode));
                    cancleRequest.Parameters.Add("BypassBusinessLogicExecution", "CustomSync,CustomAsync");
                    if (!context.dryRun)
                    {
                        context.Execute(cancleRequest);
                    }
                    else
                    {
                        context.WriteLog($"Dry run: Would set state to {cancleRequest.State} and status to {cancleRequest.Status} for Incident ID: {incident.Id}", LogLevel.Verbose);
                    }
                }

                if(!string.IsNullOrEmpty(entity.ff_substatus))
                {
                    var updateEntity = new Entity("incident", incident.Id);
                    updateEntity["ff_substatus"] = new OptionSetValue((int)Enum.Parse(typeof(ff_casesubstatus), entity.ff_substatus));
                    context.Update(updateEntity);
                }
            }
        }
        private void HandleBrokenLinks(Guid incidentId, Dictionary<string, List<string>> relations, MyXrm context)
        {
            /**
             * 
             * 
            relations["Acctivities"] = context.ActivityPointerSet
                .Where(a => a.RegardingObjectId.Id == incident.Id && a.RegardingObjectId.LogicalName == "incident")
                .Select(a => a.Id.ToString())
                .ToList();
            relations["Sagsparter"] = context.ff_incidentpartySet
                .Where(s => s.ff_incident.Id == incident.Id)
                .Select(s => s.Id.ToString())
                .ToList();
            relations["EmailMessages"] = context.EmailSet
                .Where(e => e.ff_closedcase.Id == incident.Id)
                .Select(e => e.Id.ToString())
                .ToList();
            */
            if (relations.ContainsKey("Sagsparter"))
            {
                foreach (var partyId in relations["Sagsparter"])
                {
                    var party = context.ff_incidentpartySet.FirstOrDefault(s => s.Id == Guid.Parse(partyId));
                    if (party != null)
                    {
                        context.Update(new Entity("ff_incidentparty", party.Id)
                        {
                            ["ff_incident"] = new EntityReference("incident", incidentId)
                        });
                        context.WriteLog($"Updated Incident Party with ID: {party.Id} to link with Incident ID: {incidentId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("EmailMessages"))
            {
                foreach (var emailId in relations["EmailMessages"])
                {
                    var email = context.EmailSet.FirstOrDefault(e => e.Id == Guid.Parse(emailId));
                    if (email != null)
                    {
                        context.Update(new Entity("email", email.Id) 
                        { 
                            ["ff_closedcase"] = new EntityReference("incident", incidentId)
                        });
                        context.WriteLog($"Updated Email with ID: {email.Id} to link with Incident ID: {incidentId}", LogLevel.Verbose);
                    }
                }
            }
        }
    }
}
