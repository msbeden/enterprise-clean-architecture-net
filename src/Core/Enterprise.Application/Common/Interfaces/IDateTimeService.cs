namespace Enterprise.Application.Common.Interfaces;

/// <summary>
/// Service abstraction for date and time operations.
/// </summary>
public interface IDateTimeService
{
    DateTime UtcNow { get; }
}
