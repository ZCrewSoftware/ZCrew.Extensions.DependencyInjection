# Service Key Selectors

After choosing a [service selector](service-selectors.md), you can optionally assign **service keys** to the resulting registrations using `Keyed`. This produces [keyed services](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-keys) — registrations that are resolved by both their service type and a key. `Keyed` (and `Unkeyed`) return a `ServiceLifetimeSelector`, so the chain can continue into [lifetime selection](shared-components.md) or terminate directly.

Keys can also be derived from **attributes on the implementation type** via `KeyedByAttribute` (see [Keying from attributes](#keying-from-attributes)), which likewise returns a `ServiceLifetimeSelector`.

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

## Keying from attributes

Instead of computing keys from type names or delegates, `KeyedByAttribute` reads the key from an **attribute applied to the implementation type**. This keeps the key definition next to the implementation it belongs to. All overloads share the same rules:

- **Inherited attributes are inspected by default.** Each overload has a companion that takes a leading `bool inherited` parameter; pass `false` to consider only attributes declared directly on the implementation type.
- **No match means no key.** Implementation types without a matching attribute — or for which the resolved key is `null` — are left unkeyed, exactly like a `Func` overload returning `null`.
- **A single match is required.** If a type carries more than one matching attribute, an `AmbiguousMatchException` is thrown when the chain is enumerated.

## `KeyedByAttribute()`

Reads the key from any attribute that implements the library's `IServiceKeyProvider` interface. The library ships a ready-made one — `[Keyed]` — so the common case needs no custom attribute:

```csharp
Classes.From(typeof(StripePaymentGateway), typeof(PayPalPaymentGateway))
    .AsInterface<IPaymentGateway>()
    .KeyedByAttribute()
```

Given:

```csharp
[Keyed("Stripe")]
public class StripePaymentGateway : IPaymentGateway { }

[Keyed("PayPal")]
public class PayPalPaymentGateway : IPaymentGateway { }
```

Registers:

```
StripePaymentGateway  → IPaymentGateway (key: "Stripe")
PayPalPaymentGateway  → IPaymentGateway (key: "PayPal")
```

`[Keyed]` is declared `Inherited = false`: a service key identifies a *specific* registration, so it is deliberately **not** inherited by subclasses. (This also keeps runtime and source-generated registration in agreement — a source generator only sees attributes declared directly on a type.) Types with no `IServiceKeyProvider` attribute — or whose `ServiceKey` is `null` — are left unkeyed.

To key by your own attribute instead, implement `IServiceKeyProvider`:

```csharp
public interface IServiceKeyProvider
{
    object? ServiceKey { get; }
}
```

Whether such a custom attribute is picked up on derived types follows *its* own `[AttributeUsage(Inherited = …)]`; pass `KeyedByAttribute(inherited: false)` to ignore inherited attributes.

## `KeyedByAttribute<TAttribute>(Func<TAttribute, object?>)`

Projects a specific attribute — one that need not know anything about `IServiceKeyProvider` — through a selector. `TAttribute` may be a concrete attribute type or an interface implemented by one or more attributes (marker-interface matching):

```csharp
Classes.From(typeof(RegionalCustomerStore), typeof(RegionalOrderStore))
    .AsInterface<IStore>()
    .KeyedByAttribute<RegionAttribute>(attribute => attribute.Region)
```

Given:

```csharp
[AttributeUsage(AttributeTargets.Class)]
public sealed class RegionAttribute(string region) : Attribute
{
    public string Region => region;
}

[Region("customers")]
public class RegionalCustomerStore : IStore { }

[Region("orders")]
public class RegionalOrderStore : IStore { }
```

Registers:

```
RegionalCustomerStore → IStore (key: "customers")
RegionalOrderStore    → IStore (key: "orders")
```

Types without the attribute, or for which the selector returns `null`, are left unkeyed. An `inherited` overload — `KeyedByAttribute<TAttribute>(bool inherited, Func<TAttribute, object?>)` — controls whether inherited attributes are inspected.

## `KeyedByAttribute(Type, Func<Attribute, object?>)`

The non-generic form, for when the attribute type is only known at runtime. The selector receives the matching attribute as `Attribute`, so it is cast before the key is read:

```csharp
Classes.From(typeof(RegionalCustomerStore), typeof(RegionalOrderStore))
    .AsInterface<IStore>()
    .KeyedByAttribute(typeof(RegionAttribute), attribute => ((RegionAttribute)attribute).Region)
```

This registers the same keys as the generic overload above. An `inherited` overload — `KeyedByAttribute(Type, bool inherited, Func<Attribute, object?>)` — is also available.

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

| Scenario                                        | Overload                                                               | Example                                     |
|-------------------------------------------------|------------------------------------------------------------------------|---------------------------------------------|
| Key by naming convention                        | `Keyed()`                                                              | `PayPalPaymentGateway` → key `"PayPal"`     |
| Same key for all registrations                  | `Keyed(object?)`                                                       | All keyed as `"payments"`                   |
| Key based on implementation type                | `Keyed(Func<Type, object?>)`                                           | Key is `type.Name`                          |
| Key based on both types                         | `Keyed(Func<Type, Type, object?>)`                                     | Key is `$"{impl}:{svc}"`                    |
| Key from an `IServiceKeyProvider` attribute     | `KeyedByAttribute()`                                                   | `[Keyed("Stripe")]` → key `"Stripe"`        |
| Key by projecting a typed attribute             | `KeyedByAttribute<TAttribute>(Func<TAttribute, object?>)`              | `[Region("customers")]` → key `"customers"` |
| Key by projecting an attribute known at runtime | `KeyedByAttribute(Type, Func<Attribute, object?>)`                     | As above, `Type` resolved at runtime        |
| Conditionally skip keying                       | Any `Func` overload returning `null`, or an unmatched/`null` attribute | `null` → left unkeyed                       |
| No key                                          | `Keyed(null)` or don't call at all                                     | Descriptors unchanged                       |
