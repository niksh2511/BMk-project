using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("qbMapMasterFinRecord")]
public partial class QbMapMasterFinRecord
{
    [Key]
    [Column("qbMapMasterFinRecordID")]
    public int QbMapMasterFinRecordId { get; set; }

    [Column("qbMapAccountCategoryID")]
    public int? QbMapAccountCategoryId { get; set; }

    [Column("section")]
    public int? Section { get; set; }

    [Column("sectionRank")]
    public int? SectionRank { get; set; }

    [Column("internalName")]
    [StringLength(100)]
    public string InternalName { get; set; }

    [Column("externalNAme")]
    [StringLength(100)]
    public string ExternalName { get; set; }

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

    [Column("qbMasterAccountMapping")]
    [StringLength(200)]
    [Unicode(false)]
    public string QbMasterAccountMapping { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("QbMapMasterFinRecordCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("QbMapMasterFinRecordModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("QbMapAccountCategoryId")]
    [InverseProperty("QbMapMasterFinRecords")]
    public virtual QbMapAccountCategory QbMapAccountCategory { get; set; }

    [InverseProperty("QbMapMasterFinRecord")]
    public virtual ICollection<QbMapMonthlyFinRecord> QbMapMonthlyFinRecords { get; set; } = new List<QbMapMonthlyFinRecord>();
}
