using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ZCrew.Extensions.CodeAnalysis.CSharp.Text;

namespace ZCrew.Extensions.DependencyInjection.Generator.Registration;

/// <summary>
///     Re-emits an <see cref="AttributeData"/> as the C# construction expression the user wrote, for example
///     <c>new global::ZCrew.Extensions.DependencyInjection.Registration.ServiceAttribute(typeof(global::Sample.IFoo))</c>.
///     The positional arguments are rendered from <see cref="AttributeData.ConstructorArguments"/> so the selected
///     constructor overload is reproduced exactly; named arguments become an object initializer over the <c>init</c>
///     properties.
/// </summary>
internal static class AttributeArgumentRenderer
{
    private static readonly SymbolDisplayFormat FullyQualified = SymbolDisplayFormat.FullyQualifiedFormat;

    public static string RenderConstruction(AttributeData attribute)
    {
        var typeName = attribute.AttributeClass?.ToDisplayString(FullyQualified) ?? "object";

        var builder = new FormattedStringBuilder("new ")
            .Append(typeName)
            .Append('(')
            .AppendJoined(attribute.ConstructorArguments, ", ", (b, a) => b.Append(RenderTypedConstant(a)))
            .Append(')');

        if (attribute.NamedArguments.Length > 0)
        {
            // Don't use a block here to render single-line
            builder
                .Append(" { ")
                .AppendJoined(
                    attribute.NamedArguments,
                    ", ",
                    (b, a) =>
                    {
                        b.Append(a.Key).Append(" = ").Append(RenderTypedConstant(a.Value));
                    }
                )
                .Append(" }");
        }

        return builder.ToString();
    }

    public static string RenderTypedConstant(TypedConstant constant)
    {
        if (constant.IsNull)
        {
            return "null";
        }

        switch (constant.Kind)
        {
            case TypedConstantKind.Type:
                return $"typeof({((ITypeSymbol)constant.Value!).ToDisplayString(FullyQualified)})";
            case TypedConstantKind.Enum:
                return RenderEnum(constant);
            case TypedConstantKind.Array:
                return RenderArray(constant);
            default:
                return RenderPrimitive(constant);
        }
    }

    private static string RenderEnum(TypedConstant constant)
    {
        var enumTypeName = constant.Type!.ToDisplayString(FullyQualified);

        // Prefer the named member so output reads as the user wrote it.
        foreach (var member in constant.Type.GetMembers().OfType<IFieldSymbol>())
        {
            if (member.HasConstantValue && Equals(member.ConstantValue, constant.Value))
            {
                return $"{enumTypeName}.{member.Name}";
            }
        }

        // Fall back to a cast for combined or unnamed values.
        return $"(({enumTypeName}){constant.Value})";
    }

    private static string RenderArray(TypedConstant constant)
    {
        var elementType = constant.Type is IArrayTypeSymbol array
            ? array.ElementType.ToDisplayString(FullyQualified)
            : "object";
        var elements = string.Join(", ", constant.Values.Select(RenderTypedConstant));
        return $"new {elementType}[] {{ {elements} }}";
    }

    private static string RenderPrimitive(TypedConstant constant)
    {
        // Non-null primitive values always format, so the result is never null here.
        var literal = SymbolDisplay.FormatPrimitive(constant.Value!, quoteStrings: true, useHexadecimalNumbers: false)!;

        // The service key parameter is object?, so the boxed runtime type must match what the user wrote. string,
        // bool, char and int literals already box to the right type; widen the rest with an explicit cast.
        return constant.Type?.SpecialType switch
        {
            SpecialType.System_String
            or SpecialType.System_Boolean
            or SpecialType.System_Char
            or SpecialType.System_Int32
            or null => literal,
            _ => $"({constant.Type!.ToDisplayString(FullyQualified)}){literal}",
        };
    }
}
