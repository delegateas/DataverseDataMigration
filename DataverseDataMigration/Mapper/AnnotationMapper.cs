using DG.XrmFramework.BusinessDomain.ServiceContext;
using ConsoleJobs.DataRestoration.Emun;
using Export.Mappers;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace DataRestoration.Mapper
{
    // OBS! No need to recreate these as they are being deleted in another fasttrack task!
    public static class AnnotationMapper
    {
        /** TODO
         * //Default
         * ..TODO
         * //Core
         * NoteText
         * Subject
         * //Hverve
         * -
         * //Jura
         * NoteText
         * OwnerId
         * ObjectId
         **/
        public static AnnotationDTO MapToDTO(Annotation entity)
        {
            return new AnnotationDTO
            {
                Id = entity.Id.ToString(),
                OwnerId = entity.OwnerId?.Id.ToString(),
                Lookuplogicalname = entity.OwnerId?.LogicalName,
                CreatedOn = entity.CreatedOn,
                CreatedBy = entity.CreatedBy?.Id.ToString(),
                ModifiedOn = entity.ModifiedOn,
                ModifiedBy = entity.ModifiedBy?.Id.ToString(),
                //Relations
                Type = EntityType.Annotation,
                Children = new List<DTO>(),
                // Map other properties as needed
            };
        }

        public static Annotation MapFromDTO(AnnotationDTO dot)
        {
            return new Annotation
            {
                Id = Guid.Parse(dot.Id),
                OverriddenCreatedOn = dot.CreatedOn,
                OwnerId = new EntityReference(dot.Lookuplogicalname, Guid.Parse(dot.OwnerId)),
                //Modified on, modified by and created by cant be set
                // Map other properties as needed
            };
        }

        public class AnnotationDTO : DTO
        {
            public string Subject { get; set; }
            public string NoteText { get; set; }
        }
    }
}
