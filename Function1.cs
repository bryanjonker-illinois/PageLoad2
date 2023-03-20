using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HttpRequestData = Microsoft.Azure.Functions.Worker.Http.HttpRequestData;
using HttpResponseData = Microsoft.Azure.Functions.Worker.Http.HttpResponseData;
using LogLevel = OpenQA.Selenium.LogLevel;

namespace PageLoad2 {

    public class Function1 {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        public string GetText() {
            var binaryLocation = "/usr/bin/google-chrome-stable";
            var driverPath = "/usr/local/bin/chromedriver";
            var url = "https://education.illinois.edu/faculty-finder";
            var chromeOptions = new ChromeOptions {
                PageLoadStrategy = PageLoadStrategy.Normal,
            };
            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Warning);
            chromeOptions.AddArguments("headless");
            chromeOptions.AddArguments("log-level=3");
            chromeOptions.AddArguments("no-sandbox");
            chromeOptions.AddArguments("disable-dev-shm-usage");
            chromeOptions.BinaryLocation = binaryLocation;
            var browser = new ChromeDriver(driverPath, chromeOptions);
            browser.Url = url;
            _ = browser.Navigate();

            var mainTag = browser.FindElements(By.TagName("main")).Any() ? browser.FindElement(By.TagName("main")) : browser.FindElement(By.ClassName("il-formatted"));

            return mainTag.Text;
        }

        [Function("TestSelenium")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req) {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            try {
                response.WriteString(GetText());
            } catch (Exception e) {
                response.WriteString("Error: " + e.Message);
            }
            return response;
        }
    }
}