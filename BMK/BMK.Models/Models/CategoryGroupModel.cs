using BMK.Models.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.Models
{
    public class CategoryGroupModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set;}
        public List<groupTypes> GroupTypes { get; set; }
        public Access Access{ get; set; }
    }
    public class Access
    {
        public int Everyone { get; set; }
        public int AuthenticatedUser { get; set; }
    }
        public class groupTypes
    {
        public int GroupTypesId { get; set; }
        public string GroupTypeName { get; set; }
        public List<CatGroup> UserGroups { get; set; }
    }
    public class CatGroup
    {
        public int Access { get; set; }
        public string GroupName { get; set; }
        public int UserGroupsId { get; set; }
    }
}
