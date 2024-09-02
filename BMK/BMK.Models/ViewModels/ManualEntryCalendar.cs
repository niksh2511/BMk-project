using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.ViewModels
{
    public class ManualEntryCalendar
    {
        public int Year { get; set; }
        public List<Months> Months { get; set; }
    }
    public class Months
    {
        public int Month { get; set; }
        public int iconType { get; set; }
    }
}
