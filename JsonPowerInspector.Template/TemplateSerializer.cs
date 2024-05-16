using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace JsonPowerInspector.Template;

public static class TemplateSerializer
{
    /// <summary>
    /// Serialize the specified <see cref="PackedObjectDefinition"/> data model to the jsontemplate string.
    /// The developer should save the result into a .jsontemplate file for JsonPowerInspector usage.
    /// </summary>
    /// <param name="packedObjectDefinition">The data model to serialize from.</param>
    /// <returns>A JSON string that should be saved into a .jsontemplate file for JsonPowerInspector usage.</returns>
    public static string Serialize(PackedObjectDefinition packedObjectDefinition) => 
        JsonSerializer.Serialize(packedObjectDefinition, PowerTemplateJsonContext.Default.PackedObjectDefinition);

    /// <summary>
    /// Collect the required serialization info for the specified type for JsonPowerInspector to use.
    /// </summary>
    /// <typeparam name="T">The type for collecting the info from.</typeparam>
    /// <returns>A data model for serializing to .jsontemplate file.</returns>
    public static PackedObjectDefinition CollectTypeDefinition<T>() => CollectTypeDefinition(typeof(T));

    /// <summary>
    /// Collect the required serialization info for the specified type for JsonPowerInspector to use.
    /// </summary>
    /// <param name="objectType">The type for collecting the info from.</param>
    /// <returns>A data model for serializing to .jsontemplate file.</returns>
    [RequiresUnreferencedCode("CollectDefinition is intended to be used in editor to generate JsonFileInfo only.")]
    public static PackedObjectDefinition CollectTypeDefinition(Type objectType)
    {
        var referencedPropertyInfo = new Dictionary<string, ObjectDefinition>();
        var mainObjectDefinition = CollectTypeDefinitionImpl(objectType, referencedPropertyInfo);
        return new(mainObjectDefinition, referencedPropertyInfo.Values.ToArray());
    }
    
    private static ObjectDefinition CollectTypeDefinitionImpl(Type objectType, Dictionary<string, ObjectDefinition> referencedPropertyInfo)
    {
        var propertyInfos = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(x => x.CanRead && x.CanWrite).ToArray();
        var properties = new List<BaseObjectPropertyInfo>();
        foreach (var propertyInfo in propertyInfos)
        {
            if (TryParseProperty(
                    propertyInfo.Name,
                    propertyInfo.PropertyType,
                    referencedPropertyInfo,
                    propertyInfo.GetCustomAttributes(),
                    out var parsed
                ))
            {
                properties.Add(parsed);
            }
        }
        var definition = new ObjectDefinition
        {
            ObjectTypeName = GetTypeName(objectType),
            Properties = properties.ToArray()
        };
        return definition;
    }

    private static string GetTypeName(Type type) => type.Name;

    private static readonly HashSet<Type> _serializeToStringTypes =
    [
        typeof(DateTime)
    ];

    private static bool TryParseProperty(
        string propertyName,
        Type propertyType,
        Dictionary<string, ObjectDefinition> referencedPropertyInfo,
        IEnumerable<Attribute> attributes,
        out BaseObjectPropertyInfo baseObjectPropertyInfo
    )
    {
        baseObjectPropertyInfo = null;

        var enumerable = attributes as Attribute[] ?? attributes.ToArray();
        if (_serializeToStringTypes.Contains(propertyType))
        {
            baseObjectPropertyInfo = new StringPropertyInfo();
        }
        else if (propertyType.IsArray)
        {
            var elementType = propertyType.GetElementType()!;
            var dropdown = enumerable.OfType<InspectorDropdownAttribute>().FirstOrDefault();
            if (!TryParseProperty(GetTypeName(elementType), elementType, referencedPropertyInfo, [dropdown], out var arrayElementTypeInfo))
            {
                return false;
            }
            baseObjectPropertyInfo = new ArrayPropertyInfo
            {
                ArrayElementTypeInfo = arrayElementTypeInfo,
            };
        }
        else if (propertyType.IsGenericType)
        {
            var genericTypeDef = propertyType.GetGenericTypeDefinition();
            if (genericTypeDef == typeof(List<>))
            {
                var elementType = propertyType.GetGenericArguments()[0];
                var dropdown = enumerable.OfType<InspectorDropdownAttribute>().FirstOrDefault();
                if (!TryParseProperty(GetTypeName(elementType), elementType, referencedPropertyInfo, [dropdown], out var arrayElementTypeInfo))
                {
                    return false;
                }
                baseObjectPropertyInfo = new ArrayPropertyInfo
                {
                    ArrayElementTypeInfo = arrayElementTypeInfo,
                };
            }
            else if (genericTypeDef == typeof(Dictionary<,>))
            {
                var arguments = propertyType.GetGenericArguments();
                var keyType = arguments[0];
                var valueType = arguments[1];
                var attributeArray = enumerable.ToArray();
                var keyDropdown = (InspectorDropdownAttribute)attributeArray.OfType<InspectorKeyDropdownAttribute>().FirstOrDefault();
                if (!TryParseProperty(GetTypeName(keyType), keyType, referencedPropertyInfo, [keyDropdown], out var keyTypeInfo))
                {
                    return false;
                }
                var valueDropdown = (InspectorDropdownAttribute)attributeArray.OfType<InspectorValueDropdownAttribute>().FirstOrDefault();
                if (!TryParseProperty(GetTypeName(valueType), valueType, referencedPropertyInfo, [valueDropdown], out var valueTypeInfo))
                {
                    referencedPropertyInfo.Remove(GetTypeName(keyType));
                    return false;
                }
                baseObjectPropertyInfo = new DictionaryPropertyInfo
                {
                    KeyTypeInfo = keyTypeInfo,
                    ValueTypeInfo = valueTypeInfo,
                };
            }
            else
            {
                return false;
            }
        }
        else if (propertyType == typeof(bool))
        {
            baseObjectPropertyInfo = new BooleanPropertyInfo();
        }
        else if (propertyType.IsPrimitive)
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

            var dropdown = enumerable.OfType<InspectorDropdownAttribute>().FirstOrDefault();
            if (dropdown != null)
            {
                baseObjectPropertyInfo = new DropdownPropertyInfo
                {
                    DataSourcePath = dropdown.DataPath,
                    ValueDisplayRegex = dropdown.Regex,
                    Kind = numberType switch
                    {
                        NumberPropertyInfo.NumberType.Int => DropdownPropertyInfo.DropdownKind.Int,
                        NumberPropertyInfo.NumberType.Float => DropdownPropertyInfo.DropdownKind.Float,
                        _ => throw new InvalidOperationException()
                    }
                };
            }
            else
            {
                baseObjectPropertyInfo = new NumberPropertyInfo
                {
                    NumberKind = numberType
                };
            }
        }
        else if (propertyType == typeof(string))
        {
            var dropdown = enumerable.OfType<InspectorDropdownAttribute>().FirstOrDefault();
            if (dropdown != null)
            {
                baseObjectPropertyInfo = new DropdownPropertyInfo
                {
                    DataSourcePath = dropdown.DataPath,
                    ValueDisplayRegex = dropdown.Regex,
                    Kind = DropdownPropertyInfo.DropdownKind.String
                };
            }
            else
            {
                baseObjectPropertyInfo = new StringPropertyInfo();
            }
        }
        else if (propertyType.IsEnum)
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
            
            baseObjectPropertyInfo = new EnumPropertyInfo
            {
                EnumTypeName = propertyType.Name,
                EnumValues = enumValuesList.ToArray(),
                IsFlags = propertyType.GetCustomAttributes<FlagsAttribute>().Any()
            };
        }
        else if (!propertyType.IsGenericType)
        {
            EnsureTypeExists(propertyType);
            baseObjectPropertyInfo = new ObjectPropertyInfo
            {
                ObjectTypeName = GetTypeName(propertyType),
            };
        }
        else
        {
            return false;
        }

        baseObjectPropertyInfo.Name = propertyName;
        var inspectorName = enumerable.OfType<InspectorNameAttribute>().FirstOrDefault();
        baseObjectPropertyInfo.DisplayName = inspectorName != null ? inspectorName.DisplayName : baseObjectPropertyInfo.Name;

        return true;

        void EnsureTypeExists(Type type)
        {
            if (type.IsPrimitive || type == typeof(string) || type.IsEnum) return;
            var typeName = GetTypeName(type);
            if (!referencedPropertyInfo.TryAdd(typeName, null)) return;
            var typeDefinition = CollectTypeDefinitionImpl(type, referencedPropertyInfo);
            referencedPropertyInfo[typeName] = typeDefinition;
        }
    }
}