using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.Models
{
    public class EventCategoryModel
    {
        public int EventID {  get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool? AllDayEvent { get; set; }
        public DateTime? Startdate { get; set; }
        public DateTime? Enddate { get; set; }
        public string Timezone { get; set; }
        public string MeetingLink {  get; set; }
        public bool? SentInvitation { get; set; }
        public bool? Active { get; set; }
        public IEnumerable<CategoryModel> Category { get; set; }
    }
}
