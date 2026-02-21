namespace Fixtures.SmallProject.Application.Pipelines;

public interface IPipelineBehavior<TRequest, TResponse>;

public interface IRequestHandler<TRequest>;

public interface IRequestHandler<TRequest, TResponse>;
