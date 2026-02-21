using System.Reflection;
using System.Runtime.CompilerServices;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public static class Classes
{
    public static ITypeSelector From(IEnumerable<Type> types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return new EnumerableTypeSelector(types, ClassFilter);
    }

    public static ITypeSelector From(params Type[] types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return new EnumerableTypeSelector(types, ClassFilter);
    }

    public static IAssemblyTypeSelector FromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        return new AssemblyTypeSelector(assembly, ClassFilter);
    }

    public static IAssemblyTypeSelector FromAssemblyContaining(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
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
