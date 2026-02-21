using System.Reflection;
using System.Runtime.CompilerServices;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Entry point for convention-based registration of concrete, non-abstract classes. Provides static factory
///     methods to begin a registration chain from an assembly or a collection of types, filtering to classes only.
/// </summary>
public static class Classes
{
    /// <summary>
    ///     Begins registration from the specified collection of types, filtering to concrete, non-abstract classes.
    /// </summary>
    /// <param name="types">The types to select from.</param>
    public static ITypeSelector From(IEnumerable<Type> types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return new EnumerableTypeSelector(types, ClassFilter);
    }

    /// <summary>
    ///     Begins registration from the specified types, filtering to concrete, non-abstract classes.
    /// </summary>
    /// <param name="types">The types to select from.</param>
    public static ITypeSelector From(params Type[] types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return new EnumerableTypeSelector(types, ClassFilter);
    }

    /// <summary>
    ///     Begins registration by scanning the specified assembly for concrete, non-abstract classes.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    public static IAssemblyTypeSelector FromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        return new AssemblyTypeSelector(assembly, ClassFilter);
    }

    /// <summary>
    ///     Begins registration by scanning the assembly containing the specified type for concrete, non-abstract
    ///     classes.
    /// </summary>
    /// <param name="type">A type whose containing assembly will be scanned.</param>
    public static IAssemblyTypeSelector FromAssemblyContaining(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new AssemblyTypeSelector(type.Assembly, ClassFilter);
    }

    /// <summary>
    ///     Begins registration by scanning the assembly containing <typeparamref name="T"/> for concrete, non-abstract
    ///     classes.
    /// </summary>
    /// <typeparam name="T">A type whose containing assembly will be scanned.</typeparam>
    public static IAssemblyTypeSelector FromAssemblyContaining<T>()
    {
        return FromAssemblyContaining(typeof(T));
    }

    /// <summary>
    ///     Begins registration by scanning the calling assembly for concrete, non-abstract classes.
    /// </summary>
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
