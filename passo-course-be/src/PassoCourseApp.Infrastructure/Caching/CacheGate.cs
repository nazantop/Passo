namespace PassoCourseApp.Infrastructure.Caching;

public interface ICacheGate
{
    bool IsOpen { get; }
    void Trip(TimeSpan cooldown);
}

public sealed class CacheGate : ICacheGate
{
    private DateTime _disabledUntil = DateTime.MinValue;
    public bool IsOpen => DateTime.UtcNow >= _disabledUntil;
    public void Trip(TimeSpan cooldown) => _disabledUntil = DateTime.UtcNow.Add(cooldown);
}