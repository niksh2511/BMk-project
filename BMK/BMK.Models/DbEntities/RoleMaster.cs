using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("roleMaster")]
public partial class RoleMaster
{
    [Key]
    [Column("roleMasterID")]
    public int RoleMasterId { get; set; }

    [Column("roleName")]
    [StringLength(100)]
    public string RoleName { get; set; }

    [Required]
    [Column("roleDesc")]
    [StringLength(500)]
    public string RoleDesc { get; set; }

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
    [InverseProperty("RoleMasterCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("RoleMasterModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [InverseProperty("RoleMaser")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    [InverseProperty("RoleMaster")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
