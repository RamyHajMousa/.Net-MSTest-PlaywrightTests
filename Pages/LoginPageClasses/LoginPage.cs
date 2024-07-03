using Microsoft.Playwright;
using PlaywrightTests.Pages.LandingPageClasses;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Pages.LoginPageClasses;

public class LoginPage : BasePage
{
    private readonly IPage _page;

    public LoginPage(IPage page) : base(page)
    {
        _page = page;
    }

    // Page locators...
    public ILocator ViedocLogo => page.GetByRole(AriaRole.Link, new() { Name = "Viedoc" });

    public ILocator LanguageSelect => page.GetByRole(AriaRole.Combobox);
    public ILocator EmailAddressInput => page.Locator("#Input_Username");
    public ILocator PasswordInput => page.Locator("#Input_Password");
    public ILocator LoginButton => page.GetByRole(AriaRole.Button, new() { Name = "Log in" });
    public ILocator ForgotPasswordLink => page.GetByRole(AriaRole.Link, new() { Name = "Click here" });
    public ILocator AutoLogOutMessage => page.Locator("#auto-log-out-message");

    // Page methods...

    /// <summary>
    /// Logs into the Viedoc using provided credentials.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>An instance of the LandingPage class, representing the page displayed after a successful login.</returns>
    public async Task<LandingPage> Login(string username, string password)
    {
        await EmailAddressInput.ClearAsync();
        await EmailAddressInput.FillAsync(username);
        await PasswordInput.ClearAsync();
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickWithDelayAsync();

        return new LandingPage(page);
    }
}