using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("appObjects")]
public partial class AppObject
{
    [Key]
    [Column("appObjectsID")]
    public int AppObjectsId { get; set; }

    [Column("objCategory")]
    [StringLength(100)]
    [Unicode(false)]
    public string ObjCategory { get; set; }

    [Column("objType")]
    [StringLength(100)]
    [Unicode(false)]
    public string ObjType { get; set; }

    [Column("objValue")]
    [StringLength(100)]
    [Unicode(false)]
    public string ObjValue { get; set; }

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

    [InverseProperty("AccessNavigation")]
    public virtual ICollection<CategoryGroup> CategoryGroups { get; set; } = new List<CategoryGroup>();

    [ForeignKey("CreatedBy")]
    [InverseProperty("AppObjectCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("AppObjectModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [InverseProperty("StatusNavigation")]
    public virtual ICollection<Objective> Objectives { get; set; } = new List<Objective>();
}
