using DataRestoration.Mapper;
using DataverseDataMigration.Emun;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;
using Helpers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace Export.Fetcher
{
    public class PhoneCallFetcher : BaseFetcher
    {
        private MyXrm context = null; // Placeholder for the Xrm context, should be initialized properly in actual implementation
        public PhoneCallFetcher(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public DTO FetchRelated(Entity entity)
        {
            context.WriteLog($"Fetching related entities for PhoneCall: {entity.Id}",LogLevel.Detailed);
            var phoneCall = (PhoneCall)entity;

            var children = new List<DTO>();
            /*// Fetch related Annotations
            var noteFetcher = new AnnotationFetcher(context);
            var relatedNotes = context.AnnotationSet  //TODO Annotations have been disabed since they were to be removed anyways, no need to recreate them
                .Where(a => a.PhoneCall_Annotation.Id == phoneCall.Id)
                .ToList();
            foreach (var note in relatedNotes)
            {
                children.Add(noteFetcher.FetchRelated(note));
            }*/

            return PhoneCallMapper.MapToDTO(phoneCall, children, context);
        }
    }
}
