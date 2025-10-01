using System;
using System.Linq;
using Export.Mappers;
using Microsoft.Xrm.Sdk;
using Helpers;
using static DataRestoration.Mapper.QueueItemMapper;
using ConsoleJobs.DataRestoration.Emun;
using DataverseDataMigration.Emun;

namespace Import.Creator
{
    public class QueueItemCreator : BaseCreator
    {
        private readonly MyXrm context = null;
        public QueueItemCreator(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public override void Create(DTO entity, EntityType ParentType = EntityType.None, string ParentId = "")
        {
            if (entity.Type != EntityType.QueueItem)
            {
                throw new Exception("Expected type of QueueItem");
            }
            var queueItem = MapFromDTO((QueueItemDTO)entity);
            // Validate if queue item has already been created. If it has, skip and validate children
            var existingQueueItem = context.QueueItemSet.FirstOrDefault(q => q.Id == queueItem.Id);
            if (existingQueueItem == null)
            {
                var relatedTable = "";
                switch (ParentType)
                {
                    case EntityType.CaseNote:
                        relatedTable = "ff_casenote";
                        break;
                    case EntityType.Email:
                        relatedTable = "email";
                        break;
                    default:
                        throw new NotImplementedException($"Parent type {ParentType} is not implemented for QueueItem creation.");
                }
                queueItem.ObjectId = new EntityReference(relatedTable, Guid.Parse(ParentId));
                context.Create(queueItem);
                context.WriteLog($"QueueItem created with ID: {queueItem.Id}", LogLevel.Verbose);
            }
            else
            {
                context.WriteLog($"QueueItem with ID: {queueItem.Id} already exists, skipping creation.", LogLevel.Verbose);
            }
            // Validate children
            HandleChildren(entity, context);
        }
    }
}
