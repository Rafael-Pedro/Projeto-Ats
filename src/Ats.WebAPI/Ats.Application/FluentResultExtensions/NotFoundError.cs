using FluentResults;

namespace Ats.Application.FluentResultExtensions;

public class NotFoundError(string message) : Error(message);
