using Microsoft.Playwright;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Pages.LandingPageClasses;

public class DataExportTab : LandingPage
{
    private readonly IPage _page;

    public DataExportTab(IPage page) : base(page)
    {
        this._page = page;
    }

    // Page locators...
    //public ILocator DataExportBtn => _page.GetByRole(AriaRole.Link, new() { Name = "Data Export" });
    public ILocator Site(Country country) => page.Locator("#exportform  ul.nav-sites")
        .GetByRole(AriaRole.Link, new() { Name = country.ToString() });

    public ILocator OutputFormatSection => page.GetByText("Output format");

    public ILocator DataExportConfigOptions(ConfigOption option) => page.Locator("ul.accordion > li")
        .Filter(new() { HasText = EnumHelper.GetEnumDescription(option) });

    public ILocator ConfigOtionToggleBtn(ConfigOption option) => DataExportConfigOptions(option)
        .Locator(".title > .holder");

    public ILocator OutputToDropdown => page.Locator("#select2-sel-outputFormat-container");

    public ILocator OutputFormatOption(FormatOption option) => page
        .GetByRole(AriaRole.Treeitem, new() { Name = option.ToString() });

    public ILocator DataGroupingDropdown => page.Locator("#select2-sel-grouping-container");

    public ILocator DataGroupingOption(DataGroupingOption option) => page
        .GetByRole(AriaRole.Treeitem, new() { Name = EnumHelper.GetEnumDescription(option) });

    public ILocator RowPerSubjectRadio => page.Locator("div:nth-child(2) > .radio-item > div");
    public ILocator RowPerActivityRadio => page.Locator("div:nth-child(3) > .radio-item > div");
    public ILocator RowPerItemRadio => page.Locator("div:nth-child(4) > .radio-item > div");
    public ILocator HistoryCheckbox => page.Locator("div:nth-child(12) > .field-item > .check-item > div");
    public ILocator ExportDataBtn => page.GetByRole(AriaRole.Link, new() { Name = "Export data" });

    // Since there's a brief time gap between setting dateTimeNow and finding the exported file in the list,
    // the minute part of dateTimeNow could increment.
    private string fileNameNow = DateTime.UtcNow.ToString("dd MMM yyyy HH:mm");

    private string fileNameAfterOneMinute = DateTime.UtcNow.AddMinutes(1).ToString("dd MMM yyyy HH:mm");

    public ILocator ExportedFile => page.GetByRole(AriaRole.Link, new() { Name = fileNameNow }).First.
        Or(page.GetByRole(AriaRole.Link, new() { Name = fileNameAfterOneMinute })).First;

    public ILocator ExportedFileReadyToDownloadSign() => ExportedFile.Locator("span.text-green");

    // Page methods...

    /// <summary>
    /// Selects a specific site from the list of available sites...
    /// </summary>
    /// <param name="country">The country to select.</param>
    public async Task SelectSite(Country country)
    {
        await Site(country).ClickAsync();
        // Since the forms for different sites (e.g., 'All sites', 'Japan') under the "Data Export" tab are almost identical,
        // clicking on a specific site link like 'Japan' updates the form to show data for that site. However, this change might not
        // be immediately detected by Playwright because the elements in the 'Japan' form closely resemble those in the 'All sites' form.
        // To ensure accurate navigation and data export, we wait until the 'Japan' link becomes hidden, signifying that the correct
        // form is loaded and visible. This step prevents inadvertent data export from the 'All sites' form.
        await Site(country).IsHiddenAsync();
    }

    /// <summary>
    /// Exports the data to a Microsoft Excel file with specified grouping and layout options.
    /// </summary>
    /// <param name="option">The data grouping option to apply to the export.</param>
    /// <param name="layoutOption">The layout option for how data should be presented in the Excel file.</param>
    /// <param name="includeHistory">Flag to include historical data in the export. Defaults to false.</param>
    public async Task ExportMicrosoftExcelFile(DataGroupingOption option, LayoutOption layoutOption, bool includeHistory = false)
    {
        await ConfigOtionToggleBtn(ConfigOption.Output_format).ClickWithDelayAsync();

        // Checks if Microsoft Excel is currently selected as the output format.
        var outputText = await OutputToDropdown.TextContentAsync();
        if (!outputText!.Contains(FormatOption.Excel.ToString()))
        {
            await OutputToDropdown.ClickAsync();
            await OutputFormatOption(FormatOption.Excel).ClickAsync();
        }

        // Checks if the specified data grouping option is currently selected.
        var dataGroupingText = await DataGroupingDropdown.TextContentAsync();
        if (!dataGroupingText!.Contains(EnumHelper.GetEnumDescription(option)))
        {
            await DataGroupingDropdown.ClickAsync();
            await DataGroupingOption(option).ClickAsync();
        }

        await SelectLayoutOptionAsync(layoutOption, includeHistory);

        await ExportDataBtn.ClickAsync();
    }

    //public async Task ExportCsvFile()
    //{
    //}

    //public async Task ExportPdfFile()
    //{
    //}

    //public async Task ExportXmlFile()
    //{
    //}

    /// <summary>
    /// Selects the specified layout option for the data export process.
    /// </summary>
    /// <param name="layoutOption">The layout option to select.</param>
    /// <param name="includeHistory">A value indicating whether to include history data in the export.</param>
    /// <remarks>
    /// The layout option determines how the data is grouped and displayed in the export file.
    /// If <paramref name="includeHistory"/> is set to <c>true</c>, and the selected layout option is "1 row per item",
    /// then the history data for each item will be included in the export.
    /// </remarks>
    public async Task SelectLayoutOptionAsync(LayoutOption layoutOption, bool includeHistory = false)
    {
        switch (layoutOption)
        {
            case LayoutOption.One_row_per_subject:
                await RowPerSubjectRadio.ClickWithDelayAsync();
                break;

            case LayoutOption.One_row_per_activity:
                await RowPerActivityRadio.ClickWithDelayAsync();
                break;

            case LayoutOption.One_row_per_item:
                await RowPerItemRadio.ClickWithDelayAsync();
                if (includeHistory)
                {
                    await HistoryCheckbox.ClickWithDelayAsync();
                }
                break;

            default:
                throw new ArgumentException("Invalid layout option");
        }
    }
}

/// <summary>
/// Enum representing the available sites options in the study form (When clicking on the study card).
/// This enum is used to specify and manage the sites links that can be clicked
/// </summary>
public enum Country
{
    [System.ComponentModel.Description("All sites")]
    All_sites,

    Japan,
    Sweden
}

/// <summary>
/// Enum representing the sections under every site in the Data Export tab
/// This enum is used to specify and manage the section that can be selected
/// </summary>
public enum ConfigOption
{
    [System.ComponentModel.Description("Subjects to include")]
    Subjects_to_include,

    [System.ComponentModel.Description("Events and time period")]
    Events_and_time_period,

    [System.ComponentModel.Description("Forms and items")]
    Forms_and_items,

    [System.ComponentModel.Description("Type of data")]
    Type_of_data,

    [System.ComponentModel.Description("Output format")]
    Output_format
}

/// <summary>
/// Enum representing the available options for the output format of the data export.
/// </summary>
public enum FormatOption
{
    Excel,
    CSV,
    PDF,
    CDISC
}

/// <summary>
/// Enum representing the options for grouping data in the data export process.
/// </summary>
public enum DataGroupingOption
{
    [System.ComponentModel.Description("Group data by form")]
    Group_data_by_form,

    [System.ComponentModel.Description("Do not group data")]
    Do_not_group_data
}

/// <summary>
/// Enum representing the layout options for the data export process.
/// </summary>
public enum LayoutOption
{
    [System.ComponentModel.Description("1 row per subject")]
    One_row_per_subject,

    [System.ComponentModel.Description("1 row per activity")]
    One_row_per_activity,

    [System.ComponentModel.Description("1 row per item")]
    One_row_per_item
}