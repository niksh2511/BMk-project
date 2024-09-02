using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("qbTokenDetails")]
public partial class QbTokenDetail
{
    [Key]
    [Column("qbID")]
    public int QbId { get; set; }

    [Column("qbUserID")]
    public int? QbUserId { get; set; }

    [Column("qbCustomerID")]
    public long? QbCustomerId { get; set; }

    [Column("qbAccessToken")]
    [Unicode(false)]
    public string QbAccessToken { get; set; }

    [Column("qbExpireTime", TypeName = "datetime")]
    public DateTime? QbExpireTime { get; set; }

    [Column("qbRefreshToken")]
    [StringLength(500)]
    [Unicode(false)]
    public string QbRefreshToken { get; set; }

    [Column("qbRefreshTokenExpireTime", TypeName = "datetime")]
    public DateTime? QbRefreshTokenExpireTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    public int? CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("QbTokenDetails")]
    public virtual User CreatedByNavigation { get; set; }
}
