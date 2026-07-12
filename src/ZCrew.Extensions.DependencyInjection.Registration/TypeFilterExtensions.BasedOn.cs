using System.Reflection;

namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class TypeFilterExtensions
{
    extension(ITypeFilter filter)
    {
        /// <summary>
        ///     Restricts to types that implement or inherit from <typeparamref name="T"/>. Also sets the base type
        ///     context used by <see cref="IServiceSelector.As(Func{Type, IReadOnlyList{Type}, IEnumerable{Type}})"/>.
        /// </summary>
        /// <typeparam name="T">The base type or interface to filter on.</typeparam>
        public ITypeFilter BasedOn<T>()
        {
            return filter.BasedOn(typeof(T));
        }

        /// <summary>
        ///     Restricts to types that implement or inherit from <paramref name="baseType"/>. Also sets the base type
        ///     context used by <see cref="IServiceSelector.As(Func{Type, IReadOnlyList{Type}, IEnumerable{Type}})"/>.
        /// </summary>
        /// <param name="baseType">The base type or interface to filter on.</param>
        public ITypeFilter BasedOn(Type baseType)
        {
            ArgumentNullException.ThrowIfNull(baseType);
            return filter.BasedOn(baseType);
        }
    }
}
