namespace ZCrew.Extensions.DependencyInjection.Registration;

public static partial class TypeFilterExtensions
{
    extension(TypeFilter filter)
    {
        /// <summary>
        ///     Restricts to types that implement or inherit from <typeparamref name="T"/>. Also sets the base type
        ///     context used by <see cref="ServiceSelector.As(Func{Type, IReadOnlyList{Type}, IEnumerable{Type}})"/>.
        /// </summary>
        /// <typeparam name="T">The base type or interface to filter on.</typeparam>
        public TypeFilter BasedOn<T>()
        {
            return filter.BasedOn(typeof(T));
        }

        /// <summary>
        ///     Restricts to types that implement or inherit from <paramref name="baseType"/>. Also sets the base type
        ///     context used by <see cref="ServiceSelector.As(Func{Type, IReadOnlyList{Type}, IEnumerable{Type}})"/>.
        /// </summary>
        /// <param name="baseType">The base type or interface to filter on.</param>
        public TypeFilter BasedOn(Type baseType)
        {
            ArgumentNullException.ThrowIfNull(baseType);
            return filter.BasedOn(baseType);
        }
    }
}
