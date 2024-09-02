using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("qbOrgAccountBalance")]
public partial class QbOrgAccountBalance
{
    [Key]
    [Column("qbOrgAccountBalanceID")]
    public int QbOrgAccountBalanceId { get; set; }

    [Column("qbOrgAccountListID")]
    public int? QbOrgAccountListId { get; set; }

    [Column("qbMonthYear")]
    public DateOnly? QbMonthYear { get; set; }

    [Column("qbBalanceAmount", TypeName = "decimal(15, 2)")]
    public decimal? QbBalanceAmount { get; set; }

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
    [InverseProperty("QbOrgAccountBalanceCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("QbOrgAccountBalanceModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("QbOrgAccountListId")]
    [InverseProperty("QbOrgAccountBalances")]
    public virtual QbOrgAccountList QbOrgAccountList { get; set; }
}
