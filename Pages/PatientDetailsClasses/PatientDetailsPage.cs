using Microsoft.Playwright;
using PlaywrightTests.Pages.LandingPageClasses;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Pages.PatientDetailsClasses;

public class PatientDetailsPage : BasePage
{
    private readonly IPage _page;

    public PatientDetailsPage(IPage page) : base(page)
    {
        this._page = page;
    }

    // Page locators...
    public ILocator StudyLandingPageLink => page.Locator("#tm-link-studylandingpage");

    public ILocator V2EventCard => page.Locator("h4").Filter(new() { HasText = "V2" });
    public ILocator SetAnEventDateLink => page.GetByRole(AriaRole.Link, new() { Name = "Set an event date" });
    public ILocator Visit1Cell => page.GetByRole(AriaRole.Cell, new() { Name = "Visit1" });
    public ILocator EventDateSetIcon => page.Locator("table").Filter(new() { HasText = "Event date" }).Locator("span.icon.ok");
    public ILocator SignLink => page.GetByRole(AriaRole.Link, new() { Name = "SIGN", Exact = true });

    // Page methods...

    /// <summary>
    /// Navigates to the Study Landing Page from the Patient Details Page.
    /// </summary>
    /// <returns>An instance of the LandingPage class, representing the navigated-to Study Landing Page.</returns>
    public async Task<LandingPage> NavigateToStudyLandingPage()
    {
        await StudyLandingPageLink.ClickAsync();
        return new LandingPage(page);
    }

    /// <summary>
    /// Clicks on the link to set an event date, which opens the Event Date Popup.
    /// </summary>
    /// <returns>An instance of the EventDatePopupPage class, representing the opened Event Date Popup.</returns>
    public async Task<EventDatePopupPage> ClickSetAnEventDateLink()
    {
        //await V2EventCard.WaitForAsync();
        await page.WaitForFunctionAsync("() => typeof window.viedoc?.helpers?.setupFancyBox === 'function'");
        await page.WaitForFunctionAsync("() => typeof window.viedoc?.helpers?.showModal === 'function'");
        await page.WaitForFunctionAsync("() => typeof window.viedoc?.helpers?.showOverlay === 'function'");
        await page.WaitForFunctionAsync("() => typeof window.viedoc?.helpers?.smallPopupOptions === 'function'");
        await SetAnEventDateLink.ClickAsync();
        return new EventDatePopupPage(page);
    }

    /// <summary>
    /// Sets the event date for the patient. If no date is provided, today's date is used.
    /// </summary>
    /// <param name="todayTwoDigits">Optional parameter for the day of the event. If not provided, the current day is used.</param>
    public async Task SetEventDate(int? todayTwoDigits = null)
    {
        int day = todayTwoDigits ?? DateTime.Now.Day;
        var eventDatePopup = await ClickSetAnEventDateLink();
        await eventDatePopup.InitiateEvent(day);
    }

    /// <summary>
    /// Opens the form for the first visit and returns an instance of the Visit1PopupPage.
    /// </summary>
    public async Task<Visit1PopupPage> OpenVisit1Form()
    {
        await EventDateSetIcon.WaitForAsync();
        await V2EventCard.WaitForAsync();
        await Visit1Cell.ClickAsync();
        return new Visit1PopupPage(page);
    }

    /// <summary>
    /// Adds patient data for the first visit.
    /// </summary>
    /// <param name="feeling">The feeling data to be added for the patient.</param>
    /// <param name="fecesOption">The feces option to be selected for the patient.</param>
    public async Task AddPatientVisit1Data(string feeling, Feces fecesOption)
    {
        var visit1Page = await OpenVisit1Form();
        await visit1Page.AddPatientData(feeling, fecesOption);
    }

    /// <summary>
    /// Edits the feeling data for a patient's first visit and provides a reason for the edit.
    /// </summary>
    /// <param name="feeling">The new feeling data to be entered.</param>
    /// <param name="reason">The reason for changing the feeling data.</param>
    /// <param name="otherReasonDescription">An optional description if the reason is 'Other'.</param>
    public async Task EditPatientVisit1FeelingData(string feeling, ReasonForEdit reason, string otherReasonDescription = "")
    {
        var visit1Page = await OpenVisit1Form();
        await visit1Page.EditFeelingData(feeling, reason, otherReasonDescription);
    }

    /// <summary>
    /// Edits the feces data for a patient's first visit and provides a reason for the edit.
    /// </summary>
    /// <param name="fecesOption">The new feces option to be selected.</param>
    /// <param name="reason">The reason for changing the feces data.</param>
    /// <param name="otherReasonDescription">An optional description if the reason is 'Other'.</param>
    public async Task EditPatientVisit1FecesData(Feces fecesOption, ReasonForEdit reason, string otherReasonDescription = "")
    {
        var visit1Page = await OpenVisit1Form();
        await visit1Page.EditFecesData(fecesOption, reason, otherReasonDescription);
    }

    /// <summary>
    /// Opens the sign modal and returns an instance of the SigningPage.
    /// </summary>
    public async Task<SigningPage> OpenSignModal()
    {
        await SignLink.ClickAsync();
        return new SigningPage(page);
    }

    /// <summary>
    /// Signs all forms with the given password.
    /// </summary>
    public async Task Sign(string password)
    {
        var signingPage = await OpenSignModal();
        await signingPage.SignAllForms(password);
    }
}