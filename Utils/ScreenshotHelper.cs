using Microsoft.Playwright;

namespace PlaywrightTests.Utils;

/// <summary>
/// Provides methods for capturing and managing screenshots within tests.
/// </summary>
public static class ScreenshotHelper
{
    /// <summary>
    /// Captures a screenshot of the current page and saves it to a specified file.
    /// </summary>
    /// <param name="page">The page where the screenshot will be taken.</param>
    /// <param name="testContext">The test context from the running test.</param>
    /// <param name="fileName">The name of the file to save the screenshot to. Defaults to "screenshot.png".</param>
    /// <param name="waitInSeconds">The number of seconds to wait before taking the screenshot. Defaults to 0.</param>
    /// <returns>A task that represents the asynchronous operation of taking and saving a screenshot.</returns>
    /// <remarks>
    /// The method waits for a specified amount of time before taking the screenshot. 
    /// This can be useful for waiting for animations or dynamic content to settle.
    /// If an exception occurs, the error is logged to the console.
    /// Screenshots are saved in the "Downloads/Screenshots" directory.
    /// </remarks>
    public static async Task<bool> TakeScreenshot(this IPage page, TestContext testContext, string fileName = "screenshot.png", int waitInSeconds = 0)
    {
        try
        {
            string screenshotsPath = FileDownloader.GetDownloadPath(@"Downloads\Screenshots");
            string filePath = Path.Combine(screenshotsPath, fileName);

            await page.WaitForTimeoutAsync(waitInSeconds * 1000);
            await page.ScreenshotAsync(new() { Path = filePath, FullPage = true });
            testContext.AddResultFile(filePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during taking screenshot: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Captures a screenshot when a test fails.
    /// </summary>
    /// <param name="page">The page where the screenshot will be taken.</param>
    /// <param name="testContext">The test context from the running test.</param>
    /// <param name="waitInSeconds">Optional delay before taking the screenshot.</param>
    /// <returns>A task that represents the asynchronous operation of conditionally taking a screenshot based on test outcome.</returns>
    public static async Task CaptureScreenshotOnFailure(this IPage page, TestContext testContext, int waitInSeconds = 0)
    {
        if (testContext.CurrentTestOutcome != UnitTestOutcome.Passed)
        {
            string fileName = $"{testContext.TestName}_error.png";
            bool screenshotTaken = await TakeScreenshot(page, testContext, fileName, waitInSeconds);
            if (screenshotTaken)
            {
                Console.WriteLine($"Screenshot captured on failure: {fileName} and saved in the folder Downloads.");
            }
        }
    }
}
