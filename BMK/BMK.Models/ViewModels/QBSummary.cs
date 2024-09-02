using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.ViewModels
{
    public class QBSummary
    {
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public int TotalAccounts { get; set; }
        public int UnmappedAccounts { get; set; }
        public string lastImportReprocessDate { get; set; }
    }

}
