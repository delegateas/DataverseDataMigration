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
    public class LeadFetcher : BaseFetcher
    {
        private MyXrm context = null; // Placeholder for the Xrm context, should be initialized properly in actual implementation
        public LeadFetcher(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public DTO FetchRelated(Entity entity)
        {
            context.WriteLog($"Fetching related entities for Lead: {entity.Id}",LogLevel.Detailed);
            var lead = (Lead)entity;

            var children = new List<DTO>();

            var emailFetcher = new EmailFetcher(context);
            var phoneCallFetcher = new PhoneCallFetcher(context);
            var noteFetcher = new AnnotationFetcher(context);
            var annotationFetcher = new AnnotationFetcher(context);
            // Fetch related emails
            var relatedEmails = context.EmailSet
                .Where(e => e.Lead_Emails.Id == lead.Id)
                .ToList();
            foreach (var email in relatedEmails)
            {
                children.Add(emailFetcher.FetchRelated(email));
            }
            // Fetch related phone calls
            var relatedPhoneCalls = context.PhoneCallSet
                .Where(pc => pc.Lead_Phonecalls.Id == lead.Id)
                .ToList();
            foreach (var phoneCall in relatedPhoneCalls)
            {
                children.Add(phoneCallFetcher.FetchRelated(phoneCall));
            }
            /*// Fetch related notes  //TODO Annotations have been disabed since they were to be removed anyways, no need to recreate them
            var relatedNotes = context.AnnotationSet
                .Where(a => a.Lead_Annotation.Id == lead.Id)
                .ToList();
            foreach (var note in relatedNotes)
            {
                children.Add(noteFetcher.FetchRelated(note));
            }*/

            var relations = new Dictionary<string, List<string>>();
            relations["Contacts"] = context.ContactSet
                .Where(c => c.OriginatingLeadId.Id == lead.Id)
                .Select(c => c.Id.ToString())
                .ToList();
            relations["Opportunities"] = context.OpportunitySet
                .Where(o => o.OriginatingLeadId.Id == lead.Id)
                .Select(o => o.Id.ToString())
                .ToList();
            /**
             * Contacts (invisible) contact_originating_lead
             * Opportunities (invisible) opportunity_originating_lead
             * Activities Lead_ActivityPointers
             * 
             * 
             */
            return LeadMapper.MapToDTO(lead, children, context, relations);
        }
    }
}
