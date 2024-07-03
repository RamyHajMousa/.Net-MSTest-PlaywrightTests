using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices;

namespace PlaywrightTests.Utils;

/// <summary>
/// Provides utilities for managing video recordings of test sessions.
/// </summary>
public static class VideoHelper
{
    /// <summary>
    /// Starts recording a video for the test session.
    /// </summary>
    /// <param name="browser">The Playwright browser instance.</param>
    /// <param name="folderName">Optional. The name of the folder where the video will be saved. If not provided, the video will be saved in the root directory.</param>
    /// <returns>An instance of IBrowserContext with video recording enabled.</returns>
    /// <exception cref="Exception">Thrown if an error occurs during the recording process.</exception>
    public static async Task<IBrowserContext> StartVideoRecording(this IBrowser browser, TestContext testContext, string folderName = "")
    {
        try
        {
            // Get the path to the download directory for videos
            string videoPath = FileDownloader.GetDownloadPath(@"Downloads\Videos");
            string filePath = Path.Combine(videoPath, folderName);

            // Delete all previous video files in the specified directory
            DeleteAllPreviousVideoFiles(testContext, filePath);

            // Create a new browser context with video recording enabled
            var context = await browser.NewContextAsync(new()
            {
                RecordVideoDir = filePath,
                //RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
            });

            return context;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during recording video: {ex.Message}");
            throw;
        }

    }

    /// <summary>
    /// Adds all video files from the specified folder to the TestContext.
    /// </summary>
    /// <param name="testContext">The TestContext instance to which the video files will be added.</param>
    /// <param name="folderName">The name of the folder where the video files are located. If not provided, the default folder is used.</param>
    public static void AddVideoFilesToTestContext(TestContext testContext, string folderName)
    {
        string videoPath = FileDownloader.GetDownloadPath(@"Downloads\Videos");
        string filePath = Path.Combine(videoPath, folderName);

        // Ensure the directory exists
        if (!Directory.Exists(filePath))
        {
            Console.WriteLine("Recorded videos directory does not exist.");
            return;
        }

        // Get all files in the directory
        string[] files = Directory.GetFiles(filePath);

        try
        {
            // Filter and add files to TestContext
            foreach (var file in files)
            {
                // Add to TestContext
                testContext.AddResultFile(file);
                Console.WriteLine($"Video recorded on failure and saved in: {videoPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding recorded video file to TestContext: {ex.Message}");
        }
    }

    /// <summary>
    /// Deletes all files in the specified directory.
    /// </summary>
    /// <param name="videoPath">The path to the directory from which all files will be deleted.</param>
    public static void DeleteAllPreviousVideoFiles(TestContext testContext, string videoPath)
    {
        const int maxAttempts = 3;
        const int delayOnLock = 1000;

        try
        {
            // Check if the directory exists
            if (!Directory.Exists(videoPath))
            {
                //Console.WriteLine("Recorded videos directory does not exist.");
                return;
            }

            // Get all files in the directory
            string[] files = Directory.GetFiles(videoPath);

            foreach (string file in files)
            {
                int attempts = 0;
                bool fileDeleted = false;
                while (!fileDeleted && attempts < maxAttempts)
                {
                    try
                    {
                        File.Delete(file);
                        //Console.WriteLine($"Deleted file: {file}");
                        fileDeleted = true;
                    }
                    catch (IOException ex) when (IsFileLocked(ex))
                    {
                        attempts++;
                        //Console.WriteLine($"Attempt {attempts} failed to delete file {file}. Retrying after delay...");
                        Task.Delay(delayOnLock).Wait();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                        break; // No point in retrying if error is not related to file locking
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error accessing directory {videoPath}: {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if a given IOException is caused by a file being locked.
    /// </summary>
    /// <param name="ex">The IOException to check.</param>
    /// <returns>True if the IOException is caused by a file being locked; otherwise, false.</returns>
    private static bool IsFileLocked(IOException ex)
    {
        // Marshal.GetHRForException returns the HRESULT of the exception.
        // The HRESULT is a 32-bit integer that contains information about the error.
        // The lower 16 bits (0xFFFF) represent the error code.
        // The error code 32 (ERROR_SHARING_VIOLATION) indicates that the file is being used by another process.
        // The error code 33 (ERROR_LOCK_VIOLATION) indicates that the file is being locked by another process.
        int errorCode = Marshal.GetHRForException(ex) & ((1 << 16) - 1);
        // Return true if the error code is 32 or 33, indicating a file locking issue.
        // Otherwise, return false.
        return errorCode == 32 || errorCode == 33;
    }
}
