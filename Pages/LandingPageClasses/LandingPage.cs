using Microsoft.Playwright;
using PlaywrightTests.Pages.DesignerPageClasses;
using PlaywrightTests.Pages.SelectionPageClasses;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Pages.LandingPageClasses;

public class LandingPage : BasePage
{
    private readonly IPage _page;

    public LandingPage(IPage page) : base(page)
    {
        this._page = page;
    }

    // Page loacators...
    public ILocator ViedocLogo => page.GetByRole(AriaRole.Link, new() { Name = "Viedoc Logo" });

    public ILocator WelcomeBackMessage => page.GetByRole(AriaRole.Heading, new() { Name = "Welcome back" });
    public ILocator AdminPageLink => page.GetByRole(AriaRole.Link, new() { Name = "Admin" });
    public ILocator DesignerPageLink => page.GetByRole(AriaRole.Link, new() { Name = "Designer" });

    public ILocator StudyCard(string studyName) => page.GetByRole(AriaRole.Link, new() { Name = studyName });

    public ILocator LaunchBtn => page.GetByRole(AriaRole.Link, new() { Name = "Launch" });
    public ILocator DataExportBtn => page.GetByRole(AriaRole.Link, new() { Name = "Data Export" });
    public ILocator eTMFBtn => page.Locator("#slp-link-etmf");

    // Page methods...

    /// <summary>
    /// Navigates to the Designer page from the landing page.
    /// </summary>
    /// <returns>An instance of the DesignerPage class representing the new page.</returns>
    public async Task<IPage> OpenDesignerPage()
    {
        // Navigate to User Designer page...
        var newPage = await page.Context.RunAndWaitForPageAsync(async () => await DesignerPageLink.ClickAsync());
        return newPage;
    }

    /// <summary>
    /// Selects a study card by its name to interact with.
    /// </summary>
    /// <param name="studyName">The name of the study to select.</param>
    public async Task SelectStudyCard(string studyName)
    {
        await StudyCard(studyName).ClickWithDelayAsync(3);
    }

    /// <summary>
    /// Launches the selection page for the specified study.
    /// </summary>
    /// <param name="studyName">The name of the study.</param>
    /// <returns>The selection page.</returns>
    public async Task<SelectionPage> LaunchSelectionPage(string studyName)
    {
        await SelectStudyCard(studyName);
        await LaunchBtn.ClickWithDelayAsync();

        return new SelectionPage(page);
    }

    /// <summary>
    /// Switches to the specified tab in Study landing page.
    /// </summary>
    /// <param name="tabIdentifier">The identifier of the tab to switch to.</param>
    /// <returns>The IPage instance representing the newly navigated page if a new page is triggered, or the current page otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid tab identifier is provided.</exception>
    public async Task<IPage> SelectTab(TabIdentifier tabIdentifier)
    {
        switch (tabIdentifier)
        {
            // Case TabIdentifier.Study_status:...

            case TabIdentifier.Data_Export:
                await DataExportBtn.ClickWithDelayAsync(2);
                return page;

            // Case TabIdentifier.Reference data:...
            // Case TabIdentifier.Medical coding:...
            case TabIdentifier.eTMF:
                // Navigate to User eTMF page...
                var newPage = await page.Context.RunAndWaitForPageAsync(async () =>
                {
                    await eTMFBtn.ClickWithDelayAsync(2);
                });
                //await newPage.BringToFrontAsync();
                return newPage;

            // Add more cases as needed...
            default:
                throw new ArgumentException("Invalid tab identifier");
        }
    }
}

/// <summary>
/// Defines identifiers for various tabs within the Study landing page.
/// </summary>
public enum TabIdentifier
{
    [System.ComponentModel.Description("Study status")]
    Study_status,

    Messages,

    [System.ComponentModel.Description("Data Export")]
    Data_Export,

    Metric,
    Roles,

    [System.ComponentModel.Description("Reference data")]
    Reference_data,

    [System.ComponentModel.Description("Medical coding")]
    Medical_coding,

    eTMF
    // Add more identifiers...
}