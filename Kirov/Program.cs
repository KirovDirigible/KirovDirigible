using Kirov;

using Newtonsoft.Json.Linq;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;

const int w = 1920, h = 1080; // TODO vary

// TODO: detect actual agent using non-headless version first, or just strip Headless from HeadlessChrome
const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.102 Safari/537.36";

Startup.Start();

var chromeOptions = new ChromeOptions();
chromeOptions.AddArguments(new[] {
    "--headless",
    Invariant($"--window-size={w}x{h}"),
    $"user-agent={USER_AGENT}",
});
chromeOptions.SetLoggingPreference("performance", LogLevel.Info);
chromeOptions.PerformanceLoggingPreferences = new ChromiumPerformanceLoggingPreferences {
    IsCollectingNetworkEvents = true,
};

using var driver = new ChromeDriver(chromeOptions);
using var devTools = driver.GetDevToolsSession();
while (true) {
    driver.Navigate().GoToUrl("https://www.rt.com/");
    Console.WriteLine(await devTools.SendCommand("Network.clearBrowserCache", commandParameters: new JObject()));
}
