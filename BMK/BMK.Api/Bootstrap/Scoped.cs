#region Namespace
using BMK.Api.Services;
using BMK.BoundedContext.DbContext.Main;
using BMK.Domain.Domain;
using BMK.Infrastructure.Logs;
using BMK.Infrastructure.Security;
using BMK.UnitOfWork.DbEntityAudit;
using BMK.UnitOfWork.Main;
using Microsoft.Extensions.Configuration;
using RxWeb.Core.Annotations;
using RxWeb.Core.Common.Extensions;
using RxWeb.Core.Data;
using RxWeb.Core.Security;
using RxWeb.Core.Security.Cryptography;

#endregion Namespace



namespace BMK.Api.Bootstrap
{
    public static class ScopedExtension
    {

        public static void AddScopedService(this IServiceCollection serviceCollection, ConfigurationManager configuration)
        {
            serviceCollection.AddScoped<IRepositoryProvider, RepositoryProvider>();
            serviceCollection.AddScoped<ITokenAuthorizer, TokenAuthorizer>();
            serviceCollection.AddScoped<IModelValidation, ModelValidation>();
            serviceCollection.AddScoped<IAuditLog, AuditLog>();
            serviceCollection.AddScoped<IApplicationTokenProvider, ApplicationTokenProvider>();
            serviceCollection.AddScoped(typeof(IDbContextManager<>), typeof(DbContextManager<>));
            serviceCollection.AddScoped<IPasswordHash, PasswordHash>();

            SmsServiceExtension.AddSmsService(serviceCollection, configuration);
            EmailServiceExtension.AddEmailService(serviceCollection, configuration);
            #region ContextService

            serviceCollection.AddScoped<ILoginContext, LoginContext>();
            serviceCollection.AddScoped<ILoginUow, LoginUow>();

            serviceCollection.AddScoped<IUserContext, UserContext>();
            serviceCollection.AddScoped<IUserUow, UserUow>();

            serviceCollection.AddScoped<IExceptionContext, ExceptionContext>();
            serviceCollection.AddScoped<IExceptionUow, ExceptionUow>();

            serviceCollection.AddScoped<IQuickBooksContext, QuickBooksContext>();
            serviceCollection.AddScoped<IQBUow, QBUow>();

            serviceCollection.AddScoped<IAccountMappingContext, AccountMappingContext>();
            serviceCollection.AddScoped<IAccountMappingUow, AccountMappingUow>();

            serviceCollection.AddScoped<IQuickBooksService, QuickBooksService>();

            serviceCollection.AddScoped<IPeerTeamContext, PeerTeamContext>();
            serviceCollection.AddScoped<IPeerTeamUow, PeerTeamUow>();

            serviceCollection.AddScoped<IObjectiveContext, ObjectiveContext>();
            serviceCollection.AddScoped<IObjectiveUow, ObjectiveUow>();

            serviceCollection.AddScoped<IEventContext, EventContext>();
            serviceCollection.AddScoped<IEventUow, EventUow>();
            #endregion ContextService

            #region DomainService        
            serviceCollection.AddScoped<IUserDomain, UserDomain>();
            serviceCollection.AddScoped<IOrganizationDoomain, OrganizationDomain>();
            serviceCollection.AddScoped<IOrganizationSalaryDomain, OrganizationSalaryDomain>();
            serviceCollection.AddScoped<ILogException, LogException>();
            serviceCollection.AddScoped<IQuickBookDomain, QuickBookDomain>();
            serviceCollection.AddScoped<IAccountMappingDomain, AccountMappingDomain>();
            serviceCollection.AddScoped<IRoleDomain, RoleDomain>();
            serviceCollection.AddScoped<IBmkTargetsDomain, BmkTargetsDomain>();
            serviceCollection.AddScoped<IEmailTemplateDomain, EmailTemplateDomain>();
            serviceCollection.AddScoped<IMonthlyFinancialRecordDomain, MonthlyFinancialRecordDomain>();
            serviceCollection.AddScoped<IQuickBookSummaryDomain, QuickBookSummaryDomain>();
            serviceCollection.AddScoped<IPeerTeamDomain, PeerTeamDomain>();
            serviceCollection.AddScoped<IObjectiveDomain,ObjectiveDomain>();
            serviceCollection.AddScoped<IEventDomain, EventDomain>();
            serviceCollection.AddScoped<IBmkScheduleMeetingDomain, BmkScheduleMeetingDomain>();

            serviceCollection.AddScoped<IPowerBIDomain, PowerBIDomain>();
            serviceCollection.AddScoped<IBlobService, BlobService>();
            #endregion DomainService
        }
    }
}




