namespace Kirov.Platform;

using System.Runtime.InteropServices;

using Kirov.Platform.Windows;

using Microsoft.Extensions.Logging;

class PlatformFactory {
    static readonly ILogger<PlatformFactory> logger = Logging.Factory.CreateLogger<PlatformFactory>();
    public static IPlatform CurrentPlatform { get; }

    static PlatformFactory() {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            CurrentPlatform = new WindowsPlatform();
        } else {
            logger.LogWarning("Current platform is not supported. Launching Kirovs might not be user-friendly.");
            CurrentPlatform = new UnsupportedPlatform();
        }
    }
}
