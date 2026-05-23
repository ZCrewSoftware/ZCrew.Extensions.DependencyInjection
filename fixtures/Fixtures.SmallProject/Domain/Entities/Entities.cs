namespace Fixtures.SmallProject.Domain.Entities;

public class Customer;

public class Order;

public class OrderItem;

public class Product;

public class DigitalProduct : Product;

public class SubscriptionProduct : DigitalProduct;

public class Invoice;

public struct Currency;

public enum OrderStatus
{
    Pending,
    Shipped,
    Delivered,
}
