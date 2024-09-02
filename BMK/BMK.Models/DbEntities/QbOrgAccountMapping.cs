using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("qbOrgAccountMapping")]
public partial class QbOrgAccountMapping
{
    [Key]
    [Column("qbOrgAccountMappingID")]
    public int QbOrgAccountMappingId { get; set; }

    [Column("qbOrgAccountListID")]
    public int? QbOrgAccountListId { get; set; }

    [Column("qbMapMasterAccountListID")]
    public int? QbMapMasterAccountListId { get; set; }

    [Column("mapRatio", TypeName = "decimal(5, 2)")]
    public decimal? MapRatio { get; set; }

    [Column("mapRatioFlag")]
    public bool? MapRatioFlag { get; set; }

    [Column("mapNotes")]
    [StringLength(1000)]
    public string MapNotes { get; set; }

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
    [InverseProperty("QbOrgAccountMappingCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("QbOrgAccountMappingModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("QbMapMasterAccountListId")]
    [InverseProperty("QbOrgAccountMappings")]
    public virtual QbMapMasterAccountList QbMapMasterAccountList { get; set; }

    [ForeignKey("QbOrgAccountListId")]
    [InverseProperty("QbOrgAccountMappings")]
    public virtual QbOrgAccountList QbOrgAccountList { get; set; }
}
