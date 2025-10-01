using System;
using System.Collections.Generic;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;
using Microsoft.Xrm.Sdk;
using Helpers;
using static DataRestoration.Mapper.ContactMapper;
using DataRestoration.Mapper;
using ConsoleJobs.DataRestoration.Emun;
using DataverseDataMigration.Emun;

namespace Import.Creator
{
    public class ContactCreator : BaseCreator
    {
        private MyXrm context = null;
        public ContactCreator(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public override void Create(DTO entity, EntityType ParentType = EntityType.None, string ParentId = "")
        {
            if (entity.Type != EntityType.Contact)
            {
                throw new Exception("Expected type of Contact");
            }
            var contact = ContactMapper.MapFromDTO((ContactDTO)entity, context);
            // Validate if contact has already been created. If it has, skip and validate children
            //var existingContact = context.ContactSet.FirstOrDefault(c => c.Id == contact.Id);
            var existingContact = context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == contact.ff_membershipnumber);
            if (existingContact == null)
            {
                switch (ParentType)
                {
                    case EntityType.Account:
                        contact.ParentCustomerId = new EntityReference("account", Guid.Parse(ParentId));
                        break;
                    default:
                        throw new NotImplementedException($"Parent type {ParentType} is not implemented for Contact creation.");
                }
                context.Create(contact);
                context.WriteLog($"Contact created with ID: {contact.Id}", LogLevel.Verbose);
            }
            else
            {
                if(existingContact.AccountId == null && !context.dryRun)
                {
                    var account = context.AccountSet.FirstOrDefault(a => a.AccountNumber == ((ContactDTO)entity).parentcustomerid);
                    context.Update(new Entity("contact", existingContact.Id)
                    {
                        ["parentcustomerid"] = new EntityReference("account", account.Id)
                    });
                }
                context.WriteLog($"Existing Contact ID: {existingContact.Id}, Membership Number: {existingContact.ff_membershipnumber} found overriding GUID for related content", LogLevel.Verbose);
                entity.Id = existingContact.Id.ToString();
            }

            HandleBrokenLinks(Guid.Parse(entity.Id), entity.Relations, context); //Using entity.Id to make sure we use the new contact ID instead of the old one
            // Validate children
            entity.Children.Sort(new Comparison<DTO>((x, y) =>
            {
                if(x.Type == y.Type)
                {
                    return 0;
                }
                else if (x.Type == EntityType.Incident)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }));
            HandleChildren(entity, context);
        }


        private void HandleBrokenLinks(Guid contactId, Dictionary<string, List<string>> relations, MyXrm context)
        {
            //Handle relations
            /**
             * relations["Emails"] = context.EmailSet.Where(e => e.EmailSender.Id == contact.Id)
                .Select(e => e.Id.ToString())
                .ToList();
            relations["Leads"] = context.LeadSet.Where(l => l.ParentContactId.Id == contact.Id)
                .Select(l => l.Id.ToString())
                .ToList();
            relations["Cases"] = context.IncidentSet.Where(i => i.PrimaryContactId.Id == contact.Id)
                .Select(i => i.Id.ToString())
                .ToList();
            relations["Activities"] = context.ActivityPointerSet.Where(a => a.RegardingObjectId.Id == contact.Id)
                .Select(a => a.Id.ToString())
                .ToList();
            relations["Entitlements"] = context.EntitlementSet.Where(e => e.ContactId.Id == contact.Id)
                .Select(e => e.Id.ToString())
                .ToList();
            relations["Opportunities"] = context.OpportunitySet.Where(o => o.ParentContactId.Id == contact.Id)
                .Select(o => o.Id.ToString())
                .ToList();
            relations["MembershipFees"] = context.ff_membershipfeeSet.Where(mf => mf.ff_member.Id == contact.Id)
                .Select(mf => mf.Id.ToString())
                .ToList();
            relations["PositionsOfTrust"] = context.ff_positionoftrustSet.Where(p => p.ff_member.Id == contact.Id)
                .Select(p => p.Id.ToString())
                .ToList();
            relations["Employments"] = context.ff_employmentSet.Where(e => e.ff_member.Id == contact.Id)
                .Select(e => e.Id.ToString())
                .ToList();
            relations["Incidents"] = context.IncidentSet.Where(i => i.ff_member.Id == contact.Id)
                .Select(i => i.Id.ToString())
                .ToList();
            relations["EmailMessages"] = context.EmailSet.Where(e => e.ff_member.Id == contact.Id)
                .Select(e => e.Id.ToString())
                .ToList();
            relations["IncidentParties"] = context.ff_incidentpartySet.Where(ip => ip.ff_contact.Id == contact.Id)
                .Select(ip => ip.Id.ToString())
                .ToList();
            relations["CommitteeMembers"] = context.ff_committeememberSet.Where(cm => cm.ff_member.Id == contact.Id)
                .Select(cm => cm.Id.ToString())
                .ToList();
            relations["EventAttendees"] = context.ff_eventattendeeSet.Where(ea => ea.ff_contact.Id == contact.Id)
                .Select(ea => ea.Id.ToString())
                .ToList();
            relations["GiftCards"] = context.ff_giftcardSet.Where(gc => gc.ff_contact.Id == contact.Id)
                .Select(gc => gc.Id.ToString())
                .ToList();
            relations["AwardGiftCardsNewMember"] = context.ff_awardgiftcardSet.Where(agc => agc.ff_awardgiftcard_newmember_contact.Id == contact.Id)
                .Select(agc => agc.Id.ToString())
                .ToList();
            relations["AwardGiftCardsExistingMember"] = context.ff_awardgiftcardSet.Where(agc => agc.ff_awardgiftcard_existingmember_contact.Id == contact.Id)
                .Select(agc => agc.Id.ToString())
                .ToList();
            relations["ReferrerOpportunities"] = context.OpportunitySet.Where(o => o.ff_opportunity_referrer_contact.Id == contact.Id)
                .Select(o => o.Id.ToString())
                .ToList();
            relations["MemberActivities"] = context.ff_activitiesSet.Where(a => a.ff_member.Id == contact.Id)
                .Select(a => a.Id.ToString())
                .ToList();
            relations["LeadReferrerContacts"] = context.LeadSet.Where(l => l.ff_lead_refererer_contact.Id == contact.Id)
                .Select(l => l.Id.ToString())
                .ToList();
            relations["InternalEmailMessages"] = context.EmailSet.Where(e => e.ff_email_Internpart_contact.Id == contact.Id)
                .Select(e => e.Id.ToString())
                .ToList();
            */
            /*if (relations.ContainsKey("Emails")) //Email are recreated where sender is set etc. no need for this one then
            {
                foreach (var emailId in relations["Emails"])
                {
                    var email = context.EmailSet.FirstOrDefault(e => e.Id == Guid.Parse(emailId));
                    if (email != null)
                    {
                        email.EmailSender = new EntityReference("contact", accountId);
                        context.Update(email);
                        context.WriteLog($"Updated Email with ID: {email.Id} to link with Contact ID: {accountId}");
                    }
                }
            }*/
            if (relations.ContainsKey("Leads"))
            {
                foreach (var leadId in relations["Leads"])
                {
                    var lead = context.LeadSet.FirstOrDefault(l => l.Id == Guid.Parse(leadId));
                    if (lead != null)
                    {
                        context.Update(new Entity("lead", lead.Id)
                        {
                            ["parentcontactid"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Lead with ID: {lead.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("Cases"))
            {
                foreach (var caseId in relations["Cases"])
                {
                    var incident = context.IncidentSet.FirstOrDefault(i => i.Id == Guid.Parse(caseId));
                    if (incident != null)
                    {
                        context.Update(new Entity("incident", incident.Id)
                        {
                            ["primarycontactid"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Case with ID: {incident.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("Entitlements"))
            {
                foreach (var entitlementId in relations["Entitlements"])
                {
                    var entitlementContact = new EntitlementContacts(Guid.Parse(entitlementId));
                    entitlementContact.EntitlementContactId = contactId;
                    context.Create(entitlementContact);
                }
            }
            if (relations.ContainsKey("Opportunities"))
            {
                foreach (var opportunityId in relations["Opportunities"])
                {
                    var opportunity = context.OpportunitySet.FirstOrDefault(o => o.Id == Guid.Parse(opportunityId));
                    if (opportunity != null)
                    {
                        context.Update(new Entity("opportunity", opportunity.Id)
                        {
                            ["parentcontactid"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Opportunity with ID: {opportunity.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("MembershipFees"))
            {
                foreach (var membershipFeeId in relations["MembershipFees"])
                {
                    var membershipFee = context.ff_membershipfeeSet.FirstOrDefault(mf => mf.Id == Guid.Parse(membershipFeeId));
                    if (membershipFee != null)
                    {
                        context.Update(new Entity("ff_membershipfee", membershipFee.Id)
                        {
                            ["ff_member"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Membership Fee with ID: {membershipFee.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("PositionsOfTrust"))
            {
                foreach (var positionId in relations["PositionsOfTrust"])
                {
                    var position = context.ff_positionoftrustSet.FirstOrDefault(p => p.Id == Guid.Parse(positionId));
                    if (position != null)
                    {
                        context.Update(new Entity("ff_positionoftrust", position.Id)
                        {
                            ["ff_member"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Position of Trust with ID: {position.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("Employments"))
            {
                foreach (var employmentId in relations["Employments"])
                {
                    var employment = context.ff_employmentSet.FirstOrDefault(e => e.Id == Guid.Parse(employmentId));
                    if (employment != null)
                    {
                        context.Update(new Entity("ff_employment", employment.Id)
                        {
                            ["ff_member"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Employment with ID: {employment.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("Incidents"))
            {
                foreach (var incidentId in relations["Incidents"])
                {
                    var incident = context.IncidentSet.FirstOrDefault(i => i.Id == Guid.Parse(incidentId));
                    if (incident != null)
                    {
                        context.Update(new Entity("incident", incident.Id)
                        {
                            ["ff_member"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Incident with ID: {incident.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("EmailMessages"))
            {
                foreach (var emailId in relations["EmailMessages"])
                {
                    var email = context.EmailSet.FirstOrDefault(e => e.Id == Guid.Parse(emailId));
                    if (email != null)
                    {
                        context.Update(new Entity("email", email.Id)
                        {
                            ["ff_member"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Email Message with ID: {email.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("IncidentParties"))
            {
                foreach (var incidentPartyId in relations["IncidentParties"])
                {
                    var incidentParty = context.ff_incidentpartySet.FirstOrDefault(ip => ip.Id == Guid.Parse(incidentPartyId));
                    if (incidentParty != null)
                    {
                        context.Update(new Entity("ff_incidentparty", incidentParty.Id)
                        {
                            ["ff_contact"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Incident Party with ID: {incidentParty.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("CommitteeMembers"))
            {
                foreach (var committeeMemberId in relations["CommitteeMembers"])
                {
                    var committeeMember = context.ff_committeememberSet.FirstOrDefault(cm => cm.Id == Guid.Parse(committeeMemberId));
                    if (committeeMember != null)
                    {
                        context.Update(new Entity("ff_committeemember", committeeMember.Id)
                        {
                            ["ff_member"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Committee Member with ID: {committeeMember.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("EventAttendees"))
            {
                foreach (var eventAttendeeId in relations["EventAttendees"])
                {
                    var eventAttendee = context.ff_eventattendeeSet.FirstOrDefault(ea => ea.Id == Guid.Parse(eventAttendeeId));
                    if (eventAttendee != null)
                    {
                        context.Update(new Entity("ff_eventattendee", eventAttendee.Id)
                        {
                            ["ff_contact"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Event Attendee with ID: {eventAttendee.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("GiftCards"))
            {
                foreach (var giftCardId in relations["GiftCards"])
                {
                    var giftCard = context.ff_giftcardSet.FirstOrDefault(gc => gc.Id == Guid.Parse(giftCardId));
                    if (giftCard != null)
                    {
                        context.Update(new Entity("ff_giftcard", giftCard.Id)
                        {
                            ["ff_contact"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Gift Card with ID: {giftCard.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("AwardGiftCardsNewMember"))
            {
                foreach (var awardGiftCardId in relations["AwardGiftCardsNewMember"])
                {
                    var awardGiftCard = context.ff_awardgiftcardSet.FirstOrDefault(agc => agc.Id == Guid.Parse(awardGiftCardId));
                    if (awardGiftCard != null)
                    {
                        context.Update(new Entity("ff_awardgiftcard", awardGiftCard.Id)
                        {
                            ["ff_newmember"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Award Gift Card for New Member with ID: {awardGiftCard.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("AwardGiftCardsExistingMember"))
            {
                foreach (var awardGiftCardId in relations["AwardGiftCardsExistingMember"])
                {
                    var awardGiftCard = context.ff_awardgiftcardSet.FirstOrDefault(agc => agc.Id == Guid.Parse(awardGiftCardId));
                    if (awardGiftCard != null)
                    {
                        context.Update(new Entity("ff_awardgiftcard", awardGiftCard.Id)
                        {
                            ["ff_existingmember"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Award Gift Card for Existing Member with ID: {awardGiftCard.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("ReferrerOpportunities"))
            {
                foreach (var referrerOpportunityId in relations["ReferrerOpportunities"])
                {
                    var referrerOpportunity = context.OpportunitySet.FirstOrDefault(o => o.Id == Guid.Parse(referrerOpportunityId));
                    if (referrerOpportunity != null)
                    {
                        context.Update(new Entity("opportunity", referrerOpportunity.Id)
                        {
                            ["ff_referrer"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Referrer Opportunity with ID: {referrerOpportunity.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("MemberActivities"))
            {
                foreach (var memberActivityId in relations["MemberActivities"])
                {
                    var memberActivity = context.ff_activitiesSet.FirstOrDefault(a => a.Id == Guid.Parse(memberActivityId));
                    if (memberActivity != null)
                    {
                        context.Update(new Entity("ff_activities", memberActivity.Id)
                        {
                            ["ff_member"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Member Activity with ID: {memberActivity.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("LeadReferrerContacts"))
            {
                foreach (var leadReferrerContactId in relations["LeadReferrerContacts"])
                {
                    var leadReferrerContact = context.LeadSet.FirstOrDefault(l => l.Id == Guid.Parse(leadReferrerContactId));
                    if (leadReferrerContact != null)
                    {
                        context.Update(new Entity("lead", leadReferrerContact.Id)
                        {
                            ["ff_refererer"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Lead Referrer Contact with ID: {leadReferrerContact.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("InternalEmailMessages"))
            {
                foreach (var internalEmailId in relations["InternalEmailMessages"])
                {
                    var internalEmail = context.EmailSet.FirstOrDefault(e => e.Id == Guid.Parse(internalEmailId));
                    if (internalEmail != null)
                    {
                        context.Update(new Entity("email", internalEmail.Id)
                        {
                            ["ff_Internpart"] = new EntityReference("contact", contactId)
                        });
                        context.WriteLog($"Updated Internal Email Message with ID: {internalEmail.Id} to link with Contact ID: {contactId}", LogLevel.Verbose);
                    }
                }
            }
        }
    }
}
