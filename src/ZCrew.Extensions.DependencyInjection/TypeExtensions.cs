using System.Reflection;

namespace ZCrew.Extensions.DependencyInjection;

public static class TypeExtensions
{
    extension(Type type)
    {
        public bool HasAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            return type.GetTypeInfo().IsDefined(typeof(TAttribute));
        }

        public bool HasAttribute<TAttribute>(Func<TAttribute, bool> filter)
            where TAttribute : Attribute
        {
            var attribute = type.GetTypeInfo().GetCustomAttribute<TAttribute>();
            return attribute != null && filter(attribute);
        }

        public bool IsInNamespace(string? @namespace)
        {
            return type.IsInNamespace(@namespace, false);
        }

        public bool IsInNamespace(string? @namespace, bool includeSubnamespaces)
        {
            if (includeSubnamespaces)
            {
                return type.Namespace == @namespace
                    || type.Namespace != null && type.Namespace.StartsWith(@namespace + ".");
            }

            return type.Namespace == @namespace;
        }

        public bool IsInSameNamespaceAs(Type otherType)
        {
            return type.IsInNamespace(otherType.Namespace);
        }

        public bool IsInSameNamespaceAs(Type otherType, bool includeSubnamespaces)
        {
            return type.IsInNamespace(otherType.Namespace, includeSubnamespaces);
        }

        public bool IsInSameNamespaceAs<T>()
        {
            return type.IsInSameNamespaceAs(typeof(T));
        }

        public bool IsInSameNamespaceAs<T>(bool includeSubnamespaces)
        {
            return type.IsInSameNamespaceAs(typeof(T), includeSubnamespaces);
        }

        public string GetInterfaceName()
        {
            var name = type.Name;
            if (name.Length > 1 && name[0] == 'I' && char.IsUpper(name[1]))
            {
                return name[1..];
            }
            return name;
        }

        // Returns all interfaces defined on the class and base classes, excluding base interfaces
        // this name may not be the best fit
        public IEnumerable<Type> GetTopLevelInterfaces()
        {
            var interfaces = type.GetInterfaces();
            var topLevel = new List<Type>(interfaces);

            foreach (var @interface in interfaces)
            {
                foreach (var parent in @interface.GetInterfaces())
                {
                    topLevel.Remove(parent);
                }
            }

            return topLevel;
        }
    }
}
