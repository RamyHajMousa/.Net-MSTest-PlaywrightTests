using Microsoft.Playwright;

namespace PlaywrightTests.Pages.DesignerPageClasses;

public class NewStudyDesignPage : BasePage
{
    private readonly IPage _page;

    public NewStudyDesignPage(IPage page) : base(page)
    {
        this._page = page;
    }

    // Page locators...
    public ILocator ViedocLogo => page.GetByRole(AriaRole.Link, new() { Name = "Viedoc Logo" });

    public ILocator RolesViewBtn => page.GetByRole(AriaRole.Heading, new() { Name = "Roles View" }).GetByRole(AriaRole.Link);

    // Page methods...

    /// <summary>
    /// Navigates to the Roles page by clicking the Roles View button.
    /// </summary>
    /// <returns>A <see cref="RolesPage"/> object representing the newly opened Roles page.</returns>
    public async Task<RolesPage> OpenRolesPage()
    {
        await RolesViewBtn.ClickAsync();
        return new RolesPage(page);
    }
}