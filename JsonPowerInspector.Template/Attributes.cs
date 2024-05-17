using System;
using System.Diagnostics.CodeAnalysis;

namespace JsonPowerInspector.Template;

/// <summary>
/// Instruct the jsontemplate serializer to restrict
/// the range for the annotated number property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NumberRangeAttribute : Attribute
{
    /// <summary>
    /// Construct an instance of this <see cref="NumberRangeAttribute"/>.
    /// </summary>
    /// <param name="lowerBound">The lower bound for this number property.</param>
    /// <param name="upperBound">The upper bound for this number property.</param>
    public NumberRangeAttribute(double lowerBound, double upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    internal double LowerBound { get; }
    internal double UpperBound { get; }
}

/// <summary>
/// Instruct the jsontemplate serializer to give the
/// annotated Property or Enum value a customized name when displaying this Property or Enum value.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class InspectorNameAttribute : Attribute
{
    /// <summary>
    /// Construct an instance of this <see cref="InspectorNameAttribute"/>.
    /// </summary>
    /// <param name="displayName">The alternative name for JsonPowerInspector
    /// to use when displaying this Property or Enum value.</param>
    public InspectorNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }

    internal string DisplayName { get; }
}

/// <summary>
/// Instruct the jsontemplate serializer to mark the
/// annotated Property as a dropdown.
/// The JsonPowerInspector will collect the information from the provided data source
/// and construct a dropdown for this property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class InspectorDropdownAttribute : Attribute
{
    /// <summary>
    /// Construct an instance of this <see cref="InspectorDropdownAttribute"/>.
    /// </summary>
    /// <param name="dataPath">The path to the file that contains dropdown data for this property,
    /// relative to the jsontemplate file.</param>
    /// <param name="regex">The regex to convert each line of the data file into data pairs that are required by the dropdown.</param>
    public InspectorDropdownAttribute(string dataPath, [StringSyntax("Regex")] string regex = DropdownPropertyInfo.DEFAULT_DROPDOWN_RESOLVER)
    {
        DataPath = dataPath;
        Regex = regex;
    }
    internal string DataPath { get; }
    internal string Regex { get; }
}

/// <summary>
/// Instruct the jsontemplate serializer to mark the Key of the
/// annotated Dictionary Property as a dropdown.
/// The JsonPowerInspector will collect the information from the provided data source
/// and construct a dropdown for this property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class InspectorKeyDropdownAttribute : InspectorDropdownAttribute
{
    /// <inheritdoc />
    public InspectorKeyDropdownAttribute(string dataPath, [StringSyntax("Regex")] string regex = DropdownPropertyInfo.DEFAULT_DROPDOWN_RESOLVER) : base(dataPath, regex) { }
}

/// <summary>
/// Instruct the jsontemplate serializer to mark the Value of the
/// annotated Dictionary Property as a dropdown.
/// The JsonPowerInspector will collect the information from the provided data source
/// and construct a dropdown for this property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class InspectorValueDropdownAttribute : InspectorDropdownAttribute
{
    /// <inheritdoc />
    public InspectorValueDropdownAttribute(string dataPath, [StringSyntax("Regex")] string regex = DropdownPropertyInfo.DEFAULT_DROPDOWN_RESOLVER) : base(dataPath, regex) { }
}

/// <summary>
/// Instruct the jsontemplate serializer to restrict the number range
/// for the annotated Key of the annotated Dictionary Property
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NumberRangeKeyAttribute : NumberRangeAttribute
{
    /// <inheritdoc />
    public NumberRangeKeyAttribute(double lowerBound, double upperBound) : base(lowerBound, upperBound) { }
}

/// <summary>
/// Instruct the jsontemplate serializer to restrict the number range
/// for the annotated Value of the annotated Dictionary Property
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NumberRangeValueAttribute : NumberRangeAttribute
{
    /// <inheritdoc />
    public NumberRangeValueAttribute(double lowerBound, double upperBound) : base(lowerBound, upperBound) { }
}