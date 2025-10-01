using DataRestoration.Mapper;
using DataverseDataMigration.Emun;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;
using Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Export.Fetcher
{
    public class EmailFetcher : BaseFetcher
    {
        private MyXrm context = null; // Placeholder for the Xrm context, should be initialized properly in actual implementation
        public EmailFetcher(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public DTO FetchRelated(Entity entity)
        {
            context.WriteLog($"Fetching related entities for Email: {entity.Id}",LogLevel.Detailed);
            var email = (Email)entity;

            var children = new List<DTO>();

            var noteFetcher = new AnnotationFetcher(context);
            var queueItemFetcher = new QueueItemFetcher(context);
            /*// Fetch related notes //TODO Annotations have been disabed since they were to be removed anyways, no need to recreate them
            var relatedNotes = context.AnnotationSet
                .Where(a => a.Email_Annotation.Id == email.Id)
                .ToList();
            foreach (var note in relatedNotes)
            {
                children.Add(noteFetcher.FetchRelated(note));
            }*/
            // Fetch related queue items
            var relatedQueueItems = context.QueueItemSet
                .Where(qi => qi.Email_QueueItem.Id == email.Id)
                .ToList();
            foreach (var queueItem in relatedQueueItems)
            {
                children.Add(queueItemFetcher.FetchRelated(queueItem));
            }

            //var attachments = context. TODO handle attachments
            // TODO figure out if connections are needed to be fetched too

            return EmailMapper.MapToDTO(email, children, context);
        }
    }
}
