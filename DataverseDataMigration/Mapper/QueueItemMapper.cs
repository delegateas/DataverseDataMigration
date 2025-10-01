using System;
using System.Collections.Generic;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using ConsoleJobs.DataRestoration.Emun;
using Export.Mappers;
using Microsoft.Xrm.Sdk;

namespace DataRestoration.Mapper
{
    public static class QueueItemMapper
    {
        public static QueueItemDTO MapToDTO(QueueItem entity)
        {
            return new QueueItemDTO
            {
                Id = entity.Id.ToString(),
                OwnerId = entity.OwnerId?.Id.ToString(),
                Lookuplogicalname = entity.OwnerId?.LogicalName,
                CreatedOn = entity.CreatedOn,
                CreatedBy = entity.CreatedBy?.Id.ToString(),
                ModifiedOn = entity.ModifiedOn,
                ModifiedBy = entity.ModifiedBy?.Id.ToString(),
                //Relations
                Type = EntityType.QueueItem,
                Children = new List<DTO>(),

                // Map other properties as needed
                EnteredOn = entity.EnteredOn,
                Sender = entity.Sender,
                ImportSequenceNumber = entity.ImportSequenceNumber,
                MsdynSkipursync = entity.msdyn_skipursync,
                ObjectId = entity.ObjectId?.Id.ToString(),
                OrganizationId = entity.OrganizationId?.Id.ToString(),
                Priority = entity.Priority,
                QueueId = entity.QueueId?.Id.ToString(),
                QueueItemId = entity.QueueItemId?.ToString(),
                TimeZoneRuleVersionNumber = entity.TimeZoneRuleVersionNumber,
                Title = entity.Title,
                ToRecipients = entity.ToRecipients,
                ObjectTypeCode = entity.ObjectTypeCode?.ToString(),
                UtcConversionTimeZoneCode = entity.UTCConversionTimeZoneCode,
                VersionNumber = entity.VersionNumber,
                WorkerId = entity.WorkerId?.Id.ToString(),
                WorkerLookuplogicalname = entity.WorkerId?.LogicalName ?? string.Empty,
                WorkerIdModifiedOn = entity.WorkerIdModifiedOn

            };
        }
        public static QueueItem MapFromDTO(QueueItemDTO dot)
        {
            return new QueueItem
            {
                Id = Guid.Parse(dot.Id),
                OverriddenCreatedOn = dot.CreatedOn,
                // Read only           OwnerId = new EntityReference(dot.Lookuplogicalname, Guid.Parse(dot.OwnerId)),
                //Modified on, modified by and created by cant be set
                // Map other properties as needed
                // readonly EnteredOn = dot.EnteredOn,
                Sender = dot.Sender,
                ImportSequenceNumber = dot.ImportSequenceNumber,
                msdyn_skipursync = dot.MsdynSkipursync,
                ObjectId = !string.IsNullOrEmpty(dot.ObjectId) ? new EntityReference("object", Guid.Parse(dot.ObjectId)) : null,
                // readonly OrganizationId = !string.IsNullOrEmpty(dot.OrganizationId) ? new EntityReference("organization", Guid.Parse(dot.OrganizationId)) : null,
                Priority = dot.Priority,
                QueueId = !string.IsNullOrEmpty(dot.QueueId) ? new EntityReference("queue", Guid.Parse(dot.QueueId)) : null,
                QueueItemId = Guid.Parse(dot.QueueItemId),
                TimeZoneRuleVersionNumber = dot.TimeZoneRuleVersionNumber,
                Title = dot.Title,
                ToRecipients = dot.ToRecipients,
                // readonly ObjectTypeCode = !string.IsNullOrEmpty(dot.ObjectTypeCode) ? new OptionSetValue(int.Parse(dot.ObjectTypeCode)) : null,
                UTCConversionTimeZoneCode = dot.UtcConversionTimeZoneCode,
                // readonly VersionNumber = dot.VersionNumber,
                WorkerId = !string.IsNullOrEmpty(dot.WorkerId) ? new EntityReference(dot.WorkerLookuplogicalname, Guid.Parse(dot.WorkerId)) : null,
                // readonly WorkerIdModifiedOn = dot.WorkerIdModifiedOn
            };
        }

        /**
         * enteredon
         * sender
         * importsequencenumber
         * msdyn_skipursync
         * objectid
         * organizationid
         * priority
         * queueid
         * queueitemid
         * timezoneruleversionnumber
         * title
         * torecipients
         * objecttypecode
         * utcconversiontimezonecode
         * versionnumber
         * workerid
         * workeridmodifiedon
         */

        public class QueueItemDTO : DTO
        {
            public DateTime? EnteredOn { get; set; }
            public string Sender { get; set; } = string.Empty;
            public int? ImportSequenceNumber { get; set; }
            public bool? MsdynSkipursync { get; set; }
            public string ObjectId { get; set; } = string.Empty;
            public string OrganizationId { get; set; } = string.Empty;
            public int? Priority { get; set; }
            public string QueueId { get; set; } = string.Empty;
            public string QueueItemId { get; set; } = string.Empty;
            public int? TimeZoneRuleVersionNumber { get; set; }
            public string Title { get; set; } = string.Empty;
            public string ToRecipients { get; set; } = string.Empty;
            public string ObjectTypeCode { get; set; } = string.Empty;
            public int? UtcConversionTimeZoneCode { get; set; }
            public long? VersionNumber { get; set; }
            public string WorkerId { get; set; } = string.Empty;
            public string WorkerLookuplogicalname { get; set; } = string.Empty; // This is used to identify the type of entity the worker is, e.g., "systemuser", "account", "team" etc.
            public DateTime? WorkerIdModifiedOn { get; set; }
        }
    }
}
