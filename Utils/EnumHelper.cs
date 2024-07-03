namespace PlaywrightTests.Utils;

public static class EnumHelper
{
    /// <summary>
    /// Retrieves a human-readable description for a given enum value.
    /// If a custom description is defined using the Description attribute, it returns that;
    /// otherwise, it defaults to the enum's name.
    /// </summary>
    /// <param name="value">The Setting enum value to get the description for.</param>
    /// <returns>The description string associated with the enum value.</returns>
    public static string GetEnumDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = Attribute
            .GetCustomAttribute(field!, typeof(System.ComponentModel.DescriptionAttribute))
            as System.ComponentModel.DescriptionAttribute;

        return attribute == null ? value.ToString() : attribute.Description;
    }
}
