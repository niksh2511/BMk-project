using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("qbProcessLog")]
public partial class QbProcessLog
{
    [Key]
    [Column("qbProcessLogID")]
    public int QbProcessLogId { get; set; }

    [Column("organizationID")]
    public int? OrganizationId { get; set; }

    [Column("logDate", TypeName = "datetime")]
    public DateTime? LogDate { get; set; }

    [Column("logComments")]
    public string LogComments { get; set; }

    [Column("logSeq")]
    public int? LogSeq { get; set; }

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

    [Column("responseStream")]
    public string ResponseStream { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("QbProcessLogCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("QbProcessLogModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("QbProcessLogs")]
    public virtual Organization Organization { get; set; }
}
