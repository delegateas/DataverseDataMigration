using System;
using System.Collections.Generic;
using ConsoleJobs.DataRestoration.Emun;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using Export.Mappers;
using Microsoft.Xrm.Sdk;

namespace DataRestoration.Mapper
{
    public static class AccountMapper
    {
        /**
         * //Default
         * name
         * telephone1
         * websiteurl
         * parentaccountid
         * address1_line1
         * address1_line2
         * address1_line3
         * address1_city
         * address1_stateorprovince
         * address1_postalcode
         * address1_country
         * accountnumber
         * owningbusinessunit
         * 
         * //Core
         * ff_name2
         * ff_collectiveagreement
         * //Hverve
         * -
         * //Jura
         * ff_collectiveagreement
         **/
        public static AccountDTO MapToDTO(Account entity,List<DTO> children, Dictionary<string, List<string>> relations)
        {
            return new AccountDTO
            {
                Id = entity.Id.ToString(),
                OwnerId = entity.OwnerId?.Id.ToString(),
                Lookuplogicalname = entity.OwnerId?.LogicalName,
                CreatedOn = entity.CreatedOn,
                CreatedBy = entity.CreatedBy?.Id.ToString(),
                ModifiedOn = entity.ModifiedOn,
                ModifiedBy = entity.ModifiedBy?.Id.ToString(),
                //Relations
                Type = EntityType.Account,
                Children = children,
                Relations = relations,

                // Map other properties as needed
                Name = entity.Name,                
                PhoneNumber = entity.Telephone1,
                WebsiteUrl = entity.WebSiteURL,
                ParentAccountId = entity.ParentAccountId?.Id.ToString(),
                AddressLine1 = entity.Address1_Line1,
                AddressLine2 = entity.Address1_Line2,
                AddressLine3 = entity.Address1_Line3,
                City = entity.Address1_City,
                StateOrProvince = entity.Address1_StateOrProvince,
                PostalCode = entity.Address1_PostalCode,
                Country = entity.Address1_Country,
                AccountNumber = entity.AccountNumber,
                OwningBusinessUnit = entity.OwningBusinessUnit?.Id.ToString(),
                ff_name2 = entity.ff_name2,
                ff_collectiveagreement = entity.ff_collectiveagreement?.Id.ToString(),
            };
        }

        public static Account MapFromDTO(AccountDTO dot)
        {
            return new Account
            {
                Id = Guid.Parse(dot.Id),
                OverriddenCreatedOn = dot.CreatedOn,
                OwnerId = new EntityReference(dot.Lookuplogicalname, Guid.Parse(dot.OwnerId)),
                StateCode = dot.StateCode != null ? (AccountState)Enum.Parse(typeof(AccountState), dot.StateCode) : AccountState.Active,
                StatusCode = dot.StatusCode != null ? (Account_StatusCode)Enum.Parse(typeof(Account_StatusCode), dot.StatusCode) : Account_StatusCode.Active,
                //Modified on, modified by and created by cant be set

                // Map other properties as needed
                Name = dot.Name,                
                Telephone1 = dot.PhoneNumber,                
                WebSiteURL = dot.WebsiteUrl,
                ParentAccountId = !string.IsNullOrEmpty(dot.ParentAccountId) ? new EntityReference("account", Guid.Parse(dot.ParentAccountId)) : null,
                Address1_Line1 = dot.AddressLine1,
                Address1_Line2 = dot.AddressLine2,
                Address1_Line3 = dot.AddressLine3,
                Address1_City = dot.City,
                Address1_StateOrProvince = dot.StateOrProvince,
                Address1_PostalCode = dot.PostalCode,
                Address1_Country = dot.Country,
                AccountNumber = dot.AccountNumber,
 // Is set using ownerid - OwningBusinessUnit = !string.IsNullOrEmpty(dot.OwningBusinessUnit) ? new EntityReference("businessunit", Guid.Parse(dot.OwningBusinessUnit)) : null, 
                ff_name2 = dot.ff_name2,
                ff_collectiveagreement = !string.IsNullOrEmpty(dot.ff_collectiveagreement) ? new EntityReference("ff_collectiveagreement", Guid.Parse(dot.ff_collectiveagreement)) : null,
            };
        }
    }

    public class AccountDTO : DTO
    {
        public string Name { get; set; }
        public string ff_name2 { get; set; }        
        public string PhoneNumber { get; set; }     
        public string WebsiteUrl { get; set; }
        public string ParentAccountId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string AccountNumber { get; set; }                      
        public string ff_collectiveagreement { get; set; }

    }
}
