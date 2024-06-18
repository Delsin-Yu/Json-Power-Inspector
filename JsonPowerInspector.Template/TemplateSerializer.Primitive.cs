using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace JsonPowerInspector.Template;

public static partial class TemplateSerializer
{
    private static bool SerializePrimitiveProperty(string name, Type propertyType,
        [NotNullWhen(true)] ref BaseObjectPropertyInfo? baseObjectPropertyInfo, string displayName,
        Attribute[] attributesArray, bool nullable)
    {
        if (propertyType == typeof(bool))
        {
            var defaultValue = attributesArray.OfType<BoolDefaultValueAttribute>().FirstOrDefault();
            var originValue = defaultValue == null ? false : defaultValue.DefaultValue;
            baseObjectPropertyInfo = new BooleanPropertyInfo(name, displayName, nullable, originValue);
        }
        else
        {
            NumberPropertyInfo.NumberType numberType;

            if (propertyType == typeof(byte)) numberType = NumberPropertyInfo.NumberType.Int;
            else if (propertyType == typeof(ushort)) numberType = NumberPropertyInfo.NumberType.Int;
            else if (propertyType == typeof(uint)) numberType = NumberPropertyInfo.NumberType.Int;
            else if (propertyType == typeof(ulong)) numberType = NumberPropertyInfo.NumberType.Int;
            else if (propertyType == typeof(sbyte)) numberType = NumberPropertyInfo.NumberType.Int;
            else if (propertyType == typeof(short)) numberType = NumberPropertyInfo.NumberType.Int;
            else if (propertyType == typeof(int)) numberType = NumberPropertyInfo.NumberType.Int;
            else if (propertyType == typeof(long)) numberType = NumberPropertyInfo.NumberType.Int;
            else if (propertyType == typeof(float)) numberType = NumberPropertyInfo.NumberType.Float;
            else if (propertyType == typeof(double)) numberType = NumberPropertyInfo.NumberType.Float;
            else return false;

            var dropdown = attributesArray.OfType<DropdownAttribute>().FirstOrDefault();
            if (dropdown != null)
            {
                baseObjectPropertyInfo = new DropdownPropertyInfo(
                    name,
                    displayName,
                    numberType switch
                    {
                        NumberPropertyInfo.NumberType.Int => DropdownPropertyInfo.DropdownKind.Int,
                        NumberPropertyInfo.NumberType.Float => DropdownPropertyInfo.DropdownKind.Float,
                        _ => throw new InvalidOperationException()
                    },
                    dropdown.DataPath,
                    dropdown.Regex
                );
            }
            else
            {
                NumberPropertyInfo.NumberRange? range = null;
                var numberRange = attributesArray.OfType<NumberRangeAttribute>().FirstOrDefault();
                if (numberRange != null)
                {
                    range = new(numberRange.LowerBound, numberRange.UpperBound);
                }
                if (numberType == NumberPropertyInfo.NumberType.Int)
                {
                    var intDefaultValueAttribute = attributesArray.OfType<IntDefaultValueAttribute>().FirstOrDefault();
                    var defaultValue = intDefaultValueAttribute == null ? 0 : intDefaultValueAttribute.DefaultValue;
                    baseObjectPropertyInfo = new NumberPropertyInfo(name, displayName, nullable, numberType, defaultValue.ToString(), range);
                }
                else
                {
                    var floatDefaultValueAttribute = attributesArray.OfType<FloatDefaultValueAttribute>().FirstOrDefault();
                    var defaultValue = floatDefaultValueAttribute == null ? 0f : floatDefaultValueAttribute.DefaultValue;
                    baseObjectPropertyInfo = new NumberPropertyInfo(name, displayName, nullable, numberType, defaultValue.ToString(), range);
                }
            }
        }

        return true;
    }
}