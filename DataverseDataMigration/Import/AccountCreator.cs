using System;
using System.Collections.Generic;
using System.Linq;
using Export.Mappers;
using Helpers;
using DataRestoration.Mapper;
using Microsoft.Xrm.Sdk;
using ConsoleJobs.DataRestoration.Emun;
using DataverseDataMigration.Emun;

namespace Import.Creator
{
    public class AccountCreator : BaseCreator
    {
        private MyXrm context = null;

        public AccountCreator(MyXrm context)
        {
            this.context = context;
        }

        public override void Create(DTO entity, EntityType ParentType = EntityType.None, string ParentId = "")
        {
            if (entity.Type != EntityType.Account)
            {
                throw new Exception("Expected type of Account");
            }
            var account = AccountMapper.MapFromDTO((AccountDTO)entity);
            //Validate if account have already been created. If it has skip and validate children
            var existingAccount = context.AccountSet.Where(a => a.AccountNumber == account.AccountNumber).FirstOrDefault();
            if (existingAccount == null)
            {
                context.Create(account);
                context.WriteLog($"Account created with ID: {account.Id}",LogLevel.Verbose);
            }
            else
            {
                account.Id = existingAccount.Id; // Use existing account ID
                context.WriteLog($"Account with AccountNumber: {account.AccountNumber} already exists, using existing ID: {account.Id}", LogLevel.Verbose);
            }
            HandleBrokenLinks(account.Id, entity.Relations, context);


            //Validate children
            HandleChildren(entity, context);
        }

        private void HandleBrokenLinks(Guid accountId, Dictionary<string, List<string>> relations, MyXrm context)
        {
            if (relations.ContainsKey("Opportunities"))
            {
                foreach (var opportunityId in relations["Opportunities"])
                {
                    var opportunity = context.OpportunitySet.FirstOrDefault(o => o.Id == Guid.Parse(opportunityId));
                    if (opportunity != null)
                    {
                        context.Update(new Entity("opportunity", opportunity.Id)
                        {
                            ["parentaccountid"] = new EntityReference("account", accountId)
                        });
                        context.WriteLog($"Updated Opportunity with ID: {opportunity.Id} to link with Account ID: {accountId}", LogLevel.Verbose);
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
                            ["ff_company"] = new EntityReference("account", accountId)
                        });
                        context.WriteLog($"Updated Case with ID: {incident.Id} to link with Account ID: {accountId}", LogLevel.Verbose);
                    }
                }
            }
            if (relations.ContainsKey("Contacts"))
            {
                foreach (var contactId in relations["Contacts"])
                {
                    var contact = context.ContactSet.FirstOrDefault(c => c.Id == Guid.Parse(contactId));
                    if (contact != null)
                    {
                        context.Update(new Entity("contact", contact.Id)
                        {
                            ["ff_firm"] = new EntityReference("account", accountId)
                        });
                        context.WriteLog($"Updated Contact with ID: {contact.Id} to link with Account ID: {accountId}", LogLevel.Verbose);
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
                            ["ff_workplace"] = new EntityReference("account", accountId)
                        });
                        context.WriteLog($"Updated Position of Trust with ID: {position.Id} to link with Account ID: {accountId}", LogLevel.Verbose);
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
                            ["ff_workplace"] = new EntityReference("account", accountId)
                        });
                        context.WriteLog($"Updated Employment with ID: {employment.Id} to link with Account ID: {accountId}", LogLevel.Verbose);
                    }
                }
            }
        }
    }
}
