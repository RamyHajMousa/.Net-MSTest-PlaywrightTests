using Microsoft.Playwright;

namespace PlaywrightTests.Utils;

/// <summary>
/// Provides static methods for managing Playwright's tracing feature.
/// This includes starting and stopping the tracing on a given browser context.
/// </summary>
public static class PlaywrightTracer
{
    /// <summary>
    /// Starts tracing on the specified browser context with options for screenshots, snapshots, and source codes.
    /// </summary>
    /// <param name="context">The browser context on which to start tracing.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    public static async Task StartTracing(this IBrowserContext context)
    {
        await context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true, // Capture screenshots during tracing.
            Snapshots = true,   // Capture DOM snapshots.
            Sources = true      // Capture source codes loaded in the browser.
        });
    }

    /// <summary>
    /// Stops tracing on the specified browser context and saves the trace to a file.
    /// The trace file is saved to a default or specified path within the project's "Downloads" directory.
    /// </summary>
    /// <param name="context">The browser context on which to stop tracing.</param>
    /// <param name="fileName">The name of the file to save the trace. Defaults to "trace.zip".</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    /// <remarks>
    /// If an exception occurs during the stopping or saving process, it logs the error to the console.
    /// The default trace file path is intended to be within the "Downloads" folder of the project directory.
    /// </remarks>
    public static async Task StopTracing(this IBrowserContext context, string fileName = "trace.zip")
    {
        try
        {
            string downloadPath = FileDownloader.GetDownloadPath(@"Downloads\Tracer");
            string filePath = Path.Combine(downloadPath, fileName);

            await context.Tracing.StopAsync(new TracingStopOptions
            {
                Path = filePath // The file path where the trace will be saved.
            });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during stopping tracing and saving file: {ex.Message}");
        }
    }
}
