using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace JsonPowerInspector.Template;

public static class TemplateSerializer
{
    public static JsonFileInfo Deserialize(string templateFilePath)
    {
        using var fileStream = File.OpenRead(templateFilePath);
        return JsonSerializer.Deserialize(fileStream, PowerTemplateJsonContext.Default.JsonFileInfo);
    }

    [RequiresUnreferencedCode("CollectDefinition is intended to be used in editor to generate JsonFileInfo")]
    public static ObjectDefinition CollectTypeDefinition(Type objectType, out Dictionary<Type, ObjectDefinition> referencedPropertyInfo, string objectName = null)
    {
        referencedPropertyInfo = new();
        return CollectTypeDefinitionImpl(objectType, objectName, referencedPropertyInfo);
    }
    
    private static ObjectDefinition CollectTypeDefinitionImpl(Type objectType, string objectName, Dictionary<Type, ObjectDefinition> referencedPropertyInfo)
    {
        var propertyInfos = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var properties = new List<BaseObjectPropertyInfo>();
        foreach (var propertyInfo in propertyInfos)
        {
            if (TryParseProperty(propertyInfo, referencedPropertyInfo, out var parsed))
            {
                properties.Add(parsed);
            }
        }
        var definition = new ObjectDefinition()
        {
            ObjectTypeName = objectName ?? objectType.Name,
            Properties = properties.ToArray()
        };
        return definition;
    }

    private static bool TryParseProperty(PropertyInfo propertyInfo, Dictionary<Type, ObjectDefinition> referencedPropertyInfo, out BaseObjectPropertyInfo baseObjectPropertyInfo)
    {
        baseObjectPropertyInfo = null;
        var propertyType = propertyInfo.PropertyType;

        if (propertyType.IsArray)
        {
            var elementType = propertyType.GetElementType()!;
            EnsureTypeExists(elementType);
            baseObjectPropertyInfo = new ArrayPropertyInfo()
            {
                ArrayElementTypeName = elementType.FullName,
                Type = BaseObjectPropertyInfo.PropertyType.Array
            };
        }
        else if (propertyType.IsGenericType)
        {
            var genericTypeDef = propertyType.GetGenericTypeDefinition();
            if (genericTypeDef == typeof(List<>))
            {
                var elementType = propertyType.GetGenericArguments()[0];
                EnsureTypeExists(elementType);
                baseObjectPropertyInfo = new ArrayPropertyInfo()
                {
                    ArrayElementTypeName = elementType.FullName,
                    Type = BaseObjectPropertyInfo.PropertyType.Array
                };
            }
            else if (genericTypeDef == typeof(Dictionary<,>))
            {
                var arguments = propertyType.GetGenericArguments();
                var keyType = arguments[0];
                var valueType = arguments[1];
                EnsureTypeExists(keyType);
                EnsureTypeExists(valueType);
                baseObjectPropertyInfo = new DictionaryPropertyInfo()
                {
                    KeyTypeName = keyType.FullName,
                    ValueTypeName = valueType.FullName,
                    Type = BaseObjectPropertyInfo.PropertyType.Dictionary
                };
            }
            else
            {
                return false;
            }
        }
        else if (propertyType.IsPrimitive)
        {
            if (
                propertyType == typeof(sbyte) ||
                propertyType == typeof(short) ||
                propertyType == typeof(int) ||
                propertyType == typeof(long) ||
                propertyType == typeof(byte) ||
                propertyType == typeof(ushort) ||
                propertyType == typeof(uint) ||
                propertyType == typeof(ulong)
            )
            {
                baseObjectPropertyInfo = new NumberPropertyInfo()
                {
                    Number = NumberPropertyInfo.NumberType.Int,
                    Type = BaseObjectPropertyInfo.PropertyType.Number
                };
            }
            else if (
                propertyType == typeof(float) ||
                propertyType == typeof(double)
            )
            {
                baseObjectPropertyInfo = new NumberPropertyInfo()
                {
                    Number = NumberPropertyInfo.NumberType.Float,
                    Type = BaseObjectPropertyInfo.PropertyType.Number
                }; 
            }
            else
            {
                return false;
            }
        }
        else if (propertyType == typeof(string))
        {
            baseObjectPropertyInfo = new()
            {
                Type = BaseObjectPropertyInfo.PropertyType.String
            };
        }
        else if (propertyType == typeof(bool))
        {
            baseObjectPropertyInfo = new()
            {
                Type = BaseObjectPropertyInfo.PropertyType.Bool
            };
        }
        else if (!propertyType.IsGenericType)
        {
            EnsureTypeExists(propertyType);
            baseObjectPropertyInfo = new ObjectPropertyInfo()
            {
                ObjectTypeName = propertyType.FullName,
                Type = BaseObjectPropertyInfo.PropertyType.Object
            };
        }
        else
        {
            return false;
        }

        baseObjectPropertyInfo.Name = propertyInfo.Name;
        
        return true;

        void EnsureTypeExists(Type type)
        {
            if (type.IsPrimitive || type == typeof(string) || type.IsEnum) return;
            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(referencedPropertyInfo, type, out var exists);
            if (exists) return;
            value = CollectTypeDefinitionImpl(type, type.FullName, referencedPropertyInfo);
        }
    }
}