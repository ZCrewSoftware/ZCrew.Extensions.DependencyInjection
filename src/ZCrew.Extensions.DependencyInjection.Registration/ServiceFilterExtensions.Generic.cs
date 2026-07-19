namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceFilterExtensions
{
    extension(ServiceFilter filter)
    {
        /// <summary>
        ///     Filters to services whose implementation type is generic (<see cref="Type.IsGenericType"/>).
        /// </summary>
        /// <remarks>
        ///     This is often implied when using <see cref="GenericTypeDefinitions"/> or
        ///     <see cref="ConstructedGenericTypes"/>, so it can be skipped when calling those.
        /// </remarks>
        public ServiceFilter GenericTypes()
        {
            return filter.Where(service => service.ImplementationType.IsGenericType);
        }

        /// <summary>
        ///     Filters to services whose implementation type is an open generic definition
        ///     (<see cref="Type.IsGenericTypeDefinition"/>), for example <c>Repository&lt;&gt;</c>.
        /// </summary>
        /// <remarks>
        ///     <see cref="GenericTypeDefinitions"/> and <see cref="ConstructedGenericTypes"/> are mutually exclusive.
        /// </remarks>
        public ServiceFilter GenericTypeDefinitions()
        {
            return filter.Where(service => service.ImplementationType.IsGenericTypeDefinition);
        }

        /// <summary>
        ///     Filters to services whose implementation type is a constructed generic
        ///     (<see cref="Type.IsConstructedGenericType"/>), for example <c>Repository&lt;Customer&gt;</c>.
        /// </summary>
        /// <remarks>
        ///     <see cref="GenericTypeDefinitions"/> and <see cref="ConstructedGenericTypes"/> are mutually exclusive.
        /// </remarks>
        public ServiceFilter ConstructedGenericTypes()
        {
            return filter.Where(service => service.ImplementationType.IsConstructedGenericType);
        }
    }
}
