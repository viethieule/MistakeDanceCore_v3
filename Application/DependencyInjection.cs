using System.Reflection;
using Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationModule(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScopedAsSelfByConvention(typeof(BaseService<,>).Assembly, type => type.Name.EndsWith("Service"));
            serviceCollection.AddScopedAsSelfByConvention(typeof(DTCBase<,>).Assembly, type => type.Name.EndsWith("DTC"));
            
            return serviceCollection;
        }

        public static IServiceCollection AddScopedAsSelfByConvention(this IServiceCollection services, Assembly assembly, Predicate<Type> predicate)
        {
            List<Type> implementations = assembly.ExportedTypes
                .Where(x => !x.IsInterface && !x.IsAbstract && predicate.Invoke(x))
                .ToList();

            foreach (Type implementation in implementations)
            {
                services.AddScoped(implementation);
            }

            return services;
        }
    }
}