namespace PlaywrightTests.Utils;

/// <summary>
/// Provides extension methods for accessing test run parameters from the TestContext in a typed manner.
/// These methods simplify retrieving test run configuration values, enhancing readability and type safety.
/// </summary>
public static class TestRunParameterExtensions
{
    /// <summary>
    /// Retrieves a test run parameter as a string from the TestContext.
    /// </summary>
    /// <param name="context">The TestContext from which to retrieve the parameter.</param>
    /// <param name="name">The name of the parameter to retrieve.</param>
    /// <returns>The value of the parameter as a string.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified parameter is not found or is not a string.</exception>
    public static string? GetTestRunParameter(this TestContext context, string name)
    {
        return context.Properties[name] as string
            ?? throw new ArgumentException($"Test run parameter '{name}' not found or is not a string.");
    }

    /// <summary>
    /// Retrieves a test run parameter as an integer from the TestContext.
    /// </summary>
    /// <param name="context">The TestContext from which to retrieve the parameter.</param>
    /// <param name="name">The name of the parameter to retrieve.</param>
    /// <returns>The value of the parameter as an integer.</returns>
    public static int GetTestRunParameterInt(this TestContext context, string name)
    {
        return Convert.ToInt32(context.Properties[name]);
    }
}



