using DataRestoration.Mapper;
using DataverseDataMigration.Emun;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;
using Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Export.Fetcher
{
    public class OpportunityFetcher : BaseFetcher
    {
        private MyXrm context = null; // Placeholder for the Xrm context, should be initialized properly in actual implementation
        public OpportunityFetcher(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public DTO FetchRelated(Entity entity)
        {
            context.WriteLog($"Fetching related entities for Opportunity: {entity.Id}",LogLevel.Detailed);
            var opportunity = (Opportunity)entity;

            var children = new List<DTO>();

            var emailFetcher = new EmailFetcher(context);
            var phoneCallFetcher = new PhoneCallFetcher(context);
            var noteFetcher = new AnnotationFetcher(context);
            var relatedEmails = context.EmailSet
                .Where(e => e.Opportunity_Emails.Id == opportunity.Id)
                .ToList();
            foreach (var email in relatedEmails)
            {
                children.Add(emailFetcher.FetchRelated(email));
            }
            var relatedPhoneCalls = context.PhoneCallSet
                .Where(pc => pc.Opportunity_Phonecalls.Id == opportunity.Id)
                .ToList();
            foreach (var phoneCall in relatedPhoneCalls)
            {
                children.Add(phoneCallFetcher.FetchRelated(phoneCall));
            }
            /*var relatedNotes = context.AnnotationSet  //TODO Annotations have been disabed since they were to be removed anyways, no need to recreate them
                .Where(a => a.Opportunity_Annotation.Id == opportunity.Id)
                .ToList();
            foreach (var note in relatedNotes)
            {
                children.Add(noteFetcher.FetchRelated(note));
            }*/

            var relations = new Dictionary<string, List<string>>();
            relations["Leads"] = context.LeadSet
                .Where(l => l.QualifyingOpportunityId.Id == opportunity.Id)
                .Select(l => l.Id.ToString())
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
                .ToList();
            /**
             *  Leads (invisible) lead_qualifying_opportunity
             *  Activities Opportunity_ActivityPointers
             *  Reporting of Opportunities ff_reportingofopportunity_opportunity_opportunity
             *  Award Giftcards ff_awardgiftcard_opportunity_opportunity
             *  Email Messages ff_email_closedopportunity_opportunity
             *  
             */
            return OpportunitiesMapper.MapToDTO(opportunity, children, context, relations);
        }
    }
}
