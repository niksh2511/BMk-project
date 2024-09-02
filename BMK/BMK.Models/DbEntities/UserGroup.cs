using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("userGroups")]
public partial class UserGroup
{
    [Key]
    [Column("userGroupsID")]
    public int UserGroupsId { get; set; }

    [Column("groupName")]
    [StringLength(200)]
    public string GroupName { get; set; }

    [Column("groupDesc")]
    [StringLength(500)]
    public string GroupDesc { get; set; }

    [Column("groupTypesID")]
    public int? GroupTypesId { get; set; }

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

    [InverseProperty("UserGroups")]
    public virtual ICollection<CategoryGroup> CategoryGroups { get; set; } = new List<CategoryGroup>();

    [ForeignKey("CreatedBy")]
    [InverseProperty("UserGroupCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("GroupTypesId")]
    [InverseProperty("UserGroups")]
    public virtual GroupType GroupTypes { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("UserGroupModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [InverseProperty("UserGroups")]
    public virtual ICollection<UserGroupsMember> UserGroupsMembers { get; set; } = new List<UserGroupsMember>();
}
