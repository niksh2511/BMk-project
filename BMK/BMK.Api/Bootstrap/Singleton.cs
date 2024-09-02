using BMK.Api.Services;
using BMK.BoundedContext.Singleton;
using BMK.Infrastructure.Singleton;
using RxWeb.Core.Data;

namespace BMK.Api.Bootstrap
{
    public static class Singleton
    {
        public static void AddSingletonService(this IServiceCollection serviceCollection)
        {
            #region Singleton
            serviceCollection.AddSingleton<ITenantDbConnectionInfo, TenantDbConnectionInfo>();
            serviceCollection.AddSingleton(typeof(UserAccessConfigInfo));
            serviceCollection.AddSingleton<ISessionProvider, SessionProvider>();
            //serviceCollection.AddSingleton<IQuickBooksService, QuickBooksService>();
            #endregion Singleton
        }

    }
}




