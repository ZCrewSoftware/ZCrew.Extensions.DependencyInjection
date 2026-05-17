using Fixtures.SmallProject.Domain.Entities;

namespace Fixtures.SmallProject.Application.Pipelines;

public interface IPipelineBehavior<TRequest, TResponse>;

public interface IRequestHandler<TRequest>;

public interface IRequestHandler<TRequest, TResponse>;

public abstract class Pipeline<TContext>
{
    public interface IStep;
}

public class LoggingStep<TContext> : Pipeline<TContext>.IStep;

public class OrderValidationStep : Pipeline<Order>.IStep;
