using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("rptBMKTargetReport")]
public partial class RptBmktargetReport
{
    [Key]
    [Column("rptBMKTargetReportID")]
    public int RptBmktargetReportId { get; set; }

    [Column("organizationID")]
    public int OrganizationId { get; set; }

    [Column("rptYear")]
    public int RptYear { get; set; }

    [Required]
    [Column("rptHead")]
    [StringLength(100)]
    public string RptHead { get; set; }

    [Column("rptJAN")]
    public int? RptJan { get; set; }

    [Column("rptFEB")]
    public int? RptFeb { get; set; }

    [Column("rptMAR")]
    public int? RptMar { get; set; }

    [Column("rptAPR")]
    public int? RptApr { get; set; }

    [Column("rptJUN")]
    public int? RptJun { get; set; }

    [Column("rptJUL")]
    public int? RptJul { get; set; }

    [Column("rptAUG")]
    public int? RptAug { get; set; }

    [Column("rptSEP")]
    public int? RptSep { get; set; }

    [Column("rptOCT")]
    public int? RptOct { get; set; }

    [Column("rptNOV")]
    public int? RptNov { get; set; }

    [Column("rptDEC")]
    public int? RptDec { get; set; }

    [Column("rptAVG", TypeName = "decimal(5, 2)")]
    public decimal? RptAvg { get; set; }

    [Column("rptGrpAVG", TypeName = "decimal(5, 2)")]
    public decimal? RptGrpAvg { get; set; }

    [Column("rptBMKAVG", TypeName = "decimal(5, 2)")]
    public decimal? RptBmkavg { get; set; }

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

    [Column("countryCode")]
    [StringLength(5)]
    [Unicode(false)]
    public string CountryCode { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("RptBmktargetReportCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("RptBmktargetReportModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }
}
