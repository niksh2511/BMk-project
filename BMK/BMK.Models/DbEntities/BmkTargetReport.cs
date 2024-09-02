using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("bmkTargetReport")]
public partial class BmkTargetReport
{
    [Key]
    [Column("bmkTargetID")]
    public int BmkTargetId { get; set; }

    [Column("organizationID")]
    public int OrganizationId { get; set; }

    [Column("categoryID")]
    public int CategoryId { get; set; }

    public int Year { get; set; }

    [Required]
    [Column("month")]
    [StringLength(10)]
    [Unicode(false)]
    public string Month { get; set; }

    [Required]
    [Column("rptHead")]
    [StringLength(50)]
    public string RptHead { get; set; }

    [Required]
    [Column("rptLine")]
    [StringLength(50)]
    public string RptLine { get; set; }

    [Column("rptSeqs")]
    public int RptSeqs { get; set; }

    [Column("amount", TypeName = "decimal(20, 2)")]
    public decimal? Amount { get; set; }

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

    [ForeignKey("CategoryId")]
    [InverseProperty("BmkTargetReports")]
    public virtual QbMapAccountCategory Category { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("BmkTargetReportCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("BmkTargetReportModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("BmkTargetReports")]
    public virtual Organization Organization { get; set; }
}
