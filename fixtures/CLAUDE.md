# Fixtures

These fixtures provide projects that mirror real-world code for testing and benchmarking.
Unless a test requires a method, property, or field: the classes and interfaces should remain empty of members.

## Fixtures.SmallProject

A domain-driven design fixture used by the Registration integration tests. Contains types across three layers, each in its own namespace hierarchy under `Fixtures.SmallProject`:

- **`Domain/Entities/`** — `Customer`, `Order`, `OrderItem`, `Product` (classes), `Currency` (struct), `OrderStatus` (enum)
- **`Domain/Repositories/`** — `IReadOnlyRepository<T>`, `IRepository<T>`, `IOrderRepository`, `ICustomerRepository`, `IProductRepository`
- **`Domain/Services/`** — `IValidator<T>`, `IPricingStrategy`, `OrderValidator`, `CustomerValidator`, `InternalOrderValidator` (internal), `PricingDefaults` (static), `OrderValidator.Strict` (nested)
- **`Application/Services/`** — `ICustomerService`, `IOrderService`, `IProductService`, `IAuditService`, and their implementations including `AuditServiceDecorator`, `LegacyOrderProcessor`, `CachingCustomerService`
- **`Application/Ports/`** — `IEventPublisher`, `IEventPublisher<T>`, `INotificationSender`, `IPaymentGateway`
- **`Application/Caching/`** — `ICacheProvider`
- **`Infrastructure/Persistence/`** — `RepositoryBase<T>` (abstract), `InMemoryRepository<T>`, `SqlCustomerRepository`, `SqlOrderRepository`, `SqlRepository<T>`
- **`Infrastructure/External/`** — `PayPalPaymentGateway`, `StripePaymentGateway`
- **`Infrastructure/Notifications/`** — `EmailNotificationSender`, `SmsNotificationSender`

The fixture deliberately includes internal types, nested classes, static classes, abstract classes, generics, structs, and enums to exercise the Registration API's visibility filters and type-kind filtering.
