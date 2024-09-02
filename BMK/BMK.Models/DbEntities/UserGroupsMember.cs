using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("userGroupsMembers")]
public partial class UserGroupsMember
{
    [Key]
    [Column("userGroupsMembersID")]
    public int UserGroupsMembersId { get; set; }

    [Column("userGroupsID")]
    public int UserGroupsId { get; set; }

    [Column("usersID")]
    public int UsersId { get; set; }

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
    [InverseProperty("UserGroupsMemberCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("UserGroupsMemberModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("UserGroupsId")]
    [InverseProperty("UserGroupsMembers")]
    public virtual UserGroup UserGroups { get; set; }

    [ForeignKey("UsersId")]
    [InverseProperty("UserGroupsMemberUsers")]
    public virtual User Users { get; set; }
}
