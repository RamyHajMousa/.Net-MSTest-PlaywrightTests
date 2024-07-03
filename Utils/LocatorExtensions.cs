using Microsoft.Playwright;

namespace PlaywrightTests.Utils;

/// <summary>
/// Provides extension methods for the <see cref="ILocator"/> interface to perform common actions such as clicking...
/// </summary>
public static class LocatorExtensions
{
    /// <summary>
    /// Clicks the specified locator with an optional delay.
    /// </summary>
    /// <param name="locator">The ILocator object representing the element to be clicked.</param>
    /// <param name="delayInSeconds">The delay before clicking, in seconds.</param>
    public static async Task ClickWithDelayAsync(this ILocator locator, int delayInSeconds = 1)
    {
        var page = locator.Page;
        await page.WaitForTimeoutAsync(delayInSeconds * 1000);
        await locator.ClickAsync();
    }
}
