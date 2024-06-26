using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JsonPowerInspector.Template;

public static partial class TemplateSerializer
{
    private static void SerializeEnumProperty(string name, Type propertyType,
        out BaseObjectPropertyInfo baseObjectPropertyInfo, Attribute[] attributesArray, string displayName)
    {
        var enumValuesList = new List<EnumPropertyInfo.EnumValue>();

        foreach (var enumField in propertyType.GetFields(BindingFlags.Static | BindingFlags.Public))
        {
            var inspectorNameAttribute = enumField.GetCustomAttribute<InspectorNameAttribute>();
            enumValuesList.Add(
                new(
                    inspectorNameAttribute?.DisplayName ?? enumField.Name,
                    enumField.Name,
                    Convert.ToInt64(enumField.GetRawConstantValue())
                )
            );
        }

        var defaultValue = attributesArray.OfType<EnumDefaultValueAttribute>().FirstOrDefault();

        var originValue = string.Empty;
        if (defaultValue!=null)
        {
            originValue = defaultValue.DefaultValue;
        }
        Console.WriteLine(originValue);

        baseObjectPropertyInfo = new EnumPropertyInfo(
            name,
            displayName,
            propertyType.Name,
            enumValuesList.ToArray(),
            propertyType.GetCustomAttributes<FlagsAttribute>().Any(),
            originValue
        );
    }
}