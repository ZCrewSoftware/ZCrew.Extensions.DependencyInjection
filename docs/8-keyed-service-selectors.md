# Keyed Service Selectors

After choosing a [service selector](7-service-selectors.md), you can optionally assign **service keys** to the resulting registrations using `Keyed`. This produces [keyed services](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#keyed-services) — registrations that are resolved by both their service type and a key.

## `Keyed()`

Auto-detects a string key by stripping the service type's interface name from the implementation type's name. If the implementation name ends with the service name and has a non-empty prefix, that prefix becomes the key. Otherwise the descriptor is left unkeyed.

```csharp
Classes.From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
    .AsInterface<IPaymentGateway>()
    .Keyed()
```

Given:

```csharp
public interface IPaymentGateway { }
public class PayPalPaymentGateway : IPaymentGateway { }
public class StripePaymentGateway : IPaymentGateway { }
```

Registers:

```
PayPalPaymentGateway  → IPaymentGateway (key: "PayPal")
StripePaymentGateway  → IPaymentGateway (key: "Stripe")
```

The convention strips `PaymentGateway` (from `IPaymentGateway`) off the end of each implementation name, leaving `PayPal` and `Stripe`.

### When auto-detection is skipped

If the implementation name does not end with the service name, or if stripping it would leave an empty string, the descriptor is left unkeyed:

```csharp
Classes.From(typeof(PayPalPaymentGateway))
    .AsSelf()
    .Keyed()
// PayPalPaymentGateway registered as PayPalPaymentGateway (unkeyed)
// "PayPalPaymentGateway" stripped of "PayPalPaymentGateway" leaves "", so no key is applied
```

Generic type arity suffixes (e.g., `` `1 ``) are stripped before matching, so `InMemoryRepository<T>` registered as `IRepository<T>` would yield key `InMemory`.

## `Keyed(object?)`

Assigns the same key to all registrations. When `null` is passed, the descriptors are returned unchanged (no keying applied):

```csharp
Classes.From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
    .AsInterface<IPaymentGateway>()
    .Keyed("payments")
```

Registers:

```
PayPalPaymentGateway  → IPaymentGateway (key: "payments")
StripePaymentGateway  → IPaymentGateway (key: "payments")
```

Passing `null` is a no-op, which is useful when the key comes from a configuration value that may or may not be set:

```csharp
.Keyed(config.GetValue<string>("ServiceKey"))
// If config value is null, registrations remain unkeyed
```

## `Keyed(Func<Type, object?>)`

Computes a key per registration based on the implementation type. When the function returns `null`, that descriptor is left unkeyed:

```csharp
Classes.From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
    .AsInterface<IPaymentGateway>()
    .Keyed((Func<Type, object?>)(type => type.Name))
```

Registers:

```
PayPalPaymentGateway  → IPaymentGateway (key: "PayPalPaymentGateway")
StripePaymentGateway  → IPaymentGateway (key: "StripePaymentGateway")
```

> **Note:** When passing a lambda directly, you may need the `(Func<Type, object?>)` cast to disambiguate from the `Func<Type, Type, object?>` overload.

Returning `null` for specific types lets you selectively key a subset:

```csharp
.Keyed((Func<Type, object?>)(type =>
    type == typeof(PayPalPaymentGateway) ? "PayPal" : null))
// PayPalPaymentGateway keyed as "PayPal"
// StripePaymentGateway left unkeyed
```

## `Keyed(Func<Type, Type, object?>)`

Like the single-parameter overload, but the delegate also receives the service type. This is useful when the key should depend on the relationship between the implementation and its service type:

```csharp
Classes.From(typeof(EmailNotificationSender), typeof(SmsNotificationSender))
    .AsInterface<INotificationSender>()
    .Keyed((impl, svc) => $"{impl.Name}:{svc.Name}")
```

Registers:

```
EmailNotificationSender → INotificationSender (key: "EmailNotificationSender:INotificationSender")
SmsNotificationSender   → INotificationSender (key: "SmsNotificationSender:INotificationSender")
```

## Resolving keyed services

Keyed services are resolved using `[FromKeyedServices]` or by calling `GetKeyedService` on the service provider:

```csharp
// Via attribute injection
public class CheckoutService(
    [FromKeyedServices("Stripe")] IPaymentGateway stripeGateway,
    [FromKeyedServices("PayPal")] IPaymentGateway paypalGateway)
{ }

// Via service provider
var gateway = provider.GetKeyedService<IPaymentGateway>("Stripe");
```

## Choosing the right overload

| Scenario                         | Overload                             | Example                                 |
|----------------------------------|--------------------------------------|-----------------------------------------|
| Key by naming convention         | `Keyed()`                            | `PayPalPaymentGateway` → key `"PayPal"` |
| Same key for all registrations   | `Keyed(object?)`                     | All keyed as `"payments"`               |
| Key based on implementation type | `Keyed(Func<Type, object?>)`         | Key is `type.Name`                      |
| Key based on both types          | `Keyed(Func<Type, Type, object?>)`   | Key is `$"{impl}:{svc}"`                |
| Conditionally skip keying        | Any `Func` overload returning `null` | `null` → left unkeyed                   |
| No key                           | `Keyed(null)` or don't call at all   | Descriptors unchanged                   |
