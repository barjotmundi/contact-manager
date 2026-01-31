using ContactManager.Repositories;
using ContactManager.Services;
namespace ContactManager.Extensions
{
    public static class ServiceCollectionExtensions
    {
        // Extension method to keep DI registrations in one place and keep Program.cs clean
        public static IServiceCollection AddContactManagerServices(this IServiceCollection services)
        {
            // Register the repository and service in the DI container
            services.AddSingleton<IContactRepository, InMemoryContactRepository>();
            services.AddSingleton<IContactService, ContactService>();
            return services;
        }
    }
}
