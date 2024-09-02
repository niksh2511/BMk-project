using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.ViewModels
{
    public class ViewBmkMemberMeeting
    {
        public string BmkMemberMeetingId { get; set; }
        public string MemberName { get; set; }
        public string MeetingUrl { get; set; }
        public IFormFile MemberProfile { get; set; }
    }
}
