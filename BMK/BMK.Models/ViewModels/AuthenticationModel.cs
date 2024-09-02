using System.ComponentModel.DataAnnotations;

namespace BMK.Models.ViewModels
{

    public partial class AuthenticationModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string VerificationCode { get; set; }

    }

    public partial class UserAuthenticateModel
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string AuthToken { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public int? OrganizationId { get; set; }
        public string Organization { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
        public string ProfilePicture { get; set; }

        public bool IsUserVerified { get; set; }

        public bool IsUserOtpRequired { get; set; }
        public Dictionary<int, Dictionary<string, bool>> AccessModules { get; set; } 

    }

    public partial class O365VerificationModel
    {
        public string IdToken { get; set; }
    }
}

