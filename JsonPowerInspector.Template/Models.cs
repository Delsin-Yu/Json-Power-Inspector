using System;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace JsonPowerInspector.Template;

[JsonSerializable(typeof(ApplicationJsonTypes))]
[JsonSourceGenerationOptions(UseStringEnumConverter = true, WriteIndented = true)]
public partial class PowerTemplateJsonContext : JsonSerializerContext;

internal class ApplicationJsonTypes
{
    public PackedObjectDefinition PackedObjectDefinition { get; set; }
}

/// <summary>
/// Instruct the jsontemplate serializer to give the
/// annotated Property or Enum value a customized name when displaying this Property or Enum value.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class InspectorNameAttribute : Attribute
{
    public InspectorNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }

    public string DisplayName { get; }
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
    public InspectorDropdownAttribute(string dataPath, string regex = null)
    {
        DataPath = dataPath;
        Regex = regex;
    }
    public string DataPath { get; }
    public string Regex { get; }
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
    public InspectorKeyDropdownAttribute(string dataPath, string regex = null) : base(dataPath, regex) { }
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
    public InspectorValueDropdownAttribute(string dataPath, string regex = null) : base(dataPath, regex) { }
}

/// <summary>
/// Contains the necessary information required to create an inspector for a type.
/// </summary>
public class PackedObjectDefinition
{
    public PackedObjectDefinition(ObjectDefinition mainObjectDefinition, ObjectDefinition[] referencedObjectDefinition)
    {
        MainObjectDefinition = mainObjectDefinition;
        ReferencedObjectDefinition = referencedObjectDefinition;
    }

    /// <summary>
    /// Contains the information for the type.
    /// </summary>
    public ObjectDefinition MainObjectDefinition { get; set; }
    
    /// <summary>
    /// Contains the information for the types that are referenced by the <see cref="MainObjectDefinition"/>.
    /// </summary>
    public ObjectDefinition[] ReferencedObjectDefinition { get; set; }

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

public class ObjectDefinition
{
    public string ObjectTypeName { get; set; }
    public BaseObjectPropertyInfo[] Properties { get; set; }

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder, 0);
        return builder.ToString();
    }

    public void ToString(StringBuilder stringBuilder, int indentationLevel)
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
    public string Name { get; set; }
    public string DisplayName { get; set; }
    
    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder, 0);
        return builder.ToString();
    }

    protected abstract void PrintType(StringBuilder stringBuilder);

    protected virtual void PrintAdditional(StringBuilder stringBuilder) { }
    
    public void ToString(StringBuilder stringBuilder, int indentationLevel)
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

public class StringPropertyInfo : BaseObjectPropertyInfo
{
    protected override void PrintType(StringBuilder stringBuilder) => stringBuilder.Append("String");
}
public class BooleanPropertyInfo : BaseObjectPropertyInfo
{
    protected override void PrintType(StringBuilder stringBuilder) => stringBuilder.Append("Bool");
}

public class NumberPropertyInfo : BaseObjectPropertyInfo
{
    public record struct NumberRange(double Lower, double Upper);

    public enum NumberType
    {
        Int,
        Float
    }

    public NumberType NumberKind { get; set; }
    public NumberRange? Range { get; set; }

    protected override void PrintType(StringBuilder stringBuilder)
    {
        stringBuilder
            .Append(
                NumberKind switch
                {
                    NumberType.Int => "Int",
                    NumberType.Float => "Float",
                    _ => throw new ArgumentOutOfRangeException()
                }
            );
    }

    protected override void PrintAdditional(StringBuilder stringBuilder)
    {
        if(!Range.HasValue) return;

        var value = Range.Value;
        
        stringBuilder
            .Append('[')
            .Append(value.Lower)
            .Append(" ~ ")
            .Append(value.Upper)
            .Append(']');
    }
}

public class ObjectPropertyInfo : BaseObjectPropertyInfo
{
    public string ObjectTypeName { get; set; }

    protected override void PrintType(StringBuilder stringBuilder)
    {
        stringBuilder
            .Append("Object<")
            .Append(ObjectTypeName)
            .Append('>');
    }
}

public class DropdownPropertyInfo : BaseObjectPropertyInfo
{
    public enum DropdownKind
    {
        Int,
        Float,
        String
    }
    public DropdownKind Kind { get; set; }
    public string DataSourcePath { get; set; }
    public string ValueDisplayRegex { get; set; }

    protected override void PrintType(StringBuilder stringBuilder) => 
        stringBuilder
            .Append("Dropdown<")
            .Append(Kind.ToString()).Append('>');

    protected override void PrintAdditional(StringBuilder stringBuilder) =>
        stringBuilder
            .Append('[')
            .Append(ValueDisplayRegex)
            .Append(", \"")
            .Append("./")
            .Append(DataSourcePath)
            .Append("\"]");
}

public class EnumPropertyInfo : BaseObjectPropertyInfo
{
    public record struct EnumValue(string DisplayName, string DeclareName, long Value);
    
    public string EnumTypeName { get; set; }
    public EnumValue[] EnumValues { get; set; }
    public bool IsFlags { get; set; }

    protected override void PrintType(StringBuilder stringBuilder) =>
        stringBuilder.Append(IsFlags ? "EnumFlags<" : "Enum<")
            .Append(EnumTypeName)
            .Append('>');

    protected override void PrintAdditional(StringBuilder stringBuilder) =>
        stringBuilder
            .Append('[')
            .AppendJoin(", ", EnumValues.Select(x => x.DeclareName))
            .Append(']');
}

public class ArrayPropertyInfo : BaseObjectPropertyInfo
{
    public BaseObjectPropertyInfo ArrayElementTypeInfo { get; set; }
    
    protected override void PrintType(StringBuilder stringBuilder) =>
        stringBuilder
            .Append("Array<")
            .Append(ArrayElementTypeInfo)
            .Append('>');
}

public class DictionaryPropertyInfo : BaseObjectPropertyInfo
{
    public BaseObjectPropertyInfo KeyTypeInfo { get; set; }
    public BaseObjectPropertyInfo ValueTypeInfo { get; set; }
        
    protected override void PrintType(StringBuilder stringBuilder) =>
        stringBuilder
            .Append("Dictionary<")
            .Append(KeyTypeInfo)
            .Append(", ")
            .Append(ValueTypeInfo)
            .Append('>');
}