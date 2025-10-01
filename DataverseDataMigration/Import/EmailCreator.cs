using System;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;
using Microsoft.Xrm.Sdk;
using Helpers;
using static DataRestoration.Mapper.EmailMapper;
using ConsoleJobs.DataRestoration.Emun;
using DataverseDataMigration.Emun;

namespace Import.Creator
{
    public class EmailCreator : BaseCreator
    {
        private MyXrm context = null;
        public EmailCreator(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public override void Create(DTO entity, EntityType ParentType = EntityType.None, string ParentId = "")
        {
            if (entity.Type != EntityType.Email)
            {
                throw new Exception("Expected type of Email");
            }
            var email = MapFromDTO((EmailDTO)entity, context);
            // Validate if email has already been created. If it has, skip and validate children
            var existingEmail = context.EmailSet.FirstOrDefault(e => e.Id == email.Id);
            if (existingEmail == null)
            {
                var relatedTable = "";
                switch (ParentType)
                {
                    case EntityType.Account:
                        relatedTable = "account";
                        break;
                    case EntityType.Contact:
                        relatedTable = "contact";
                        break;
                    case EntityType.Incident:
                        relatedTable = "incident";
                        break;
                    case EntityType.Lead:
                        relatedTable = "lead";
                        break;
                    case EntityType.Opportunity:
                        relatedTable = "opportunity";
                        break;
                    default:
                        throw new NotImplementedException($"Parent type {ParentType} is not implemented for Email creation.");
                }
                email.RegardingObjectId = new EntityReference(relatedTable, Guid.Parse(ParentId));
                context.Create(email);
                context.WriteLog($"Email created with ID: {email.Id}", LogLevel.Verbose);
            }
            else
            {
                context.WriteLog($"Email with ID: {email.Id} already exists, skipping creation.", LogLevel.Verbose);
            }
            // Validate children
            HandleStatus((EmailDTO)entity, context);
            HandleChildren(entity, context);
        }

        private void HandleStatus(EmailDTO entity, MyXrm context)
        {
            if (context.dryRun) return;
            var email = context.EmailSet.Where(e => e.Id == Guid.Parse(entity.Id)).FirstOrDefault();
            if (email.StateCode.ToString() != entity.StateCode || email.StatusCode.ToString() != entity.StatusCode)
            {
                context.Update(new Entity("email", email.Id)
                {
                    ["statecode"] = new OptionSetValue((int)(entity.StateCode != null ? (EmailState)Enum.Parse(typeof(EmailState), entity.StateCode) : EmailState.Open)),
                    ["statuscode"] = new OptionSetValue((int)(entity.StatusCode != null ? (Email_StatusCode)Enum.Parse(typeof(Email_StatusCode), entity.StatusCode) : Email_StatusCode.Kladde))
                });
            }
        }
    }
}