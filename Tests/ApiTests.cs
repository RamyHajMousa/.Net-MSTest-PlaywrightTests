using Microsoft.Playwright;
using PlaywrightTests.Utils;
using System.Text.Json;

namespace PlaywrightTests.Tests;

[TestClass]
public class ApiTests : PageTest
{
    private string? TokenURL;
    private string? BaseURL;

    private IAPIRequestContext? Request;

    [TestInitialize]
    public async Task TestInitialize()
    {
        TokenURL = TestContext.GetTestRunParameter("TokenURL");
        BaseURL = TestContext.GetTestRunParameter("BaseURL");

        Request = await Playwright.APIRequest.NewContextAsync();
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        if (Request != null)
        {
            await Request.DisposeAsync();
        }
    }

    [TestMethod]
    [TestCategory("ApiTests")]
    public async Task PostTokenRequest_ShouldReturnAccessToken()
    {
        var formData = Context.APIRequest.CreateFormData();
        formData.Set("client_id", "add the client id here");
        formData.Set("client_secret", "add the secret here");
        formData.Set("grant_type", "client_credentials");

        try
        {
            var response = await Request!.PostAsync(TokenURL!, new()
            {
                Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" }
            },
                Form = formData
            });

            var responseText = await response.TextAsync();
            TestContext.WriteLine($"Response Text: {responseText}");

            if (!response.Ok)
            {
                var errorBody = await response.TextAsync();
                Assert.Fail($"Response was not OK. Status: {response.Status}, Body: {errorBody}");
            }

            // Read and parse the response body
            var responseBody = await response.JsonAsync();
            var accessToken = responseBody.Value.GetProperty("access_token").GetString();
            var tokenType = responseBody.Value.GetProperty("token_type").GetString();
            var expiresIn = responseBody.Value.GetProperty("expires_in").GetInt32();
            var scope = responseBody.Value.GetProperty("scope").GetString();

            // Validate the response content
            Assert.IsNotNull(accessToken, "Access token is null");
            Assert.AreEqual("Bearer", tokenType, "Token type is not Bearer");
            Assert.IsTrue(expiresIn == 3600, "Expires in is not equal to 3600");
            Assert.IsNotNull(scope, "Scope is null");
        }
        catch (PlaywrightException ex)
        {
            Assert.Fail($"PlaywrightException: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Assert.Fail($"JsonException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception: {ex.Message}");
        }
    }

    [TestMethod]
    [TestCategory("ApiTests")]
    public async Task GetStudySites_ShouldReturnSites()
    {
        string token = await GetAccessToken();

        try
        {
            var response = await Request!.GetAsync($"{BaseURL}/admin/studysites", new()
            {
                Headers = new Dictionary<string, string>
                {
                     { "Authorization", $"Bearer { token }" }
                },
            });

            Assert.IsTrue(response.Ok, "API request failed");

            var responseBody = await response.JsonAsync();
            Assert.IsNotNull(responseBody, "Response body is null");

            // Additional assertions can be made here based on the response
            TestContext.WriteLine($"Response Body: {responseBody}");
        }
        catch (PlaywrightException ex)
        {
            Assert.Fail($"PlaywrightException: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Assert.Fail($"JsonException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Exception: {ex.Message}");
        }
    }

    [TestMethod]
    [TestCategory("ApiTests")]
    public async Task PostStudySite()
    {
        string token = await GetAccessToken();

        var dataObject = new
        {
            siteCode = "ST4",
            siteName = "Site 4",
            countryCode = "SE",
            timeZoneId = "UTC",
            expectedNumberOfSubjectsScreened = "24",
            expectedNumberOfSubjectsEnrolled = "20",
            maximumNumberOfSubjectsScreened = "26",
            isTrainingEnabled = true,
            isProductionEnabled = false
        };

        var response = await Request!.PostAsync($"{BaseURL}/admin/studysites", new()
        {
            Headers = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                },

            DataObject = dataObject,
        });

        Assert.IsTrue(response.Ok, "API request failed");
        var responseBody = await response.JsonAsync();
        Assert.IsNotNull(responseBody, "Response body is null");

        // Additional assertions can be made here based on the response
    }

    private async Task<string> GetAccessToken()
    {
        var formData = Context.APIRequest.CreateFormData();
        formData.Set("client_id", "Add the client id here");
        formData.Set("client_secret", "Add the secret here");
        formData.Set("grant_type", "client_credentials");

        var response = await Request!.PostAsync(TokenURL!, new()
        {
            Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/x-www-form-urlencoded" }
                },
            Form = formData
        });

        var responseBody = await response.JsonAsync();
        string? token = responseBody.Value.GetProperty("access_token").GetString();

        return token!;
    }
}