using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("rolePermission")]
public partial class RolePermission
{
    [Key]
    [Column("rolePermissionID")]
    public int RolePermissionId { get; set; }

    [Column("roleMaserID")]
    public int? RoleMaserId { get; set; }

    [Column("moduleID")]
    public int? ModuleId { get; set; }

    [Column("rolePermissionView")]
    public bool? RolePermissionView { get; set; }

    [Column("rolePermissionAdd")]
    public bool? RolePermissionAdd { get; set; }

    [Column("rolePermissionUpdate")]
    public bool? RolePermissionUpdate { get; set; }

    [Column("rolePermissionDelete")]
    public bool? RolePermissionDelete { get; set; }

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
    [InverseProperty("RolePermissionCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("RolePermissionModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("ModuleId")]
    [InverseProperty("RolePermissions")]
    public virtual Module Module { get; set; }

    [ForeignKey("RoleMaserId")]
    [InverseProperty("RolePermissions")]
    public virtual RoleMaster RoleMaser { get; set; }
}
