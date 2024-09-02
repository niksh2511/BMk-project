using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("categoryGroups")]
public partial class CategoryGroup
{
    [Key]
    [Column("categoryGroupsID")]
    public int CategoryGroupsId { get; set; }

    [Column("catgeoryId")]
    public int? CatgeoryId { get; set; }

    [Column("userGroupsID")]
    public int? UserGroupsId { get; set; }

    [Column("groupName")]
    [StringLength(50)]
    [Unicode(false)]
    public string GroupName { get; set; }

    [Column("access")]
    public int? Access { get; set; }

    [Column("categoryName")]
    [StringLength(50)]
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

    [ForeignKey("Access")]
    [InverseProperty("CategoryGroups")]
    public virtual AppObject AccessNavigation { get; set; }

    [ForeignKey("CatgeoryId")]
    [InverseProperty("CategoryGroups")]
    public virtual Category Catgeory { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("CategoryGroupCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("CategoryGroupModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("UserGroupsId")]
    [InverseProperty("CategoryGroups")]
    public virtual UserGroup UserGroups { get; set; }
}
