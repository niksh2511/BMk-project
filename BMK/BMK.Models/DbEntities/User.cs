using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models.DbEntities;

[Table("users")]
public partial class User
{
    [Key]
    [Column("usersID")]
    public int UsersId { get; set; }

    [Column("firstName")]
    [StringLength(500)]
    public string FirstName { get; set; }

    [Column("lastName")]
    [StringLength(500)]
    public string LastName { get; set; }

    [Column("title")]
    [StringLength(20)]
    public string Title { get; set; }

    [Column("workPhone")]
    [StringLength(40)]
    public string WorkPhone { get; set; }
    [NotMapped]
    public string Password { get; set; }

    [Column("profilePicture")]

    public string ProfilePicture { get; set; }
    [Column("bmkExecutiveReports")]
    public bool? BmkExecutiveReports { get; set; }

    [Required]
    [Column("mobilePhone")]
    [StringLength(20)]
    public string MobilePhone { get; set; }

    [Required]
    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; }

    [Column("organizationID")]
    public int? OrganizationId { get; set; }

    [Column("credential")]
    [StringLength(1000)]
    public string Credential { get; set; }

    [Column("salt")]
    [StringLength(128)]
    public string Salt { get; set; }

    [Column("isO365user")]
    public bool? IsO365user { get; set; }

    [Column("isVerified")]
    public bool? IsVerified { get; set; }

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

    [Required]
    [Column("countryCode")]
    [StringLength(5)]
    [Unicode(false)]
    public string CountryCode { get; set; }

    public bool IsUserOtpRequired { get; set; }

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<AppObject> AppObjectCreatedByNavigations { get; set; } = new List<AppObject>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<AppObject> AppObjectModifiedByNavigations { get; set; } = new List<AppObject>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<AuditEntry> AuditEntryCreatedByNavigations { get; set; } = new List<AuditEntry>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<AuditEntry> AuditEntryModifiedByNavigations { get; set; } = new List<AuditEntry>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<BmkTargetReport> BmkTargetReportCreatedByNavigations { get; set; } = new List<BmkTargetReport>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<BmkTargetReport> BmkTargetReportModifiedByNavigations { get; set; } = new List<BmkTargetReport>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<CategoryGroup> CategoryGroupCreatedByNavigations { get; set; } = new List<CategoryGroup>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<CategoryGroup> CategoryGroupModifiedByNavigations { get; set; } = new List<CategoryGroup>();

    [ForeignKey("CreatedBy")]
    [InverseProperty("InverseCreatedByNavigation")]
    public virtual User CreatedByNavigation { get; set; }

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<EmailTemplate> EmailTemplateCreatedByNavigations { get; set; } = new List<EmailTemplate>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<EmailTemplate> EmailTemplateModifiedByNavigations { get; set; } = new List<EmailTemplate>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<GroupType> GroupTypeCreatedByNavigations { get; set; } = new List<GroupType>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<GroupType> GroupTypeModifiedByNavigations { get; set; } = new List<GroupType>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<User> InverseCreatedByNavigation { get; set; } = new List<User>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<User> InverseModifiedByNavigation { get; set; } = new List<User>();

    [ForeignKey("ModifiedBy")]
    [InverseProperty("InverseModifiedByNavigation")]
    public virtual User ModifiedByNavigation { get; set; }

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Module> ModuleCreatedByNavigations { get; set; } = new List<Module>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Module> ModuleModifiedByNavigations { get; set; } = new List<Module>();

    [InverseProperty("CommentByUser")]
    public virtual ICollection<ObjectiveComment> ObjectiveCommentCommentByUsers { get; set; } = new List<ObjectiveComment>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<ObjectiveComment> ObjectiveCommentCreatedByNavigations { get; set; } = new List<ObjectiveComment>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<ObjectiveComment> ObjectiveCommentModifiedByNavigations { get; set; } = new List<ObjectiveComment>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Objective> ObjectiveCreatedByNavigations { get; set; } = new List<Objective>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Objective> ObjectiveModifiedByNavigations { get; set; } = new List<Objective>();

    [InverseProperty("Users")]
    public virtual ICollection<Objective> ObjectiveUsers { get; set; } = new List<Objective>();

    [ForeignKey("OrganizationId")]
    [InverseProperty("Users")]
    public virtual Organization Organization { get; set; }

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Organization> OrganizationCreatedByNavigations { get; set; } = new List<Organization>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<Organization> OrganizationModifiedByNavigations { get; set; } = new List<Organization>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<OrganizationPortalSetting> OrganizationPortalSettingCreatedByNavigations { get; set; } = new List<OrganizationPortalSetting>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<OrganizationPortalSetting> OrganizationPortalSettingModifiedByNavigations { get; set; } = new List<OrganizationPortalSetting>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<QbMapAccountCategory> QbMapAccountCategoryCreatedByNavigations { get; set; } = new List<QbMapAccountCategory>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<QbMapAccountCategory> QbMapAccountCategoryModifiedByNavigations { get; set; } = new List<QbMapAccountCategory>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<QbMapMasterAccountList> QbMapMasterAccountListCreatedByNavigations { get; set; } = new List<QbMapMasterAccountList>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<QbMapMasterAccountList> QbMapMasterAccountListModifiedByNavigations { get; set; } = new List<QbMapMasterAccountList>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<QbMapMasterFinRecord> QbMapMasterFinRecordCreatedByNavigations { get; set; } = new List<QbMapMasterFinRecord>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<QbMapMasterFinRecord> QbMapMasterFinRecordModifiedByNavigations { get; set; } = new List<QbMapMasterFinRecord>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<QbMapMonthlyFinRecord> QbMapMonthlyFinRecordCreatedByNavigations { get; set; } = new List<QbMapMonthlyFinRecord>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<QbMapMonthlyFinRecord> QbMapMonthlyFinRecordModifiedByNavigations { get; set; } = new List<QbMapMonthlyFinRecord>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<QbOrgAccountBalance> QbOrgAccountBalanceCreatedByNavigations { get; set; } = new List<QbOrgAccountBalance>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<QbOrgAccountBalance> QbOrgAccountBalanceModifiedByNavigations { get; set; } = new List<QbOrgAccountBalance>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<QbOrgAccountList> QbOrgAccountListCreatedByNavigations { get; set; } = new List<QbOrgAccountList>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<QbOrgAccountList> QbOrgAccountListModifiedByNavigations { get; set; } = new List<QbOrgAccountList>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<QbOrgAccountMapping> QbOrgAccountMappingCreatedByNavigations { get; set; } = new List<QbOrgAccountMapping>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<QbOrgAccountMapping> QbOrgAccountMappingModifiedByNavigations { get; set; } = new List<QbOrgAccountMapping>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<QbProcessLog> QbProcessLogCreatedByNavigations { get; set; } = new List<QbProcessLog>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<QbProcessLog> QbProcessLogModifiedByNavigations { get; set; } = new List<QbProcessLog>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<QbTokenDetail> QbTokenDetails { get; set; } = new List<QbTokenDetail>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<RoleMaster> RoleMasterCreatedByNavigations { get; set; } = new List<RoleMaster>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<RoleMaster> RoleMasterModifiedByNavigations { get; set; } = new List<RoleMaster>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<RolePermission> RolePermissionCreatedByNavigations { get; set; } = new List<RolePermission>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<RolePermission> RolePermissionModifiedByNavigations { get; set; } = new List<RolePermission>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<RptBmktargetReport> RptBmktargetReportCreatedByNavigations { get; set; } = new List<RptBmktargetReport>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<RptBmktargetReport> RptBmktargetReportModifiedByNavigations { get; set; } = new List<RptBmktargetReport>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<State> StateCreatedByNavigations { get; set; } = new List<State>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<State> StateModifiedByNavigations { get; set; } = new List<State>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<UserForgotPwdToken> UserForgotPwdTokenCreatedByNavigations { get; set; } = new List<UserForgotPwdToken>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<UserForgotPwdToken> UserForgotPwdTokenModifiedByNavigations { get; set; } = new List<UserForgotPwdToken>();

    [InverseProperty("Users")]
    public virtual ICollection<UserForgotPwdToken> UserForgotPwdTokenUsers { get; set; } = new List<UserForgotPwdToken>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<UserGroup> UserGroupCreatedByNavigations { get; set; } = new List<UserGroup>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<UserGroup> UserGroupModifiedByNavigations { get; set; } = new List<UserGroup>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<UserGroupsMember> UserGroupsMemberCreatedByNavigations { get; set; } = new List<UserGroupsMember>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<UserGroupsMember> UserGroupsMemberModifiedByNavigations { get; set; } = new List<UserGroupsMember>();

    [InverseProperty("Users")]
    public virtual ICollection<UserGroupsMember> UserGroupsMemberUsers { get; set; } = new List<UserGroupsMember>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<UserRole> UserRoleCreatedByNavigations { get; set; } = new List<UserRole>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<UserRole> UserRoleModifiedByNavigations { get; set; } = new List<UserRole>();

    [InverseProperty("Users")]
    public virtual ICollection<UserRole> UserRoleUsers { get; set; } = new List<UserRole>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<UserToken> UserTokenCreatedByNavigations { get; set; } = new List<UserToken>();

    [InverseProperty("ModifiedByNavigation")]
    public virtual ICollection<UserToken> UserTokenModifiedByNavigations { get; set; } = new List<UserToken>();

    [InverseProperty("Users")]
    public virtual ICollection<UserToken> UserTokenUsers { get; set; } = new List<UserToken>();
}
