using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("userRole")]
public partial class UserRole
{
    [Key]
    [Column("userRoleID")]
    public int UserRoleId { get; set; }

    [Column("usersID")]
    public int? UsersId { get; set; }

    [Column("roleMasterID")]
    public int? RoleMasterId { get; set; }

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
    [InverseProperty("UserRoleCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("UserRoleModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("RoleMasterId")]
    [InverseProperty("UserRoles")]
    public virtual RoleMaster RoleMaster { get; set; }

    [ForeignKey("UsersId")]
    [InverseProperty("UserRoleUsers")]
    public virtual User Users { get; set; }
}
