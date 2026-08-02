using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using ZCrew.Extensions.CodeAnalysis.CSharp;
using ZCrew.Extensions.DependencyInjection.Registration;

namespace ZCrew.Extensions.DependencyInjection.Generator.Registration;

/// <summary>
///     Renders the arguments a <c>[Service]</c> type contributes to its <c>Service.From(typeof(impl), ...)</c> call:
///     the lifetime (from <c>[Singleton]</c>/<c>[Scoped]</c>/<c>[Transient]</c>, defaulting to singleton), the
///     implementation key (from <c>[Keyed]</c>), and each service type paired with its key (from <c>[As]</c> /
///     <c>[As&lt;T&gt;]</c>). The modifier attributes are matched by name in the embedded attributes' namespace.
/// </summary>
internal static partial class ServiceConstructionRenderer
{
    private const string LifetimeType = "global::Microsoft.Extensions.DependencyInjection.ServiceLifetime";

    private static readonly SymbolDisplayFormat FullyQualified = SymbolDisplayFormat.FullyQualifiedFormat;

    public static string Render(INamedTypeSymbol type)
    {
        var attributes = type.GetAttributes();

        var builder = new StringBuilder();
        builder.Append(RenderLifetime(attributes)).Append(", ").Append(RenderImplementationKey(attributes));

        foreach (var attribute in attributes)
        {
            if (IsAsAttribute(attribute.AttributeClass) || IsAsOfTAttribute(attribute.AttributeClass))
            {
                builder.Append(", ").Append(RenderServiceType(attribute.AttributeClass, attribute));
            }
        }

        return builder.ToString();
    }

    private static string RenderLifetime(ImmutableArray<AttributeData> attributes)
    {
        // First matching lifetime marker wins; a type carrying more than one is reported by the analyzer (ZCDI004).
        foreach (var attribute in attributes)
        {
            if (IsSingletonAttribute(attribute.AttributeClass))
            {
                return LifetimeType + ".Singleton";
            }

            if (IsScopedAttribute(attribute.AttributeClass))
            {
                return LifetimeType + ".Scoped";
            }

            if (IsTransientAttribute(attribute.AttributeClass))
            {
                return LifetimeType + ".Transient";
            }
        }

        return LifetimeType + ".Singleton";
    }

    private static string RenderImplementationKey(ImmutableArray<AttributeData> attributes)
    {
        foreach (var attribute in attributes)
        {
            if (IsKeyedAttribute(attribute.AttributeClass) && attribute.ConstructorArguments.Length > 0)
            {
                return AttributeArgumentRenderer.RenderTypedConstant(attribute.ConstructorArguments[0]);
            }
        }

        return "null";
    }

    private static string RenderServiceType(INamedTypeSymbol attributeClass, AttributeData attribute)
    {
        string serviceType;
        string key;

        if (attributeClass.IsGenericType)
        {
            // [As<T>(object? key = null)]
            serviceType = $"typeof({attributeClass.TypeArguments[0].ToDisplayString(FullyQualified)})";
            key =
                attribute.ConstructorArguments.Length > 0
                    ? AttributeArgumentRenderer.RenderTypedConstant(attribute.ConstructorArguments[0])
                    : "null";
        }
        else
        {
            // [As(Type serviceType, object? key = null)]
            serviceType =
                attribute.ConstructorArguments.Length > 0
                    ? AttributeArgumentRenderer.RenderTypedConstant(attribute.ConstructorArguments[0])
                    : "typeof(object)";
            key =
                attribute.ConstructorArguments.Length > 1
                    ? AttributeArgumentRenderer.RenderTypedConstant(attribute.ConstructorArguments[1])
                    : "null";
        }

        return $"({serviceType}, {key})";
    }

    [IsType<SingletonAttribute>]
    private static partial bool IsSingletonAttribute(INamedTypeSymbol? attributeClass);

    [IsType<ScopedAttribute>]
    private static partial bool IsScopedAttribute(INamedTypeSymbol? attributeClass);

    [IsType<TransientAttribute>]
    private static partial bool IsTransientAttribute(INamedTypeSymbol? attributeClass);

    [IsType<KeyedAttribute>]
    private static partial bool IsKeyedAttribute(INamedTypeSymbol? attributeClass);

    [IsType<AsAttribute>]
    private static partial bool IsAsAttribute(INamedTypeSymbol? attributeClass);

    [IsType(typeof(AsAttribute<>))]
    private static partial bool IsAsOfTAttribute(INamedTypeSymbol? attributeClass);
}
