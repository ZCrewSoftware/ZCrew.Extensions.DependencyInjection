using System.Globalization;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class TypeFilterExtensions
{
    extension(ITypeFilter filter)
    {
        /// <summary>
        ///     Filters each type to only the types that end with the <paramref name="suffix"/> with same casing. The
        ///     generic arity is discarded before matching, so <see cref="IEnumerable{T}"/> would end with <c>"able"</c>.
        /// </summary>
        /// <param name="suffix">The string to compare to the substring at the end of each type name.</param>
        public ITypeFilter NameEndsWith(string suffix)
        {
            return filter.Where(type => type.GetNonGenericName().EndsWith(suffix));
        }

        /// <summary>
        ///     Filters each type to only the types that end with the <paramref name="suffix"/>. The generic arity is
        ///     discarded before matching, so <see cref="IEnumerable{T}"/> would end with <c>"able"</c>.
        /// </summary>
        /// <param name="suffix">The string to compare to the substring at the end of each type name.</param>
        /// <param name="ignoreCase">
        ///     If <see langword="true"/>, then the suffix is matched ignoring case using.
        ///     If <see langword="false"/>, then the suffix is matched with the exact casing.
        /// </param>
        /// <remarks>
        ///     <see cref="CultureInfo.CurrentCulture"/> is used here.
        /// </remarks>
        public ITypeFilter NameEndsWith(string suffix, bool ignoreCase)
        {
            return filter.NameEndsWith(suffix, ignoreCase, CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Filters each type to only the types that end with the <paramref name="suffix"/>. The generic arity is
        ///     discarded before matching, so <see cref="IEnumerable{T}"/> would end with <c>"able"</c>.
        /// </summary>
        /// <param name="suffix">The string to compare to the substring at the end of each type name.</param>
        /// <param name="ignoreCase">
        ///     If <see langword="true"/>, then the suffix is matched ignoring case using.
        ///     If <see langword="false"/>, then the suffix is matched with the exact casing.
        /// </param>
        /// <param name="cultureInfo">
        ///     Cultural information that determines how this instance and value are compared. If culture is
        ///     <see langword="null"/>, the current culture is used.
        /// </param>
        public ITypeFilter NameEndsWith(string suffix, bool ignoreCase, CultureInfo? cultureInfo)
        {
            return filter.Where(type => type.GetNonGenericName().EndsWith(suffix, ignoreCase, cultureInfo));
        }

        /// <summary>
        ///     Filters each type to only the types that end with the <paramref name="suffix"/>. The generic arity is
        ///     discarded before matching, so <see cref="IEnumerable{T}"/> would end with <c>"able"</c>.
        /// </summary>
        /// <param name="suffix">The string to compare to the substring at the end of each type name.</param>
        /// <param name="comparisonType">
        ///     One of the enumeration values that determines how this string and value are compared.
        /// </param>
        public ITypeFilter NameEndsWith(string suffix, StringComparison comparisonType)
        {
            return filter.Where(type => type.GetNonGenericName().EndsWith(suffix, comparisonType));
        }
    }
}
