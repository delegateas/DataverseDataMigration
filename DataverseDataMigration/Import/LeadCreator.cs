using System;
using System.Collections.Generic;
using System.Linq;
using Export.Mappers;
using Microsoft.Xrm.Sdk;
using Helpers;
using static DataRestoration.Mapper.LeadMapper;
using ConsoleJobs.DataRestoration.Emun;
using DataverseDataMigration.Emun;

namespace Import.Creator
{
    public class LeadCreator : BaseCreator
    {
        private MyXrm context = null;
        public LeadCreator(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public override void Create(DTO entity, EntityType ParentType = EntityType.None, string ParentId = "")
        {
            if (entity.Type != EntityType.Lead)
            {
                throw new Exception("Expected type of Lead");
            }
            var lead = MapFromDTO((LeadDTO)entity, context);
            // Validate if lead has already been created. If it has, skip and validate children
            var existingLead = context.LeadSet.FirstOrDefault(l => l.Id == lead.Id);
            if (existingLead == null)
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
                        throw new NotImplementedException($"Parent type {ParentType} is not implemented for Lead creation.");
                }

                lead.ParentContactId = new EntityReference(relatedTable, Guid.Parse(ParentId));
                context.Create(lead);
                context.WriteLog($"Lead created with ID: {lead.Id}", LogLevel.Verbose);
            }
            else
            {
                context.WriteLog($"Lead with ID: {lead.Id} already exists, skipping creation.", LogLevel.Verbose);
            }

            HandleBrokenLinks(lead.Id, entity.Relations, context);
            // Validate children
            HandleChildren(entity, context);
        }

        private void HandleBrokenLinks(Guid leadId, Dictionary<string, List<string>> relations, MyXrm context)
        {
            /**
             * relations["Contacts"] = context.ContactSet
                .Where(c => c.OriginatingLeadId.Id == lead.Id)
                .Select(c => c.Id.ToString())
                .ToList();
            relations["Opportunities"] = context.OpportunitySet
                .Where(o => o.OriginatingLeadId.Id == lead.Id)
                .Select(o => o.Id.ToString())
                .ToList();
            relations["Activities"] = context.ActivityPointerSet
                .Where(a => a.RegardingObjectId.Id == lead.Id)
                .Select(a => a.Id.ToString())
                .ToList();*/
            if (relations.ContainsKey("Contacts"))
            {
                foreach (var contactId in relations["Contacts"])
                {
                    var contact = context.ContactSet.FirstOrDefault(c => c.Id == Guid.Parse(contactId));
                    if (contact != null)
                    {
                        context.Update(new Entity("contact", contact.Id)
                        {
                            ["originatingleadid"] = new EntityReference("lead", leadId)
                        });
                        context.WriteLog($"Updated Contact with ID: {contact.Id} to link with Lead ID: {leadId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("Opportunities"))
            {
                foreach (var opportunityId in relations["Opportunities"])
                {
                    var opportunity = context.OpportunitySet.FirstOrDefault(o => o.Id == Guid.Parse(opportunityId));
                    if (opportunity != null)
                    {
                        context.Update(new Entity("opportunity", opportunity.Id)
                        {
                            ["originatingleadid"] = new EntityReference("lead", leadId)
                        });
                        context.WriteLog($"Updated Opportunity with ID: {opportunity.Id} to link with Lead ID: {leadId}", LogLevel.Verbose);
                    }
                }
            }
        }
    }
}
