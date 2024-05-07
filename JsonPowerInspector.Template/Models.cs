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
            .Append(' ', indentationLevel * 4)
            .AppendLine("}");
    }
}

public class BaseObjectPropertyInfo
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
            .Append(' ', indentationLevel * 4);

        PrintType(stringBuilder);

        stringBuilder
            .Append(' ')
            .Append(Name)
            .Append(' ');
        
        PrintAdditional(stringBuilder);

        stringBuilder.AppendLine();
    }
}

public class NumberPropertyInfo : BaseObjectPropertyInfo
{
    public record struct NumberRange(double Lower, double Upper);

    public enum NumberType
    {
        Int,
        Float
    }

    public NumberType Number { get; set; }
    public NumberRange? Range { get; set; }

    protected override void PrintType(StringBuilder stringBuilder)
    {
        stringBuilder
            .Append(
                Number switch
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
        stringBuilder
            .Append('[')
            .Append(Range.Value.Lower)
            .Append(" ~ ")
            .Append(Range.Value.Upper)
            .Append(']');
    }
}

public class ObjectPropertyInfo : BaseObjectPropertyInfo
{
    public string ObjectTypeName { get; set; }

    protected override void PrintType(StringBuilder stringBuilder)
    {
        stringBuilder.Append(ObjectTypeName);
    }
}

public class ArrayPropertyInfo : BaseObjectPropertyInfo
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

public class DictionaryPropertyInfo : BaseObjectPropertyInfo
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