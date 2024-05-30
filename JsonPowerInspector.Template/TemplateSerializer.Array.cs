using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace JsonPowerInspector.Template;

public static partial class TemplateSerializer
{
    private static bool SerializeArrayProperty(string name, Type propertyType,
        Dictionary<string, ObjectDefinition> referencedPropertyInfo,
        [NotNullWhen(true)] ref BaseObjectPropertyInfo? baseObjectPropertyInfo,
        Attribute[] attributesArray, 
        string displayName)
    {
        var elementType = propertyType.GetElementType()!;
        if (!TryParseProperty(
                GetTypeName(elementType),
                elementType,
                referencedPropertyInfo,
                attributesArray,
                out var arrayElementTypeInfo
            ))
        {
            return false;
        }

        baseObjectPropertyInfo = new ArrayPropertyInfo(name, displayName, arrayElementTypeInfo);
        return true;
    }
}