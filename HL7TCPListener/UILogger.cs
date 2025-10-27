using Microsoft.Extensions.Logging;

public class UILogger : ILoggerProvider, ILogger
{
    public event Action<string>? OnLog;

    public ILogger CreateLogger(string categoryName) => this;

    public void Info(string message) => Log(LogLevel.Information, new EventId(), message, null, (s, e) => s);
    public void Warn(string message) => Log(LogLevel.Warning, new EventId(), message, null, (s, e) => s);
    public void Error(string message, Exception? ex = null) => Log(LogLevel.Error, new EventId(), message, ex, (s, e) => s);
    public void Debug(string message) => Log(LogLevel.Debug, new EventId(), message, null, (s, e) => s);

    public void Dispose() { }

    public IDisposable BeginScope<TState>(TState state) => default!;
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId,
        TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var msg = formatter(state, exception);
        OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {logLevel}: {msg}");
    }

    public void Log(string message)
    {
        OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
    }
}
