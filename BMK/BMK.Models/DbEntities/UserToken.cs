using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("userToken")]
public partial class UserToken
{
    [Key]
    [Column("userTokenID")]
    public int UserTokenId { get; set; }

    [Column("usersID")]
    public int? UsersId { get; set; }

    [Column("authToken")]
    public string AuthToken { get; set; }

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

    [Column("securityKey")]
    [StringLength(200)]
    public string SecurityKey { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("UserTokenCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("UserTokenModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("UsersId")]
    [InverseProperty("UserTokenUsers")]
    public virtual User Users { get; set; }
}
