using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.Models
{
    public class EventModel
    {
        public int? EventID {  get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool? AllDayEvent { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Timezone { get; set; }
        public List<int> Categories { get; set; }
        public bool? SendInvitation { get; set; }
    }
}
