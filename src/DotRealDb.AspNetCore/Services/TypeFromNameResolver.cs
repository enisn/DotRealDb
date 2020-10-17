using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotRealDb.AspNetCore.Services
{
    public class TypeFromNameResolver : ITypeFromNameResolver
    {
        private readonly Dictionary<string, Type> _alreadyFoundTypes = new Dictionary<string, Type>();

        private readonly IServiceProvider serviceProvider;
        public TypeFromNameResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public object Resolve(string name)
        {
            var type = GetTypeFromName(name);
            return serviceProvider.GetRequiredService(type);
        }

        public T Resolve<T>(string name)
        {
            var type = GetTypeFromName(name);

            if (type != typeof(T))
            {
                throw new ArgumentException($"Generic parameter T ({typeof(T).FullName}) doesn't match the given type name ('{name}'). {type.FullName} is found by given {name} parameter.");
            }

            return (T)serviceProvider.GetRequiredService(type);
        }

        private Type GetTypeFromName(string name)
        {
            var existing = _alreadyFoundTypes[name];
            if (existing != null)
                return existing;

            var found = FindTypeFromName(name);
            if (found == null)
                throw new Exception($"'{name}' type couldn't be found in any loaded assembly on application. Be sure your project have a class with name '{name}'");

            _alreadyFoundTypes.Add(name, found);

            return found;
        }

        private Type FindTypeFromName(string name)
        {
            var type = FindTypeInAssembly(name, Assembly.GetExecutingAssembly());

            if (type == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = FindTypeInAssembly(name, assembly);
                    if (type != null)
                        break;
                }
            }

            return type;
        }

        private Type FindTypeInAssembly(string name, Assembly assembly)
            => assembly.GetTypes().FirstOrDefault(x => x.Name == name);
    }
}
