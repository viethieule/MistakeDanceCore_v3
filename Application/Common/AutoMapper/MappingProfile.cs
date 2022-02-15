using System.Reflection;
using AutoMapper;

namespace Application.Common.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            List<Type> types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (Type type in types)
            {
                var instance = Activator.CreateInstance(type);
                var method = type.GetMethod("Mapping");
                if (method != null)
                {
                    method.Invoke(instance, new object[] { this });
                }
            }
        }
    }
}