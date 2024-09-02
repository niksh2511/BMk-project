using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("event")]
public partial class Event
{
    [Key]
    [Column("eventID")]
    public int EventId { get; set; }

    [Required]
    [Column("eventName")]
    [StringLength(100)]
    [Unicode(false)]
    public string EventName { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("location")]
    [StringLength(100)]
    public string Location { get; set; }

    [Column("allDayEvent")]
    public bool? AllDayEvent { get; set; }

    [Column("startDate", TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column("endDate", TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [Column("timeZone")]
    [StringLength(100)]
    public string TimeZone { get; set; }

    [Column("meetingLink")]
    public string MeetingLink { get; set; }

    [Column("sentInvitation")]
    public bool? SentInvitation { get; set; }

    [Column("active")]
    public bool? Active { get; set; }

    [Column("createdBy")]
    public int? CreatedBy { get; set; }

    [Column("createdDate", TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column("modifiedBy")]
    public int? ModifiedBy { get; set; }

    [Column("modifiedDate", TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [InverseProperty("Event")]
    public virtual ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();
}
