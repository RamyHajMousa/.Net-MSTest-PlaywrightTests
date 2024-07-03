using Microsoft.Playwright;
using PlaywrightTests.Pages.PatientDetailsClasses;

namespace PlaywrightTests.Pages.SelectionPageClasses;

public class PatientInfoPopup : BasePage
{
    private IPage _page;

    public PatientInfoPopup(IPage page) : base(page)
    {
        _page = page;
    }

    // Page locators...
    public ILocator FirstNameInput => page.Locator("#FIRSTN");

    public ILocator LastNameInput => page.Locator("#LASTN");
    public ILocator GenderRadioGroup => page.GetByText("GENDER");
    public ILocator MaleRadioOption => page.Locator("#Form1 div").First;
    public ILocator FemaleRadioOption => page.Locator("#Form1 div", new() { HasText = "Female" });
    public ILocator SaveChangesBtn => page.GetByRole(AriaRole.Link, new() { Name = "Save changes" });
    public ILocator CloseBtn => page.GetByRole(AriaRole.Link, new() { Name = "Close" });

    // Page methods...

    /// <summary>
    /// Adds a new patient to the system using the Add Patient popup.
    /// </summary>
    /// <param name="firstName">The first name of the patient.</param>
    /// <param name="lastName">The last name of the patient.</param>
    /// <param name="gender">The gender of the patient.</param>
    /// <returns>A PatientDetailsPage object for the newly added patient.</returns>
    public async Task<PatientDetailsPage> AddPatient(string firstName, string lastName, Gender gender)
    {
        await FirstNameInput.ClearAsync();
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.ClearAsync();
        await LastNameInput.FillAsync(lastName);
        // Click on the appropriate radio button for the selected gender
        await (gender == Gender.Male ? MaleRadioOption : FemaleRadioOption).ClickAsync();
        await SaveChangesBtn.ClickAsync();

        return new PatientDetailsPage(page);
    }
}

/// <summary>
/// Enum representing the available genders in the Add Patient popup (When clicking on NEW CARD icon in the Selection page).
/// </summary>
public enum Gender
{
    Male,
    Female
}