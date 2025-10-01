using ConsoleJobs.DataRestoration.Emun;
using DataverseDataMigration.Emun;
using Export.Mappers;
using Helpers;
using System;
using System.Linq;
using static DataRestoration.Mapper.AnnotationMapper;

namespace Import.Creator
{
    public class AnnotationCreator : BaseCreator
    {
        private MyXrm context = null;
        public AnnotationCreator(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override void Create(DTO entity, EntityType ParentType = EntityType.None, string ParentId = "")
        {
            if (entity.Type != EntityType.Annotation)
            {
                throw new Exception("Expected type of Annotation");
            }
            var annotation = MapFromDTO((AnnotationDTO)entity);
            // Validate if annotation has already been created. If it has, skip and validate children
            var existingAnnotation = context.AnnotationSet.FirstOrDefault(a => a.Id == annotation.Id);
            if (existingAnnotation == null)
            {
                context.Create(annotation);
                context.WriteLog($"Annotation created with ID: {annotation.Id}", LogLevel.Verbose);
            }
            else
            {
                context.WriteLog($"Annotation with ID: {annotation.Id} already exists, skipping creation.", LogLevel.Verbose);
            }
            // Validate children
            HandleChildren(entity, context);
        }
    }
}
