using Microsoft.Playwright;

namespace PlaywrightTests.Utils;

/// <summary>
/// Facilitates downloading files using Playwright's IPage interface.
/// This class provides methods to initiate file downloads and save them to a specified directory.
/// </summary>
public class FileDownloader
{
    private readonly IPage page;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDownloader"/> class.
    /// </summary>
    /// <param name="page">The Playwright IPage object used for downloading files.</param>
    public FileDownloader(IPage page)
    {
        this.page = page;
    }

    /// <summary>
    /// Initiates a download for the specified file locator and saves the file to a specified subfolder within the project directory.
    /// </summary>
    /// <param name="fileLocator">The locator for the element that triggers the download when clicked.</param>
    /// <param name="downloadSubFolder">The subfolder within the project's directory where the downloaded file will be saved. Defaults to "Downloads".</param>
    /// <returns>The path to the saved file or null if an error occurs.</returns>
    /// <remarks>
    /// This method waits for the file download to complete before saving it to the specified location.
    /// If an error occurs during the download or save process, it logs the error to the console.
    /// </remarks>
    public async Task<string?> DownloadFileAndSave(ILocator fileLocator, string downloadSubFolder = "Downloads")
    {
        try
        {
            // Start waiting for the download
            var waitForDownloadTask = page.WaitForDownloadAsync();
            // Click the element to initiate the download
            await fileLocator.ClickAsync();
            // Wait for the download to complete
            var download = await waitForDownloadTask;

            string downloadPath = GetDownloadPath(downloadSubFolder);
            string filePath = Path.Combine(downloadPath, download.SuggestedFilename);
            // Save the download to the specified file path
            await download.SaveAsAsync(filePath);
            return filePath; // Return the file path for further use
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during file download: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Constructs the path to the specified subfolder within the project directory.
    /// Ensures that the directory exists before returning the path.
    /// </summary>
    /// <param name="subFolder">The name of the subfolder within the project's directory. Defaults to "Downloads".</param>
    /// <returns>The full path to the specified subfolder.</returns>
    public static string GetDownloadPath(string subFolder = "Downloads")
    {
        // Get the current working directory (where the binary is running)
        string currentDirectory = Directory.GetCurrentDirectory();
        // Navigate up to the main project directory (adjust the number of ".." based on the project structure)
        string projectDirectory = Path.GetFullPath(Path.Combine(currentDirectory, @"..\..\.."));
        // Construct the path to the "Downloads" folder within the project directory
        string downloadPath = Path.Combine(projectDirectory, subFolder);
        // Ensure that the "Downloads" directory exists
        Directory.CreateDirectory(downloadPath);
        return downloadPath; // Return the file path for further use
    }

}

