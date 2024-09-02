using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Infrastructure.Model
{
    public class UserInfo
    {
        public string Email { get; set; }

        public int CountryId { get; set; }
        public int RoleId { get; set; }
        public int OrganizationId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }
    }
}
