using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("auditEntries")]
public partial class AuditEntry
{
    [Key]
    [Column("auditEntriesID")]
    public int AuditEntriesId { get; set; }

    [Column("events")]
    [StringLength(50)]
    [Unicode(false)]
    public string Events { get; set; }

    [Column("tableName")]
    [StringLength(100)]
    [Unicode(false)]
    public string TableName { get; set; }

    [Column("tableID")]
    public long? TableId { get; set; }

    [Column("columnName")]
    [StringLength(100)]
    [Unicode(false)]
    public string ColumnName { get; set; }

    [Required]
    [Column("oldValue")]
    [StringLength(500)]
    public string OldValue { get; set; }

    [StringLength(500)]
    public string NewValue { get; set; }

    [Column("eventBy")]
    public int? EventBy { get; set; }

    [Column("eventDt", TypeName = "datetime")]
    public DateTime? EventDt { get; set; }

    [Column("createdBy")]
    public int? CreatedBy { get; set; }

    [Column("createdDate", TypeName = "datetime")]
    public DateTime? CreatedDate { get; set; }

    [Column("modifiedBy")]
    public int? ModifiedBy { get; set; }

    [Column("modifiedDate", TypeName = "datetime")]
    public DateTime? ModifiedDate { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("AuditEntryCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("AuditEntryModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }
}
