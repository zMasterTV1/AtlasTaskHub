using Atlas.Application.Abstractions.Time;

namespace Atlas.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
