using Microsoft.Playwright;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Pages.LandingPageClasses;

public class ETmfPage : LandingPage
{
    private readonly IPage _page;

    public ETmfPage(IPage page) : base(page)
    {
        this._page = page;
    }

    // Page locators
    public ILocator Heading => page.GetByRole(AriaRole.Heading, new() { Name = "eTMF" });

    public ILocator TrialManagementSection => page.GetByText("Trial Management");
    public ILocator TrialOversightSection => page.GetByText("Trial Oversight");
    public ILocator ArtifactsAndDocumentsContainer => page.GetByRole(AriaRole.Cell, new() { Name = "Artifacts & documents in" });

    // Page methods...

    /// <summary>
    /// Navigates to the Artifacts and Documents section within the ETmfPage.
    /// </summary>
    /// <remarks>
    /// This method first clicks on the 'Trial Management' section and then on the 'Trial Oversight' section.
    /// It assumes that navigating to these sections will reveal the Artifacts and Documents container.
    /// </remarks>
    public async Task NavigateToArtifactsAndDocuments()
    {
        await TrialManagementSection.IsVisibleAsync();
        await TrialManagementSection.ClickWithDelayAsync();
        await TrialOversightSection.ClickWithDelayAsync();
    }
}