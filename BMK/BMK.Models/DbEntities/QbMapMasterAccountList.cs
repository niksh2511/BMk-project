using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("qbMapMasterAccountList")]
public partial class QbMapMasterAccountList
{
    [Key]
    [Column("qbMapMasterAccountListID")]
    public int QbMapMasterAccountListId { get; set; }

    [Column("qbMapAccountCategoryID")]
    public int? QbMapAccountCategoryId { get; set; }

    [Column("mapAccountNameInternal")]
    [StringLength(100)]
    public string MapAccountNameInternal { get; set; }

    [Column("mapAccountNameExternal")]
    [StringLength(100)]
    public string MapAccountNameExternal { get; set; }

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
    [InverseProperty("QbMapMasterAccountListCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("QbMapMasterAccountListModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("QbMapAccountCategoryId")]
    [InverseProperty("QbMapMasterAccountLists")]
    public virtual QbMapAccountCategory QbMapAccountCategory { get; set; }

    [InverseProperty("QbMapMasterAccountList")]
    public virtual ICollection<QbOrgAccountMapping> QbOrgAccountMappings { get; set; } = new List<QbOrgAccountMapping>();
}
