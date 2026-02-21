using Fixtures.SmallProject.Domain.Entities;

namespace Fixtures.SmallProject.Domain.Services;

public interface IValidator<T>;

public interface IPricingStrategy;

internal class InternalOrderValidator : IValidator<Order>;

public static class PricingDefaults;

public class OrderValidator : IValidator<Order>
{
    public class Strict : IValidator<Order>;
}

public class CustomerValidator : IValidator<Customer>;
