using Fixtures.SmallProject.Application.Ports;

namespace Fixtures.SmallProject.Infrastructure.External;

public class PayPalPaymentGateway : IPaymentGateway
{
    public void Dispose() { }
}

public class StripePaymentGateway : IPaymentGateway
{
    public void Dispose() { }
}
