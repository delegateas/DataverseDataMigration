using System;
using System.Collections.Generic;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using ConsoleJobs.DataRestoration.Emun;
using Export.Mappers;
using Microsoft.Xrm.Sdk;

namespace DataRestoration.Mapper
{
    public static class OpportunitiesMapper
    {
        public static OpportunityDTO MapToDTO(Opportunity entity, List<DTO> children, Xrm context, Dictionary<string, List<string>> relations)
        {
            var ParentContactId = entity.ParentContactId != null ? context.ContactSet.Where(c => c.Id == entity.ParentContactId.Id).FirstOrDefault()?.ff_membershipnumber : null;
            var Customerid = entity.CustomerId != null ? context.AccountSet.Where(a => a.Id == entity.CustomerId.Id).FirstOrDefault()?.AccountNumber : null;
            return new OpportunityDTO
            {
                Id = entity.Id.ToString(),
                OwnerId = entity.OwnerId?.Id.ToString(),
                Lookuplogicalname = entity.OwnerId?.LogicalName,
                CreatedOn = entity.CreatedOn,
                CreatedBy = entity.CreatedBy?.Id.ToString(),
                ModifiedOn = entity.ModifiedOn,
                ModifiedBy = entity.ModifiedBy?.Id.ToString(),

                //Relations
                Type = EntityType.Opportunity,
                Children = children,
                Relations = relations,

                // Map other properties as needed
                Campaignid = entity.CampaignId?.Id.ToString(),
                Ff_campaignactivity = entity.ff_campaignactivity?.Id.ToString(),
                Ff_company = entity.ff_company,
                Contactid = entity.ContactId?.Id.ToString(),
                Parentcontactid = ParentContactId,
                Ff_personalidentificationnumber = entity.ff_personalidentificationnumber,
                Emailaddress = entity.EmailAddress,
                Ff_followupdate = entity.ff_followupdate,
                Ff_preferredlanguage = entity.ff_preferredlanguage,
                Ff_giftcard = entity.ff_giftcard.Id.ToString(),
                Ff_interactiondate = entity.ff_interactiondate,
                Ff_enrollmenttype = (int)entity.ff_enrollmenttype,
                Ff_jobtitle = entity.ff_jobtitle,
                Originatingleadid = entity.OriginatingLeadId?.Id.ToString(),
                Owningbusinessunit = entity.OwningBusinessUnit?.Id.ToString(),
                Owninguser = entity.OwningUser?.Id.ToString(),
                Customerid = Customerid,
                Ff_referrer = entity.ff_referrer.Id.ToString(),
                Ff_salesscore = entity.ff_salesscore,
                Ff_specifiedindustry = entity.ff_specifiedindustry,
                Statecode = (int)entity.StateCode,
                Statuscode = (int)entity.StatusCode,
                Ff_studyprogram = entity.ff_studyprogram,
                Ff_placeofstudy = entity.ff_placeofstudy,
                Name = entity.Name,
                Transactioncurrencyid = entity.TransactionCurrencyId?.Id.ToString(),
                Description = entity.Description,
            };
        }
        public static Opportunity MapFromDTO(OpportunityDTO dot, Xrm context)
        {
            var ParentContactId = dot.Parentcontactid != null ? context.ContactSet.Where(c => c.ff_membershipnumber == dot.Parentcontactid).FirstOrDefault()?.Id : null;
            var Customerid = dot.Customerid != null ? context.AccountSet.Where(a => a.AccountNumber == dot.Customerid).FirstOrDefault()?.Id : null;
            return new Opportunity
            {
                Id = Guid.Parse(dot.Id),
                OverriddenCreatedOn = dot.CreatedOn,
                OwnerId = new EntityReference(dot.Lookuplogicalname, Guid.Parse(dot.OwnerId)),
    //Maybe cannot be set to other than open            StateCode = dot.StateCode != null ? (OpportunityState)Enum.Parse(typeof(OpportunityState), dot.StateCode) : OpportunityState.Open,
                StatusCode = dot.StatusCode != null ? (Opportunity_StatusCode)Enum.Parse(typeof(Opportunity_StatusCode), dot.StatusCode) : Opportunity_StatusCode.InProgress,                
                //Modified on, modified by and created by cant be set

                // Map other properties as needed
                CampaignId = dot.Campaignid != null ? new EntityReference("campaign", Guid.Parse(dot.Campaignid)) : null,
                ff_campaignactivity = dot.Ff_campaignactivity != null ? new EntityReference("ff_campaignactivity", Guid.Parse(dot.Ff_campaignactivity)) : null,
                ff_company = dot.Ff_company,
//                ContactId = dot.Contactid != null ? new EntityReference("contact", Guid.Parse(dot.Contactid)) : null, tells to use parentContact id instead
                ParentContactId = dot.Parentcontactid != null ? new EntityReference("contact", (Guid)ParentContactId) : null,
//                ff_personalidentificationnumber = dot.Ff_personalidentificationnumber, //Auto filled from contact person
                EmailAddress = dot.Emailaddress,
                ff_followupdate = dot.Ff_followupdate,
                ff_preferredlanguage = dot.Ff_preferredlanguage,
                ff_giftcard = dot.Ff_giftcard != null ? new EntityReference("ff_giftcard", Guid.Parse(dot.Ff_giftcard)) : null,
                ff_interactiondate = dot.Ff_interactiondate,
                ff_enrollmenttype = (Opportunity_ff_enrollmenttype)dot.Ff_enrollmenttype,
                ff_jobtitle = dot.Ff_jobtitle,
                OriginatingLeadId = dot.Originatingleadid != null ? new EntityReference("lead", Guid.Parse(dot.Originatingleadid)) : null,
// TODO cannot assign                OwningBusinessUnit = dot.Owningbusinessunit != null ? new EntityReference("businessunit", Guid.Parse(dot.Owningbusinessunit)) : null,
// TODO cannot assign                OwningUser = dot.Owninguser != null ? new EntityReference("systemuser", Guid.Parse(dot.Owninguser)) : null,
                CustomerId = dot.Customerid != null ? new EntityReference("account", (Guid)Customerid) : null,
                ff_referrer = dot.Ff_referrer != null ? new EntityReference("ff_referrer", Guid.Parse(dot.Ff_referrer)) : null,
                ff_salesscore = dot.Ff_salesscore,
                ff_specifiedindustry = dot.Ff_specifiedindustry,
                StateCode = (OpportunityState)dot.Statecode,
                //StatusCode = (Opportunity_StatusCode)dot.Statuscode,
                ff_studyprogram = dot.Ff_studyprogram,
                ff_placeofstudy = dot.Ff_placeofstudy,
                Name = dot.Name,
                TransactionCurrencyId = dot.Transactioncurrencyid != null ? new EntityReference("transactioncurrency", Guid.Parse(dot.Transactioncurrencyid)) : null,
                Description = dot.Description,
            };
        }

        /**
         * //Default
         * 
         * //Core
         * -
         * //Hverve
         * campaignid
         * ff_campaignactivity
         * ff_company
         * contactid
         * parentcontactid
         * ff_personalidentificationnumber
         * emailaddress
         * ff_followupdate
         * ff_preferredlanguage
         * ff_giftcard
         * ff_interactiondate
         * ff_enrollmenttype
         * ff_jobtitle
         * originatingleadid
         * owningbusinessunit
         * owninguser
         * customerid
         * ff_referrer
         * ff_salesscore
         * ff_specifiedindustry
         * statecode
         * statuscode
         * ff_studyprogram
         * ff_placeofstudy
         * name
         * transactioncurrencyid
         * 
         * //Jura
         * -
         * //UI
         * description
         **/
        public class OpportunityDTO : DTO
        {
            public string Campaignid { get; set; }
            public string Ff_campaignactivity { get; set; }
            public string Ff_company { get; set; }
            public string Contactid { get; set; }
            public string Parentcontactid { get; set; }
            public string Ff_personalidentificationnumber { get; set; }
            public string Emailaddress { get; set; }
            public DateTime? Ff_followupdate { get; set; }
            public bool? Ff_preferredlanguage { get; set; }
            public string Ff_giftcard { get; set; }
            public DateTime? Ff_interactiondate { get; set; }
            public int Ff_enrollmenttype { get; set; }
            public string Ff_jobtitle { get; set; }
            public string Originatingleadid { get; set; }
            public string Owningbusinessunit { get; set; }
            public string Owninguser { get; set; }
            public string Customerid { get; set; }
            public string Ff_referrer { get; set; }
            public int? Ff_salesscore { get; set; }
            public string Ff_specifiedindustry { get; set; }
            public int Statecode { get; set; }
            public int Statuscode { get; set; }
            public string Ff_studyprogram { get; set; }
            public string Ff_placeofstudy { get; set; }
            public string Name { get; set; }
            public string Transactioncurrencyid { get; set; }
            public string Description { get; set; }
        }
    }
}
