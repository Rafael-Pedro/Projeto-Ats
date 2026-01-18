using FluentResults;

namespace Ats.Application.FluentResultExtensions;

public class ApplicationError(string message) : Error(message);