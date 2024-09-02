using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("qbMapAccountCategory")]
public partial class QbMapAccountCategory
{
    [Key]
    [Column("qbMapAccountCategoryID")]
    public int QbMapAccountCategoryId { get; set; }

    [StringLength(100)]
    public string CategoryName { get; set; }

    [StringLength(4000)]
    public string CategoryDesc { get; set; }

    [Column("sectionRank")]
    public int? SectionRank { get; set; }

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

    [Column("categoryDisplayName")]
    [StringLength(100)]
    public string CategoryDisplayName { get; set; }

    [Column("isDept")]
    public bool? IsDept { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<BmkTargetReport> BmkTargetReports { get; set; } = new List<BmkTargetReport>();

    [ForeignKey("CreatedBy")]
    [InverseProperty("QbMapAccountCategoryCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("QbMapAccountCategoryModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [InverseProperty("QbMapAccountCategory")]
    public virtual ICollection<QbMapMasterAccountList> QbMapMasterAccountLists { get; set; } = new List<QbMapMasterAccountList>();

    [InverseProperty("QbMapAccountCategory")]
    public virtual ICollection<QbMapMasterFinRecord> QbMapMasterFinRecords { get; set; } = new List<QbMapMasterFinRecord>();
}
