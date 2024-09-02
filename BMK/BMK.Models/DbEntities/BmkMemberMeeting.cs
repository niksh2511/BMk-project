using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("bmkMemberMeetings")]
public partial class BmkMemberMeeting
{
    [Key]
    [Column("bmkMemberMeetingID")]
    public int BmkMemberMeetingId { get; set; }

    [Required]
    [Column("memberProfile")]
    public string MemberProfile { get; set; }

    [Required]
    [Column("meetingUrl")]
    [StringLength(500)]
    [Unicode(false)]
    public string MeetingUrl { get; set; }

    [Column("createdBy")]
    public int? CreatedBy { get; set; }

    [Column("createdDate", TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column("modifiedBy")]
    public int? ModifiedBy { get; set; }

    [Column("modifiedDate", TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [Column("isActive")]
    public bool? IsActive { get; set; }

    [Required]
    [Column("memberName")]
    [StringLength(200)]
    [Unicode(false)]
    public string MemberName { get; set; }
}
