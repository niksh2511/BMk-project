using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("userForgotPwdToken")]
public partial class UserForgotPwdToken
{
    [Key]
    [Column("userForgotPwdTokenID")]
    public int UserForgotPwdTokenId { get; set; }

    [Column("usersID")]
    public int? UsersId { get; set; }

    public string VerificationKey { get; set; }

    [Column("isEmailSent")]
    public bool? IsEmailSent { get; set; }

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
    [InverseProperty("UserForgotPwdTokenCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; }

    [ForeignKey("ModifiedBy")]
    [InverseProperty("UserForgotPwdTokenModifiedByNavigations")]
    public virtual User ModifiedByNavigation { get; set; }

    [ForeignKey("UsersId")]
    [InverseProperty("UserForgotPwdTokenUsers")]
    public virtual User Users { get; set; }
}
