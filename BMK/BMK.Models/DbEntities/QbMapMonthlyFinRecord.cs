using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("qbMapMonthlyFinRecord")]
public partial class QbMapMonthlyFinRecord
{
    [Key]
    [Column("qbMapMonthlyFinRecordID")]
    public int QbMapMonthlyFinRecordId { get; set; }

    [Column("qbMapMasterFinRecordID")]
    public int? QbMapMasterFinRecordId { get; set; }

    [Column("organizationID")]
    public int? OrganizationId { get; set; }

    [Column("internalName")]
    [StringLength(100)]
    public string InternalName { get; set; }

    [Column("externalNAme")]
    [StringLength(100)]
    public string ExternalName { get; set; }

    [Column("monthYear")]
    public DateOnly? MonthYear { get; set; }

    [Column("amount", TypeName = "decimal(15, 2)")]
    public decimal? Amount { get; set; }

    [Column("isImported")]
    public bool? IsImported { get; set; }

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
    [InverseProperty("QbMapMonthlyFinRecordCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("QbMapMonthlyFinRecordModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("QbMapMonthlyFinRecords")]
    public virtual Organization Organization { get; set; }

    [ForeignKey("QbMapMasterFinRecordId")]
    [InverseProperty("QbMapMonthlyFinRecords")]
    public virtual QbMapMasterFinRecord QbMapMasterFinRecord { get; set; }
}
