namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class TypeFilterExtensions
{
    extension(TypeFilter typeFilter)
    {
        /// <summary>
        ///     Filters to types where <see cref="Type.IsGenericType"/> is <see langword="true"/>.
        /// </summary>
        /// <example>
        ///     <list type="bullet">
        ///         <item><c>typeof(Repository&lt;Customer&gt;)</c></item>
        ///         <item><c>typeof(Cache&lt;T, long&gt;)</c></item>
        ///         <item><c>typeof(Validator&lt;&gt;)</c></item>
        ///     </list>
        /// </example>
        /// <remarks>
        ///     This is often implied if using other generic methods like: <see cref="GenericTypeDefinitions"/> or
        ///     <see cref="ConstructedGenericTypes"/>, so this can be skipped when calling those other methods.
        /// </remarks>
        TypeFilter GenericTypes()
        {
            return typeFilter.Where(type => type.IsGenericType);
        }

        /// <summary>
        ///     Filters to types where <see cref="Type.IsGenericTypeDefinition"/> is <see langword="true"/> because the
        ///     type is generic and has open generic type parameters.
        /// </summary>
        /// <example>
        ///     <list type="bullet">
        ///         <item><c>typeof(Repository&lt;T&gt;)</c></item>
        ///         <item><c>typeof(Cache&lt;T, long&gt;)</c></item>
        ///         <item><c>typeof(Validator&lt;&gt;)</c></item>
        ///     </list>
        /// </example>
        /// <remarks>
        ///     When <see cref="Type.IsGenericType"/> is <see langword="true"/> then <b>exactly one of</b>
        ///     <see cref="Type.IsGenericTypeDefinition"/> and <see cref="Type.IsConstructedGenericType"/> will be
        ///     <see langword="true"/>. This means that <see cref="GenericTypeDefinitions"/> and
        ///     <see cref="ConstructedGenericTypes"/> are mutually exclusive and will not select the same types.
        /// </remarks>
        TypeFilter GenericTypeDefinitions()
        {
            return typeFilter.Where(type => type.IsGenericTypeDefinition);
        }

        /// <summary>
        ///     Filters to types where <see cref="Type.IsGenericTypeDefinition"/> is <see langword="true"/> because the
        ///     type is generic and does not have open generic type parameters.
        /// </summary>
        /// <example>
        ///     <list type="bullet">
        ///         <item><c>typeof(Repository&lt;Customer&gt;)</c></item>
        ///         <item><c>typeof(Cache&lt;Customer, long&gt;)</c></item>
        ///         <item><c>typeof(Validator&lt;Customer&gt;)</c></item>
        ///     </list>
        /// </example>
        /// <remarks>
        ///     When <see cref="Type.IsGenericType"/> is <see langword="true"/> then <b>exactly one of</b>
        ///     <see cref="Type.IsGenericTypeDefinition"/> and <see cref="Type.IsConstructedGenericType"/> will be
        ///     <see langword="true"/>. This means that <see cref="GenericTypeDefinitions"/> and
        ///     <see cref="ConstructedGenericTypes"/> are mutually exclusive and will not select the same types.
        /// </remarks>
        TypeFilter ConstructedGenericTypes()
        {
            return typeFilter.Where(type => type.IsConstructedGenericType);
        }
    }
}
