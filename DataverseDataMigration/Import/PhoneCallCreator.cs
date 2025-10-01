using System;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;
using Microsoft.Xrm.Sdk;
using Helpers;
using static DataRestoration.Mapper.PhoneCallMapper;
using ConsoleJobs.DataRestoration.Emun;
using DataverseDataMigration.Emun;

namespace Import.Creator
{
    public class PhoneCallCreator : BaseCreator
    {
        private readonly MyXrm context = null;
        public PhoneCallCreator(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public override void Create(DTO entity, EntityType ParentType = EntityType.None, string ParentId = "")
        {
            if (entity.Type != EntityType.PhoneCall)
            {
                throw new Exception("Expected type of PhoneCall");
            }
            var phoneCall = MapFromDTO((PhoneCallDTO)entity, context);
            // Validate if phone call has already been created. If it has, skip and validate children
            var existingPhoneCall = context.PhoneCallSet.FirstOrDefault(pc => pc.Id == phoneCall.Id);
            if (existingPhoneCall == null)
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
                        throw new NotImplementedException($"Parent type {ParentType} is not implemented for PhoneCall creation.");
                }
                phoneCall.RegardingObjectId = new EntityReference(relatedTable, Guid.Parse(ParentId));
                context.Create(phoneCall);
                context.WriteLog($"Phone Call created with ID: {phoneCall.Id}", LogLevel.Verbose);
            }
            else
            {
                context.WriteLog($"Phone Call with ID: {phoneCall.Id} already exists, skipping creation.", LogLevel.Verbose);
            }
            HandleStatus((PhoneCallDTO)entity, context);
            // Validate children
            HandleChildren(entity, context);
        }

        private void HandleStatus(PhoneCallDTO entity, MyXrm context)
        {
            if (context.dryRun) return;
            var phoneCall = context.PhoneCallSet.Where(e => e.Id == Guid.Parse(entity.Id)).FirstOrDefault();
            if (phoneCall.StateCode.ToString() != entity.StateCode || phoneCall.StatusCode.ToString() != entity.StatusCode)
            {
                context.Update(new Entity("phonecall", phoneCall.Id)
                {
                    ["statecode"] = new OptionSetValue((int)(Enum.Parse(typeof(PhoneCallState), entity.StateCode))),
                    ["statuscode"] = new OptionSetValue((int)(Enum.Parse(typeof(PhoneCall_StatusCode), entity.StatusCode)))
                });
            }
        }
    }
}
