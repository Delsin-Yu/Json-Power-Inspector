using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace JsonPowerInspector.Template;

public static class TemplateSerializer
{
    public static string Serialize(PackedObjectDefinition packedObjectDefinition)
    {
        return JsonSerializer.Serialize(packedObjectDefinition, PowerTemplateJsonContext.Default.PackedObjectDefinition);
    }
    
    public static PackedObjectDefinition Deserialize(string templateFilePath)
    {
        using var fileStream = File.OpenRead(templateFilePath);
        return JsonSerializer.Deserialize(fileStream, PowerTemplateJsonContext.Default.PackedObjectDefinition);
    }

    public static PackedObjectDefinition CollectTypeDefinition<T>() => CollectTypeDefinition(typeof(T));

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
        out BaseObjectPropertyInfo baseObjectPropertyInfo
    )
    {
        baseObjectPropertyInfo = null;

        if (_serializeToStringTypes.Contains(propertyType))
        {
            baseObjectPropertyInfo = new StringPropertyInfo();
        }
        else if (propertyType.IsArray)
        {
            var elementType = propertyType.GetElementType()!;
            if (!TryParseProperty(GetTypeName(elementType), elementType, referencedPropertyInfo, out var arrayElementTypeInfo))
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
                if (!TryParseProperty(GetTypeName(elementType), elementType, referencedPropertyInfo, out var arrayElementTypeInfo))
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
                if (!TryParseProperty(GetTypeName(keyType), keyType, referencedPropertyInfo, out var keyTypeInfo))
                {
                    return false;
                }
                if (!TryParseProperty(GetTypeName(valueType), valueType, referencedPropertyInfo, out var valueTypeInfo))
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
            double min;
            double max;


            if (propertyType == typeof(byte))
            {
                numberType = NumberPropertyInfo.NumberType.Int;
            }
            else if (propertyType == typeof(ushort))
            {
                numberType = NumberPropertyInfo.NumberType.Int;
            }
            else if (propertyType == typeof(uint))
            {
                numberType = NumberPropertyInfo.NumberType.Int;
            }
            else if (propertyType == typeof(ulong))
            {
                numberType = NumberPropertyInfo.NumberType.Int;
            }
            else if (propertyType == typeof(sbyte))
            {
                numberType = NumberPropertyInfo.NumberType.Int;
            }
            else if (propertyType == typeof(short))
            {
                numberType = NumberPropertyInfo.NumberType.Int;
            }
            else if (propertyType == typeof(int))
            {
                numberType = NumberPropertyInfo.NumberType.Int;
            }
            else if (propertyType == typeof(long))
            {
                numberType = NumberPropertyInfo.NumberType.Int;
            }
            else if (propertyType == typeof(float))
            {
                numberType = NumberPropertyInfo.NumberType.Float;
            }
            else if (propertyType == typeof(double))
            {
                numberType = NumberPropertyInfo.NumberType.Float;
            }
            else
            {
                return false;
            }

            baseObjectPropertyInfo = new NumberPropertyInfo
            {
                NumberKind = numberType
            };
        }
        else if (propertyType == typeof(string))
        {
            baseObjectPropertyInfo = new StringPropertyInfo();
        }
        else if (propertyType.IsEnum)
        {
            var enumValuesArray = propertyType.GetEnumValues();
            var typedEnumValuesArray = new List<long>(enumValuesArray.Length);

            foreach (var enumValue in enumValuesArray)
            {
                typedEnumValuesArray.Add(Convert.ToInt64(enumValue));
            }

            var enumValues = propertyType.GetEnumNames().Zip(
                typedEnumValuesArray,
                (name, value) =>
                    new EnumPropertyInfo.EnumValue(name, value)
            ).ToArray();
            baseObjectPropertyInfo = new EnumPropertyInfo
            {
                EnumTypeName = propertyType.Name,
                EnumValues = enumValues,
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