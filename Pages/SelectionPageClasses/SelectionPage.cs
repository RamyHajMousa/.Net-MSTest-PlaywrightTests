using Microsoft.Playwright;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Pages.SelectionPageClasses;

public class SelectionPage : BasePage
{
    private readonly IPage _page;

    public SelectionPage(IPage page) : base(page)
    {
        _page = page;
    }

    // Page locators...
    public ILocator ChooseStudySiteDropdown => page.Locator("#studysiteids_chosen");

    public ILocator StudySiteName(string siteName) => page.Locator(".chosen-results > li", new() { HasText = siteName });

    public ILocator ChosenStudySite(string siteName) => page.Locator("a").Filter(new() { HasTextRegex = new Regex($"^{siteName}$") });

    public ILocator NewCard => page.GetByRole(AriaRole.Link, new() { Name = "NEW CARD", Exact = true });

    // Page methods...

    /// <summary>
    /// Selects a study site from the dropdown menu.
    /// </summary>
    /// <param name="siteName">The name of the study site to select.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SelectSite(string siteName)
    {
        await ChooseStudySiteDropdown.ClickWithDelayAsync();
        await StudySiteName(siteName).ClickAsync();
        await ChosenStudySite(siteName).WaitForAsync();
    }

    /// <summary>
    /// Clicks on the 'New Card' button and creates a new instance of the PatientInfoPopup class.
    /// </summary>
    /// <returns>A new instance of the PatientInfoPopup class.</returns>
    public async Task<PatientInfoPopup> AddNewCard()
    {
        //await ChooseStudySiteDropdown.WaitForAsync();
        await NewCard.IsEnabledAsync();
        await NewCard.ClickAsync();

        return new PatientInfoPopup(page);
    }
}