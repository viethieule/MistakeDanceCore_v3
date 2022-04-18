using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceModule(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddDbContext<MistakeDanceDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("MistakeDanceDatabase"));
            });

            serviceCollection.AddScoped<IMistakeDanceDbContext, MistakeDanceDbContext>();

            return serviceCollection;
        }
    }
}