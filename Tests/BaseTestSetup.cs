using Microsoft.Playwright;
using PlaywrightTests.Pages.LoginPageClasses;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Tests;

/// <summary>
/// Provides a base setup for all Playwright tests, handling common initialization and cleanup tasks.
/// </summary>
public abstract class BaseTestSetup : PageTest
{
    protected new IPage? Page;
    protected new IBrowser? Browser;
    protected new IBrowserContext? Context;
    protected LoginPage? LoginPage;
    protected string? Url;
    protected string? UserName;
    protected string? UserEmail;
    protected string? UserPassword;

    /// <summary>
    /// This method is called before each test method runs.
    /// </summary>
    [TestInitialize]
    public virtual async Task TestSetup()
    {
        string? browserName = TestContext.GetTestRunParameter("BrowserName");
        // Retrieve parameters from TestContext
        Url = TestContext.GetTestRunParameter("Url");
        UserName = TestContext.GetTestRunParameter("UserName");
        UserEmail = TestContext.GetTestRunParameter("UserEmail");
        UserPassword = TestContext.GetTestRunParameter("UserPassword");
        string? recordedVideoFolderName = TestContext.TestName;

        // Initialize the browser using a separate method
        Browser = await InitializeBrowser(browserName, false);
        //Context = await Browser.NewContextAsync();
        Context = await Browser.StartVideoRecording(TestContext, recordedVideoFolderName!);

        await Context.StartTracing();
        Page = await Context.NewPageAsync();
        await Page.GotoAsync(Url!);
        LoginPage = new LoginPage(Page);
    }

    /// <summary>
    /// This method is called after each test method runs.
    /// </summary>
    [TestCleanup]
    public virtual async Task TestCleanup()
    {
        string? testName = TestContext.TestName;

        // Take a screenshot on test failure
        if (Page != null)
        {
            await Page.CaptureScreenshotOnFailure(TestContext, 2);
        }

        // Stop tracing and save the trace file
        if (Context != null)
        {     
            await Context.StopTracing($"{testName}.zip");
        }

        // Close the browser context and browser
        if (Browser != null)
        {
            await Browser.CloseAsync();
        }

        // Deletes the recorded video if the test passes...
        // Keep it if the test fails
        if (TestContext.CurrentTestOutcome == UnitTestOutcome.Passed)
        {
            string videoPath = FileDownloader.GetDownloadPath(@"Downloads\Videos");
            string filePath = Path.Combine(videoPath, testName!);
            VideoHelper.DeleteAllPreviousVideoFiles(TestContext, filePath);
        }
        else
        {
            VideoHelper.AddVideoFilesToTestContext(TestContext, testName!);
        }

    }


    /// <summary>
    /// Initializes a browser instance based on the specified browser name and headless option.
    /// </summary>
    /// <param name="browserName">The name of the browser to initialize.</param>
    /// <param name="headless">Indicates whether the browser should be launched in headless mode.</param>
    /// <returns>An instance of <see cref="IBrowser"/> that is ready to be used in tests.</returns>
    private async Task<IBrowser> InitializeBrowser(string? browserName, bool headless)
    {
        switch (browserName?.ToLower())
        {
            case "msedge":
                return await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless, Channel = "msedge" });
            case "chrome":
                return await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless, Channel = "chrome" });
            case "firefox":
                return await Playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless });
            case "webkit":
                return await Playwright.Webkit.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless });
            default:
                // Default to Chromium if no specific browser is specified or if the name doesn't match
                return await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless });
        }
    }
}
