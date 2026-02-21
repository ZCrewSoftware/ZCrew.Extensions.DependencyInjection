namespace Fixtures.SmallProject.Application.Ports;

public interface IEventPublisher;

public interface IEventPublisher<T>;

public interface INotificationSender : IDisposable;

public interface IPaymentGateway : IDisposable;
