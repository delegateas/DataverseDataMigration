using System;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;
using DataRestoration.Mapper;
using Microsoft.Xrm.Sdk;

namespace Export.Fetcher
{
    public class AnnotationFetcher : BaseFetcher
    {
        private Xrm context = null; // Placeholder for the Xrm context, should be initialized properly in actual implementation
        public AnnotationFetcher(Xrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public DTO FetchRelated(Entity entity)
        {
            var annotation = (Annotation)entity;
            return AnnotationMapper.MapToDTO(annotation);
        }
    }
}
