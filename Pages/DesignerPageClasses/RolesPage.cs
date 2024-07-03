using Microsoft.Playwright;

namespace PlaywrightTests.Pages.DesignerPageClasses;

public class RolesPage : NewStudyDesignPage
{
    private readonly IPage _page;

    public RolesPage(IPage page) : base(page)
    {
        this._page = page;
    }

    // Page locators...
    public ILocator Header => page.GetByRole(AriaRole.Heading, new() { Name = "Roles", Exact = true });

    public ILocator CloseBtn => page.GetByRole(AriaRole.Navigation).GetByRole(AriaRole.Link, new() { Name = "Close" });
    public ILocator InvestigatorCell => page.GetByRole(AriaRole.Cell, new() { Name = "Investigator on Role ID: R1" });
    public ILocator StudyCoordinatorCell => page.GetByRole(AriaRole.Cell, new() { Name = "Study Coordinator off Role ID" });
    public ILocator MonitorCell => page.GetByRole(AriaRole.Cell, new() { Name = "Monitor off Role ID: R3" });
    public ILocator ProjectManagerCell => page.GetByRole(AriaRole.Cell, new() { Name = "Project Manager off Role ID:" });
    public ILocator DataManagerCell => page.GetByRole(AriaRole.Cell, new() { Name = "Data Manager on Role ID: R5" });
    public ILocator MedicalCoderCell => page.GetByRole(AriaRole.Cell, new() { Name = "Medical Coder off Role ID: R6" });
    public ILocator StudySupplyManagerCell => page.GetByRole(AriaRole.Cell, new() { Name = "Study Supply Manager off Role" });
    public ILocator SiteSupplyManagerCell => page.GetByRole(AriaRole.Cell, new() { Name = "Study Supply Manager off Role" });
    public ILocator RegulatoryInspectorCell => page.GetByRole(AriaRole.Cell, new() { Name = "Regulatory Inspector off Role" });

    // Page Methods...

    /// <summary>
    /// Closes the Roles page and returns to the New Study Design page.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation of clicking the Close button.
    /// The task result is a <see cref="NewStudyDesignPage"/> object representing the page returned to after the Roles page is closed.</returns>
    public async Task<NewStudyDesignPage> Close()
    {
        await CloseBtn.ClickAsync();
        return new NewStudyDesignPage(page);
    }
}