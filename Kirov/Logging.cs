namespace Kirov;
using Microsoft.Extensions.Logging;
class Logging {
    public static ILoggerFactory Factory { get; } = LoggerFactory.Create(
        builder => builder.AddConsole()
    );
}
