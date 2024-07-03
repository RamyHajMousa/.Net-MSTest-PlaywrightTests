using Microsoft.Playwright;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Pages.PatientDetailsClasses;

public class EventDatePopupPage : BasePage
{
    private readonly IPage _page;

    public EventDatePopupPage(IPage page) : base(page)
    {
        this._page = page;
    }

    // Page locators...
    public ILocator PlanEventLink => page.GetByRole(AriaRole.Link, new() { Name = "Plan event" });

    public ILocator InitiateEventLink => page.GetByRole(AriaRole.Link, new() { Name = "Initiate event" });
    public ILocator CloseBtn => page.GetByRole(AriaRole.Link, new() { Name = "Close" });
    public ILocator DateTable => page.Locator("ul#details-add-visit-nav").GetByRole(AriaRole.Table);

    public ILocator DateTableGridcell(int day) => DateTable.Locator("td:not(.old):not(.disabled)") // Exclude old and disabled dates
        .GetByText(day.ToString(), new() { Exact = true });

    public ILocator EventDate => page.Locator("#details-studyevent-edit").GetByText("Event date", new() { Exact = true });
    public ILocator SaveChangesBtn => page.GetByRole(AriaRole.Link, new() { Name = "Save changes" });

    // Page methods...

    /// <summary>
    /// Clicks on the "Initiate Event" link and selects the specified date from the date table in the popup.
    /// </summary>
    /// <param name="day">The day of the month to select from the date table.</param>
    /// <returns>A new instance of the PatientDetailsPage class.</returns>
    public async Task<PatientDetailsPage> InitiateEvent(int day)
    {
        await InitiateEventLink.ClickWithDelayAsync();
        await DateTableGridcell(day).ClickAsync();
        await EventDate.WaitForAsync();
        await SaveChangesBtn.ClickAsync();
        return new PatientDetailsPage(page);
    }
}