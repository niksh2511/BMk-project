using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("organizationPortalSettings")]
public partial class OrganizationPortalSetting
{
    [Key]
    [Column("organizationPortalSettingsID")]
    public int OrganizationPortalSettingsId { get; set; }

    [Column("organizationID")]
    public int? OrganizationId { get; set; }

    [Column("targets")]
    [StringLength(100)]
    public string Targets { get; set; }

    [Column("IT_Services", TypeName = "decimal(5, 2)")]
    public decimal? ItServices { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Develoment { get; set; }

    [Column("Cloud_Datacenter", TypeName = "decimal(5, 2)")]
    public decimal? CloudDatacenter { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Voice { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Other { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Combined { get; set; }

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
    [InverseProperty("OrganizationPortalSettingCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("OrganizationPortalSettingModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("OrganizationPortalSettings")]
    public virtual Organization Organization { get; set; }
}
