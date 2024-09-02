using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("category")]
public partial class Category
{
    [Key]
    [Column("categoryID")]
    public int CategoryId { get; set; }

    [Column("categoryName")]
    [StringLength(100)]
    [Unicode(false)]
    public string CategoryName { get; set; }

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

    public bool? IsSendNotification { get; set; }

    [InverseProperty("Catgeory")]
    public virtual ICollection<CategoryGroup> CategoryGroups { get; set; } = new List<CategoryGroup>();

    [ForeignKey("CreatedBy")]
    [InverseProperty("Categories")]
    public virtual User CreatedByNavigation { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();
}
