using System;
using System.Collections.Generic;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using ConsoleJobs.DataRestoration.Emun;
using Export.Mappers;
using Microsoft.Xrm.Sdk;

namespace DataRestoration.Mapper
{
    public static class LeadMapper
    {
        /** TODO
         * //Default
         * TODO
         * 
         * //Core
         * -
         * 
         * //Hverve
         * firstname
         * lastname
         * telephone2
         * telephone1
         * mobilephone
         * emailaddress1
         * emailaddress2
         * ff_preferredlanguage (optionset)
         * ff_companyname
         * jobtitle
         * ff_specifiedindustry
         * ff_placeofstudy
         * ff_studyprogram
         * description
         * preferredcontactmethodcode (optionset)
         * donotemail (optionset)
         * donotphone (optionset)
         * donotbulkemail (optionset)
         * campaignid
         * ff_refererer
         * ff_referererstring
         * parentcontactid
         * qualifyingopportunityid
         * ff_automaticdisqualificationbool
         * ff_automaticdisqualificationreasontext
         * ff_salesscore
         * 
         * //Jura
         * -
         **/
        public static LeadDTO MapToDTO(Lead entity, List<DTO> children, Xrm context, Dictionary<string, List<string>> relations)
        {
            var ff_refererer = entity.ff_refererer != null ? context.ContactSet.Where(c => c.Id == entity.ff_refererer.Id).FirstOrDefault()?.ff_membershipnumber : null;
            var ParentContactId = entity.ParentContactId != null ? context.ContactSet.Where(c => c.Id == entity.ParentContactId.Id).FirstOrDefault()?.ff_membershipnumber : null;
            return new LeadDTO
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
                Type = EntityType.Lead,
                Children = children,
                Relations = relations,

                // Map other properties as needed
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                PrivatePhone = entity.Telephone2,
                WorkPhone = entity.Telephone1,
                MobilePhone = entity.MobilePhone,
                PrivateEmail = entity.EMailAddress1,
                WorkEmail = entity.EMailAddress2,
                PreferredLanguage = entity.ff_preferredlanguage,
                Company = entity.ff_companyname,
                Jobtitle = entity.JobTitle,
                SpecifiedIndustry = entity.ff_specifiedindustry,
                PlaceOfStudy = entity.ff_placeofstudy,
                StudyProgram = entity.ff_studyprogram,
                Description = entity.Description,
                PreferredContactMethodCode = entity.PreferredContactMethodCode?.ToString(),
                DoNotEmail = entity.DoNotEMail,
                DoNotPhone = entity.DoNotPhone,
                DoNotBulkEmail = entity.DoNotBulkEMail,
                CampaignId = entity.CampaignId?.Id.ToString(),
                CampaignActivityId = entity.ff_campaignactivity?.Id.ToString(),
                Refererer = ff_refererer,
                ReferererString = entity.ff_referererstring,
                ParentContactId = ParentContactId,
                QualifyingOpportunityId = entity.QualifyingOpportunityId?.Id.ToString(),
                AutomaticDisqualification = entity.ff_automaticdisqualificationbool,
                AutomaticDisqualificationReason = entity.ff_automaticdisqualificationreasontext,
                SalesScore = entity.ff_salesscore

            };
        }
        public static Lead MapFromDTO(LeadDTO dot, Xrm context)
        {
            var Refererer = !string.IsNullOrEmpty(dot.Refererer) ? context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == dot.Refererer)?.Id : null;
            var ParentContactId = !string.IsNullOrEmpty(dot.ParentContactId) ? context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == dot.ParentContactId)?.Id : null;
            return new Lead
            {
                Id = Guid.Parse(dot.Id),
                OverriddenCreatedOn = dot.CreatedOn,
                OwnerId = new EntityReference(dot.Lookuplogicalname, Guid.Parse(dot.OwnerId)),
                StateCode = dot.StateCode != null ? (LeadState)Enum.Parse(typeof(LeadState), dot.StateCode) : LeadState.Open,
                StatusCode = dot.StatusCode != null ? (Lead_StatusCode)Enum.Parse(typeof(Lead_StatusCode), dot.StatusCode) : Lead_StatusCode.New,
                //Modified on, modified by and created by cant be set
                
                // Map other properties as needed
                FirstName = dot.FirstName,                  
                LastName = dot.LastName,
                Telephone2 = dot.PrivatePhone,
                Telephone1 = dot.WorkPhone,
                MobilePhone = dot.MobilePhone,
                EMailAddress1 = dot.PrivateEmail,
                EMailAddress2 = dot.WorkEmail,
                ff_preferredlanguage = dot.PreferredLanguage,
                ff_companyname = dot.Company,
                JobTitle = dot.Jobtitle,
                ff_specifiedindustry = dot.SpecifiedIndustry,
                ff_placeofstudy = dot.PlaceOfStudy,
                ff_studyprogram = dot.StudyProgram,
                Description = dot.Description,
                PreferredContactMethodCode = dot.PreferredContactMethodCode != null ? (Lead_PreferredContactMethodCode)Enum.Parse(typeof(Lead_PreferredContactMethodCode), dot.PreferredContactMethodCode) : Lead_PreferredContactMethodCode.Any,                
                DoNotEMail = dot.DoNotEmail,
                DoNotPhone = dot.DoNotPhone,
                DoNotBulkEMail = dot.DoNotBulkEmail,
                CampaignId = !string.IsNullOrEmpty(dot.CampaignId) ? new EntityReference("campaign", Guid.Parse(dot.CampaignId)) : null,
                ff_campaignactivity = !string.IsNullOrEmpty(dot.CampaignActivityId) ? new EntityReference("campaignactivity", Guid.Parse(dot.CampaignActivityId)) : null,
                ff_refererer = Refererer != null ? new EntityReference("contact",(Guid)Refererer) : null,
                ff_referererstring = dot.ReferererString,
                ParentContactId = ParentContactId != null ? new EntityReference("contact", (Guid)ParentContactId) : null,
                QualifyingOpportunityId = !string.IsNullOrEmpty(dot.QualifyingOpportunityId) ? new EntityReference("opportunity", Guid.Parse(dot.QualifyingOpportunityId)) : null,
                ff_automaticdisqualificationbool = dot.AutomaticDisqualification,
                ff_automaticdisqualificationreasontext = dot.AutomaticDisqualificationReason,
                ff_salesscore = dot.SalesScore

            };
        }
        public class LeadDTO : DTO
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PrivatePhone { get; set; }
            public string WorkPhone { get; set; }
            public string MobilePhone { get; set; }
            public string PrivateEmail { get; set; }
            public string WorkEmail { get; set; }
            public string Company { get; set; }
            public string Jobtitle { get; set; }
            public string SpecifiedIndustry { get; set; }
            public string PlaceOfStudy { get; set; }
            public string StudyProgram { get; set; }
            public string Description { get; set; }
            public bool? PreferredLanguage { get; set; }
            public string PreferredContactMethodCode { get; set; }
            public bool? DoNotEmail { get; set; }
            public bool? DoNotPhone { get; set; }
            public bool? DoNotBulkEmail { get; set; }
            public string CampaignId { get; set; }
            public string CampaignActivityId { get; set; }           
            public string Refererer { get; set; }
            public string ReferererString { get; set; }
            public string ParentContactId { get; set; }
            public string QualifyingOpportunityId { get; set; }
            public bool? AutomaticDisqualification { get; set; }
            public string AutomaticDisqualificationReason { get; set; }
            public int? SalesScore { get; set; }
        }

    }
}
