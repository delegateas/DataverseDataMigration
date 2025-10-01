using System;
using System.Collections.Generic;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;
using DataRestoration.Mapper;
using Microsoft.Xrm.Sdk;
using DataverseDataMigration.Emun;
using Helpers;

namespace Export.Fetcher
{
    public class ContactFetcher : BaseFetcher
    {
        private MyXrm context = null; // Placeholder for the Xrm context, should be initialized properly in actual implementation
        public ContactFetcher(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public DTO FetchRelated(Entity entity)
        {
            context.WriteLog($"Fetching related entities for Contact: {entity.Id}",LogLevel.Detailed);
            var contact = (Contact)entity;

            var children = new List<DTO>();

            var emailFetcher = new EmailFetcher(context);
            var phoneCallFetcher = new PhoneCallFetcher(context);
            var leadFetcher = new LeadFetcher(context);
            var noteFetcher = new AnnotationFetcher(context);
            var addressFetcher = new AddressFetcher(context);
            var caseFetcher = new IncidentFetcher(context);
            var opportunityFetcher = new OpportunityFetcher(context);
            // Fetch related emails
            var relatedEmails = context.EmailSet
                .Where(e => e.Contact_Emails.Id == contact.Id)
                .ToList();
            foreach (var email in relatedEmails)
            {
                children.Add(emailFetcher.FetchRelated(email));
            }
            // Fetch related phone calls
            var relatedPhoneCalls = context.PhoneCallSet
                .Where(pc => pc.Contact_Phonecalls.Id == contact.Id)
                .ToList();
            foreach (var phoneCall in relatedPhoneCalls)
            {
                children.Add(phoneCallFetcher.FetchRelated(phoneCall));
            }
            // Fetch related leads
            var relatedLeads = context.LeadSet
                .Where(l => l.ContactId.Id == contact.Id)
                .ToList();
            foreach (var lead in relatedLeads)
            {
                children.Add(leadFetcher.FetchRelated(lead));
            }

            /*// Fetch related notes //TODO Annotations have been disabled since they were to be deleted anyways no need to recreate them
            var relatedNotes = context.AnnotationSet
                .Where(a => a.Contact_Annotation.Id == contact.Id)
                .ToList();
            foreach (var note in relatedNotes)
            {
                children.Add(noteFetcher.FetchRelated(note));
            }*/
            /*// Fetch addresses // TODO Addresses have been disabled since we aren't sure if they are needed 
            var addresses = context.CustomerAddressSet
                .Where(a => a.Contact_CustomerAddress.Id == contact.Id)
                .ToList();
            foreach (var address in addresses)
            {
                children.Add(addressFetcher.FetchRelated(address));
            }*/
            // Fetch cases
            var cases = context.IncidentSet
                .Where(i => i.CustomerId.Id == contact.Id)
                .ToList();
            foreach (var incident in cases)
            {
                children.Add(caseFetcher.FetchRelated(incident));
            }
            // Fetch opportunities
            var opportunities = context.OpportunitySet
                .Where(o => o.CustomerId.Id == contact.Id)
                .ToList();
            foreach (var opportunity in opportunities)
            {
                children.Add(opportunityFetcher.FetchRelated(opportunity));
            }

            var relations = new Dictionary<string, List<string>>();
            relations["Emails"] = context.EmailSet.Where(e => e.EmailSender.Id == contact.Id)
                .Select(e => e.Id.ToString())
                .ToList();
            relations["Leads"] = context.LeadSet.Where(l => l.ParentContactId.Id == contact.Id)
                .Select(l => l.Id.ToString())
                .ToList();
            relations["Cases"] = context.IncidentSet.Where(i => i.PrimaryContactId.Id == contact.Id)
                .Select(i => i.Id.ToString())
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
            /**
             * Email  Contact_Email_EmailSender
             * Leads lead_parent_contact
             * Cases contact_as_primary_contact
             * Activities Contact_ActivityPointer
             * Entitlements contact_entitlement_ContactId
             * Opportunities opportunity_parent_contact
             * Kontingentgrupper ff_membershipfee_member_contact
             * Tillidshverv ff_positionoftrust_member_contact
             * Ansættelser ff_employment_member_contact
             * Cases ff_incident_member_contact
             * Email Messages ff_email_member_contact
             * Sagsparter ff_incidentparty_contact_contact
             * Udvalgsmedlemmer ff_committeemember_member_contact
             * Arrangementstilmeldinger ff_eventattendee_contact_contact
             * Giftcards ff_giftcard_contact_contact
             * Award Giftcards ff_awardgiftcard_newmember_contact
             * Award Giftcards ff_awardgiftcard_existingmember_contact
             * Opportunities ff_opportunity_referrer_contact
             * Activities ff_Contact_member_ff_activities
             * Leads ff_lead_refererer_contact
             * Email Messages ff_email_Internpart_contact
             * 
             */

            return ContactMapper.MapToDTO(contact, children, context,relations);
        }
    }
}
