using System;
using System.Collections.Generic;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using ConsoleJobs.DataRestoration.Emun;
using Export.Mappers;
using Microsoft.Xrm.Sdk;

namespace DataRestoration.Mapper
{
    public static class EmailMapper
    {
        /** TODO
         * //Default
         * TODO
         * 
         * //Core
         * -
         * 
         * //Hverve
         * ff_closedopportunity
         * 
         * //Jura
         * ff_sendersmailaddress
         * ff_closedcase
         * ff_emailhandledby
         * from
         * Sender
         * ff_receivingmailbox
         * ff_getattachments - DO NOT MAP.
         * ff_Internpart - DO NOT MAP. Only used for helping the user copying a secondary email address from a contact.
         * ff_member
         * ff_Note
         * RegardingObjectId
         * ff_case
         * ff_casenumber
         * ff_sharepointurl
         * to
         **/
        public static EmailDTO MapToDTO(Email entity, List<DTO> children, Xrm context)
        {
            var ff_member = entity.ff_member != null ? context.ContactSet.FirstOrDefault(c => c.Id == entity.ff_member.Id)?.ff_membershipnumber : null;
            return new EmailDTO
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
                Type = EntityType.Email,
                Children = children,

                // Map other properties as needed
                Subject = entity.Subject,
                Description = entity.Description,
                From = entity.From.Select((p) => {
                    var type = p.PartyId?.LogicalName ?? "";
                    if(type == "contact")
                    {
                        //Fetch contact ff_medlemsnumber instead
                        var contact = context.ContactSet.FirstOrDefault(c => c.Id == p.PartyId.Id);
                        return new EmailPerson()
                        {
                            AddressUsed = p.AddressUsed,
                            Type = "contact",
                            Id = p.Id.ToString() ?? "",
                            Membershipnumber = contact.ff_membershipnumber
                        };
                    }
                    return new EmailPerson()
                    {
                        AddressUsed = p.AddressUsed,
                        Type = p.PartyId?.LogicalName ?? "",
                        Id = p.Id.ToString() ?? "",
                        PartyId = p.PartyId?.Id.ToString() ?? ""
                    };
                }).ToList(),
                To = entity.To.Select((p) =>
                {
                    var type = p.PartyId?.LogicalName ?? "";
                    if (type == "contact")
                    {
                        //Fetch contact ff_medlemsnumber instead
                        var contact = context.ContactSet.FirstOrDefault(c => c.Id == p.PartyId.Id);
                        if (contact != null && !string.IsNullOrEmpty(contact.ff_membershipnumber))
                        {
                            return new EmailPerson()
                            {
                                AddressUsed = p.AddressUsed,
                                Type = "contact",
                                Id = p.Id.ToString() ?? "",
                                Membershipnumber = contact.ff_membershipnumber
                            };
                        }
                    }
                    return new EmailPerson()
                    {
                        AddressUsed = p.AddressUsed,
                        Type = p.PartyId?.LogicalName ?? "",
                        Id = p.Id.ToString() ?? "",
                        PartyId = p.PartyId?.Id.ToString() ?? ""
                    };
                }).ToList(),
                Cc = entity.Cc.Select((p) =>
                {
                    var type = p.PartyId?.LogicalName ?? "";
                    if (type == "contact")
                    {
                        //Fetch contact ff_medlemsnumber instead
                        var contact = context.ContactSet.FirstOrDefault(c => c.Id == p.PartyId.Id);
                        if (contact != null && !string.IsNullOrEmpty(contact.ff_membershipnumber))
                        {
                            return new EmailPerson()
                            {
                                AddressUsed = p.AddressUsed,
                                Type = "contact",
                                Id = p.Id.ToString() ?? "",
                                Membershipnumber = contact.ff_membershipnumber
                            };
                        }
                    }
                    return new EmailPerson()
                    {
                        AddressUsed = p.AddressUsed,
                        Type = p.PartyId?.LogicalName ?? "",
                        Id = p.Id.ToString() ?? "",
                        PartyId = p.PartyId?.Id.ToString() ?? ""
                    };
                }).ToList(),
                Bcc = entity.Bcc.Select((p) =>
                {
                    var type = p.PartyId?.LogicalName ?? "";
                    if (type == "contact")
                    {
                        //Fetch contact ff_medlemsnumber instead
                        var contact = context.ContactSet.FirstOrDefault(c => c.Id == p.PartyId.Id);
                        if (contact != null && !string.IsNullOrEmpty(contact.ff_membershipnumber))
                        {
                            return new EmailPerson()
                            {
                                AddressUsed = p.AddressUsed,
                                Type = "contact",
                                Id = p.Id.ToString() ?? "",
                                Membershipnumber = contact.ff_membershipnumber
                            };
                        }
                    }
                    return new EmailPerson()
                    {
                        AddressUsed = p.AddressUsed,
                        Type = p.PartyId?.LogicalName ?? "",
                        Id = p.Id.ToString() ?? "",
                        PartyId = p.PartyId?.Id.ToString() ?? ""
                    };
                }).ToList(),
                RegardingObjectId = entity.RegardingObjectId?.Id.ToString(),
                ff_closedopportunity = entity.ff_closedopportunity?.Id.ToString(),
                ff_sendersmailaddress = entity.ff_sendersmailaddress,
                ff_closedcase = entity.ff_closedcase?.Id.ToString(),
                ff_emailhandledby = entity.ff_emailhandledby?.Id.ToString(),
                ff_emailhandledbyLogicalName = entity.ff_emailhandledby?.LogicalName,
                Sender = entity.Sender,
                ff_receivingmailbox = entity.ff_receivingmailbox?.Id.ToString(),
                ff_member = ff_member,
                ff_Note = entity.ff_Note,
                ff_case = entity.ff_case?.Id.ToString(),
                ff_casenumber = entity.ff_casenumber,
                ff_sharepointurl = entity.ff_sharepointurl
            };
        }
        public static Email MapFromDTO(EmailDTO dot, Xrm context)
        {
            var ff_member = !string.IsNullOrEmpty(dot.ff_member) ? context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == dot.ff_member)?.Id : null;
            return new Email
            {
                Id = Guid.Parse(dot.Id),
                OverriddenCreatedOn = dot.CreatedOn,
                OwnerId = new EntityReference(dot.Lookuplogicalname, Guid.Parse(dot.OwnerId)),
                /* TODO Handle state and status codes
                StateCode = dot.StateCode != null ? (EmailState)Enum.Parse(typeof(EmailState), dot.StateCode) : EmailState.Open,
                StatusCode = dot.StatusCode != null ? (Email_StatusCode)Enum.Parse(typeof(Email_StatusCode), dot.StatusCode) : Email_StatusCode.Kladde,*/
                StateCode = EmailState.Open, // Default state
                StatusCode = Email_StatusCode.Kladde, // Default status
                //Modified on, modified by and created by cant be set
                To = dot.To.Select(p =>
                    {
                        if (string.IsNullOrEmpty(p.Type)) {
                            return new ActivityParty
                            {
                                AddressUsed = p.AddressUsed,
                            };
                        }

                        if(p.Type == "contact")
                        {
                            // Fetch contact ff_medlemsnumber instead
                            var contact = context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == p.Membershipnumber);
                            return new ActivityParty
                            {
                                Id = Guid.Parse(p.Id),
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
                From = dot.From.Select(p =>
                    {
                        if (string.IsNullOrEmpty(p.Type))
                        {
                            return new ActivityParty
                            {
                                AddressUsed = p.AddressUsed,
                            };
                        }

                        if (p.Type == "contact")
                        {
                            // Fetch contact ff_medlemsnumber instead
                            var contact = context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == p.Membershipnumber);
                            return new ActivityParty
                            {
                                Id = Guid.Parse(p.Id),
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
                Cc = dot.Cc.Select(p =>
                    {
                        if (string.IsNullOrEmpty(p.Type))
                        {
                            return new ActivityParty
                            {
                                AddressUsed = p.AddressUsed,
                            };
                        }
                        if (p.Type == "contact")
                        {
                            // Fetch contact ff_medlemsnumber instead
                            var contact = context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == p.Membershipnumber);
                            return new ActivityParty
                            {
                                Id = Guid.Parse(p.Id),
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
                Bcc = dot.Bcc.Select(p =>
                    {
                        if (string.IsNullOrEmpty(p.Type))
                        {
                            return new ActivityParty
                            {
                                AddressUsed = p.AddressUsed,
                            };
                        }
                        if (p.Type == "contact")
                        {
                            // Fetch contact ff_medlemsnumber instead
                            var contact = context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == p.Membershipnumber);
                            return new ActivityParty
                            {
                                Id = Guid.Parse(p.Id),
                                PartyId = new EntityReference("contact", contact.Id)
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
                RegardingObjectId = !string.IsNullOrEmpty(dot.RegardingObjectId) ? new EntityReference("incident", Guid.Parse(dot.RegardingObjectId)) : null, // TODO extend to include other types, opportunity and contact.
                ff_closedopportunity = !string.IsNullOrEmpty(dot.ff_closedopportunity) ? new EntityReference("opportunity", Guid.Parse(dot.ff_closedopportunity)) : null,
                ff_sendersmailaddress = dot.ff_sendersmailaddress,
                ff_closedcase = !string.IsNullOrEmpty(dot.ff_closedcase) ? new EntityReference("incident", Guid.Parse(dot.ff_closedcase)) : null,
                ff_emailhandledby = !string.IsNullOrEmpty(dot.ff_emailhandledby) ? new EntityReference(dot.ff_emailhandledbyLogicalName, Guid.Parse(dot.ff_emailhandledby)) : null,
                Sender = dot.Sender,
                ff_receivingmailbox = !string.IsNullOrEmpty(dot.ff_receivingmailbox) ? new EntityReference("mailbox", Guid.Parse(dot.ff_receivingmailbox)) : null,
                ff_member = ff_member != null ? new EntityReference("contact", (Guid)ff_member) : null,
                ff_Note = dot.ff_Note,
                ff_case = !string.IsNullOrEmpty(dot.ff_case) ? new EntityReference("incident", Guid.Parse(dot.ff_case)) : null,
                ff_casenumber = dot.ff_casenumber,
                ff_sharepointurl = dot.ff_sharepointurl

            };
        }
        public class EmailDTO : DTO
        {
            public string Subject { get; set; }
            public string Description { get; set; }
            public string RegardingObjectId { get; set; }
            public string ff_closedopportunity { get; set; }
            public string ff_sendersmailaddress { get; set; }
            public string ff_closedcase { get; set; }
            public string ff_emailhandledby { get; set; }
            public string ff_emailhandledbyLogicalName { get; set; } // Logical name of the entity for ff_emailhandledby
            public List<EmailPerson> From { get; set; }
            public string Sender { get; set; }
            public string ff_receivingmailbox { get; set; }
            public string ff_member { get; set; }
            public string ff_Note { get; set; }
            public string ff_case { get; set; }
            public string ff_casenumber { get; set; }
            public string ff_sharepointurl { get; set; }
            public List<EmailPerson> To { get; set; }
            public List<EmailPerson> Cc { get; set; }
            public List<EmailPerson> Bcc { get; set; }

        }

        public class EmailPerson
        {
            public string AddressUsed { get; set; }
            public string Id { get; set; }
            public string PartyId { get; set; }
            public string Type { get; set; }
            public string Membershipnumber { get; set; } // Only used for contacts to store the membership number
        }
    }
}
