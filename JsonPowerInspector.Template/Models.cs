using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace JsonPowerInspector.Template;

[JsonSerializable(typeof(PackedObjectDefinition))]
[JsonSourceGenerationOptions(UseStringEnumConverter = true, WriteIndented = true)]
internal partial class PowerTemplateJsonContext : JsonSerializerContext;

/// <summary>
/// Contains the necessary information required to create an inspector for a type.
/// </summary>
public class PackedObjectDefinition
{
    [JsonConstructor]
    internal PackedObjectDefinition(ObjectDefinition mainObjectDefinition, ObjectDefinition[] referencedObjectDefinition)
    {
        MainObjectDefinition = mainObjectDefinition;
        ReferencedObjectDefinition = referencedObjectDefinition;
    }

    /// <summary>
    /// Contains the serialization info for a type. 
    /// </summary>
    public ObjectDefinition MainObjectDefinition { get; set; }

    /// <summary>
    /// Contains the serialization info for the types that are referenced by the <see cref="MainObjectDefinition"/>.
    /// </summary>
    public ObjectDefinition[] ReferencedObjectDefinition { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.AppendLine("Main Object:\n");

        MainObjectDefinition.ToString(builder, 1);

        if (ReferencedObjectDefinition.Length != 0)
        {
            builder.AppendLine("\nReferenced Objects:\n");

            foreach (var referencedDefinition in ReferencedObjectDefinition.AsSpan())
            {
                referencedDefinition.ToString(builder, 1);
                builder.AppendLine();
            }
        }

        return builder.ToString();
    }
}

/// <summary>
/// Contains the serialization info for a type. 
/// </summary>
public class ObjectDefinition
{
    [JsonConstructor]
    internal ObjectDefinition(string objectTypeName, BaseObjectPropertyInfo[] properties)
    {
        ObjectTypeName = objectTypeName;
        Properties = properties;
    }

    /// <summary>
    /// The type name.
    /// </summary>
    public string ObjectTypeName { get; }

    /// <summary>
    /// The type properties.
    /// </summary>
    public BaseObjectPropertyInfo[] Properties { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder, 0);
        return builder.ToString();
    }

    internal void ToString(StringBuilder stringBuilder, int indentationLevel)
    {
        stringBuilder
            .Append(' ', indentationLevel * 4)
            .AppendLine(ObjectTypeName)
            .Append(' ', indentationLevel * 4)
            .AppendLine("{");

        indentationLevel++;

        foreach (var property in Properties.AsSpan())
        {
            property.ToString(stringBuilder, indentationLevel);
            stringBuilder.AppendLine();
        }

        indentationLevel--;


        stringBuilder
            .Append(' ', indentationLevel * 4)
            .AppendLine("}");
    }
}

/// <summary>
/// Contains serialization info for a property.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "PropertyType", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(StringPropertyInfo), typeDiscriminator: "String")]
[JsonDerivedType(typeof(NumberPropertyInfo), typeDiscriminator: "Number")]
[JsonDerivedType(typeof(ObjectPropertyInfo), typeDiscriminator: "Object")]
[JsonDerivedType(typeof(BooleanPropertyInfo), typeDiscriminator: "Bool")]
[JsonDerivedType(typeof(ArrayPropertyInfo), typeDiscriminator: "Array")]
[JsonDerivedType(typeof(DictionaryPropertyInfo), typeDiscriminator: "Dictionary")]
[JsonDerivedType(typeof(EnumPropertyInfo), typeDiscriminator: "Enum")]
[JsonDerivedType(typeof(DropdownPropertyInfo), typeDiscriminator: "Dropdown")]
public abstract class BaseObjectPropertyInfo
{
    private protected BaseObjectPropertyInfo(string name, string displayName)
    {
        Name = name;
        DisplayName = displayName;
    }

    /// <summary>
    /// The property name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The name displayed on the inspector,
    /// same as the <see cref="Name"/> if this property is not annotated with <see cref="InspectorNameAttribute"/>.
    /// </summary>
    public string DisplayName { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder, 0);
        return builder.ToString();
    }

    private protected abstract void PrintType(StringBuilder stringBuilder);

    private protected virtual void PrintAdditional(StringBuilder stringBuilder)
    {
    }

    internal void ToString(StringBuilder stringBuilder, int indentationLevel)
    {
        stringBuilder
            .Append(' ', indentationLevel * 4);

        PrintType(stringBuilder);

        stringBuilder
            .Append(' ')
            .Append(Name);

        PrintAdditional(stringBuilder);
    }
}

/// <summary>
/// Contains serialization info for a <see cref="string"/> property.
/// </summary>
public class StringPropertyInfo : BaseObjectPropertyInfo
{
    private protected override void PrintType(StringBuilder stringBuilder) => stringBuilder.Append("String");

    /// <summary>
    /// Default Value display on inspector.
    /// </summary>
    public string DefaultValue { get; }

    [JsonConstructor]
    internal StringPropertyInfo(string name, string displayName, string defaultValue) : base(name, displayName)
    {
        DefaultValue = defaultValue;
    }
}

/// <summary>
/// Contains serialization info for a <see cref="bool"/> property.
/// </summary>
public class BooleanPropertyInfo : BaseObjectPropertyInfo
{
    /// <summary>
    /// Is this property nullable.
    /// </summary>
    public bool Nullable { get; }

    /// <summary>
    /// Default Value display on inspector.
    /// </summary>
    public bool DefaultValue { get; }
    private protected override void PrintType(StringBuilder stringBuilder) => stringBuilder.Append("Bool");

    [JsonConstructor]
    internal BooleanPropertyInfo(string name, string displayName, bool nullable, bool defaultValue) : base(name, displayName)
    {
        Nullable = nullable;
        DefaultValue = defaultValue;
    }
}

/// <summary>
/// Contains serialization info for a number property.
/// </summary>
public class NumberPropertyInfo : BaseObjectPropertyInfo
{
    /// <summary>
    /// Describes the range for a <see cref="NumberPropertyInfo"/>.
    /// </summary>
    /// <param name="Lower">The lower bound.</param>
    /// <param name="Upper">The upper bound.</param>
    public record struct NumberRange(double Lower, double Upper);

    /// <summary>
    /// The Type for a <see cref="NumberPropertyInfo"/>.
    /// </summary>
    public enum NumberType
    {
        /// <summary>
        /// Represents a property type of <see cref="byte"/>, <see cref="ushort"/>,
        /// <see cref="uint"/>, <see cref="ulong"/>, <see cref="sbyte"/>,
        /// <see cref="short"/>, <see cref="int"/>, <see cref="long"/>.
        /// </summary>
        Int,

        /// <summary>
        /// Represents a property type of <see cref="float"/>, <see cref="double"/>.
        /// </summary>
        Float
    }

    /// <summary>
    /// The type for this number.
    /// </summary>
    public NumberType NumberKind { get; }

    /// <summary>
    /// The range for this number.
    /// </summary>
    public NumberRange? Range { get; }

    /// <summary>
    /// Is this property nullable.
    /// </summary>
    public bool Nullable { get; }

    /// <summary>
    /// Default Value display on inspector.
    /// </summary>
    public string DefaultValue { get; }

    [JsonConstructor]
    internal NumberPropertyInfo(string name, string displayName, bool nullable, NumberType numberKind, string defaultValue, NumberRange? range = null) : base(name, displayName)
    {
        NumberKind = numberKind;
        Range = range;
        DefaultValue = defaultValue;
        Nullable = nullable;
    }

    private protected override void PrintType(StringBuilder stringBuilder)
    {
        stringBuilder
            .Append(
                NumberKind switch
                {
                    NumberType.Int => "Int",
                    NumberType.Float => "Float",
                    _ => throw new InvalidOperationException()
                }
            );
    }

    private protected override void PrintAdditional(StringBuilder stringBuilder)
    {
        if (!Range.HasValue) return;

        var value = Range.Value;

        stringBuilder
            .Append('[')
            .Append(value.Lower)
            .Append(" ~ ")
            .Append(value.Upper)
            .Append(']');
    }
}

/// <summary>
/// Contains serialization info for an object property.
/// </summary>
public class ObjectPropertyInfo : BaseObjectPropertyInfo
{
    /// <summary>
    /// The name for this object property.
    /// </summary>
    public string ObjectTypeName { get; }

    [JsonConstructor]
    internal ObjectPropertyInfo(string name, string displayName, string objectTypeName) : base(name, displayName)
    {
        ObjectTypeName = objectTypeName;
    }

    private protected override void PrintType(StringBuilder stringBuilder)
    {
        stringBuilder
            .Append("Object<")
            .Append(ObjectTypeName)
            .Append('>');
    }
}

/// <summary>
/// Contains serialization info for a property
/// that have dropdown value selection.
/// </summary>
public class DropdownPropertyInfo : BaseObjectPropertyInfo
{
    [StringSyntax("Regex")] internal const string DEFAULT_DROPDOWN_RESOLVER = @"(?<Value>.+?)\t(?<Display>.+)";

    /// <summary>
    /// Describes the value type for a <see cref="DropdownPropertyInfo"/>. 
    /// </summary>
    public enum DropdownKind
    {
        /// <summary>
        /// Represents a property type of <see cref="byte"/>, <see cref="ushort"/>,
        /// <see cref="uint"/>, <see cref="ulong"/>, <see cref="sbyte"/>,
        /// <see cref="short"/>, <see cref="int"/>, <see cref="long"/>.
        /// </summary>
        Int,

        /// <summary>
        /// Represents a property type of <see cref="float"/>, <see cref="double"/>.
        /// </summary>
        Float,

        /// <summary>
        /// Represents a property type of <see cref="string"/>.
        /// </summary>
        String
    }

    /// <summary>
    /// The type for this <see cref="DropdownPropertyInfo"/>.
    /// </summary>
    public DropdownKind Kind { get; }

    /// <summary>
    /// The path to the file that contains dropdown data for this property,
    /// relative to the jsontemplate file.
    /// </summary>
    public string DataSourcePath { get; }

    /// <summary>
    /// The regex to convert each line of the data file into data pairs that are required by the dropdown.
    /// </summary>
    public string ValueDisplayRegex { get; }

    [JsonConstructor]
    internal DropdownPropertyInfo(string name, string displayName, DropdownKind kind, string dataSourcePath, string valueDisplayRegex) : base(name, displayName)
    {
        Kind = kind;
        DataSourcePath = dataSourcePath;
        ValueDisplayRegex = valueDisplayRegex;
    }

    private protected override void PrintType(StringBuilder stringBuilder) =>
        stringBuilder
            .Append("Dropdown<")
            .Append(Kind.ToString()).Append('>');

    private protected override void PrintAdditional(StringBuilder stringBuilder) =>
        stringBuilder
            .Append('[')
            .Append(ValueDisplayRegex)
            .Append(", \"")
            .Append("./")
            .Append(DataSourcePath)
            .Append("\"]");
}

/// <summary>
/// Contains serialization info for an enum property.
/// </summary>
public class EnumPropertyInfo : BaseObjectPropertyInfo
{
    /// <summary>
    /// Describes a value information for the enum.
    /// </summary>
    /// <param name="DisplayName">The alternative name for JsonPowerInspector
    /// to use when displaying this Enum value.</param>
    /// <param name="DeclareName">The declared name for this Enum value.</param>
    /// <param name="Value">The value for this Enum value.</param>
    public record struct EnumValue(string DisplayName, string DeclareName, long Value);

    /// <summary>
    /// The type name for this enum value.
    /// </summary>
    public string EnumTypeName { get; }

    /// <summary>
    /// The values for this enum type.
    /// </summary>
    public EnumValue[] EnumValues { get; }

    /// <summary>
    /// Has this enum annotated with <see cref="FlagsAttribute"/>.
    /// </summary>
    public bool IsFlags { get; }

    /// <summary>
    /// Default Value display on inspector.
    /// </summary>
    public string DefaultValue { get; }


    [JsonConstructor]
    internal EnumPropertyInfo(string name, string displayName, string enumTypeName, EnumValue[] enumValues, bool isFlags, string defaultValue) : base(name, displayName)
    {
        EnumTypeName = enumTypeName;
        EnumValues = enumValues;
        IsFlags = isFlags;
        DefaultValue = defaultValue;
    }

    private protected override void PrintType(StringBuilder stringBuilder) =>
        stringBuilder.Append(IsFlags ? "EnumFlags<" : "Enum<")
            .Append(EnumTypeName)
            .Append('>');

    private protected override void PrintAdditional(StringBuilder stringBuilder) =>
        stringBuilder
            .Append('[')
            .AppendJoin(", ", EnumValues.Select(x => x.DeclareName))
            .Append(']');
}

/// <summary>
/// Contains serialization info for an array property.
/// </summary>
public class ArrayPropertyInfo : BaseObjectPropertyInfo
{
    /// <summary>
    /// Describes the type for the elements in this array. 
    /// </summary>
    public BaseObjectPropertyInfo ArrayElementTypeInfo { get; }

    [JsonConstructor]
    internal ArrayPropertyInfo(string name, string displayName, BaseObjectPropertyInfo arrayElementTypeInfo) : base(name, displayName)
    {
        ArrayElementTypeInfo = arrayElementTypeInfo;
    }

    private protected override void PrintType(StringBuilder stringBuilder) =>
        stringBuilder
            .Append("Array<")
            .Append(ArrayElementTypeInfo)
            .Append('>');
}

/// <summary>
/// Contains serialization info for an array property.
/// </summary>
public class DictionaryPropertyInfo : BaseObjectPropertyInfo
{
    /// <summary>
    /// Describes the type for key of this dictionary. 
    /// </summary>
    public BaseObjectPropertyInfo KeyTypeInfo { get; }

    /// <summary>
    /// Describes the type for value of this dictionary. 
    /// </summary>
    public BaseObjectPropertyInfo ValueTypeInfo { get; }

    [JsonConstructor]
    internal DictionaryPropertyInfo(string name, string displayName, BaseObjectPropertyInfo keyTypeInfo, BaseObjectPropertyInfo valueTypeInfo) : base(name, displayName)
    {
        KeyTypeInfo = keyTypeInfo;
        ValueTypeInfo = valueTypeInfo;
    }

    private protected override void PrintType(StringBuilder stringBuilder) =>
        stringBuilder
            .Append("Dictionary<")
            .Append(KeyTypeInfo)
            .Append(", ")
            .Append(ValueTypeInfo)
            .Append('>');
}