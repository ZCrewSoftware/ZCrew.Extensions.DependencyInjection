namespace ZCrew.Extensions.DependencyInjection.Registration;

/// <summary>
///     Extensions for the <see cref="ServiceComponent"/> type to extend existing functionality with convenient helpers.
/// </summary>
public static partial class ServiceComponentExtensions
{
    extension(ServiceComponent component)
    {
        /// <summary>
        ///     Adds <typeparamref name="T1"/> to this component. This verifies that the
        ///     <see cref="ServiceComponent.ImplementationType"/> is assignable to the service. A duplicate service can
        ///     be added; but, it is excluded when registering the component.
        /// </summary>
        /// <typeparam name="T1">The service to add.</typeparam>
        /// <returns>The modified component.</returns>
        /// <exception cref="ArgumentException">If the service isn't a base type of the implementation.</exception>
        public ServiceComponent As<T1>()
        {
            return component.As(typeof(T1));
        }

        /// <summary>
        ///     Adds the services to this component. This verifies that the
        ///     <see cref="ServiceComponent.ImplementationType"/> is assignable to each service. Duplicate services can
        ///     be added; but, they are excluded when registering the component.
        /// </summary>
        /// <typeparam name="T1">The first service to add.</typeparam>
        /// <typeparam name="T2">The second service to add.</typeparam>
        /// <returns>The modified component.</returns>
        /// <exception cref="ArgumentException">If any service isn't a base type of the implementation.</exception>
        public ServiceComponent As<T1, T2>()
        {
            return component.As([typeof(T1), typeof(T2)]);
        }

        /// <summary>
        ///     Adds the services to this component. This verifies that the
        ///     <see cref="ServiceComponent.ImplementationType"/> is assignable to each service. Duplicate services can
        ///     be added; but, they are excluded when registering the component.
        /// </summary>
        /// <typeparam name="T1">The first service to add.</typeparam>
        /// <typeparam name="T2">The second service to add.</typeparam>
        /// <typeparam name="T3">The third service to add.</typeparam>
        /// <returns>The modified component.</returns>
        /// <exception cref="ArgumentException">If any service isn't a base type of the implementation.</exception>
        public ServiceComponent As<T1, T2, T3>()
        {
            return component.As([typeof(T1), typeof(T2), typeof(T3)]);
        }

        /// <summary>
        ///     Adds the services to this component. This verifies that the
        ///     <see cref="ServiceComponent.ImplementationType"/> is assignable to each service. Duplicate services can
        ///     be added; but, they are excluded when registering the component.
        /// </summary>
        /// <typeparam name="T1">The first service to add.</typeparam>
        /// <typeparam name="T2">The second service to add.</typeparam>
        /// <typeparam name="T3">The third service to add.</typeparam>
        /// <typeparam name="T4">The fourth service to add.</typeparam>
        /// <returns>The modified component.</returns>
        /// <exception cref="ArgumentException">If any service isn't a base type of the implementation.</exception>
        public ServiceComponent As<T1, T2, T3, T4>()
        {
            return component.As([typeof(T1), typeof(T2), typeof(T3), typeof(T4)]);
        }

        /// <summary>
        ///     Adds the services to this component. This verifies that the
        ///     <see cref="ServiceComponent.ImplementationType"/> is assignable to each service. Duplicate services can
        ///     be added; but, they are excluded when registering the component.
        /// </summary>
        /// <typeparam name="T1">The first service to add.</typeparam>
        /// <typeparam name="T2">The second service to add.</typeparam>
        /// <typeparam name="T3">The third service to add.</typeparam>
        /// <typeparam name="T4">The fourth service to add.</typeparam>
        /// <typeparam name="T5">The fifth service to add.</typeparam>
        /// <returns>The modified component.</returns>
        /// <exception cref="ArgumentException">If any service isn't a base type of the implementation.</exception>
        public ServiceComponent As<T1, T2, T3, T4, T5>()
        {
            return component.As([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)]);
        }

        /// <summary>
        ///     Adds the services to this component. This verifies that the
        ///     <see cref="ServiceComponent.ImplementationType"/> is assignable to each service. Duplicate services can
        ///     be added; but, they are excluded when registering the component.
        /// </summary>
        /// <typeparam name="T1">The first service to add.</typeparam>
        /// <typeparam name="T2">The second service to add.</typeparam>
        /// <typeparam name="T3">The third service to add.</typeparam>
        /// <typeparam name="T4">The fourth service to add.</typeparam>
        /// <typeparam name="T5">The fifth service to add.</typeparam>
        /// <typeparam name="T6">The sixth service to add.</typeparam>
        /// <returns>The modified component.</returns>
        /// <exception cref="ArgumentException">If any service isn't a base type of the implementation.</exception>
        public ServiceComponent As<T1, T2, T3, T4, T5, T6>()
        {
            return component.As([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)]);
        }

        /// <summary>
        ///     Adds the services to this component. This verifies that the
        ///     <see cref="ServiceComponent.ImplementationType"/> is assignable to each service. Duplicate services can
        ///     be added; but, they are excluded when registering the component.
        /// </summary>
        /// <typeparam name="T1">The first service to add.</typeparam>
        /// <typeparam name="T2">The second service to add.</typeparam>
        /// <typeparam name="T3">The third service to add.</typeparam>
        /// <typeparam name="T4">The fourth service to add.</typeparam>
        /// <typeparam name="T5">The fifth service to add.</typeparam>
        /// <typeparam name="T6">The sixth service to add.</typeparam>
        /// <typeparam name="T7">The seventh service to add.</typeparam>
        /// <returns>The modified component.</returns>
        /// <exception cref="ArgumentException">If any service isn't a base type of the implementation.</exception>
        public ServiceComponent As<T1, T2, T3, T4, T5, T6, T7>()
        {
            return component.As(
                [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)]
            );
        }

        /// <summary>
        ///     Adds the services to this component. This verifies that the
        ///     <see cref="ServiceComponent.ImplementationType"/> is assignable to each service. Duplicate services can
        ///     be added; but, they are excluded when registering the component.
        /// </summary>
        /// <typeparam name="T1">The first service to add.</typeparam>
        /// <typeparam name="T2">The second service to add.</typeparam>
        /// <typeparam name="T3">The third service to add.</typeparam>
        /// <typeparam name="T4">The fourth service to add.</typeparam>
        /// <typeparam name="T5">The fifth service to add.</typeparam>
        /// <typeparam name="T6">The sixth service to add.</typeparam>
        /// <typeparam name="T7">The seventh service to add.</typeparam>
        /// <typeparam name="T8">The eighth service to add.</typeparam>
        /// <returns>The modified component.</returns>
        /// <exception cref="ArgumentException">If any service isn't a base type of the implementation.</exception>
        public ServiceComponent As<T1, T2, T3, T4, T5, T6, T7, T8>()
        {
            return component.As(
                [typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)]
            );
        }
    }
}
