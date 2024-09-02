namespace RxWeb.Core.Security
{
    public interface IUserClaim
    {
        string Email { get; }
        bool Anonymous { get; }

        string Locale { get; }

        int CountryId { get; }
        int RoleId { get; }
        int OrganizationId { get; }

        string Uri { get; }

        int UserId { get; }

        string UserName { get; }
    }
}
