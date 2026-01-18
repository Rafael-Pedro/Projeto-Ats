namespace Ats.Application.FluentResultExtensions;

public class NotFoundError(string message) : ApplicationError(message);
