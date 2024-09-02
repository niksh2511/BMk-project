using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.Models
{
    public class CategoryModel
    {
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
        public bool? IsSendNotification { get; set; }
    }
}
