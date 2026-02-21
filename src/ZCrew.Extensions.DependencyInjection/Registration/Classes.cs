using System.Reflection;
using System.Runtime.CompilerServices;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public static class Classes
{
    public static ITypeSelector From(IEnumerable<Type> types)
    {
        return new EnumerableTypeSelector(types, ClassFilter);
    }

    public static ITypeSelector From(params Type[] types)
    {
        return new EnumerableTypeSelector(types, ClassFilter);
    }

    public static IAssemblyTypeSelector FromAssembly(Assembly assembly)
    {
        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }
        return new AssemblyTypeSelector(assembly, ClassFilter);
    }

    public static IAssemblyTypeSelector FromAssemblyContaining(Type type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        return new AssemblyTypeSelector(type.Assembly, ClassFilter);
    }

    public static IAssemblyTypeSelector FromAssemblyContaining<T>()
    {
        return FromAssemblyContaining(typeof(T));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IAssemblyTypeSelector FromThisAssembly()
    {
        return FromAssembly(Assembly.GetCallingAssembly());
    }

    private static bool ClassFilter(Type type)
    {
        return type is { IsClass: true, IsAbstract: false };
    }
}
