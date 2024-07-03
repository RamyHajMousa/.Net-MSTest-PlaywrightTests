# PlaywrightTests

# PlaywrightTests

## Brief Description
This project is a collection of automated tests using Playwright, a powerful end-to-end testing framework for web applications. The repository contains test scripts written to perform various checks and validations on web elements, demonstrating the implementation of Playwright for quality assurance purposes.

## Setting Up The Project

### Prerequisites

- Ensure you have [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed as Playwright is distributed as a .NET Standard 2.0 library.
- Supported operating systems: Windows 10+, Windows Server 2016+, MacOS 12 Monterey or higher, Debian 11, Debian 12, Ubuntu 20.04 or 22.04.

### Project Setup

1. **Create a New Project:**   

   ```powershell
   dotnet new mstest -n PlaywrightTests
   ```  
    This command creates a new MSTest project with the name `PlaywrightTests`.
   ```powershell
   cd PlaywrightTests
   ```

2. **Install Playwright Dependencies:**
   ```powershell
   dotnet add package Microsoft.Playwright.MSTest
   ```

3. **Build the Project:**
   ```powershell
   dotnet build
   ```
   Building the project ensures that `playwright.ps1` script is available in the `bin` directory.

4. **Install Required Browsers:**
   Replace `netX` with your actual output folder name (e.g., `net8.0`):
   ```powershell
   pwsh bin/Debug/netX/playwright.ps1 install
   ```
   *Note:* If PowerShell (`pwsh`) is not installed, you need to <a href="https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell?view=powershell-7.4" target="_blank">install it</a>


### Running the Tests

- By default, tests run on Chromium in headless mode (no browser UI).
- You can adjust the browser and mode using the `BROWSER` environment variable or launch configuration options.

#### Running Tests via Command Line Interface (CLI)

```powershell
dotnet test -- MSTest.Parallelize.Workers=5
```
This command runs tests using MSTest with parallel execution enabled.

#### Running Tests in Visual Studio

- Open the `PlaywrightTests.sln` solution file in Visual Studio.
- Build the solution.
- Run the tests using the Test Explorer in Visual Studio.

### Configuring Run Settings with `chromium.runsettings`

This XML configuration file allows us to define various test run parameters and environment settings, enhancing the flexibility and control over your test execution.

#### `chromium.runsettings` File:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
    <!-- MSTest adapter -->
    <MSTest>
        <Parallelize>
            <Workers>4</Workers>
            <Scope>ClassLevel</Scope>
        </Parallelize>
    </MSTest>
    <!-- General run configuration -->
    <RunConfiguration>
        <EnvironmentVariables>
            <!-- For debugging selectors, it's recommend to set the following environment variable -->
            <DEBUG>pw:api</DEBUG>
        </EnvironmentVariables>
    </RunConfiguration>
    <!-- Playwright -->
    <Playwright>
        <BrowserName>chromium</BrowserName>
        <ExpectTimeout>5000</ExpectTimeout>
        <LaunchOptions>
            <Headless>false</Headless>
            <SlowMo>2000</SlowMo>
            <Channel>chrome</Channel>
        </LaunchOptions>
    </Playwright>
    <!--Define test parameters like URL, user credentials... -->
    <TestRunParameters>
        <Parameter name="Url" value="https://website.com/" />
        <Parameter name="UserName" value="FirstName LastName" />
        <Parameter name="UserEmail" value="name@mail.com" />
        <Parameter name="UserPassword" value="Swq9hMf9pgq3yvr!Kdt" />
    </TestRunParameters>
</RunSettings>
```

#### Benefits and Configuration:

1. **Parallel Test Execution:**
   - Configures MSTest to run 4 workers in parallel at the class level, improving test execution speed.

2. **Playwright Settings:**
   - Specifies `chromium` as the browser with a custom timeout, non-headless mode for visible UI testing, and a slower motion for better observation of test flows.

3. **Debugging:**
   - Sets an environment variable for debugging Playwright selectors (`pw:api`).

4. **Custom Test Parameters:**
   - Defines test parameters like URL, user credentials, ensuring flexibility and reusability of tests across different environments.

#### Integrating into Visual Studio:

To apply the `chromium.runsettings` in Visual Studio, follow these steps:
- Click on the 'Test' menu at the top of Visual Studio.
- Select 'Configure Run Settings'.
- Choose 'Select Solution Wide runsettings File'.
- Browse to and select your `chromium.runsettings` file.
- Once selected, these settings will apply to all test runs in the solution.

This procedure ensures that your test runs in Visual Studio are configured according to the custom settings defined in `chromium.runsettings`, providing consistent and controlled test environments.

### Utilizing the `TestContextExtension` Class

The `TestContextExtension` class in the `PlaywrightTests.Utils` namespace offers enhanced flexibility and ease of use when accessing test run parameters.

#### `TestContextExtension` Class

```c#
namespace PlaywrightTests.Utils
{
    public static class TestContextExtension
    {
        public static string? GetTestRunParameter(this TestContext context, string name)
        {
            return context.Properties[name] as string 
                ?? throw new ArgumentException($"Test run parameter '{name}' not found or is not a string.");
        }

        public static int GetTestRunParameterInt(this TestContext context, string name)
        {
            return Convert.ToInt32(context.Properties[name]); 
        }
    }
}

```

#### Benefits and Usage:

1. **Simplified Parameter Access:**
   - Provides a method to easily retrieve test run parameters from the `TestContext`, such as URLs and user credentials.

2. **Type-Safe Retrieval:**
   - Offers a method to safely convert and retrieve parameters as integers, reducing the risk of type-related errors.

3. **Error Handling:**
   - Includes error handling to notify developers if a parameter is missing or not in the expected string format, enhancing the robustness of test scripts.

By integrating `TestContextExtension`, we streamline the process of accessing and utilizing test parameters, making our tests more maintainable and less prone to errors due to incorrect parameter handling.

---

## Playwright Trace Viewer

Playwright Trace Viewer is a GUI tool designed for exploring recorded Playwright traces. It allows users to:

- Visualize each action of a test.
- See what was happening at each step.
- Inspect logs, source, and network information.
- Interact with DOM snapshots.

More details can be found in the official [documentation](https://playwright.dev/dotnet/docs/trace-viewer-intro).

### `PlaywrightTracer` Class

`PlaywrightTracer` provides a centralized and reusable way to handle tracing in Playwright tests. Key benefits include:

- **Modularity**: Encapsulates tracing logic separately from test logic.
- **Reusability**: Can be used across multiple test classes.
- **Ease of Maintenance**: Centralizes tracing configuration and updates.

### Integrating `PlaywrightTracer` in Test Classes

#### PlaywrightTracer Class:
```csharp
using Microsoft.Playwright;

namespace PlaywrightTests.Utils
{
    // The PlaywrightTracer class provides static methods for starting and stopping Playwright tracing.
    public class PlaywrightTracer
    {
        // Starts tracing on the given browser context.
        public static async Task StartTracing(IBrowserContext context)
        {
            await context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true, // Capture screenshots during tracing
                Snapshots = true,   // Capture DOM snapshots
                Sources = true      // Capture source codes loaded in the browser
            });
        }

        // Stops tracing on the given browser context and saves the trace to a file.
        // Takes an IBrowserContext object and an optional file path (default is "trace.zip").
        // The "trace.zip" can be found in "YourProject\bin\Debug\netX"
        public static async Task StopTracing(IBrowserContext context, string filePath = "trace.zip")
        {
            await context.Tracing.StopAsync(new TracingStopOptions
            {
                Path = filePath // The file path where the trace will be saved
            });
        }
    }
}
}
```

#### Usage in Test Class (`UnitTest1`):
```csharp
[TestClass]
public class UnitTest1 : PageTest
{
    [TestMethod]
    public async Task ManageVisitInStudy()
    {
        // ... Initialize context and page

        // Start tracing
        await PlaywrightTracer.StartTracing(Context);

        // ... Test actions

        // Stop tracing
        await PlaywrightTracer.StopTracing(Context);
    }
}
```

### Using <a href="https://trace.playwright.dev/" target="_blank">trace.playwright.dev</a> and Its Benefits

- **trace.playwright.dev** is a web-based platform to view Playwright traces.
- **Benefits**:
  - **Accessibility**: Easily upload and view traces without CLI commands.
  - **User-Friendly**: Intuitive interface for navigating through test steps.
  - **Cross-Platform Compatibility**: Works on any system with a web browser.

To use it, simply visit [trace.playwright.dev](https://trace.playwright.dev/) and upload the trace file.

---
