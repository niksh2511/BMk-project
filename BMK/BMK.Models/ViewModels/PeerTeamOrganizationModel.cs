using BMK.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.ViewModels
{
    public class PeerTeamOrganizationModel
    {
        public int organizationID { get; set; }
        public string organizationName { get; set; }
        public int staffSize { get; set; }
        public string PSA { get; set; }
        public string RMM { get; set; }
        public string otherServices { get; set; }
        public IEnumerable<VOrganizationSalary> Salaries { get; set; }

    }
}
