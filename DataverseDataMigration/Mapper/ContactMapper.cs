using System;
using System.Collections.Generic;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using ConsoleJobs.DataRestoration.Emun;
using Export.Mappers;
using Microsoft.Xrm.Sdk;

namespace DataRestoration.Mapper
{
    public static class ContactMapper
    {
        /** TODO
         * //Default
         * jobtitle 
         * 
         * //Core
         * accountid
         * ff_age
         * emailaddress2
         * ff_personalidentificationnumber
         * ff_hasactivepositionsoftrust
         * educationcode
         * ff_educationlevel
         * ff_enrollmentdate
         * ff_workingenvironmentrepresentative
         * ff_membershipnumber
         * mobilephone
         * ff_numberofactivepositionsoftrust // Rollup do not map
         * ff_placeofeducation
         * ff_primarymail
         * emailaddress1
         * ff_provisioningstatus
         * ff_withdrawaldate
         * ff_sharepointurl
         * statuscode
         * telephone1
         * telephone2
         * ff_disenrollmentreason
         * parentcustomerid
         * ff_positionoftrust // Deprecated - do not map
         * ff_preferredemail // Deprecated - do not map
         * ff_ff_masterdata // Deprecated - do not map
         * ff_masterdata // Deprecated - do not map
         * ff_reasonfortermination // Deprecated - do not map
         * 
         * //Hverve
         * ff_numberofopportunitieslinked // Rollup do not map
         * emailaddress2
         * ff_unemploymentinsurancestatus
         * ff_areyourecentlygraduated
         * ff_alreadymember
         * ff_campaign
         * ff_campaignactivity
         * ff_companystring
         * ff_firm
         * ff_companyaddress
         * ff_consentthirdparty
         * ff_quotagroup
         * ff_quotaend
         * ff_quotastart
         * ff_personalidentificationnumber
         * ff_isPayrollDeductionPreferred
         * ff_eboks
         * EducationCode
         * ff_educationlevel
         * PreferredContactMethodCode
         * GenderCode
         * ff_graduatedate
         * ff_priorunemploymentfundmembership
         * ff_protectingyourrights
         * ff_iwhichtobe
         * ff_informationaboutmemberoffer
         * ff_worksituation
         * ff_enrollunemploymentfund
         * ff_leaveotherunion
         * ff_membershipnumber
         * MobilePhone
         * OriginatingLeadId
         * ff_formofpayment
         * ff_placeofeducation
         * ff_companyzipcode
         * ff_preferredlanguage
         * emailaddress1
         * ff_residenceindenmarkstudystart
         * ff_withdrawaldate
         * ff_switchunion
         * statuscode
         * Telephone1
         * Telephone2
         * ff_disenrollmentreason
         * ff_akasse
         * parentcustomerid
         * ff_whichunemploymentfund
         * ff_unionselection
         * ff_workarea
         * ff_company // Deprecated - do not map
         * ff_englishspeaking // Deprecated - do not map
         * ff_worksitutation // Deprecated - do not map
         * ff_jobsituation // Deprecated - do not map
         * ff_trygmeetingbooking // Deprecated - do not map
         * ff_reasonfortermination // Deprecated - do not map
         * 
         * //Jura
         * ff_age
         * ff_hasactivepositionsoftrust
         * ff_numberofactivepositionsoftrust
         * ff_provisioningstatus
         * ff_sharepointurl
         * statuscode
         * ff_caseparty // Deprecated - do not map
         * ff_Sagspartfirma // Deprecated - do not map
         * ff_numberofactivecases // Deprecated - do not map
         **/
        public static ContactDTO MapToDTO(Contact entity, List<DTO> children, Xrm context, Dictionary<string, List<string>> relations)
        {
            //Fetch ff_membershipnumber from entity.ff_workingenvironmentrepresentative to be able to lookup on import
            
            var ff_workingenvironmentrepresentative = entity.ff_workingenvironmentrepresentative != null ? (context.ContactSet.Where(c => c.Id == entity.ff_workingenvironmentrepresentative.Id).FirstOrDefault()?.ff_membershipnumber ?? null) : null;
            var parentcustomerid = entity.ParentCustomerId != null ? context.AccountSet.Where(a => a.Id == entity.ParentCustomerId.Id).FirstOrDefault()?.AccountNumber : null;
            var ff_firm = entity.ff_firm != null ? context.AccountSet.Where(a => a.Id == entity.ff_firm.Id).FirstOrDefault()?.AccountNumber : null;
            return new ContactDTO
            {
                Id = entity.Id.ToString(),
                OwnerId = entity.OwnerId?.Id.ToString(),
                Lookuplogicalname = entity.OwnerId?.LogicalName,
                CreatedOn = entity.CreatedOn,
                CreatedBy = entity.CreatedBy?.Id.ToString(),
                ModifiedOn = entity.ModifiedOn,
                ModifiedBy = entity.ModifiedBy?.Id.ToString(),
                StateCode = entity.StateCode.ToString(),
                StatusCode = entity.StatusCode.ToString(),

                //Relations
                Type = EntityType.Contact,
                Children = children,
                Relations = relations,

                // Map other properties as needed
                AccountId = entity.AccountId?.Id.ToString(),
                ff_age = entity.ff_age ?? 0,
                emailaddress2 = entity.EMailAddress2,
                ff_personalidentificationnumber = entity.ff_personalidentificationnumber,
                ff_hasactivepositionsoftrust = entity.ff_hasactivepositionsoftrust,                
                ff_educationlevel = entity.ff_educationlevel,
                ff_enrollmentdate = entity.ff_enrollmentdate,
                ff_workingenvironmentrepresentative = ff_workingenvironmentrepresentative,
                ff_membershipnumber = entity.ff_membershipnumber,
                mobilephone = entity.MobilePhone,                
                ff_placeofeducation = entity.ff_placeofeducation?.Id.ToString(),
                ff_primarymail = entity.ff_primarymail,
                emailaddress1 = entity.EMailAddress1,
                ff_provisioningstatus = entity.ff_provisioningstatus?.ToString(),
                ff_withdrawaldate = entity.ff_withdrawaldate,
                ff_sharepointurl = entity.ff_sharepointurl,
                telephone1 = entity.Telephone1,
                telephone2 = entity.Telephone2,
                ff_disenrollmentreason = entity.ff_disenrollmentreason?.Id.ToString(),
                parentcustomerid = parentcustomerid,
                ff_unemploymentinsurancestatus = entity.ff_unemploymentinsurancestatus?.ToString(),
                ff_areyourecentlygraduated = entity.ff_areyourecentlygraduated,
                ff_alreadymember = entity.ff_alreadymember,
                ff_campaign = entity.ff_campaign?.Id.ToString(),
                ff_campaignactivity = entity.ff_campaignactivity?.Id.ToString(),
                ff_companystring = entity.ff_companystring,
                ff_firm = ff_firm,
                ff_companyaddress = entity.ff_companyaddress,
                ff_consentthirdparty = entity.ff_consentthirdparty,
                ff_quotagroup = entity.ff_quotagroup?.ToString(),
                ff_quotaend = entity.ff_quotaend,
                ff_quotastart = entity.ff_quotastart,
                ff_isPayrollDeductionPreferred = entity.ff_isPayrollDeductionPreferred,
                ff_eboks = entity.ff_eboks,
                EducationCode = entity.EducationCode?.ToString(),
                PreferredContactMethodCode = entity.PreferredContactMethodCode?.ToString(),
                GenderCode = entity.GenderCode?.ToString(),
                ff_graduatedate = entity.ff_graduatedate,
                ff_priorunemploymentfundmembership = entity.ff_priorunemploymentfundmembership,
                ff_protectingyourrights = entity.ff_protectingyourrights,
                ff_iwhichtobe = entity.ff_iwhichtobe,
                ff_informationaboutmemberoffer = entity.ff_informationaboutmemberoffer,
                ff_worksituation = entity.ff_worksituation?.ToString(),
                ff_enrollunemploymentfund = entity.ff_enrollunemploymentfund,
                ff_leaveotherunion = entity.ff_leaveotherunion,
                MobilePhone = entity.MobilePhone,
                OriginatingLeadId = entity.OriginatingLeadId?.Id.ToString(),
                ff_formofpayment = entity.ff_formofpayment?.ToString(),
                ff_companyzipcode = entity.ff_companyzipcode,
                ff_preferredlanguage = entity.ff_preferredlanguage,
                ff_residenceindenmarkstudystart = entity.ff_residenceindenmarkstudystart,
                ff_switchunion = entity.ff_switchunion,
                ff_akasse = entity.ff_akasse,
                ff_whichunemploymentfund = entity.ff_whichunemploymentfund?.ToString(),
                ff_unionselection = entity.ff_unionselection?.ToString(),
                ff_workarea = entity.ff_workarea?.ToString()

            };
        }
        public static Contact MapFromDTO(ContactDTO dot, Xrm context)
        {
            var ff_workingenvironmentrepresentative = !string.IsNullOrEmpty(dot.ff_workingenvironmentrepresentative) ? context.ContactSet.Where(c => c.ff_membershipnumber == dot.ff_workingenvironmentrepresentative).FirstOrDefault()?.Id : null;
            var parentcustomerid = !string.IsNullOrEmpty(dot.parentcustomerid) ? context.AccountSet.Where(a => a.AccountNumber == dot.parentcustomerid).FirstOrDefault()?.Id : null;
            var ff_firm = !string.IsNullOrEmpty(dot.ff_firm) ? context.AccountSet.Where(a => a.AccountNumber == dot.ff_firm).FirstOrDefault()?.Id : null;
            return new Contact
            {
                Id = Guid.Parse(dot.Id),
                OverriddenCreatedOn = dot.CreatedOn,
                OwnerId = new Microsoft.Xrm.Sdk.EntityReference(dot.Lookuplogicalname, Guid.Parse(dot.OwnerId)),
                StateCode = dot.StateCode != null ? (ContactState)Enum.Parse(typeof(ContactState), dot.StateCode) : ContactState.Active,
                StatusCode = dot.StatusCode != null ? (Contact_StatusCode)Enum.Parse(typeof(Contact_StatusCode), dot.StatusCode) : Contact_StatusCode.Active,


                //Modified on, modified by and created by cant be set
                // Map other properties as needed
     // Read only           AccountId = !string.IsNullOrEmpty(dot.AccountId) ? new EntityReference("account", Guid.Parse(dot.AccountId)) : null,
                ff_age = dot.ff_age,
                EMailAddress2 = dot.emailaddress2,
                ff_personalidentificationnumber = dot.ff_personalidentificationnumber,
                ff_hasactivepositionsoftrust = dot.ff_hasactivepositionsoftrust,
                EducationCode = dot.EducationCode != null ? (Contact_EducationCode)Enum.Parse(typeof(Contact_EducationCode), dot.EducationCode) : Contact_EducationCode.Standardværdi,
                ff_educationlevel = dot.ff_educationlevel,
                ff_enrollmentdate = dot.ff_enrollmentdate,
                ff_workingenvironmentrepresentative = ff_workingenvironmentrepresentative != null ? new EntityReference("contact",(Guid)ff_workingenvironmentrepresentative) : null,
                ff_membershipnumber = dot.ff_membershipnumber,
                MobilePhone = dot.mobilephone,
     // Read only           ff_numberofactivepositionsoftrust = dot.ff_numberofactivepositionsoftrust,
                ff_placeofeducation = !string.IsNullOrEmpty(dot.ff_placeofeducation) ? new EntityReference("ff_placeofeducation", Guid.Parse(dot.ff_placeofeducation)) : null,
                ff_primarymail = dot.ff_primarymail,
                EMailAddress1 = dot.emailaddress1,
                ff_provisioningstatus = dot.ff_provisioningstatus != null ? (Contact_ff_provisioningstatus)Enum.Parse(typeof(Contact_ff_provisioningstatus), dot.ff_provisioningstatus) : Contact_ff_provisioningstatus.Completed,
                ff_withdrawaldate = dot.ff_withdrawaldate,
                ff_sharepointurl = dot.ff_sharepointurl,
                Telephone1 = dot.telephone1,
                Telephone2 = dot.telephone2,
                ff_disenrollmentreason = !string.IsNullOrEmpty(dot.ff_disenrollmentreason) ? new EntityReference("ff_disenrollmentreason", Guid.Parse(dot.ff_disenrollmentreason)) : null,
                ParentCustomerId = parentcustomerid != null ? new EntityReference("account", (Guid)parentcustomerid) : null,
                ff_unemploymentinsurancestatus = dot.ff_unemploymentinsurancestatus != null ? (Contact_ff_unemploymentinsurancestatus)Enum.Parse(typeof(Contact_ff_unemploymentinsurancestatus), dot.ff_unemploymentinsurancestatus) : Contact_ff_unemploymentinsurancestatus.Nej,
                ff_areyourecentlygraduated = dot.ff_areyourecentlygraduated,
                ff_alreadymember = dot.ff_alreadymember,
                ff_campaign = !string.IsNullOrEmpty(dot.ff_campaign) ? new EntityReference("campaign", Guid.Parse(dot.ff_campaign)) : null,
                ff_campaignactivity = !string.IsNullOrEmpty(dot.ff_campaignactivity) ? new EntityReference("campaignactivity", Guid.Parse(dot.ff_campaignactivity)) : null,
                ff_companystring = dot.ff_companystring,
                ff_firm = ff_firm != null ? new EntityReference("account", (Guid)ff_firm) : null,
                ff_companyaddress = dot.ff_companyaddress,
                ff_consentthirdparty = dot.ff_consentthirdparty,
                ff_quotagroup = dot.ff_quotagroup != null ? (Contact_ff_quotagroup)Enum.Parse(typeof(Contact_ff_quotagroup), dot.ff_quotagroup) : Contact_ff_quotagroup.Uoplyst,
                ff_quotaend = dot.ff_quotaend,
                ff_quotastart = dot.ff_quotastart,
                ff_isPayrollDeductionPreferred = dot.ff_isPayrollDeductionPreferred,
                ff_eboks = dot.ff_eboks,                
                PreferredContactMethodCode = !string.IsNullOrEmpty(dot.PreferredContactMethodCode) ? (Contact_PreferredContactMethodCode)Enum.Parse(typeof(Contact_PreferredContactMethodCode), dot.PreferredContactMethodCode) : (Contact_PreferredContactMethodCode?)null,
                GenderCode = !string.IsNullOrEmpty(dot.GenderCode) ? (Contact_GenderCode)Enum.Parse(typeof(Contact_GenderCode), dot.GenderCode) : (Contact_GenderCode?)null,
                ff_graduatedate = dot.ff_graduatedate,
                ff_priorunemploymentfundmembership = dot.ff_priorunemploymentfundmembership,
                ff_protectingyourrights = dot.ff_protectingyourrights,
                ff_iwhichtobe = dot.ff_iwhichtobe,
                ff_informationaboutmemberoffer = dot.ff_informationaboutmemberoffer,
                ff_worksituation = dot.ff_worksituation != null ? (Contact_ff_worksituation)Enum.Parse(typeof(Contact_ff_worksituation), dot.ff_worksituation) : Contact_ff_worksituation.Uoplyst,
                ff_enrollunemploymentfund = dot.ff_enrollunemploymentfund,
                ff_leaveotherunion = dot.ff_leaveotherunion,
                OriginatingLeadId = !string.IsNullOrEmpty(dot.OriginatingLeadId) ? new EntityReference("lead", Guid.Parse(dot.OriginatingLeadId)) : null,
                ff_formofpayment = dot.ff_formofpayment != null ? (Contact_ff_formofpayment)Enum.Parse(typeof(Contact_ff_formofpayment), dot.ff_formofpayment) : Contact_ff_formofpayment.Løntræk,
                ff_companyzipcode = dot.ff_companyzipcode,
                ff_preferredlanguage = dot.ff_preferredlanguage,
                ff_residenceindenmarkstudystart = dot.ff_residenceindenmarkstudystart,
                ff_switchunion = dot.ff_switchunion,
                ff_akasse = dot.ff_akasse,
                ff_whichunemploymentfund = dot.ff_whichunemploymentfund != null ? (Contact_ff_whichunemploymentfund)Enum.Parse(typeof(Contact_ff_whichunemploymentfund), dot.ff_whichunemploymentfund) : Contact_ff_whichunemploymentfund.AkademikernesAkasse,
                ff_unionselection = dot.ff_unionselection != null ? (Contact_ff_unionselection)Enum.Parse(typeof(Contact_ff_unionselection), dot.ff_unionselection) : Contact_ff_unionselection.ASE,
                ff_workarea = dot.ff_workarea != null ? (Contact_ff_workarea)Enum.Parse(typeof(Contact_ff_workarea), dot.ff_workarea) : Contact_ff_workarea.Rådgivningogkundekontakt
            };
        }

        
        public class ContactDTO : DTO
        {            
            public string AccountId { get; set; }
            public decimal ff_age { get; set; }
            public string emailaddress2 { get; set; }
            public string ff_personalidentificationnumber { get; set; }               
            public bool? ff_hasactivepositionsoftrust { get; set; }            
            public string ff_educationlevel { get; set; }
            public DateTime? ff_enrollmentdate { get; set; }
            public string ff_workingenvironmentrepresentative { get; set; }
            public string ff_membershipnumber { get; set; }
            public string mobilephone { get; set; }
            public int? ff_numberofactivepositionsoftrust { get; set; }
            public string ff_placeofeducation { get; set; }
            public bool? ff_primarymail { get; set; }
            public string emailaddress1 { get; set; }
            public string ff_provisioningstatus { get; set; }
            public DateTime? ff_withdrawaldate { get; set; }
            public string ff_sharepointurl { get; set; }
            public int? statuscode { get; set; }
            public string telephone1 { get; set; }
            public string telephone2 { get; set; }
            public string ff_disenrollmentreason { get; set; }
            public string parentcustomerid { get; set; }
            public string ff_unemploymentinsurancestatus { get; set; }
            public bool? ff_areyourecentlygraduated { get; set; }
            public bool? ff_alreadymember { get; set; }
            public string ff_campaign { get; set; }
            public string ff_campaignactivity { get; set; }
            public string ff_companystring { get; set; }
            public string ff_firm { get; set; }
            public string ff_companyaddress { get; set; }
            public bool? ff_consentthirdparty { get; set; }
            public string ff_quotagroup { get; set; }
            public DateTime? ff_quotaend { get; set; }
            public DateTime? ff_quotastart { get; set; }
            public bool? ff_isPayrollDeductionPreferred { get; set; }
            public bool? ff_eboks { get; set; }
            public string EducationCode { get; set; }
            public string PreferredContactMethodCode { get; set; }
            public string GenderCode { get; set; }
            public DateTime? ff_graduatedate { get; set; }
            public bool? ff_priorunemploymentfundmembership { get; set; }
            public bool? ff_protectingyourrights { get; set; }
            public bool? ff_iwhichtobe { get; set; }
            public bool? ff_informationaboutmemberoffer { get; set; }
            public string ff_worksituation { get; set; }
            public bool? ff_enrollunemploymentfund { get; set; }
            public bool? ff_leaveotherunion { get; set; }
            public string MobilePhone { get; set; }
            public string OriginatingLeadId { get; set; }
            public string ff_formofpayment { get; set; }
            public string ff_companyzipcode { get; set; }
            public bool? ff_preferredlanguage { get; set; }
            public bool? ff_residenceindenmarkstudystart { get; set; }
            public bool? ff_switchunion { get; set; }
            public bool? ff_akasse { get; set; }
            public string ff_whichunemploymentfund { get; set; }
            public string ff_unionselection { get; set; }
            public string ff_workarea { get; set; }

        }
    }
}
