using System;
using System.Collections.Generic;
using System.Linq;
using DG.XrmFramework.BusinessDomain.ServiceContext;
using ConsoleJobs.DataRestoration.Emun;
using Export.Mappers;
using Microsoft.Xrm.Sdk;

namespace DataRestoration.Mapper
{
    public static class IncidentMapper
    {
        /** TODO
         * //Default
         * emailaddress
         * parentcaseid // lookup
         * 
         * //Core
         * ff_collectiveagreement
         * ff_deactivatedon
         * ff_queue
         * 
         * //Hverve
         * -
         * 
         * //Jura
         * AccountId
         * ff_formofemployment
         * Description
         * CustomerId
         * ff_deactivatedon
         * ff_finduslink
         * ff_finduscaseworker
         * ff_sharedcase
         * ff_retryspfoldercreation // Do not map
         * ff_category //lookup Kategori
         * PrimaryContactId // lookup
         * ff_member // lookup
         * FollowupBy
         * ResolveBy
         * ff_casemanager // lookup
         * TicketNumber //Autonumber
         * Title
         * ff_casetype // lookup
         * ff_sharepointstatus
         * ff_sharepointurl
         * ff_masterdata 
         * ff_subcategory // lookup Kategori
         * ff_substatus //Choice
         * ff_company // Lookup
         * ff_companywithinff
         * ff_companynamefreetext
         * ff_preferredmail // Deprecated - do not map
         * 
         **/
        public static IncidentDTO MapToDTO(Incident entity, List<DTO> children, Xrm context, Dictionary<string, List<string>> relations)
        {

            var CustomerId = entity.CustomerId != null ? (context.ContactSet.Where(c => c.Id == entity.CustomerId.Id).FirstOrDefault()?.ff_membershipnumber ?? null) : null;
            var PrimaryContactId = entity.PrimaryContactId != null ? (context.ContactSet.Where(c => c.Id == entity.PrimaryContactId.Id).FirstOrDefault()?.ff_membershipnumber ?? null) : null;
            var ff_member = entity.ff_member != null ? (context.ContactSet.Where(c => c.Id == entity.ff_member.Id).FirstOrDefault()?.ff_membershipnumber ?? null) : null;
            var ff_company = entity.ff_company != null ? (context.AccountSet.Where(c => c.Id == entity.ff_company.Id).FirstOrDefault()?.AccountNumber ?? null) : null;
            return new IncidentDTO
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
                Type = EntityType.Incident,
                Children = children,
                Relations = relations,


                // Map other properties as needed
                Title = entity.Title,
                Description = entity.Description,
                ff_collectiveagreement = entity.ff_collectiveagreement?.Id.ToString(),
                ff_deactivatedon = entity.ff_deactivatedon,
                ff_queue = entity.ff_queue?.Id.ToString(),
                AccountId = entity.AccountId?.Id.ToString(),
                ff_formofemployment = entity.ff_formofemployment?.ToString(),
                CustomerId = CustomerId,
                ff_finduslink = entity.ff_finduslink,
                ff_finduscaseworker = entity.ff_finduscaseworker,
                ff_sharedcase = entity.ff_sharedcase,
                ff_category = entity.ff_category?.Id.ToString(),
                PrimaryContactId = PrimaryContactId,
                ff_member = ff_member,
                FollowupBy = entity.FollowupBy,
                ResolveBy = entity.ResolveBy,
                ff_casemanager = entity.ff_casemanager?.Id.ToString(),
                ff_casemanagerLogicalName = entity.ff_casemanager?.LogicalName,
                TicketNumber = entity.TicketNumber,
                ff_casetype = entity.ff_casetype?.Id.ToString(),
                ff_sharepointstatus = entity.ff_sharepointstatus,
                ff_sharepointurl = entity.ff_sharepointurl,
                ff_masterdata = entity.ff_masterdata,
                ff_subcategory = entity.ff_subcategory?.Id.ToString(),
                ff_substatus = entity.ff_substatus.ToString(),
                ff_company = ff_company,
                ff_companywithinff = entity.ff_companywithinff,
                ff_companynamefreetext = entity.ff_companynamefreetext,
                emailaddress = entity.EmailAddress,
                parentcaseid = entity.ParentCaseId?.Id.ToString()
            };
        }
        public static Incident MapFromDTO(IncidentDTO dot, Xrm context)
        {
            var CustomerId = dot.CustomerId != null ? context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == dot.CustomerId)?.Id : null;
            var PrimaryContactId = dot.PrimaryContactId != null ? context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == dot.PrimaryContactId)?.Id : null;
            var ff_member = dot.ff_member != null ? context.ContactSet.FirstOrDefault(c => c.ff_membershipnumber == dot.ff_member)?.Id : null;
            var ff_company = dot.ff_company != null ? context.AccountSet.FirstOrDefault(c => c.AccountNumber == dot.ff_company)?.Id : null;
            var incident = new Incident
            {
                Id = Guid.Parse(dot.Id),
                OverriddenCreatedOn = dot.CreatedOn,
                OwnerId = new EntityReference(dot.Lookuplogicalname, Guid.Parse(dot.OwnerId)),
    //Cannot be other than open - introduce new state?            StateCode = dot.StateCode != null ? (IncidentState)Enum.Parse(typeof(IncidentState), dot.StateCode) : IncidentState.Active,
                //StatusCode = dot.StatusCode != null ? (Incident_StatusCode)Enum.Parse(typeof(Incident_StatusCode), dot.StatusCode) : Incident_StatusCode.Oprettet,
                //Modified on, modified by and created by cant be set

                // Map other properties as needed
                Title = dot.Title,
                Description = dot.Description,
                ff_collectiveagreement = !string.IsNullOrEmpty(dot.ff_collectiveagreement) ? new EntityReference("ff_collectiveagreement", Guid.Parse(dot.ff_collectiveagreement)) : null,
                ff_deactivatedon = dot.ff_deactivatedon,
                ff_queue = !string.IsNullOrEmpty(dot.ff_queue) ? new EntityReference("queue", Guid.Parse(dot.ff_queue)) : null,
   // Read only             AccountId = !string.IsNullOrEmpty(dot.AccountId) ? new EntityReference("account", Guid.Parse(dot.AccountId)) : null,
                CustomerId = CustomerId != null ? new EntityReference("contact", (Guid)CustomerId) : null,
                ff_finduslink = dot.ff_finduslink,
                ff_finduscaseworker = dot.ff_finduscaseworker,
                ff_sharedcase = dot.ff_sharedcase,
                ff_category = !string.IsNullOrEmpty(dot.ff_category) ? new EntityReference("ff_casecategory", Guid.Parse(dot.ff_category)) : null,
                PrimaryContactId = PrimaryContactId != null ? new EntityReference("contact", (Guid)PrimaryContactId) : null,
                ff_member = ff_member != null ? new EntityReference("contact", (Guid)ff_member) : null,
                FollowupBy = dot.FollowupBy,
                ResolveBy = dot.ResolveBy,
                ff_casemanager = !string.IsNullOrEmpty(dot.ff_casemanager) ? new EntityReference(dot.ff_casemanagerLogicalName, Guid.Parse(dot.ff_casemanager)) : null,
                TicketNumber = dot.TicketNumber,
                ff_casetype = !string.IsNullOrEmpty(dot.ff_casetype) ? new EntityReference("ff_casetype", Guid.Parse(dot.ff_casetype)) : null,
                ff_sharepointstatus = dot.ff_sharepointstatus,
                ff_sharepointurl = dot.ff_sharepointurl,
                ff_masterdata = dot.ff_masterdata,
                ff_subcategory = !string.IsNullOrEmpty(dot.ff_subcategory) ? new EntityReference("ff_casecategory", Guid.Parse(dot.ff_subcategory)) : null,
                ff_company = ff_company != null ? new EntityReference("account", (Guid)ff_company) : null,
                ff_companywithinff = dot.ff_companywithinff,
                ff_companynamefreetext = dot.ff_companynamefreetext,
                EmailAddress = dot.emailaddress,
                ParentCaseId = !string.IsNullOrEmpty(dot.parentcaseid) ? new EntityReference("incident", Guid.Parse(dot.parentcaseid)) : null
            };
            if (!string.IsNullOrEmpty(dot.ff_formofemployment))
            {
                incident.ff_formofemployment = (ff_formofemployment)Enum.Parse(typeof(ff_formofemployment), dot.ff_formofemployment);
            }
            if (!string.IsNullOrEmpty(dot.ff_substatus))
            {
                incident.ff_substatus = (ff_casesubstatus)Enum.Parse(typeof(ff_casesubstatus), dot.ff_substatus);
            }
            return incident;
        }
        public class IncidentDTO : DTO
        {
            public string Title { get; set; }
            public string Description { get; set; }            
            public string ff_collectiveagreement { get; set; }
            public DateTime? ff_deactivatedon { get; set; }
            public string ff_queue { get; set; }            
            public string AccountId { get; set; }
            public string ff_formofemployment { get; set; }
            public string CustomerId { get; set; }
            public string ff_finduslink { get; set; }
            public string ff_finduscaseworker { get; set; }
            public bool? ff_sharedcase { get; set; }
            public string ff_category { get; set; }           // Lookup
            public string PrimaryContactId { get; set; }      // Lookup
            public string ff_member { get; set; }             // Lookup
            public DateTime? FollowupBy { get; set; }
            public DateTime? ResolveBy { get; set; }
            public string ff_casemanager { get; set; }        // Lookup
            public string ff_casemanagerLogicalName { get; set; }
            public string TicketNumber { get; set; }          // Autonumber
            public string ff_casetype { get; set; }           // Lookup
            public string ff_sharepointstatus { get; set; }
            public string ff_sharepointurl { get; set; }
            public string ff_masterdata { get; set; }
            public string ff_subcategory { get; set; }        // Lookup
            public string ff_substatus { get; set; }          // Choice
            public string ff_company { get; set; }            // Lookup
            public bool? ff_companywithinff { get; set; }
            public string ff_companynamefreetext { get; set; }
            public string emailaddress { get; set; }
            public string parentcaseid { get; set; } // Lookup
        }
    }
}
