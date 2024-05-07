using System;
using System.Text;
using System.Text.Json.Serialization;

namespace JsonPowerInspector.Template;

[JsonSerializable(typeof(ApplicationJsonTypes))]
public partial class PowerTemplateJsonContext : JsonSerializerContext { }

public class ApplicationJsonTypes
{
    public JsonFileInfo JsonFileInfo { get; set; }
}

public class JsonFileInfo
{
    public string DataPath { get; set; }
    public ObjectDefinition MainObjectDefinition { get; set; }
    public ObjectDefinition[] ReferencedObjectDefinition { get; set; }
}

public class ObjectDefinition
{
    public string ObjectTypeName { get; set; }
    public ObjectPropertyInfo[] Properties { get; set; }

    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder, 0);
        return builder.ToString();
    }

    public void ToString(StringBuilder stringBuilder, int indentationLevel)
    {
        stringBuilder
            .Append(' ', indentationLevel)
            .AppendLine(ObjectTypeName)
            .Append(' ', indentationLevel)
            .AppendLine("{");

        indentationLevel++;
        
        foreach (var property in Properties.AsSpan())
        {
            property.ToString(stringBuilder, indentationLevel);
        }
        
        indentationLevel--;

        
        stringBuilder
            .Append(' ', indentationLevel)
            .AppendLine("}");
    }
}

public class ObjectPropertyInfo
{
    public enum PropertyType
    {
        String,
        Number,
        Object,
        Bool,
        Array,
        Dictionary
    }

    public PropertyType Type { get; set; }
    public string Name { get; set; }
    
    public override string ToString()
    {
        var builder = new StringBuilder();
        ToString(builder, 0);
        return builder.ToString();
    }

    protected virtual void PrintType(StringBuilder stringBuilder)
    {
        stringBuilder.Append(Type.ToString());
    }

    protected virtual void PrintAdditional(StringBuilder stringBuilder)
    {
        
    }
    
    public void ToString(StringBuilder stringBuilder, int indentationLevel)
    {
        stringBuilder
            .Append(' ', indentationLevel);

        PrintType(stringBuilder);

        stringBuilder
            .Append(' ')
            .Append(Name)
            .Append(' ');
        
        PrintAdditional(stringBuilder);

        stringBuilder.AppendLine();
    }
}

public class NumberProperty : ObjectPropertyInfo
{
    public record struct NumberRange(double Lower, double Upper);

    public enum NumberType
    {
        Int,
        Float
    }

    public enum PrecisionType
    {
        Bit8,
        Bit16,
        Bit32,
        Bit64,
    }

    public enum SignType
    {
        Signed,
        Unsigned
    }

    public NumberType Number { get; set; }
    public PrecisionType Precision { get; set; }
    public SignType Sign { get; set; }
    public NumberRange? Range { get; set; }

    protected override void PrintType(StringBuilder stringBuilder)
    {
        stringBuilder
            .Append(
                Sign switch
                {
                    SignType.Signed => string.Empty,
                    SignType.Unsigned => "Unsigned ",
                    _ => throw new ArgumentOutOfRangeException()
                }
            )
            .Append(
                Number switch
                {
                    NumberType.Int => "Int",
                    NumberType.Float => "Float",
                    _ => throw new ArgumentOutOfRangeException()
                }
            )
            .Append(
                Precision switch
                {
                    PrecisionType.Bit8 => "8",
                    PrecisionType.Bit16 => "16",
                    PrecisionType.Bit32 => "32",
                    PrecisionType.Bit64 => "64",
                    _ => throw new ArgumentOutOfRangeException()
                }
            );
    }

    protected override void PrintAdditional(StringBuilder stringBuilder)
    {
        if(!Range.HasValue) return;
        stringBuilder
            .Append('[')
            .Append(Range.Value.Lower)
            .Append(" ~ ")
            .Append(Range.Value.Upper)
            .Append(']');
    }
}

public class ObjectProperty : ObjectPropertyInfo
{
    public string ObjectTypeName { get; set; }

    protected override void PrintType(StringBuilder stringBuilder)
    {
        stringBuilder.Append(ObjectTypeName);
    }
}

public class ArrayProperty : ObjectPropertyInfo
{
    public string ArrayElementTypeName { get; set; }
    
    protected override void PrintType(StringBuilder stringBuilder)
    {
        stringBuilder
            .Append("Array<")
            .Append(ArrayElementTypeName)
            .Append('>');
    }
}

public class DictionaryProperty : ObjectPropertyInfo
{
    public string KeyTypeName { get; set; }
    public string ValueTypeName { get; set; }
        
    protected override void PrintType(StringBuilder stringBuilder)
    {
        stringBuilder
            .Append("Dictionary<")
            .Append(KeyTypeName)
            .Append(", ")
            .Append(ValueTypeName)
            .Append('>');
    }
}