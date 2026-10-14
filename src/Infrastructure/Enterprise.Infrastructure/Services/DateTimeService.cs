using Enterprise.Application.Common.Interfaces;

namespace Enterprise.Infrastructure.Services;

/// <summary>
/// Implementation of IDateTimeService using UTC system clock.
/// </summary>
public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
