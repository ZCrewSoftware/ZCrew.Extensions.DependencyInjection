using System.Reflection;
using System.Runtime.CompilerServices;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Entry point for convention-based registration of all types (classes, structs, interfaces, etc.). Provides
///     static factory methods to begin a registration chain from an assembly or a collection of types without any
///     type-kind filter.
/// </summary>
public static class Types
{
    /// <summary>
    ///     Begins registration from the specified collection of types.
    /// </summary>
    /// <param name="types">The types to select from.</param>
    public static ITypeSelector From(IEnumerable<Type> types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return new EnumerableTypeSelector(types);
    }

    /// <summary>
    ///     Begins registration from the specified types.
    /// </summary>
    /// <param name="types">The types to select from.</param>
    public static ITypeSelector From(params Type[] types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return new EnumerableTypeSelector(types);
    }

    /// <summary>
    ///     Begins registration by scanning the specified assembly for all types.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    public static IAssemblyTypeSelector FromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        return new AssemblyTypeSelector(assembly);
    }

    /// <summary>
    ///     Begins registration by scanning the assembly containing the specified type.
    /// </summary>
    /// <param name="type">A type whose containing assembly will be scanned.</param>
    public static IAssemblyTypeSelector FromAssemblyContaining(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new AssemblyTypeSelector(type.Assembly);
    }

    /// <summary>
    ///     Begins registration by scanning the assembly containing <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">A type whose containing assembly will be scanned.</typeparam>
    public static IAssemblyTypeSelector FromAssemblyContaining<T>()
    {
        return FromAssemblyContaining(typeof(T));
    }

    /// <summary>
    ///     Begins registration by scanning the calling assembly for all types.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IAssemblyTypeSelector FromThisAssembly()
    {
        return FromAssembly(Assembly.GetCallingAssembly());
    }
}
