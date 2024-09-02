using System;
using System.Collections.Generic;
using BMK.Models.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace BMK.Models;

public partial class BMKDbContext : DbContext
{
    public BMKDbContext()
    {
    }

    public BMKDbContext(DbContextOptions<BMKDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppObject> AppObjects { get; set; }

    public virtual DbSet<AuditEntry> AuditEntries { get; set; }

    public virtual DbSet<BmkMemberMeeting> BmkMemberMeetings { get; set; }

    public virtual DbSet<BmkTarget> BmkTargets { get; set; }

    public virtual DbSet<BmkTargetReport> BmkTargetReports { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CategoryGroup> CategoryGroups { get; set; }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventCategory> EventCategories { get; set; }

    public virtual DbSet<ExceptionLog> ExceptionLogs { get; set; }

    public virtual DbSet<GroupType> GroupTypes { get; set; }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<Objective> Objectives { get; set; }

    public virtual DbSet<ObjectiveComment> ObjectiveComments { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<OrganizationPortalSetting> OrganizationPortalSettings { get; set; }

    public virtual DbSet<OrganizationSalary> OrganizationSalaries { get; set; }

    public virtual DbSet<ProcessExceptionLog> ProcessExceptionLogs { get; set; }

    public virtual DbSet<PsaInput> PsaInputs { get; set; }

    public virtual DbSet<QbExceptionLog> QbExceptionLogs { get; set; }

    public virtual DbSet<QbMapAccountCategory> QbMapAccountCategories { get; set; }

    public virtual DbSet<QbMapMasterAccountList> QbMapMasterAccountLists { get; set; }

    public virtual DbSet<QbMapMasterFinRecord> QbMapMasterFinRecords { get; set; }

    public virtual DbSet<QbMapMonthlyFinRecord> QbMapMonthlyFinRecords { get; set; }

    public virtual DbSet<QbOrgAccountBalance> QbOrgAccountBalances { get; set; }

    public virtual DbSet<QbOrgAccountList> QbOrgAccountLists { get; set; }

    public virtual DbSet<QbOrgAccountMapping> QbOrgAccountMappings { get; set; }

    public virtual DbSet<QbProcessLog> QbProcessLogs { get; set; }

    public virtual DbSet<QbTokenDetail> QbTokenDetails { get; set; }

    public virtual DbSet<RoleMaster> RoleMasters { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<RptBmktargetReport> RptBmktargetReports { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserForgotPwdToken> UserForgotPwdTokens { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    public virtual DbSet<UserGroupsMember> UserGroupsMembers { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    public virtual DbSet<VCategoryGroup> VCategoryGroups { get; set; }

    public virtual DbSet<VOrganizationForPeerTeamProfile> VOrganizationForPeerTeamProfiles { get; set; }

    public virtual DbSet<VOrganizationSalary> VOrganizationSalaries { get; set; }

    public virtual DbSet<VUserGroup> VUserGroups { get; set; }

    public virtual DbSet<VUserGroupsMember> VUserGroupsMembers { get; set; }

    public virtual DbSet<Vorganizaion> Vorganizaions { get; set; }

    public virtual DbSet<VqbAccount> VqbAccounts { get; set; }

    public virtual DbSet<Vuser> Vusers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=RXDBSERVER\\MSSQLSERVER2019;Database=BMKPortal;Persist Security Info=True;User ID=BMKPortal2024;Password=BMKPortal2024;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppObject>(entity =>
        {
            entity.HasKey(e => e.AppObjectsId).HasName("PK_appObjectsID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AppObjectCreatedByNavigations).HasConstraintName("FK_102948_appObjectscreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.AppObjectModifiedByNavigations).HasConstraintName("FK_102948_appObjectsmodifiedBy");
        });

        modelBuilder.Entity<AuditEntry>(entity =>
        {
            entity.HasKey(e => e.AuditEntriesId).HasName("PK_auditEntriesID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.AuditEntryCreatedByNavigations).HasConstraintName("FK_102948_auditEntriescreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.AuditEntryModifiedByNavigations).HasConstraintName("FK_102948_auditEntriesmodifiedBy");
        });

        modelBuilder.Entity<BmkMemberMeeting>(entity =>
        {
            entity.HasKey(e => e.BmkMemberMeetingId).HasName("PK__bmkMemberMeetingID");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<BmkTarget>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<BmkTargetReport>(entity =>
        {
            entity.HasKey(e => e.BmkTargetId).HasName("PK_bmkTargetID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.BmkTargetReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_bmkTR_departmentID");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BmkTargetReportCreatedByNavigations).HasConstraintName("FK_bmkTR_ObjectivescreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.BmkTargetReportModifiedByNavigations).HasConstraintName("FK_bmkTR_ObjectivesmodifiedBy");

            entity.HasOne(d => d.Organization).WithMany(p => p.BmkTargetReports)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_bmkTR_organizationID");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__category__23CAF1F8D733169E");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Categories).HasConstraintName("FK__category__create__75C27486");
        });

        modelBuilder.Entity<CategoryGroup>(entity =>
        {
            entity.HasKey(e => e.CategoryGroupsId).HasName("PK__category__6729D15E6791403D");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.AccessNavigation).WithMany(p => p.CategoryGroups).HasConstraintName("FK__categoryG__acces__0D99FE17");

            entity.HasOne(d => d.Catgeory).WithMany(p => p.CategoryGroups).HasConstraintName("FK__categoryG__catge__0BB1B5A5");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CategoryGroupCreatedByNavigations).HasConstraintName("FK__categoryG__creat__0F824689");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.CategoryGroupModifiedByNavigations).HasConstraintName("FK__categoryG__modif__125EB334");

            entity.HasOne(d => d.UserGroups).WithMany(p => p.CategoryGroups).HasConstraintName("FK__categoryG__userG__0CA5D9DE");
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasKey(e => e.EmailTemplatesId).HasName("PK_emailTemplatesID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.EmailTemplateCreatedByNavigations).HasConstraintName("FK_102948_emailTemplatescreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.EmailTemplateModifiedByNavigations).HasConstraintName("FK_102948_emailTemplatesmodifiedBy");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__event__2DC7BD69368FC2A4");
        });

        modelBuilder.Entity<EventCategory>(entity =>
        {
            entity.HasKey(e => e.EventCategoryId).HasName("PK__eventCat__4675F61734C3C353");

            entity.HasOne(d => d.Category).WithMany(p => p.EventCategories).HasConstraintName("FK__eventCate__categ__61BB7BD9");

            entity.HasOne(d => d.Event).WithMany(p => p.EventCategories).HasConstraintName("FK__eventCate__event__60C757A0");
        });

        modelBuilder.Entity<ExceptionLog>(entity =>
        {
            entity.HasKey(e => e.ExceptionLogsId).HasName("PK_exceptionLogsID");
        });

        modelBuilder.Entity<GroupType>(entity =>
        {
            entity.HasKey(e => e.GroupTypesId).HasName("PK_groupTypesID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.GroupTypeCreatedByNavigations).HasConstraintName("FK_102948_groupTypescreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.GroupTypeModifiedByNavigations).HasConstraintName("FK_102948_groupTypesmodifiedBy");
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.ModuleId).HasName("PK_moduleID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ModuleCreatedByNavigations).HasConstraintName("FK_102948_modulescreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ModuleModifiedByNavigations).HasConstraintName("FK_102948_modulesmodifiedBy");
        });

        modelBuilder.Entity<Objective>(entity =>
        {
            entity.HasKey(e => e.ObjectiveId).HasName("PK_objectiveID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ObjectiveCreatedByNavigations).HasConstraintName("FK_102948_ObjectivescreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ObjectiveModifiedByNavigations).HasConstraintName("FK_102948_ObjectivesmodifiedBy");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Objectives).HasConstraintName("FK_102948_statusid");

            entity.HasOne(d => d.Users).WithMany(p => p.ObjectiveUsers).HasConstraintName("FK_102948_Objectivesusersid");
        });

        modelBuilder.Entity<ObjectiveComment>(entity =>
        {
            entity.HasKey(e => e.ObjectiveCommentsId).HasName("PK_objectiveCommentsID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CommentByUser).WithMany(p => p.ObjectiveCommentCommentByUsers).HasConstraintName("FK_1029438_commentbyuserid");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ObjectiveCommentCreatedByNavigations).HasConstraintName("FK_102948_objectiveCommentscreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.ObjectiveCommentModifiedByNavigations).HasConstraintName("FK_102948_objectiveCommentsmodifiedBy");

            entity.HasOne(d => d.Objective).WithMany(p => p.ObjectiveComments).HasConstraintName("FK_10223438_Objectiveid");
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.OrganizationId).HasName("PK_organizationID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrganizationCreatedByNavigations).HasConstraintName("FK_102948_organizationcreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.OrganizationModifiedByNavigations).HasConstraintName("FK_102948_organizationmodifiedBy");
        });

        modelBuilder.Entity<OrganizationPortalSetting>(entity =>
        {
            entity.HasKey(e => e.OrganizationPortalSettingsId).HasName("PK_organizationPortalSettingsID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.OrganizationPortalSettingCreatedByNavigations).HasConstraintName("FK_102948_organizationPortalSettingscreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.OrganizationPortalSettingModifiedByNavigations).HasConstraintName("FK_102948_organizationPortalSettingsmodifiedBy");

            entity.HasOne(d => d.Organization).WithMany(p => p.OrganizationPortalSettings).HasConstraintName("FK_8001555organizationID");
        });

        modelBuilder.Entity<OrganizationSalary>(entity =>
        {
            entity.HasKey(e => e.OrganizationSalaryId).HasName("PK__OrganizationSalaryId");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Organization).WithMany(p => p.OrganizationSalaries).HasConstraintName("FK__OrganizationID");
        });

        modelBuilder.Entity<ProcessExceptionLog>(entity =>
        {
            entity.HasKey(e => e.ProcessExceptionLogsId).HasName("PK_processExceptionLogsID");
        });

        modelBuilder.Entity<PsaInput>(entity =>
        {
            entity.HasKey(e => e.Psaid).HasName("PK__psaInput__0D4C993636640850");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Organization).WithMany(p => p.PsaInputs).HasConstraintName("FK_Organization");
        });

        modelBuilder.Entity<QbExceptionLog>(entity =>
        {
            entity.HasKey(e => e.QbExceptionLogsId).HasName("PK_qbExceptionLogsID");
        });

        modelBuilder.Entity<QbMapAccountCategory>(entity =>
        {
            entity.HasKey(e => e.QbMapAccountCategoryId).HasName("PK_qbMapAccountCategoryID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QbMapAccountCategoryCreatedByNavigations).HasConstraintName("FK_102948_qbMapAccountCategorycreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.QbMapAccountCategoryModifiedByNavigations).HasConstraintName("FK_102948_qbMapAccountCategorymodifiedBy");
        });

        modelBuilder.Entity<QbMapMasterAccountList>(entity =>
        {
            entity.HasKey(e => e.QbMapMasterAccountListId).HasName("PK_qbMapMasterAccountListID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QbMapMasterAccountListCreatedByNavigations).HasConstraintName("FK_102948_qbMapMasterAccountListcreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.QbMapMasterAccountListModifiedByNavigations).HasConstraintName("FK_102948_qbMapMasterAccountListmodifiedBy");

            entity.HasOne(d => d.QbMapAccountCategory).WithMany(p => p.QbMapMasterAccountLists).HasConstraintName("FK_1479439_qbMapAccountCategoryID");
        });

        modelBuilder.Entity<QbMapMasterFinRecord>(entity =>
        {
            entity.HasKey(e => e.QbMapMasterFinRecordId).HasName("PK_qbMapMasterFinRecordID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QbMapMasterFinRecordCreatedByNavigations).HasConstraintName("FK_102948_qbMapMasterFinRecordcreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.QbMapMasterFinRecordModifiedByNavigations).HasConstraintName("FK_102948_qbMapMasterFinRecordmodifiedBy");

            entity.HasOne(d => d.QbMapAccountCategory).WithMany(p => p.QbMapMasterFinRecords).HasConstraintName("FK_3099558_qbMapAccountCategoryID");
        });

        modelBuilder.Entity<QbMapMonthlyFinRecord>(entity =>
        {
            entity.HasKey(e => e.QbMapMonthlyFinRecordId).HasName("PK_qbMapMonthlyFinRecordID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QbMapMonthlyFinRecordCreatedByNavigations).HasConstraintName("FK_102948_qbMapMonthlyFinRecordcreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.QbMapMonthlyFinRecordModifiedByNavigations).HasConstraintName("FK_102948_qbMapMonthlyFinRecordmodifiedBy");

            entity.HasOne(d => d.Organization).WithMany(p => p.QbMapMonthlyFinRecords).HasConstraintName("FK_1313393_organizationID");

            entity.HasOne(d => d.QbMapMasterFinRecord).WithMany(p => p.QbMapMonthlyFinRecords).HasConstraintName("FK_1706329_qbMapMasterFinRecordID");
        });

        modelBuilder.Entity<QbOrgAccountBalance>(entity =>
        {
            entity.HasKey(e => e.QbOrgAccountBalanceId).HasName("PK_qbOrgAccountBalanceID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QbOrgAccountBalanceCreatedByNavigations).HasConstraintName("FK_102948_qbOrgAccountBalancecreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.QbOrgAccountBalanceModifiedByNavigations).HasConstraintName("FK_102948_qbOrgAccountBalancemodifiedBy");

            entity.HasOne(d => d.QbOrgAccountList).WithMany(p => p.QbOrgAccountBalances).HasConstraintName("FK_593630_qbOrgAccountListID");
        });

        modelBuilder.Entity<QbOrgAccountList>(entity =>
        {
            entity.HasKey(e => e.QbOrgAccountListId).HasName("PK_qbOrgAccountListID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QbOrgAccountListCreatedByNavigations).HasConstraintName("FK_102948_qbOrgAccountListcreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.QbOrgAccountListModifiedByNavigations).HasConstraintName("FK_102948_qbOrgAccountListmodifiedBy");

            entity.HasOne(d => d.Organization).WithMany(p => p.QbOrgAccountLists).HasConstraintName("FK_3168844_organizationID");
        });

        modelBuilder.Entity<QbOrgAccountMapping>(entity =>
        {
            entity.HasKey(e => e.QbOrgAccountMappingId).HasName("PK_qbOrgAccountMappingID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QbOrgAccountMappingCreatedByNavigations).HasConstraintName("FK_102948_qbOrgAccountMappingcreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.QbOrgAccountMappingModifiedByNavigations).HasConstraintName("FK_102948_qbOrgAccountMappingmodifiedBy");

            entity.HasOne(d => d.QbMapMasterAccountList).WithMany(p => p.QbOrgAccountMappings).HasConstraintName("FK_4136784_qbMapMasterAccountListID");

            entity.HasOne(d => d.QbOrgAccountList).WithMany(p => p.QbOrgAccountMappings).HasConstraintName("FK_180862_qbOrgAccountListID");
        });

        modelBuilder.Entity<QbProcessLog>(entity =>
        {
            entity.HasKey(e => e.QbProcessLogId).HasName("PK_qbProcessLogID");

            entity.ToTable("qbProcessLog", tb => tb.HasTrigger("qbProcessLog_InsertSeq"));

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QbProcessLogCreatedByNavigations).HasConstraintName("FK_9425133_createdBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.QbProcessLogModifiedByNavigations).HasConstraintName("FK_497586_modifiedBy");

            entity.HasOne(d => d.Organization).WithMany(p => p.QbProcessLogs).HasConstraintName("FK_4550299_organizationID");
        });

        modelBuilder.Entity<QbTokenDetail>(entity =>
        {
            entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.QbTokenDetails).HasConstraintName("FK_102948_qbTokenDetailscreatedBy");
        });

        modelBuilder.Entity<RoleMaster>(entity =>
        {
            entity.HasKey(e => e.RoleMasterId).HasName("PK_roleMasterID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RoleMasterCreatedByNavigations).HasConstraintName("FK_102948_roleMastercreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.RoleMasterModifiedByNavigations).HasConstraintName("FK_102948_roleMastermodifiedBy");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId).HasName("PK_rolePermissionID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RolePermissionCreatedByNavigations).HasConstraintName("FK_102948_rolePermissioncreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.RolePermissionModifiedByNavigations).HasConstraintName("FK_102948_rolePermissionmodifiedBy");

            entity.HasOne(d => d.Module).WithMany(p => p.RolePermissions).HasConstraintName("FK_8293209moduleID");

            entity.HasOne(d => d.RoleMaser).WithMany(p => p.RolePermissions).HasConstraintName("FK_6851953roleMaserID");
        });

        modelBuilder.Entity<RptBmktargetReport>(entity =>
        {
            entity.HasKey(e => e.RptBmktargetReportId).HasName("PK_rptBMKTargetReportID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.RptBmktargetReportCreatedByNavigations).HasConstraintName("FK_10294833_organizationcreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.RptBmktargetReportModifiedByNavigations).HasConstraintName("FK_10294833_organizationmodifiedBy");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StatesId).HasName("PK_statesID");

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StateCreatedByNavigations).HasConstraintName("FK_102948_statescreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.StateModifiedByNavigations).HasConstraintName("FK_102948_statesmodifiedBy");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UsersId).HasName("PK_usersID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsUserOtpRequired).HasDefaultValue(true);
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation).HasConstraintName("FK_102948_userscreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.InverseModifiedByNavigation).HasConstraintName("FK_102948_usersmodifiedBy");

            entity.HasOne(d => d.Organization).WithMany(p => p.Users).HasConstraintName("FK_5561404organizationID");
        });

        modelBuilder.Entity<UserForgotPwdToken>(entity =>
        {
            entity.HasKey(e => e.UserForgotPwdTokenId).HasName("PK_userForgotPwdTokenID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserForgotPwdTokenCreatedByNavigations).HasConstraintName("FK_102948_userForgotPwdTokencreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.UserForgotPwdTokenModifiedByNavigations).HasConstraintName("FK_102948_userForgotPwdTokenmodifiedBy");

            entity.HasOne(d => d.Users).WithMany(p => p.UserForgotPwdTokenUsers).HasConstraintName("FK_5692516usersID");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => e.UserGroupsId).HasName("PK_userGroupsID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserGroupCreatedByNavigations).HasConstraintName("FK_102948_userGroupscreatedBy");

            entity.HasOne(d => d.GroupTypes).WithMany(p => p.UserGroups).HasConstraintName("FK_4968673groupTypesID");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.UserGroupModifiedByNavigations).HasConstraintName("FK_102948_userGroupsmodifiedBy");
        });

        modelBuilder.Entity<UserGroupsMember>(entity =>
        {
            entity.HasKey(e => e.UserGroupsMembersId).HasName("PK_userGroupsMembersID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserGroupsMemberCreatedByNavigations).HasConstraintName("FK_102948_userGroupsMemberscreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.UserGroupsMemberModifiedByNavigations).HasConstraintName("FK_102948_userGroupsMembersmodifiedBy");

            entity.HasOne(d => d.UserGroups).WithMany(p => p.UserGroupsMembers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_23233_userGroupsID");

            entity.HasOne(d => d.Users).WithMany(p => p.UserGroupsMemberUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_23233_usersID");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK_userRoleID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserRoleCreatedByNavigations).HasConstraintName("FK_102948_userRolecreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.UserRoleModifiedByNavigations).HasConstraintName("FK_102948_userRolemodifiedBy");

            entity.HasOne(d => d.RoleMaster).WithMany(p => p.UserRoles).HasConstraintName("FK_420685roleMasterID");

            entity.HasOne(d => d.Users).WithMany(p => p.UserRoleUsers).HasConstraintName("FK_8547329usersID");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasKey(e => e.UserTokenId).HasName("PK_userTokenID");

            entity.Property(e => e.CreatedBy).HasDefaultValue(10000);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModifiedBy).HasDefaultValue(10000);
            entity.Property(e => e.ModifiedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserTokenCreatedByNavigations).HasConstraintName("FK_102948_userTokencreatedBy");

            entity.HasOne(d => d.ModifiedByNavigation).WithMany(p => p.UserTokenModifiedByNavigations).HasConstraintName("FK_102948_userTokenmodifiedBy");

            entity.HasOne(d => d.Users).WithMany(p => p.UserTokenUsers).HasConstraintName("FK_5257765usersID");
        });

        modelBuilder.Entity<VCategoryGroup>(entity =>
        {
            entity.ToView("vCategoryGroups");
        });

        modelBuilder.Entity<VOrganizationForPeerTeamProfile>(entity =>
        {
            entity.ToView("vOrganizationForPeerTeamProfile");
        });


        modelBuilder.Entity<VUserGroup>(entity =>
        {
            entity.ToView("vUserGroups");
        });

        modelBuilder.Entity<VUserGroupsMember>(entity =>
        {
            entity.ToView("vUserGroupsMember");
        });

        modelBuilder.Entity<VOrganizationSalary>(entity =>
        {
            entity.ToView("vOrganizationSalary");
        });

        modelBuilder.Entity<Vorganizaion>(entity =>
        {
            entity.ToView("vorganizaions");
        });

        modelBuilder.Entity<VqbAccount>(entity =>
        {
            entity.ToView("vqbAccounts");
        });

        modelBuilder.Entity<Vuser>(entity =>
        {
            entity.ToView("vusers");
        });

        modelBuilder.Entity<VwBmktargetReport>(entity =>
        {
            entity.ToView("vwBMKTargetReport");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
