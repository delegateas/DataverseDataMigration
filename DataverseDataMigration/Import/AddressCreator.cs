using System;
using System.Linq;
using Export.Mappers;
using Microsoft.Xrm.Sdk;
using Helpers;
using ConsoleJobs.DataRestoration.Emun;
using static DataRestoration.Mapper.AddressMapper;
using DataverseDataMigration.Emun;

namespace Import.Creator
{
    public class AddressCreator : BaseCreator
    {
        private MyXrm context = null;
        public AddressCreator(MyXrm context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override void Create(DTO entity, EntityType ParentType = EntityType.None, string ParentId = "")
        {
            if (entity.Type != EntityType.Address)
            {
                throw new Exception("Expected type of Address");
            }
            var address = MapFromDTO((AddressDTO)entity);
            // Validate if address has already been created. If it has, skip and validate children
            var existingAddress = context.CustomerAddressSet.FirstOrDefault(a => a.Id == address.Id);
            if (existingAddress == null)
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
                    default:
                        throw new NotImplementedException($"Parent type {ParentType} is not implemented for Address creation.");
                }
                address.ParentId = new EntityReference(relatedTable, Guid.Parse(ParentId));
                context.Create(address);
                context.WriteLog($"Address created with ID: {address.Id}", LogLevel.Verbose);
            }
            else
            {
                context.WriteLog($"Address with ID: {address.Id} already exists, skipping creation.", LogLevel.Verbose);
            }
            // Validate children
            HandleChildren(entity, context);
        }
    }
}
