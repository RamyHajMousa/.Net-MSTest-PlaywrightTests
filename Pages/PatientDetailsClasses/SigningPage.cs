using Microsoft.Playwright;

namespace PlaywrightTests.Pages.PatientDetailsClasses;

public class SigningPage : BasePage
{
    private readonly IPage _page;

    public SigningPage(IPage page) : base(page)
    {
        this._page = page;
    }

    // Page locators...
    public ILocator SignAllLink => page.GetByRole(AriaRole.Link, new() { Name = "Sign all?" }).First;

    public ILocator CancelBtn => page.GetByRole(AriaRole.Link, new() { Name = "Cancel" });
    public ILocator ReadyBtn => page.GetByRole(AriaRole.Link, new() { Name = "Ready" });

    public ILocator PasswordTextbox => page.FrameLocator("iframe[title=\"sigign on Idp\"]")
        .GetByRole(AriaRole.Textbox);

    public ILocator SubmitBtn => page.FrameLocator("iframe[title=\"sigign on Idp\"]")
        .GetByRole(AriaRole.Button, new() { Name = "Submit" });

    public ILocator PasswordErrorMessage => page.GetByText("The password you typed is wrong");

    // Page methods...

    /// <summary>
    /// Clicks the submit button and returns an instance of the PatientDetailsPage.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the PatientDetailsPage instance.</returns>
    public async Task<PatientDetailsPage> ClickSubmitBtn()
    {
        await SubmitBtn.WaitForAsync();
        await SubmitBtn.ClickAsync();

        await page.WaitForTimeoutAsync(2000);
        if (await PasswordErrorMessage.IsVisibleAsync())
        {
            //Console.WriteLine("The password you typed is wrong.");
            //Assert.Fail("The password you typed is wrong.");
            throw new InvalidOperationException("The password you typed is wrong.");
        }
        return new PatientDetailsPage(page);
    }

    /// <summary>
    /// Fills the password textbox with the provided password.
    /// </summary>
    /// <param name="password">The password to fill into the password textbox.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task FillPassword(string password)
    {
        await PasswordTextbox.FocusAsync();
        // When using FillAsync() instead, Submit button stays inactive
        await page.Keyboard.TypeAsync(password);
    }

    /// <summary>
    /// Signs all forms by clicking the 'Sign all' link, filling in the password, and submitting.
    /// </summary>
    /// <param name="password">The password to use for signing all forms.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the PatientDetailsPage instance.</returns>
    public async Task<PatientDetailsPage> SignAllForms(string password)
    {
        await SignAllLink.ClickAsync();
        await ReadyBtn.ClickAsync();
        await FillPassword(password);
        var patientDetailsPage = await ClickSubmitBtn();

        return patientDetailsPage;
    }
}