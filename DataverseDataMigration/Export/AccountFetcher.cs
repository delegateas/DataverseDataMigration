using System;
using System.Collections.Generic;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;
using DataRestoration.Mapper;
using Microsoft.Xrm.Sdk;
using Helpers;
using DataverseDataMigration.Emun;

namespace Export.Fetcher
{
    public class AccountFetcher : BaseFetcher
    {
        private MyXrm context = null; // Placeholder for the Xrm context, should be initialized properly in actual implementation
        public AccountFetcher(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public DTO FetchRelated(Entity entity)
        {
            context.WriteLog($"Fetching related entities for Account: {entity.Id}",LogLevel.Detailed);
            var account = (Account)entity;

            var children = new List<DTO>();

            //Fetch related phone calls
            var phoneCallFetcher = new PhoneCallFetcher(context);
            var relatedPhoneCalls = context.PhoneCallSet
                .Where(pc => pc.Account_Phonecalls.Id == account.Id)
                .ToList();
            foreach (var phoneCall in relatedPhoneCalls)
            {
                children.Add(phoneCallFetcher.FetchRelated(phoneCall));
            }
            //Fetch related leads
            var leadFetcher = new LeadFetcher(context);
            var relatedLeads = context.LeadSet
                .Where(l => l.CustomerId.Id == account.Id)
                .ToList();
            foreach (var lead in relatedLeads)
            {
                children.Add(leadFetcher.FetchRelated(lead));
            }
            /*//Fetch related notes //TODO Annotations have been disabed since they were to be removed anyways, no need to recreate them
            var noteFetcher = new AnnotationFetcher(context);
            var relatedNotes = context.AnnotationSet
                .Where(a => a.Account_Annotation.Id == account.Id)
                .ToList();
            foreach (var note in relatedNotes)
            {
                children.Add(noteFetcher.FetchRelated(note));
            }*/
            //Fetch related emails
            var emailFetcher = new EmailFetcher(context);
            var relatedEmails = context.EmailSet
                .Where(e => e.Account_Emails.Id == account.Id)
                .ToList();
            foreach (var email in relatedEmails)
            {
                children.Add(emailFetcher.FetchRelated(email));
            }

            //Fetch related opportunities
            var opportunityFetcher = new OpportunityFetcher(context);
            var relatedOpportunities = context.OpportunitySet
                .Where(o => o.CustomerId.Id == account.Id)
                .ToList();
            foreach (var opportunity in relatedOpportunities)
            {
                children.Add(opportunityFetcher.FetchRelated(opportunity));
            }
            //Fetch related cases
            var caseFetcher = new IncidentFetcher(context);
            var relatedCases = context.IncidentSet
                .Where(i => i.CustomerId.Id == account.Id)
                .ToList();
            foreach (var incident in relatedCases)
            {
                children.Add(caseFetcher.FetchRelated(incident));
            }
            //Fetch related contacts
            var contactFetcher = new ContactFetcher(context);
            var relatedContacts = context.ContactSet
                .Where(c => c.AccountId.Id == account.Id)
                .ToList();
            foreach (var contact in relatedContacts)
            {
                children.Add(contactFetcher.FetchRelated(contact));
            }

            /*//Fetch related addresses //TODO Addresses have been disabled since we aren't sure if they are needed
            var addressFetcher = new AddressFetcher(context);
            var relatedAddresses = context.CustomerAddressSet
                .Where(a => a.Account_CustomerAddress.Id == account.Id)
                .ToList();
            foreach (var address in relatedAddresses)
            {
                children.Add(addressFetcher.FetchRelated(address));
            }*/

            var relations = new Dictionary<string, List<string>>();
            relations["Opportunities"] = context.OpportunitySet
                .Where(o => o.ParentAccountId.Id == account.Id)
                .Select(o => o.Id.ToString()).ToList();
            relations["Cases"] = context.IncidentSet
                .Where(c => c.ff_company.Id == account.Id)
                .Select(c => c.Id.ToString())
                .ToList();
            relations["Contacts"] = context.ContactSet
                .Where(c => c.ff_firm.Id == account.Id)
                .Select(c => c.Id.ToString())
                .ToList();
            relations["PositionsOfTrust"] = context.ff_positionoftrustSet
                .Where(p => p.ff_workplace.Id == account.Id)
                .Select(p => p.Id.ToString())
                .ToList();
            relations["Employments"] = context.ff_employmentSet
                .Where(e => e.ff_workplace.Id == account.Id)
                .Select(e => e.Id.ToString())
                .ToList();
            return AccountMapper.MapToDTO(account, children, relations);
        }
    }
}
