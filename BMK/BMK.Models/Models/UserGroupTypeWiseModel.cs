using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.Models
{
    public  class UserGroupTypeWiseModel
    {
        public int GroupTypesId { get; set; }
        public string GroupName { get; set; }
        public string UserGroupsId { get; set; }
        public bool Active { get; set; }
    }
}
