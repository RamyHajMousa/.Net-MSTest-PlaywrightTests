using Microsoft.Playwright;

namespace PlaywrightTests.Pages.DesignerPageClasses;

public class DesignerPage : BasePage
{
    private readonly IPage _page;

    public DesignerPage(IPage page) : base(page)
    {
        this._page = page;
    }

    // Page locators...
    public ILocator NewStudyDesignViewBtn => page.GetByRole(AriaRole.Link, new() { Name = "View" });

    // Page methods...

    /// <summary>
    /// Opens the New Study Design page by clicking on the 'View' button.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an instance of the NewStudyDesignPage.</returns>
    public async Task<NewStudyDesignPage> OpenNewStudyDesignPage()
    {
        await NewStudyDesignViewBtn.ClickAsync();
        return new NewStudyDesignPage(page);
    }
}