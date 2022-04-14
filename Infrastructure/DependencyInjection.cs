using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureModule(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}