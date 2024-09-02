using RxWeb.Core.Annotations;
using RxWeb.Core.Data.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMK.Models.DbEntities.Main
{
    [Table("ApplicationUserTokens", Schema = "dbo")]
    public partial class ApplicationUserToken
    {
        #region ApplicationUserTokenId Annotations

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [System.ComponentModel.DataAnnotations.Key]
        #endregion ApplicationUserTokenId Annotations

        public int ApplicationUserTokenId { get; set; }

        #region UserId Annotations

        [Range(1, int.MaxValue)]
        [Required]
        [RelationshipTableAttribue("Users", "dbo", "", "UserId")]
        #endregion UserId Annotations

        public int UserId { get; set; }

        #region SecurityKey Annotations

        [Required]
        [MaxLength(200)]
        #endregion SecurityKey Annotations

        public string SecurityKey { get; set; }

        #region JwtToken Annotations

        [Required]
        #endregion JwtToken Annotations

        public string JwtToken { get; set; }

        #region AudienceType Annotations

        [Required]
        [MaxLength(50)]
        #endregion AudienceType Annotations

        public string AudienceType { get; set; }

        #region CreatedDateTime Annotations

        [Required]
        #endregion CreatedDateTime Annotations

        public DateTimeOffset CreatedDateTime { get; set; }

        #region User Annotations

        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(Main.User.ApplicationUserTokens))]
        #endregion User Annotations

        public virtual User User { get; set; }


        public ApplicationUserToken()
        {
        }
    }
}