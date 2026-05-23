namespace Fixtures.SmallProject.Domain.Auditing;

public interface IIdentifiable;

public interface IAuditable : IIdentifiable;

public interface ISoftDeletable : IIdentifiable;

public class AuditableSoftDeletableEntity : IAuditable, ISoftDeletable;
