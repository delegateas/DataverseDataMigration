using System;
using System.Collections.Generic;
using ConsoleJobs.DataRestoration.Emun;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;

namespace DataRestoration.Mapper
{
    // What is the purpose of this one?
    public static class AddressMapper
    {
        /** TODO
         * //Default
         * 
         * //Core
         * 
         * //Hverve
         * 
         * //Jura
         * 
         **/
        public static AddressDTO MapToDTO(CustomerAddress entity)
        {
            return new AddressDTO
            {
                Id = entity.Id.ToString(),
                OwnerId = entity.OwnerId?.Id.ToString(),
                Lookuplogicalname = entity.OwnerId?.LogicalName,
                CreatedOn = entity.CreatedOn,
                CreatedBy = entity.CreatedBy?.Id.ToString(),
                ModifiedOn = entity.ModifiedOn,
                ModifiedBy = entity.ModifiedBy?.Id.ToString(),

                //Relations
                Type = EntityType.Address,
                Children = new List<DTO>()

                // Map other properties as needed
            };
        }
        public static CustomerAddress MapFromDTO(AddressDTO dot)
        {
            return new CustomerAddress
            {
                Id = Guid.Parse(dot.Id),
                OverriddenCreatedOn = dot.CreatedOn,
                //OwnerId = new EntityReference("systemuser", Guid.Parse(dot.OwnerId)),
                //Modified on, modified by and created by cant be set
                // Map other properties as needed
            };
        }
        public class AddressDTO : DTO
        {
        }
    }
}
