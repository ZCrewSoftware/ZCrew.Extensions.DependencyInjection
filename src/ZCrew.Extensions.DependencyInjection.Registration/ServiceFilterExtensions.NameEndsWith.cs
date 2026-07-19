using System.Globalization;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class ServiceFilterExtensions
{
    extension(ServiceFilter filter)
    {
        /// <summary>
        ///     Filters to services whose implementation type name ends with the <paramref name="suffix"/> with the same
        ///     casing. The generic arity is discarded before matching, so <see cref="List{T}"/> would end with
        ///     <c>"List"</c>.
        /// </summary>
        /// <param name="suffix">The string to compare to the substring at the end of each implementation name.</param>
        public ServiceFilter NameEndsWith(string suffix)
        {
            return filter.Where(service => service.ImplementationType.GetNonGenericName().EndsWith(suffix));
        }

        /// <summary>
        ///     Filters to services whose implementation type name ends with the <paramref name="suffix"/>. The generic
        ///     arity is discarded before matching, so <see cref="List{T}"/> would end with <c>"List"</c>.
        /// </summary>
        /// <param name="suffix">The string to compare to the substring at the end of each implementation name.</param>
        /// <param name="ignoreCase">
        ///     If <see langword="true"/>, then the suffix is matched ignoring case.
        ///     If <see langword="false"/>, then the suffix is matched with the exact casing.
        /// </param>
        /// <remarks>
        ///     <see cref="CultureInfo.CurrentCulture"/> is used here.
        /// </remarks>
        public ServiceFilter NameEndsWith(string suffix, bool ignoreCase)
        {
            return filter.NameEndsWith(suffix, ignoreCase, CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Filters to services whose implementation type name ends with the <paramref name="suffix"/>. The generic
        ///     arity is discarded before matching, so <see cref="List{T}"/> would end with <c>"List"</c>.
        /// </summary>
        /// <param name="suffix">The string to compare to the substring at the end of each implementation name.</param>
        /// <param name="ignoreCase">
        ///     If <see langword="true"/>, then the suffix is matched ignoring case.
        ///     If <see langword="false"/>, then the suffix is matched with the exact casing.
        /// </param>
        /// <param name="cultureInfo">
        ///     Cultural information that determines how this instance and value are compared. If culture is
        ///     <see langword="null"/>, the current culture is used.
        /// </param>
        public ServiceFilter NameEndsWith(string suffix, bool ignoreCase, CultureInfo? cultureInfo)
        {
            return filter.Where(service =>
                service.ImplementationType.GetNonGenericName().EndsWith(suffix, ignoreCase, cultureInfo)
            );
        }

        /// <summary>
        ///     Filters to services whose implementation type name ends with the <paramref name="suffix"/>. The generic
        ///     arity is discarded before matching, so <see cref="List{T}"/> would end with <c>"List"</c>.
        /// </summary>
        /// <param name="suffix">The string to compare to the substring at the end of each implementation name.</param>
        /// <param name="comparisonType">
        ///     One of the enumeration values that determines how this string and value are compared.
        /// </param>
        public ServiceFilter NameEndsWith(string suffix, StringComparison comparisonType)
        {
            return filter.Where(service =>
                service.ImplementationType.GetNonGenericName().EndsWith(suffix, comparisonType)
            );
        }
    }
}
