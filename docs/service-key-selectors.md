# Service key selectors

Once you've picked a [service selector](service-selectors.md), you can give the resulting registrations a service key with `Keyed`. That produces [keyed services](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection#service-keys), which are resolved by service type and key together.

`Keyed` and `Unkeyed` return a `ServiceLifetimeSelector`, so you can carry on into [lifetime selection](shared-services.md) or stop there.

Keys can also come from an attribute on the implementation, via `KeyedByAttribute`. See [Keying from attributes](#keying-from-attributes).

## `Keyed()`

Works the key out by stripping the service interface name off the end of the implementation name. If the implementation name ends with the service name and something is left over, that becomes the key. Otherwise the registration is left unkeyed.

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

You get:

```
PayPalPaymentGateway  → IPaymentGateway (key: "PayPal")
StripePaymentGateway  → IPaymentGateway (key: "Stripe")
```

`PaymentGateway` (from `IPaymentGateway`) comes off the end of each name, leaving `PayPal` and `Stripe`.

### When you get no key

If the implementation name doesn't end with the service name, or stripping it leaves nothing, the registration stays unkeyed:

```csharp
Classes.From(typeof(PayPalPaymentGateway))
    .AsSelf()
    .Keyed()
// PayPalPaymentGateway registered as PayPalPaymentGateway, unkeyed.
// Stripping "PayPalPaymentGateway" from "PayPalPaymentGateway" leaves nothing, so no key.
```

Generic arity suffixes (`` `1 ``) are stripped before matching, so `InMemoryRepository<T>` registered as `IRepository<T>` gets the key `InMemory`.

## `Keyed(object?)`

Gives every registration the same key. Pass `null` and nothing is keyed:

```csharp
Classes.From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
    .AsInterface<IPaymentGateway>()
    .Keyed("payments")
```

You get:

```
PayPalPaymentGateway  → IPaymentGateway (key: "payments")
StripePaymentGateway  → IPaymentGateway (key: "payments")
```

The `null` case is handy when the key comes from config and might not be set:

```csharp
.Keyed(config.GetValue<string>("ServiceKey"))
// Config value missing? The registrations stay unkeyed.
```

## `Keyed(Func<Type, object?>)`

Computes a key from the implementation type. Return `null` and that one is left unkeyed:

```csharp
Classes.From(typeof(PayPalPaymentGateway), typeof(StripePaymentGateway))
    .AsInterface<IPaymentGateway>()
    .Keyed((Func<Type, object?>)(type => type.Name))
```

You get:

```
PayPalPaymentGateway  → IPaymentGateway (key: "PayPalPaymentGateway")
StripePaymentGateway  → IPaymentGateway (key: "StripePaymentGateway")
```

> Passing a bare lambda can be ambiguous with the `Func<Type, Type, object?>` overload, so you may need the `(Func<Type, object?>)` cast.

Returning `null` for some types lets you key only a subset:

```csharp
.Keyed((Func<Type, object?>)(type =>
    type == typeof(PayPalPaymentGateway) ? "PayPal" : null))
// PayPalPaymentGateway keyed as "PayPal"
// StripePaymentGateway left unkeyed
```

## `Keyed(Func<Type, Type, object?>)`

Same as above, except the delegate also gets the service type. Use it when the key depends on both:

```csharp
Classes.From(typeof(EmailNotificationSender), typeof(SmsNotificationSender))
    .AsInterface<INotificationSender>()
    .Keyed((impl, svc) => $"{impl.Name}:{svc.Name}")
```

You get:

```
EmailNotificationSender → INotificationSender (key: "EmailNotificationSender:INotificationSender")
SmsNotificationSender   → INotificationSender (key: "SmsNotificationSender:INotificationSender")
```

## Keying from attributes

Instead of deriving keys from type names or a delegate, `KeyedByAttribute` reads the key from an attribute on the implementation, which keeps the key next to the class it belongs to. The rules are the same across all the overloads:

- Inherited attributes count by default. Each overload has a twin that takes a leading `bool inherited`. Pass `false` to only look at attributes declared on the type itself.
- No match means no key. A type without a matching attribute, or one where the key comes back `null`, is left unkeyed. Same as a `Func` overload returning `null`.
- Exactly one match. Two matching attributes on a type throws an `AmbiguousMatchException` when the chain is enumerated.

## `KeyedByAttribute<TAttribute>(Func<TAttribute, object?>)`

Reads a specific attribute. `TAttribute` can be a concrete attribute type, or an interface that one or more attributes implement:

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

You get:

```
RegionalCustomerStore → IStore (key: "customers")
RegionalOrderStore    → IStore (key: "orders")
```

Types without the attribute, or where the selector returns `null`, stay unkeyed. There is also an `inherited` overload, `KeyedByAttribute<TAttribute>(bool inherited, Func<TAttribute, object?>)`.

## `KeyedByAttribute(Type, Func<Attribute, object?>)`

The non-generic form, for when you only know the attribute type at runtime. The selector gets an `Attribute`, so cast it before reading the key:

```csharp
Classes.From(typeof(RegionalCustomerStore), typeof(RegionalOrderStore))
    .AsInterface<IStore>()
    .KeyedByAttribute(typeof(RegionAttribute), attribute => ((RegionAttribute)attribute).Region)
```

Same keys as the generic overload above. There is an `inherited` overload too, `KeyedByAttribute(Type, bool inherited, Func<Attribute, object?>)`.

## Resolving keyed services

Use `[FromKeyedServices]` or `GetKeyedService` on the provider:

```csharp
// Injected
public class CheckoutService(
    [FromKeyedServices("Stripe")] IPaymentGateway stripeGateway,
    [FromKeyedServices("PayPal")] IPaymentGateway paypalGateway)
{ }

// Resolved directly
var gateway = provider.GetKeyedService<IPaymentGateway>("Stripe");
```

## Choosing the right overload

| What you want                                   | Overload                                                               | Example                                     |
|-------------------------------------------------|------------------------------------------------------------------------|---------------------------------------------|
| Key from the naming convention                  | `Keyed()`                                                              | `PayPalPaymentGateway` → key `"PayPal"`     |
| One key for everything                          | `Keyed(object?)`                                                       | All keyed as `"payments"`                   |
| Key from the implementation type                | `Keyed(Func<Type, object?>)`                                           | Key is `type.Name`                          |
| Key from both types                             | `Keyed(Func<Type, Type, object?>)`                                     | Key is `$"{impl}:{svc}"`                    |
| Key from a typed attribute                      | `KeyedByAttribute<TAttribute>(Func<TAttribute, object?>)`              | `[Region("customers")]` → key `"customers"` |
| Key from an attribute known at runtime          | `KeyedByAttribute(Type, Func<Attribute, object?>)`                     | As above, `Type` resolved at runtime        |
| Skip keying for some types                      | Any `Func` overload returning `null`, or an unmatched/`null` attribute | `null` → left unkeyed                       |
| No keys at all                                  | `Keyed(null)`, or just don't call it                                   | Registrations unchanged                     |
