using Microsoft.Playwright;

namespace PlaywrightTests.Pages;

public abstract class BasePage
{
    protected readonly IPage page;

    protected BasePage(IPage page)
    {
        this.page = page;
    }
}