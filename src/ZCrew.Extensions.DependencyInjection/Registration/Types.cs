using System.Reflection;
using System.Runtime.CompilerServices;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public static class Types
{
    public static ITypeSelector From(IEnumerable<Type> types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return new EnumerableTypeSelector(types);
    }

    public static ITypeSelector From(params Type[] types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return new EnumerableTypeSelector(types);
    }

    public static IAssemblyTypeSelector FromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        return new AssemblyTypeSelector(assembly);
    }

    public static IAssemblyTypeSelector FromAssemblyContaining(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new AssemblyTypeSelector(type.Assembly);
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
}
