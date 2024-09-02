using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("groupTypes")]
public partial class GroupType
{
    [Key]
    [Column("groupTypesID")]
    public int GroupTypesId { get; set; }

    [Column("groupTypeName")]
    [StringLength(100)]
    public string GroupTypeName { get; set; }

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

    [ForeignKey("CreatedBy")]
    [InverseProperty("GroupTypeCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("GroupTypeModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [InverseProperty("GroupTypes")]
    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}
