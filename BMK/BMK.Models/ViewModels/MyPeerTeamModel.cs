using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.ViewModels
{
    public class MyPeerTeamModel
    {
        public int userGroupsId { get; set; }
        public string groupName { get; set; }
        public int usersId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string profilePicture { get; set; }
        public int organizationId { get; set; }
        public string organizationName { get; set; }
        public string city { get; set; }
        public int staffSize { get; set; }
        public string psa { get; set; }
        public string rmm { get; set; }
        public string otherTools { get; set; }
        public string lastImport { get; set; }
        
        public List<vOrgRevenue> orgRevenue { get; set; }
    }
    [Keyless]
    public class vOrgRevenue
    {
        public int year { get; set; }
        public int organizationID { get; set; }
        public string amount { get; set; }

        public string months { get; set; }

    }
}
