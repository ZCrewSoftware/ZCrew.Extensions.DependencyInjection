using Fixtures.SmallProject.Application.Ports;

namespace Fixtures.SmallProject.Infrastructure.Notifications;

public class EmailNotificationSender : INotificationSender
{
    public void Dispose() { }
}

public class SmsNotificationSender : INotificationSender
{
    public void Dispose() { }
}
