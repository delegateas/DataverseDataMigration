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
    public class IncidentFetcher : BaseFetcher
    {
        private MyXrm context = null; // Placeholder for the Xrm context, should be initialized properly in actual implementation

        public IncidentFetcher(MyXrm context) {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public DTO FetchRelated(Entity entity)
        {
            context.WriteLog($"Fetching related entities for Case: {entity.Id}",LogLevel.Detailed);
            var incident = (Incident)entity;

            var relatedEmails = context.EmailSet
                .Where(e => e.Incident_Emails.Id == incident.Id)
                .ToList();
            var relatedPhoneCalls = context.PhoneCallSet
                .Where(pc => pc.Incident_Phonecalls.Id == incident.Id)
                .ToList();
            /*var relatedNotes = context.AnnotationSet  //TODO Annotations have been disabed since they were to be removed anyways, no need to recreate them
                .Where(a => a.Incident_Annotation.Id == incident.Id)
                .ToList();*/
            var relatedffMedlemsnoter = context.ff_medlemsnoteSet
                .Where(fn => fn.incident_ff_medlemsnotes.Id == incident.CustomerId.Id)
                .ToList();
            var relatedCaseNotes = context.ff_casenoteSet
                .Where(cn => cn.incident_ff_casenotes.Id == incident.Id)
                .ToList();
            var relatedResolutions = context.IncidentResolutionSet
                .Where(ir => ir.IncidentId.Id == incident.Id).ToList();

            var emailFetcher = new EmailFetcher(context);
            var phoneCallFetcher = new PhoneCallFetcher(context);
            var noteFetcher = new AnnotationFetcher(context);

            var children = new List<DTO>();
            foreach (var email in relatedEmails)
            {
                children.Add(emailFetcher.FetchRelated(email));
            }
            foreach (var phoneCall in relatedPhoneCalls)
            {
                children.Add(phoneCallFetcher.FetchRelated(phoneCall));
            }

            var relations = new Dictionary<string, List<string>>();
            relations["Sagsparter"] = context.ff_incidentpartySet
                .Where(s => s.ff_incident.Id == incident.Id)
                .Select(s => s.Id.ToString())
                .ToList();
            relations["EmailMessages"] = context.EmailSet
                .Where(e => e.ff_closedcase.Id == incident.Id)
                .Select(e => e.Id.ToString())
                .ToList();
            /**
             * Activities Incident_ActivityPointers
             * Sagsparter ff_incidentparty_incident_incident
             * Email Messages ff_email_closedcase_incident 
             * 
             */

            return IncidentMapper.MapToDTO(incident, children, context, relations);
        }
    }
}
