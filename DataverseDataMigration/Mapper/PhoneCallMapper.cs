using System;
using System.Collections.Generic;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using ConsoleJobs.DataRestoration.Emun;
using Export.Mappers;
using Microsoft.Xrm.Sdk;
using Helpers;

namespace DataRestoration.Mapper
{
    public static class PhoneCallMapper
    {
        /**
         * //Default
         * 
         * //Core
         * -
         * //Hverve
         * From
         * To
         * Subject
         * //Jura
         * Description
         * From
         * To
         * Subject
         **/
        public static PhoneCallDTO MapToDTO(PhoneCall entity, List<DTO> children, Xrm context)
        {
            return new PhoneCallDTO
            {
                Id = entity.Id.ToString(),
                OwnerId = entity.OwnerId?.Id.ToString(),
                Lookuplogicalname = entity.OwnerId?.LogicalName,
                CreatedOn = entity.CreatedOn,
                CreatedBy = entity.CreatedBy?.Id.ToString(),
                ModifiedOn = entity.ModifiedOn,
                ModifiedBy = entity.ModifiedBy?.Id.ToString(),
                StateCode = entity.StateCode.ToString(),
                StatusCode = entity.StatusCode.ToString(),

                //Relations
                Type = EntityType.PhoneCall,
                Children = children,

                // Map other properties as needed
                Subject = entity.Subject,
                Description = entity.Description,
                From = entity.From.Select((p) =>
                {
                    var type = p.PartyId.LogicalName;
                    if (type == "contact")
                    {
                        var contact = context.ContactSet.FirstOrDefault(c => c.Id == p.PartyId.Id);
                        return new PhoneCallPerson()
                        {
                            Type = "contact",
                            Id = p.Id.ToString() ?? "",
                            Membershipnumber = contact.ff_membershipnumber
                        };
                    }
                    return new PhoneCallPerson()
                    {
                        Type = p.PartyId.LogicalName,
                        Id = p.Id.ToString() ?? "",
                        PartyId = p.PartyId?.Id.ToString() ?? ""
                    };
                }).ToList(),
                To = entity.To.Select((p) =>
                {
                    var type = p.PartyId.LogicalName;
                    if (type == "contact")
                    {
                        var contact = context.ContactSet.FirstOrDefault(c => c.Id == p.PartyId.Id);
                        return new PhoneCallPerson()
                        {
                            Type = "contact",
                            Id = p.Id.ToString() ?? "",
                            Membershipnumber = contact.ff_membershipnumber
                        };
                    }
                    return new PhoneCallPerson()
                    {
                        Type = p.PartyId.LogicalName,
                        Id = p.Id.ToString() ?? "",
                        PartyId = p.PartyId?.Id.ToString() ?? ""
                    };
                }).ToList(),
                PhoneNumber = entity.PhoneNumber,
                DirectionCode = entity.DirectionCode,
                RegardingObjectId = entity.RegardingObjectId?.Id.ToString(),
                ActualDurationMinutes = entity.ActualDurationMinutes,
                PriorityCode = entity.PriorityCode?.ToString(),
                
                
                //Due = ??
            };
        }
        public static PhoneCall MapFromDTO(PhoneCallDTO dot, MyXrm context)
        {
            return new PhoneCall
            {
                Id = Guid.Parse(dot.Id),
                OverriddenCreatedOn = dot.CreatedOn,
                OwnerId = new EntityReference(dot.Lookuplogicalname, Guid.Parse(dot.OwnerId)),
                /*StateCode = dot.StateCode != null ? (PhoneCallState)Enum.Parse(typeof(PhoneCallState), dot.StateCode) : PhoneCallState.Open, TODO Handle state and status codes
                StatusCode = dot.StatusCode != null ? (PhoneCall_StatusCode)Enum.Parse(typeof(PhoneCall_StatusCode), dot.StatusCode) : PhoneCall_StatusCode.Open,
                */
                StateCode = PhoneCallState.Open, // Default state
                StatusCode = PhoneCall_StatusCode.Open, // Default status
                //Modified on, modified by and created by cant be set
                From = dot.From.Select(p => 
                    {
                        if (p.Type == "contact")
                        {
                            // Fetch contact ff_medlemsnumber instead
                            var contact = context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == p.Membershipnumber);
                            return new ActivityParty
                            {
                                PartyId = new EntityReference("contact", contact.Id),
                            };
                        }
                        return new ActivityParty
                        {
                            Id = Guid.Parse(p.Id),
                            PartyId = new EntityReference(p.Type, Guid.Parse(p.PartyId))
                        };
                    }
                ).ToList(),
                To = dot.To.Select(p => 
                    {
                        if (p.Type == "contact")
                        {
                            // Fetch contact ff_medlemsnumber instead
                            var contact = context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == p.Membershipnumber);
                            return new ActivityParty
                            {
                                PartyId = new EntityReference("contact", contact.Id),
                            };
                        }
                        return new ActivityParty
                        {
                            Id = Guid.Parse(p.Id),
                            PartyId = new EntityReference(p.Type, Guid.Parse(p.PartyId))
                        };
                    }
                ).ToList(),
                // Map other properties as needed
                Subject = dot.Subject,
                Description = dot.Description,
                PhoneNumber = dot.PhoneNumber,
                DirectionCode = dot.DirectionCode,
                RegardingObjectId = dot.RegardingObjectId != null ? new EntityReference("incident", Guid.Parse(dot.RegardingObjectId)) : null, // TODO include other types, contact, opportunity etc.
                ActualDurationMinutes = dot.ActualDurationMinutes,
                PriorityCode = dot.PriorityCode != null ? (PhoneCall_PriorityCode)Enum.Parse(typeof(PhoneCall_PriorityCode), dot.PriorityCode) : PhoneCall_PriorityCode.Normal,                
            };
        }
        public class PhoneCallDTO : DTO
        {
            public string Subject { get; set; }
            public string Description { get; set; }
            public List<PhoneCallPerson> From { get; set; }
            public List<PhoneCallPerson> To { get; set; }
            public string PhoneNumber { get; set; }
            public bool? DirectionCode { get; set; }
            public string RegardingObjectId { get; set; }
            public int? ActualDurationMinutes { get; set; }
            public string PriorityCode { get; set; }            
            //public DateTime? DueDate { get; set; }

        }
        public class PhoneCallPerson
        {
            public string Type { get; set; }
            public string Id { get; set; }
            public string PartyId { get; set; }
            public string LogicalName { get; set; }
            public string Membershipnumber { get; set; } // Optional, used for contacts
        }
    }
}
