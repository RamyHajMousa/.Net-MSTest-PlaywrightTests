using Microsoft.Playwright;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Pages.PatientDetailsClasses;

public class GiveReasonPopup : BasePage
{
    private readonly IPage _page;

    public GiveReasonPopup(IPage page) : base(page)
    {
        this._page = page;
    }

    // Page locators...
    public ILocator TranscriptionErrorRadio => page.Locator("#rfc_transcriptionerror div");

    public ILocator QueryResolutionRadio => page.Locator("#rfc_queryresolution div");
    public ILocator OtherReasonRadio => page.Locator("#rfc_other div");
    public ILocator OtherReasonDescription => page.Locator("#rfc_other_reason");
    public ILocator ReadyBtn => page.GetByRole(AriaRole.Link, new() { Name = "Ready" });
    public ILocator CancelBtn => page.GetByRole(AriaRole.Link, new() { Name = "Cancel" });

    // Page methods...

    /// <summary>
    /// Submits the reason for a change made in patient data within the Visit 1 Popup Page.
    /// </summary>
    /// <param name="reason">The reason for the edit, chosen from predefined options.</param>
    /// <param name="otherReasonDescription">An optional description if 'Other reason' is selected. Defaults to an empty string, and 'Not specified' is used if left blank.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a new instance of Visit1PopupPage.</returns>
    public async Task<Visit1PopupPage> SubmitReasonForChange(ReasonForEdit reason, string otherReasonDescription = "")
    {
        var reasonForEdit = EnumHelper.GetEnumDescription(reason);
        switch (reasonForEdit)
        {
            case "Transcription error":
                await TranscriptionErrorRadio.ClickAsync();
                break;

            case "Query resolution":
                await QueryResolutionRadio.ClickAsync();
                break;

            case "Other reason":
                await OtherReasonRadio.ClickAsync();
                if (!string.IsNullOrEmpty(otherReasonDescription))
                {
                    await OtherReasonDescription.FillAsync(otherReasonDescription);
                }
                else
                {
                    await OtherReasonDescription.FillAsync("Not specified");
                }
                break;
        }
        await ReadyBtn.ClickAsync();

        return new Visit1PopupPage(page);
    }
}

/// <summary>
/// Enumerates the reasons for editing patient data within the system.
/// </summary>
public enum ReasonForEdit
{
    [System.ComponentModel.Description("Transcription error")]
    Transcription_error,

    [System.ComponentModel.Description("Query resolution")]
    Query_resolution,

    [System.ComponentModel.Description("Other reason")]
    Other_reason,
}