using System;
using System.Collections.Generic;
using ConsoleJobs.DataRestoration.Emun;

namespace Export.Mappers
{
    public class  DTO
    {
        //Base properties
        public string Id { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public string Lookuplogicalname { get; set; } = string.Empty; // This is used to identify the type of entity the owner is, e.g., "systemuser", "account", "team" etc.
        public string OwningBusinessUnit { get; set; } = string.Empty; 
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public string StateCode { get; set; }
        public string StatusCode { get; set; }


        //Relations
        public EntityType Type { get; set; } = EntityType.None;
        public List<DTO> Children { get; set; } = new List<DTO>();
        public Dictionary<string, List<string>> Relations = new Dictionary<string, List<string>>();
    }
}
