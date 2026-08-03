using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Entry point for convention-based registration of all types (classes, structs, interfaces, etc.). Provides
///     static factory methods to begin a registration chain from an assembly or a collection of types without any
///     type-kind filter.
/// </summary>
/// <remarks>
///     This API is not compatible with trimming or native AOT. Use the <c>[Service]</c> source generator
///     (<c>Services.FromThisAssembly()</c>) or <see cref="Service.From(Type)"/> instead.
/// </remarks>
public static class Types
{
    /// <summary>
    ///     Begins registration from the specified collection of types.
    /// </summary>
    /// <param name="types">The types to select from.</param>
    [RequiresUnreferencedCode(Classes.RequiresUnreferencedCodeMessage)]
    public static TypeFilter From(IEnumerable<Type> types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return new TypeFilter(types);
    }

    /// <summary>
    ///     Begins registration from the specified types.
    /// </summary>
    /// <param name="types">The types to select from.</param>
    [RequiresUnreferencedCode(Classes.RequiresUnreferencedCodeMessage)]
    public static TypeFilter From(params Type[] types)
    {
        ArgumentNullException.ThrowIfNull(types);
        return new TypeFilter(types);
    }

    /// <summary>
    ///     Begins registration by scanning the specified assembly for all types.
    /// </summary>
    /// <param name="assembly">The assembly to scan.</param>
    [RequiresUnreferencedCode(Classes.RequiresUnreferencedCodeMessage)]
    public static AssemblyTypeSelector FromAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        return new AssemblyTypeSelector(assembly, filter: null);
    }

    /// <summary>
    ///     Begins registration by scanning the assembly containing the specified type.
    /// </summary>
    /// <param name="type">A type whose containing assembly will be scanned.</param>
    [RequiresUnreferencedCode(Classes.RequiresUnreferencedCodeMessage)]
    public static AssemblyTypeSelector FromAssemblyContaining(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new AssemblyTypeSelector(type.Assembly, filter: null);
    }

    /// <summary>
    ///     Begins registration by scanning the assembly containing <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">A type whose containing assembly will be scanned.</typeparam>
    [RequiresUnreferencedCode(Classes.RequiresUnreferencedCodeMessage)]
    public static AssemblyTypeSelector FromAssemblyContaining<T>()
    {
        return FromAssemblyContaining(typeof(T));
    }

    /// <summary>
    ///     Begins registration by scanning the calling assembly for all types.
    /// </summary>
    [RequiresUnreferencedCode(Classes.RequiresUnreferencedCodeMessage)]
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static AssemblyTypeSelector FromThisAssembly()
    {
        return FromAssembly(Assembly.GetCallingAssembly());
    }
}
