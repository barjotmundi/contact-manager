using ContactManager.Repositories;
using ContactManager.Services;
namespace ContactManager.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContactManagerServices(this IServiceCollection services)
        {
            services.AddSingleton<IContactRepository, InMemoryContactRepository>();
            services.AddSingleton<IContactService, ContactService>();
            return services;
        }
    }
}
