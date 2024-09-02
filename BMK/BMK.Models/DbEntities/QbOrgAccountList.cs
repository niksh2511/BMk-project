using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("qbOrgAccountList")]
public partial class QbOrgAccountList
{
    [Key]
    [Column("qbOrgAccountListID")]
    public int QbOrgAccountListId { get; set; }

    [Column("organizationID")]
    public int? OrganizationId { get; set; }

    [Column("qbAccountType")]
    [StringLength(100)]
    public string QbAccountType { get; set; }

    [Column("qbAccountNumber")]
    [StringLength(100)]
    public string QbAccountNumber { get; set; }

    [Column("qbAccountName")]
    [StringLength(100)]
    public string QbAccountName { get; set; }

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

    [Column("qbAccountID")]
    [StringLength(100)]
    public string QbAccountId { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("QbOrgAccountListCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("QbOrgAccountListModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("QbOrgAccountLists")]
    public virtual Organization Organization { get; set; }

    [InverseProperty("QbOrgAccountList")]
    public virtual ICollection<QbOrgAccountBalance> QbOrgAccountBalances { get; set; } = new List<QbOrgAccountBalance>();

    [InverseProperty("QbOrgAccountList")]
    public virtual ICollection<QbOrgAccountMapping> QbOrgAccountMappings { get; set; } = new List<QbOrgAccountMapping>();
}
