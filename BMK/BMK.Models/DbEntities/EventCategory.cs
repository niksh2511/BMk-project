using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("eventCategories")]
public partial class EventCategory
{
    [Key]
    [Column("eventCategoryID")]
    public int EventCategoryId { get; set; }

    [Column("eventID")]
    public int? EventId { get; set; }

    [Column("categoryID")]
    public int? CategoryId { get; set; }

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

    [ForeignKey("CategoryId")]
    [InverseProperty("EventCategories")]
    public virtual Category Category { get; set; }

    [ForeignKey("EventId")]
    [InverseProperty("EventCategories")]
    public virtual Event Event { get; set; }
}
