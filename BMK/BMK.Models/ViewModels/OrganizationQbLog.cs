using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.ViewModels
{
    public class OrganizationQbLog
    {
        public string OrganizationName { get; set; }
        public List<QbLogs> OrganizationLogs { get; set; }
    }

    public class QbLogs
    {
        public string LogInfo { get; set; }
    }
}
