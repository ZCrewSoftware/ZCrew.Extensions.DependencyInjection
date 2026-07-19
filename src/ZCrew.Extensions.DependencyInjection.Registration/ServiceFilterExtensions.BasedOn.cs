namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceFilterExtensions
{
    extension(ServiceFilter filter)
    {
        /// <summary>
        ///     Filters to services whose implementation implements or inherits from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The base type or interface to filter on.</typeparam>
        public ServiceFilter BasedOn<T>()
        {
            return filter.Where(service => service.ImplementationType.IsBasedOn(typeof(T)));
        }

        /// <summary>
        ///     Filters to services whose implementation implements or inherits from <paramref name="baseType"/>.
        /// </summary>
        /// <param name="baseType">The base type or interface to filter on.</param>
        public ServiceFilter BasedOn(Type baseType)
        {
            ArgumentNullException.ThrowIfNull(baseType);
            return filter.Where(service => service.ImplementationType.IsBasedOn(baseType));
        }

        /// <summary>
        ///     Filters to services whose implementation implements or inherits from any of the
        ///     <paramref name="baseTypes"/>.
        /// </summary>
        /// <param name="baseTypes">The base types or interfaces to filter on.</param>
        public ServiceFilter BasedOn(params Type[] baseTypes)
        {
            ArgumentNullException.ThrowIfNull(baseTypes);
            return filter.Where(service =>
                Array.Exists(baseTypes, baseType => service.ImplementationType.IsBasedOn(baseType))
            );
        }
    }
}
