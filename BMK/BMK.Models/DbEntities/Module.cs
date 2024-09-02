using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("modules")]
public partial class Module
{
    [Key]
    [Column("moduleID")]
    public int ModuleId { get; set; }

    [Column("moduleName")]
    [StringLength(100)]
    public string ModuleName { get; set; }

    [Required]
    [Column("moduleDesc")]
    [StringLength(500)]
    public string ModuleDesc { get; set; }

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

    [Column("isAdmin")]
    public bool? IsAdmin { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("ModuleCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("ModuleModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [InverseProperty("Module")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
