using System;
using System.Diagnostics;
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
public class DropdownAttribute : Attribute
{
    /// <summary>
    /// Construct an instance of this <see cref="DropdownAttribute"/>.
    /// </summary>
    /// <param name="dataPath">The path to the file that contains dropdown data for this property,
    /// relative to the jsontemplate file.</param>
    /// <param name="regex">The regex to convert each line of the data file into data pairs that are required by the dropdown.</param>
    public DropdownAttribute(string dataPath, [StringSyntax("Regex")] string regex = DropdownPropertyInfo.DEFAULT_DROPDOWN_RESOLVER)
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
public class KeyDropdownAttribute : DropdownAttribute
{
    /// <inheritdoc />
    public KeyDropdownAttribute(string dataPath, [StringSyntax("Regex")] string regex = DropdownPropertyInfo.DEFAULT_DROPDOWN_RESOLVER) : base(dataPath, regex)
    {
    }
}

/// <summary>
/// Instruct the jsontemplate serializer to mark the Value of the
/// annotated Dictionary Property as a dropdown.
/// The JsonPowerInspector will collect the information from the provided data source
/// and construct a dropdown for this property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ValueDropdownAttribute : DropdownAttribute
{
    /// <inheritdoc />
    public ValueDropdownAttribute(string dataPath, [StringSyntax("Regex")] string regex = DropdownPropertyInfo.DEFAULT_DROPDOWN_RESOLVER) : base(dataPath, regex)
    {
    }
}

/// <summary>
/// Instruct the jsontemplate serializer to restrict the number range
/// for the annotated Key of the annotated Dictionary Property
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class KeyNumberRangeAttribute : NumberRangeAttribute
{
    /// <inheritdoc />
    public KeyNumberRangeAttribute(double lowerBound, double upperBound) : base(lowerBound, upperBound)
    {
    }
}

/// <summary>
/// Instruct the jsontemplate serializer to restrict the number range
/// for the annotated Value of the annotated Dictionary Property
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ValueNumberRangeAttribute : NumberRangeAttribute
{
    /// <inheritdoc />
    public ValueNumberRangeAttribute(double lowerBound, double upperBound) : base(lowerBound, upperBound)
    {
    }
}

/// <summary>
/// Instruct the jsontemplate serializer to restrict
/// Set the default value of this string Property
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class StringDefaultValueAttribute : Attribute
{
    /// <summary>
    /// Construct an instance of this <see cref="StringDefaultValueAttribute"/>.
    /// </summary>
    /// <param name="defaultValue">The default value for this property.</param>
    public StringDefaultValueAttribute(string defaultValue)
    {
        DefaultValue = defaultValue;
    }

    internal string DefaultValue { get; }
}

/// <summary>
/// Instruct the jsontemplate serializer to restrict
/// Set the default value of this boolean Property
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class BoolDefaultValueAttribute : Attribute
{
    /// <summary>
    /// Construct an instance of this <see cref="BoolDefaultValueAttribute"/>.
    /// </summary>
    /// <param name="defaultValue">The default value for this property.</param>
    public BoolDefaultValueAttribute(bool defaultValue)
    {
        DefaultValue = defaultValue;
    }

    internal bool DefaultValue { get; }
}

/// <summary>
/// Instruct the jsontemplate serializer to restrict
/// Set the default value of this integer Property
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IntDefaultValueAttribute : Attribute
{
    /// <summary>
    /// Construct an instance of this <see cref="IntDefaultValueAttribute"/>.
    /// </summary>
    /// <param name="defaultValue">The default value for this property.</param>
    public IntDefaultValueAttribute(int defaultValue)
    {
        DefaultValue = defaultValue;
    }

    internal int DefaultValue { get; }
}

/// <summary>
/// Instruct the jsontemplate serializer to restrict
/// Set the default value of this float Property
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class FloatDefaultValueAttribute : Attribute
{
    /// <summary>
    /// Construct an instance of this <see cref="FloatDefaultValueAttribute"/>.
    /// </summary>
    /// <param name="defaultValue">The default value for this property.</param>
    public FloatDefaultValueAttribute(double defaultValue)
    {
        DefaultValue = defaultValue;
    }

    internal double DefaultValue { get; }
}

/// <summary>
/// Instruct the jsontemplate serializer to restrict
/// Set the default value of this enum Property
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class EnumDefaultValueAttribute : Attribute
{
    /// <summary>
    /// Construct an instance of this <see cref="EnumDefaultValueAttribute"/>.
    /// </summary>
    /// <param name="defaultValue">The default value for this property.</param>
    public EnumDefaultValueAttribute(string defaultValue)
    {
        DefaultValue = defaultValue;
    }

    internal string DefaultValue { get; }
}