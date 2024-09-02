using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.ViewModels
{
    public class AccountMapping
    {
        public int AccountId { get; set; }
        public string Type { get; set; }
        public string AccountNumber { get; set; }
        public int OrganizationId { get; set; }
        public string AccountDescription { get; set; }
        public bool MapRatioFlag { get; set; }
        public string MapNotes { get; set; }
        public IEnumerable<MappingRatio> Mappings { get; set; }
    }

    public class MappingRatio
    {
        public int MappingId { get; set; }
        public decimal MapRatio { get; set; }
        public int OrgAccountMappingId { get; set; }
    }
}
