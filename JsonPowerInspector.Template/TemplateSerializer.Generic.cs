using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace JsonPowerInspector.Template;

public static partial class TemplateSerializer
{
    private static bool SerializeGenericProperty(string name, Type propertyType,
        Dictionary<string, ObjectDefinition> referencedPropertyInfo,
        [NotNullWhen(true)] ref BaseObjectPropertyInfo? baseObjectPropertyInfo, 
        Attribute[] attributesArray, string displayName)
    {
        var genericTypeDef = propertyType.GetGenericTypeDefinition();
        if (genericTypeDef == typeof(List<>))
        {
            var elementType = propertyType.GetGenericArguments()[0];
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
        }
        else if (genericTypeDef == typeof(Dictionary<,>))
        {
            var arguments = propertyType.GetGenericArguments();
            var keyType = arguments[0];
            var valueType = arguments[1];
            var attributeArray = attributesArray.ToArray();

            var attributesList = new List<Attribute>(2);

            var keyDropdown = (DropdownAttribute?)attributeArray.OfType<KeyDropdownAttribute>().FirstOrDefault();
            var keyNumberRange =
                (NumberRangeAttribute?)attributeArray.OfType<KeyNumberRangeAttribute>().FirstOrDefault();

            if (keyDropdown != null) attributesList.Add(keyDropdown);
            if (keyNumberRange != null) attributesList.Add(keyNumberRange);

            if (!TryParseProperty(
                    GetTypeName(keyType),
                    keyType,
                    referencedPropertyInfo,
                    attributesList,
                    out var keyTypeInfo
                ))
            {
                return false;
            }

            attributesList.Clear();

            var valueDropdown = (DropdownAttribute?)attributeArray.OfType<ValueDropdownAttribute>().FirstOrDefault();
            var valueNumberRange =
                (NumberRangeAttribute?)attributeArray.OfType<ValueNumberRangeAttribute>().FirstOrDefault();

            if (valueDropdown != null) attributesList.Add(valueDropdown);
            if (valueNumberRange != null) attributesList.Add(valueNumberRange);

            if (!TryParseProperty(
                    GetTypeName(valueType),
                    valueType,
                    referencedPropertyInfo,
                    attributesList,
                    out var valueTypeInfo
                ))
            {
                referencedPropertyInfo.Remove(GetTypeName(keyType));
                return false;
            }

            baseObjectPropertyInfo = new DictionaryPropertyInfo(name, displayName, keyTypeInfo, valueTypeInfo);
        }
        else if (genericTypeDef == typeof(Nullable<>))
        {
            var arguments = propertyType.GetGenericArguments();
            if (!SerializePrimitiveProperty(name, arguments[0], ref baseObjectPropertyInfo, displayName,
                    attributesArray, true)) return false;
        }
        else
        {
            return false;
        }

        return true;
    }
}