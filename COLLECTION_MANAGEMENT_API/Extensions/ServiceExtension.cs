using COLLECTION_MANAGEMENT_REPOSITORY.Interface;
using COLLECTION_MANAGEMENT_REPOSITORY.Repository;
using COLLECTION_MANAGEMENT_REPOSITORY.UoW;
using COLLECTION_MANAGEMENT_SERVICE.Interface;
using COLLECTION_MANAGEMENT_SERVICE.Manager;

namespace COLLECTION_MANAGEMENT_API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
        {
            try
            {
                services.AddScoped<ICommonRepository, CommonRepository>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();

                return services;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
        {
			try
			{
                services.AddScoped<ICommonManager, CommonManager>();
                services.AddScoped<IResponseMessageCacheManager, ResponseMessageCacheManager>();

                services.AddScoped<IRoleManager, RoleManager>();
                services.AddScoped<IUserRoleManager, UserRoleManager>();
                services.AddScoped<IModuleManager, ModuleManager>();
                services.AddScoped<IMenuManager, MenuManager>();
                services.AddScoped<IMenuPermissionManager, MenuPermissionManager>();
                services.AddScoped<IUserManager, UserManager >();

                return services;
            }
			catch (Exception ex)
			{

				throw;
			}
        }
    }
}
