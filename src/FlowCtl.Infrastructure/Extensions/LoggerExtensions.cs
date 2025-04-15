using FlowCtl.Infrastructure.Logging.ConsoleLogger;
using Microsoft.Extensions.Logging;

namespace FlowCtl.Infrastructure.Extensions;

public static class LoggerExtensions
{
    public static ILoggingBuilder AddConsoleLogger(this ILoggingBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.AddConsoleLogger(options =>
        {
            options.OutputTemplate = "[time={timestamp} | level={level}] message=\"{message}\"";
            options.MinLevel = LogLevel.Information;
        });
        return builder;
    }

    public static ILoggingBuilder AddConsoleLogger(this ILoggingBuilder builder, Action<ConsoleLoggerOptions> options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        var loggerOptions = new ConsoleLoggerOptions();
        options(loggerOptions);

        builder.AddProvider(new ConsoleLoggerProvider(loggerOptions));

        return builder;
    }
}