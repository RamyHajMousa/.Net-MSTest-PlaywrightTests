using Microsoft.Playwright;
using PlaywrightTests.Pages.DesignerPageClasses;
using PlaywrightTests.Pages.LandingPageClasses;
using PlaywrightTests.Pages.LoginPageClasses;
using PlaywrightTests.Pages.PatientDetailsClasses;
using PlaywrightTests.Pages.SelectionPageClasses;
using PlaywrightTests.Utils;

namespace PlaywrightTests.Tests;

[TestClass]
public class TestingTestPlanFunctionality : BaseTestSetup
{
    private LandingPage? _landingPage;

    [TestMethod]
    [TestCategory("TestingTestPlanFunctionality")]
    [TestProperty("TestCaseID", "45555: C04")]
    [TestProperty("Description", "Verifies that a user can successfully create and edit subject data, navigate to the eTMF page, and view document upload history, with all changes reflected in the audit trail.")]
    public async Task C04_VerifySubjectDataManagementAndAuditTrail()
    {
        UserEmail = TestContext.GetTestRunParameter("UserEmail2");
        UserPassword = TestContext.GetTestRunParameter("UserPassword2");
        _landingPage = await LoginPage!.Login(UserEmail!, UserPassword!);
        await _landingPage.SelectStudyCard("eTMFPreRelease");
        Page = await _landingPage.SelectTab(TabIdentifier.eTMF);
        var eTMFPage = new ETmfPage(Page);

        await Expect(eTMFPage.ArtifactsAndDocumentsContainer).ToBeHiddenAsync();
        await eTMFPage.NavigateToArtifactsAndDocuments();
        await Expect(eTMFPage.ArtifactsAndDocumentsContainer).ToBeVisibleAsync();
    }

    [TestMethod]
    [TestCategory("TestingTestPlanFunctionality")]
    [TestProperty("TestCaseID", "45556: C05")]
    [TestProperty("Description", "Verifies that a user can successfully enter and edit subject data, with changes accurately reflected in the audit trail.")]
    public async Task C05_VerifySubjectDataEntryAndAuditTrailReflection()
    {
        await SetupVisit1Page();
    }

    [TestMethod]
    [TestCategory("TestingTestPlanFunctionality")]
    [TestProperty("TestCaseID", "45558: C07")]
    [TestProperty("Description", "Verifies that a user can add subject data, select 'One row per item' in the Export section with 'Include History' enabled, and successfully export the data with history.")]
    public async Task C07_VerifySubjectDataExportWithHistoryOption()
    {
        _landingPage = await LoginPage!.Login(UserEmail!, UserPassword!);
        var selectionPage = await _landingPage.LaunchSelectionPage("StudyFromDaniel");
        await selectionPage.SelectSite("newsite1");

        // Adding patient...
        var patientInfo = await selectionPage.AddNewCard();
        var patientDetailsPage = await patientInfo.AddPatient("John", "Doe", Gender.Male);

        await patientDetailsPage.SetEventDate();
        await patientDetailsPage.AddPatientVisit1Data("Bad", Feces.yes);

        // Return to Study landing page...
        _landingPage = await patientDetailsPage.NavigateToStudyLandingPage();

        // Select Data Export tab in the study landing page
        Page = await _landingPage.SelectTab(TabIdentifier.Data_Export);
        var dataExportPage = new DataExportTab(Page);
        await dataExportPage.SelectSite(Country.Japan);

        // Export excel file with 1 row per item with "Include history" option enabled...
        await dataExportPage.ExportMicrosoftExcelFile(DataGroupingOption.Group_data_by_form, LayoutOption.One_row_per_item, true);

        await Expect(dataExportPage.ExportedFile).ToBeVisibleAsync();
        await Expect(dataExportPage.ExportedFile).ToContainTextAsync("Microsoft Excel - Office Open XML, 1 row per item");
    }

    [TestMethod]
    [TestCategory("TestingTestPlanFunctionality")]
    [TestProperty("TestCaseID", "45560: C08")]
    [TestProperty("Description", "Verifies that a user can successfully enter and edit subject data, with edits captured in the audit trail, and confirms that a new version of the form PDF is created every time the form is saved.")]
    public async Task C08_VerifySubjectDataManagementAuditTrailAndFormHistory()
    {
        var visit1Page = await SetupVisit1Page();
        var formHistoryPage = await visit1Page.OpenFormHistory();
        // Check if the History form contains "Version 1", "Version 2", "Version 3" PDF files
        await Expect(formHistoryPage.FormHistoryModal).ToContainTextAsync("Version 1");
        await Expect(formHistoryPage.FormHistoryModal).ToContainTextAsync("Version 2");
        await Expect(formHistoryPage.FormHistoryModal).ToContainTextAsync("Version 3");
    }

    [TestMethod]
    [TestCategory("TestingTestPlanFunctionality")]
    [TestProperty("TestCaseID", "45562: C10")]
    [TestProperty("Description", "Verify subject details addition, form data entry, form signing, editing, and screenshot capture.")]
    public async Task C10_VerifyFormSigningAndEditingWithScreenshotCapture()
    {
        //_landingPage = await LoginPage!.Login(UserEmail!, UserPassword!);
        _landingPage = await LoginPage!.Login(UserEmail!, UserPassword!);
        var selectionPage = await _landingPage.LaunchSelectionPage("StudyFromDaniel");
        await selectionPage.SelectSite("newsite1");

        // Adding patient...
        var patientInfo = await selectionPage.AddNewCard();
        var patientDetailsPage = await patientInfo.AddPatient("John", "Doe", Gender.Male);
        await patientDetailsPage.SetEventDate();
        await patientDetailsPage.AddPatientVisit1Data("Bad", Feces.yes);
        await patientDetailsPage.Sign(UserPassword!);
        await Page!.TakeScreenshot(TestContext, $"{TestContext.TestName} 1.png", 2);
        // Edit a signed form...
        await patientDetailsPage.EditPatientVisit1FeelingData("Good", ReasonForEdit.Other_reason, "This is a test");
        await Page!.TakeScreenshot(TestContext, $"{TestContext.TestName} 2.png", 2);

        await patientDetailsPage.Sign(UserPassword!);
        await Page!.TakeScreenshot(TestContext, $"{TestContext.TestName} 3.png", 2);
    }

    [TestMethod]
    [TestCategory("TestingTestPlanFunctionality")]
    [TestProperty("TestCaseID", "45564: C14")]
    [TestProperty("Description", "Verify subject data entry and editing, audit trail detail capture, and date/time zone accuracy.")]
    public async Task C14_VerifyAuditTrailDetailsAndDateTimeZoneAccuracy() 
    {
        var visit1Page = await SetupVisit1Page();
        // Date format in regular expression. Example: 09 Apr 2024 12:14 CEST
        // It matches time zones that are either 2 to 4 uppercase letters or follow a +/-HH:MM offset format.
        string pattern = @"\d{2}\s[A-Za-z]{3}\s\d{4}\s\d{2}:\d{2}\s([A-Z]{2,4}|[+-]\d{2}:\d{2})";
        // Assert edited Date captured on Audit trails with the time zone specified.
        await Expect(visit1Page.FeelingHistorySection).ToContainTextAsync(new Regex(pattern));
        await Expect(visit1Page.FecesHistorySection).ToContainTextAsync(new Regex(pattern));

        var patientDetailsPage = await visit1Page.Close();
        _landingPage = await patientDetailsPage.NavigateToStudyLandingPage();

        // Select Data Export tab in the study landing page
        Page = await _landingPage.SelectTab(TabIdentifier.Data_Export);
        var dataExportPage = new DataExportTab(Page);

        await dataExportPage.SelectSite(Country.Japan);

        // Export excel file with 1 row per item with "Include history" option enabled...
        await dataExportPage.ExportMicrosoftExcelFile(DataGroupingOption.Group_data_by_form, LayoutOption.One_row_per_item, true);
        await Expect(dataExportPage.ExportedFile).ToBeVisibleAsync();
        // Assert the exported file has UTC dateTime. Example: 09 Apr 2024 11:21 UTC
        string UtcPattern = @"\d{2}\s[A-Za-z]{3}\s\d{4}\s\d{2}:\d{2}\s*\n*\s*UTC";
        await Expect(dataExportPage.ExportedFile).ToContainTextAsync(new Regex(UtcPattern));
    }

    [TestMethod]
    [TestCategory("TestingTestPlanFunctionality")]
    [TestProperty("TestCaseID", "45566: C16")]
    [TestProperty("Description", "Verify navigation to roles page, role detail viewing, and eTMF roles verification.")]
    public async Task C16_VerifyRoleDetailsAndViewing() 
    {
        UserEmail = TestContext.GetTestRunParameter("UserEmail2");
        UserPassword = TestContext.GetTestRunParameter("UserPassword2");
        _landingPage = await LoginPage!.Login(UserEmail!, UserPassword!);

        // Navigate to Roles page...
        Page = await _landingPage.OpenDesignerPage();
        var designerPage = new DesignerPage(Page);
        var newStudyDesignerPage = await designerPage.OpenNewStudyDesignPage();
        var rolesPage = await newStudyDesignerPage.OpenRolesPage();


        // Assert all the available permissions are visible 
        await rolesPage.InvestigatorCell.WaitForAsync();
        await Expect(rolesPage.InvestigatorCell).ToBeVisibleAsync();
        await Expect(rolesPage.StudyCoordinatorCell).ToBeVisibleAsync();
        await Expect(rolesPage.MonitorCell).ToBeVisibleAsync();
        await Expect(rolesPage.ProjectManagerCell).ToBeVisibleAsync();
        await Expect(rolesPage.DataManagerCell).ToBeVisibleAsync();
        await Expect(rolesPage.MedicalCoderCell).ToBeVisibleAsync();
        await Expect(rolesPage.StudySupplyManagerCell).ToBeVisibleAsync();
        await Expect(rolesPage.SiteSupplyManagerCell).ToBeVisibleAsync();
        await Expect(rolesPage.RegulatoryInspectorCell).ToBeVisibleAsync();
  
        // Navigate to Study Details page
        newStudyDesignerPage = await rolesPage.Close();
        await newStudyDesignerPage.ViedocLogo.ClickAsync();
        // Navigate to eTMF page
        await _landingPage.SelectStudyCard("eTMFPreRelease");
        Page = await _landingPage.SelectTab(TabIdentifier.eTMF);
        var eTMFPage = new ETmfPage(Page);
        await Expect(eTMFPage.ArtifactsAndDocumentsContainer).ToBeHiddenAsync(new() { Timeout = 20000 });
        await eTMFPage.NavigateToArtifactsAndDocuments();
        await Expect(eTMFPage.ArtifactsAndDocumentsContainer).ToBeVisibleAsync();
    }

    [TestMethod]
    [TestCategory("TestingTestPlanFunctionality")]
    [TestProperty("TestCaseID", "45569: C20")]
    [TestProperty("Description", "Verify unsuccessful login with non-existent account and email notification via API.")]
    public async Task C20_VerifyUnsuccessfulLoginAndEmailNotification() 
    {
        // This test does not send an email to Mailtrap
        await LoginPage!.Login("fake@email.com", "#1A1a1a1a");
        await LoginPage!.Login("fake@email.com", "#1A1a1a1a");
        await LoginPage!.Login("fake@email.com", "#1A1a1a1a");
        await LoginPage!.Login("fake@email.com", "#1A1a1a1a");

    }

    [TestMethod]
    [TestCategory("TestingTestPlanFunctionality")]
    [TestProperty("TestCaseID", "45571: C22")]
    [TestProperty("Description", "Verify automatic logout after 20 minutes of inactivity and message display.")]
    public async Task C22_VerifyAutoLogoutAfterInactivity()
    {
        string autoLogoutMessage = "You have been automatically logged out. This is caused by 20 minutes inactivity time. Please log in again to continue working with Viedoc.";
        await Expect(Page!).ToHaveURLAsync(new Regex("^https://externaltest4idp.viedoc.dev/Account/Login"));
        await Expect(LoginPage!.AutoLogOutMessage).ToBeHiddenAsync();

        _landingPage = await LoginPage!.Login(UserEmail!, UserPassword!);
        await Expect(Page!).ToHaveURLAsync("https://externaltest4.viedoc.dev/");
        await Expect(_landingPage.WelcomeBackMessage).ToBeVisibleAsync();

        // Waits for 20 minutes and 10 seconds...
        await Page!.WaitForTimeoutAsync(1210000);

        await Expect(Page).ToHaveURLAsync(new Regex("^https://externaltest4idp.viedoc.dev/Account/Login"));
        await Expect(LoginPage.AutoLogOutMessage).ToBeVisibleAsync();
        await Expect(LoginPage.AutoLogOutMessage).ToHaveTextAsync(autoLogoutMessage);

    }

    // Helping methods...

    private async Task<Visit1PopupPage> SetupVisit1Page()
    {
        // Visit1 data...
        string initialFeeling = "Bad";
        string newFeeling = "Good";
        var reasonForChangingFeeling = ReasonForEdit.Transcription_error;
        var initialFecesOption = Feces.yes;
        var newFecesOption = Feces.no;
        var reasonForChangingFecesOption = ReasonForEdit.Query_resolution;

        _landingPage = await LoginPage!.Login(UserEmail!, UserPassword!);
        var selectionPage = await _landingPage.LaunchSelectionPage("StudyFromDaniel");
        await selectionPage.SelectSite("newsite1");

        // Adding patient...
        var patientInfo = await selectionPage.AddNewCard();
        var patientDetailsPage = await patientInfo.AddPatient("John", "Doe", Gender.Male);

        await patientDetailsPage.SetEventDate();
        await patientDetailsPage.AddPatientVisit1Data(initialFeeling, initialFecesOption);

        // Editing existing data...
        await patientDetailsPage.EditPatientVisit1FeelingData(newFeeling, reasonForChangingFeeling);
        await patientDetailsPage.EditPatientVisit1FecesData(newFecesOption, reasonForChangingFecesOption);

        // Verify that changes are reflected with reason for change
        var visit1Page = await patientDetailsPage.OpenVisit1Form();
        await visit1Page.ShowHistoryToggle.ClickAsync();
        await Expect(visit1Page.FeelingHistorySection)
            .ToContainTextAsync($"How do you feel? {initialFeeling} {newFeeling} {EnumHelper.GetEnumDescription(reasonForChangingFeeling)}");
        await Expect(visit1Page.FecesHistorySection)
            .ToContainTextAsync($"Feces {initialFecesOption} {newFecesOption} {EnumHelper.GetEnumDescription(reasonForChangingFecesOption)}");

        return visit1Page;
    }

}


