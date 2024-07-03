using Microsoft.Playwright;

namespace PlaywrightTests.Pages.PatientDetailsClasses;

public class FormHistory : BasePage
{
    private readonly IPage _page;

    public FormHistory(IPage page) : base(page)
    {
        _page = page;
    }

    // Page locators...
    public ILocator Header => page.GetByRole(AriaRole.Heading, new() { Name = "Form History" });

    public ILocator FormHistoryModal => page.Locator("#formhistory");
    public ILocator CloselBtn => page.Locator("#action_popup_cancel");
}