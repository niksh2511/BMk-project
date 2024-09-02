using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.ViewModels
{
    public class MonthlyFinancialRecord
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public bool isImported { get; set; }
        public List<ParentCategory> parentCategory { get; set; }
    }

    public class ParentCategory
    {
        public int parentCategoryId { get; set; }
        public string parentCategoryName { get; set; }
        public string parentCategoryDesc { get; set; }
        public List<Section> sections { get; set; }
    }
    public class Section
    {
        public int sectionId { get; set; }
        public List<ChildCategory> childCategory { get; set; }
    }

    public class ChildCategory
    {
        public int childCategoryId { get; set; }
        public int qbMapMonthlyFinRecordId { get; set; }
        public string childCategoryName { get; set; }
        public double amount { get; set; }
    }

    
}
