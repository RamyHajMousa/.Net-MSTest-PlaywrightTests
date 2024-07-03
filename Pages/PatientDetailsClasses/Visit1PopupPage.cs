using Microsoft.Playwright;
using static System.Collections.Specialized.BitVector32;

namespace PlaywrightTests.Pages.PatientDetailsClasses;

public class Visit1PopupPage : BasePage
{
    private readonly IPage _page;

    public Visit1PopupPage(IPage page) : base(page)
    {
        this._page = page;
    }

    // Page locators...
    public ILocator Visit1Header => base.page.GetByRole(AriaRole.Heading, new() { Name = "Visit1" });

    public ILocator SaveChangesBtn => base.page.GetByRole(AriaRole.Link, new() { Name = "Save changes" });
    public ILocator CloseBtn => base.page.GetByRole(AriaRole.Link, new() { Name = "Close" });
    public ILocator FeelingsQuestionLabel => base.page.GetByText("How do you feel?");
    public ILocator FeelingsTextInput => base.page.Locator("#Form11");
    public ILocator FecesRadioLabel => base.page.GetByText("Feces");
    public ILocator FecesYesOption => base.page.Locator(".radio-btn").First;
    public ILocator FecesNoOption => base.page.Locator(".radio-btn").Nth(1);
    public ILocator ShowHistoryToggle => base.page.GetByRole(AriaRole.Link, new() { Name = "SHOW HISTORY" });
    public ILocator FeelingsGiveReasonSection => base.page.Locator("#Form11_rfc");
    public ILocator FecesGiveReasonSection => base.page.Locator("#Form12_rfc");

    public ILocator FeelingHistorySection => base.page.Locator("#formcanvas").GetByRole(AriaRole.List).First;
    public ILocator FecesHistorySection => base.page.Locator("#formcanvas").GetByRole(AriaRole.List).Nth(1);
    public ILocator EditBtn => base.page.GetByRole(AriaRole.Link, new() { Name = "Edit" });
    public ILocator ResultMessage => base.page.Locator("#resultmessage");
    public ILocator FormHistoryLink => base.page.GetByRole(AriaRole.Link, new() { Name = "Form History" });

    // Page methods...

    /// <summary>
    /// Closes the Visit 1 Popup and returns to the Patient Details Page.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, returning to the Patient Details Page.</returns>
    public async Task<PatientDetailsPage> Close()
    {
        await CloseBtn.ClickAsync();
        return new PatientDetailsPage(base.page);
    }

    /// <summary>
    /// Saves the changes made in the Visit 1 Popup and returns to the Patient Details Page.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, returning to the Patient Details Page.</returns>
    public async Task<PatientDetailsPage> SaveChanges()
    {
        await SaveChangesBtn.ClickAsync();
        return new PatientDetailsPage(base.page);
    }

    /// <summary>
    /// Clears the text input field and enters the specified text.
    /// </summary>
    /// <param name="feeling">The feeling to be entered into the text input field.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    public async Task FillFeelingsTextInput(string feeling)
    {
        await FeelingsTextInput.ClearAsync();
        await FeelingsTextInput.FillAsync(feeling);
    }

    /// <summary>
    /// This function is used to select the feces option from the visit 1 popup form.
    /// </summary>
    /// <param name="fecesOption">The feces option to be selected.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    public async Task SelectFecesOption(Feces fecesOption)
    {
        await (fecesOption.ToString() == "yes" ? FecesYesOption : FecesNoOption).ClickAsync();
    }

    /// <summary>
    /// This function is used to add patient details to the Visit 1 popup form.
    /// </summary>
    /// <param name="feeling">The user's feeling at the time of the visit.</param>
    /// <param name="fecesOption">The user's response to the feces question.</param>
    /// <returns>A new instance of the PatientDetailsPage class.</returns>
    public async Task<PatientDetailsPage> AddPatientData(string feeling, Feces fecesOption)
    {
        await FillFeelingsTextInput(feeling);
        await SelectFecesOption(fecesOption);
        await SaveChangesBtn.ClickAsync();

        return new PatientDetailsPage(base.page);
    }

    /// <summary>
    /// Triggers the reason for change process for a given section in the Visit 1 Popup.
    /// </summary>
    /// <param name="section">The section to provide a change reason for.</param>
    /// <returns>A task that represents the asynchronous operation, returning a new GiveReasonPopup object.</returns>
    public async Task<GiveReasonPopup> ClickGiveReasonBtn(Visit1PageSection section)
    {
        await (section.ToString() == "feelingsSection" ? FeelingsGiveReasonSection : FecesGiveReasonSection).ClickAsync();

        return new GiveReasonPopup(base.page);
    }

    /// <summary>
    /// Edits the feeling data in the Visit 1 Popup and provides a reason for the change.
    /// </summary>
    /// <param name="feeling">The new feeling to be entered.</param>
    /// <param name="reason">The reason for editing the feeling data.</param>
    /// <param name="otherReasonDescription">An additional description for the reason, if "Other" is selected.</param>
    /// <returns>A task that represents the asynchronous operation, returning to the Patient Details Page.</returns>
    public async Task<PatientDetailsPage> EditFeelingData(string feeling, ReasonForEdit reason, string otherReasonDescription = "")
    {
        await EditBtn.ClickAsync();
        await FillFeelingsTextInput(feeling);
        var giveReasonPage = await ClickGiveReasonBtn(Visit1PageSection.feelingsSection);
        await giveReasonPage.SubmitReasonForChange(reason, otherReasonDescription);
        var patientDetailsPage = await SaveChanges();

        return patientDetailsPage;
    }

    /// <summary>
    /// Edits the feces data in the Visit 1 Popup and provides a reason for the change.
    /// </summary>
    /// <param name="fecesOption">The new feces option to be selected.</param>
    /// <param name="reason">The reason for editing the feces data.</param>
    /// <param name="otherReasonDescription">An additional description for the reason, if "Other" is selected.</param>
    /// <returns>A task that represents the asynchronous operation, returning to the Patient Details Page.</returns>
    public async Task<PatientDetailsPage> EditFecesData(Feces fecesOption, ReasonForEdit reason, string otherReasonDescription = "")
    {
        var yesClassAtribute = await FecesYesOption.GetAttributeAsync("class");
        var noClassAtribute = await FecesNoOption.GetAttributeAsync("class");
        bool yesIsAlreadySelected = yesClassAtribute?.Split(' ').Contains("radioAreaChecked") == true;
        bool noIsAlreadySelected = noClassAtribute?.Split(' ').Contains("radioAreaChecked") == true;

        await EditBtn.ClickAsync();
        if (fecesOption.ToString() == "yes" && !yesIsAlreadySelected) await FecesYesOption.ClickAsync();
        if (fecesOption.ToString() == "no" && !noIsAlreadySelected) await FecesNoOption.ClickAsync();

        var giveReasonPage = await ClickGiveReasonBtn(Visit1PageSection.FecesSection);
        await giveReasonPage.SubmitReasonForChange(reason, otherReasonDescription);
        var patientDetailsPage = await SaveChanges();

        return patientDetailsPage;
    }

    /// <summary>
    /// Opens the form history to review the changes made.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, returning a new FormHistory object.</returns>
    public async Task<FormHistory> OpenFormHistory()
    {
        await FormHistoryLink.ClickAsync();

        return new FormHistory(page);
    }
}

/// <summary>
/// Enum indicating the selection of the feces option in the Visit 1 Popup form.
/// </summary>
public enum Feces
{
    yes,
    no
}

/// <summary>
/// Enum indicating the sections available in the Visit 1 Popup page.
/// </summary>
public enum Visit1PageSection
{
    feelingsSection,
    FecesSection
}