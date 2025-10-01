using System;
using System.Collections.Generic;
using System.Linq;
using Export.Mappers;
using Microsoft.Xrm.Sdk;
using Helpers;
using static DataRestoration.Mapper.OpportunitiesMapper;
using ConsoleJobs.DataRestoration.Emun;
using DataverseDataMigration.Emun;

namespace Import.Creator
{
    public class OpportunityCreator : BaseCreator
    {
        private readonly MyXrm context = null;
        public OpportunityCreator(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public override void Create(DTO entity, EntityType ParentType = EntityType.None, string ParentId = "")
        {
            if (entity.Type != EntityType.Opportunity)
            {
                throw new Exception("Expected type of Opportunity");
            }
            var opportunity = MapFromDTO((OpportunityDTO)entity,context);
            // Validate if opportunity has already been created. If it has, skip and validate children
            var existingOpportunity = context.OpportunitySet.FirstOrDefault(o => o.Id == opportunity.Id);
            if (existingOpportunity == null)
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
                        throw new NotImplementedException($"Parent type {ParentType} is not implemented for Opportunity creation.");
                }
                opportunity.ParentContactId = new EntityReference(relatedTable, Guid.Parse(ParentId));
                context.Create(opportunity);
                context.WriteLog($"Opportunity created with ID: {opportunity.Id}", LogLevel.Verbose);
            }
            else
            {
                context.WriteLog($"Opportunity with ID: {opportunity.Id} already exists, skipping creation.", LogLevel.Verbose);
            }
            HandleBrokenLinks(opportunity.Id, entity.Relations, context);
            // Validate children
            HandleChildren(entity, context);
        }
        private void HandleBrokenLinks(Guid opportunityId, Dictionary<string, List<string>> relations, MyXrm context)
        {
            /**
             * 
             * relations["Leads"] = context.LeadSet
                .Where(l => l.QualifyingOpportunityId.Id == opportunity.Id)
                .Select(l => l.Id.ToString())
                .ToList();
            relations["Activities"] = context.ActivityPointerSet
                .Where(ap => ap.RegardingObjectId.Id == opportunity.Id)
                .Select(ap => ap.Id.ToString())
                .ToList();
            relations["ReportingOfOpportunities"] = context.ff_reportingofopportunitySet
                .Where(r => r.ff_opportunity.Id == opportunity.Id)
                .Select(r => r.Id.ToString())
                .ToList();
            relations["AwardGiftcards"] = context.ff_awardgiftcardSet
                .Where(a => a.ff_opportunity.Id == opportunity.Id)
                .Select(a => a.Id.ToString())
                .ToList();
            relations["EmailMessages"] = context.EmailSet
                .Where(e => e.ff_closedopportunity.Id == opportunity.Id)
                .Select(e => e.Id.ToString())
                .ToList();*/
            if (relations.ContainsKey("Leads"))
            {
                foreach (var leadId in relations["Leads"])
                {
                    var lead = context.LeadSet.FirstOrDefault(l => l.Id == Guid.Parse(leadId));
                    if (lead != null)
                    {
                        context.Update(new Entity("lead", lead.Id)
                        {
                            ["qualifyingopportunityid"] = new EntityReference("opportunity", opportunityId)
                        });
                        context.WriteLog($"Updated Lead with ID: {lead.Id} to link with Opportunity ID: {opportunityId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("ReportingOfOpportunities"))
            {
                foreach (var reportingId in relations["ReportingOfOpportunities"])
                {
                    var reporting = context.ff_reportingofopportunitySet.FirstOrDefault(r => r.Id == Guid.Parse(reportingId));
                    if (reporting != null)
                    {
                        context.Update(new Entity("ff_reportingofopportunity", reporting.Id)
                        {
                            ["ff_opportunity"] = new EntityReference("opportunity", opportunityId)
                        });
                        context.WriteLog($"Updated ReportingOfOpportunity with ID: {reporting.Id} to link with Opportunity ID: {opportunityId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("AwardGiftcards"))
            {
                foreach (var giftcardId in relations["AwardGiftcards"])
                {
                    var giftcard = context.ff_awardgiftcardSet.FirstOrDefault(g => g.Id == Guid.Parse(giftcardId));
                    if (giftcard != null)
                    {
                        context.Update(new Entity("ff_awardgiftcard", giftcard.Id)
                        {
                            ["ff_opportunity"] = new EntityReference("opportunity", opportunityId)
                        });
                        context.WriteLog($"Updated AwardGiftcard with ID: {giftcard.Id} to link with Opportunity ID: {opportunityId}", LogLevel.Verbose);
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
                            ["ff_closedopportunity"] = new EntityReference("opportunity", opportunityId)
                        });
                        context.WriteLog($"Updated EmailMessage with ID: {email.Id} to link with Opportunity ID: {opportunityId}", LogLevel.Verbose);
                    }
                }
            }
        }
    }
}
