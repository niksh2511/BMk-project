using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BMK.Models.DbEntities
{
    public partial class BmkMemberMeeting
    {
        [NotMapped]
        public bool IsEditable { get; set; }
    }
}
