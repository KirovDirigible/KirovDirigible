namespace Kirov {
    using System.Runtime.InteropServices;

    using Kirov.Platform.Windows;

    class Startup {
        public static void Start() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                WindowsStartup.Startup();
            }
        }
    }
}
