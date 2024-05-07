using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
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
    public static ObjectDefinition CollectTypeDefinition(Type objectType, out IDictionary<Type, ObjectDefinition> referencedPropertyInfo, string objectName = null)
    {
        referencedPropertyInfo = new Dictionary<Type, ObjectDefinition>();
        return CollectTypeDefinitionImpl(objectType, objectName, referencedPropertyInfo);
    }

    private static ObjectDefinition CollectTypeDefinitionImpl(Type objectType, string objectName, IDictionary<Type, ObjectDefinition> referencedPropertyInfo)
    {
        var propertyInfos = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var properties = new List<ObjectPropertyInfo>();
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

    private static bool TryParseProperty(PropertyInfo propertyInfo, IDictionary<Type, ObjectDefinition> referencedPropertyInfo, out ObjectPropertyInfo objectPropertyInfo)
    {
        objectPropertyInfo = null;
        var propertyType = propertyInfo.PropertyType;

        if (propertyType.IsArray)
        {
            var elementType = propertyType.GetElementType()!;
            EnsureTypeExists(elementType);
            objectPropertyInfo = new ArrayProperty()
            {
                ArrayElementTypeName = elementType.FullName,
                Type = ObjectPropertyInfo.PropertyType.Array
            };
        }
        else if (propertyType.IsGenericType)
        {
            var genericTypeDef = propertyType.GetGenericTypeDefinition();
            if (genericTypeDef == typeof(List<>))
            {
                var elementType = propertyType.GetGenericArguments()[0];
                EnsureTypeExists(elementType);
                objectPropertyInfo = new ArrayProperty()
                {
                    ArrayElementTypeName = elementType.FullName,
                    Type = ObjectPropertyInfo.PropertyType.Array
                };
            }
            else if (genericTypeDef == typeof(Dictionary<,>))
            {
                var arguments = propertyType.GetGenericArguments();
                var keyType = arguments[0];
                var valueType = arguments[1];
                EnsureTypeExists(keyType);
                EnsureTypeExists(valueType);
                objectPropertyInfo = new DictionaryProperty()
                {
                    KeyTypeName = keyType.FullName,
                    ValueTypeName = valueType.FullName,
                    Type = ObjectPropertyInfo.PropertyType.Dictionary
                };
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        objectPropertyInfo.Name = propertyInfo.Name;
        
        return true;

        void EnsureTypeExists(Type type)
        {
            if (!referencedPropertyInfo.ContainsKey(type))
            {
                referencedPropertyInfo.Add(
                    type,
                    CollectTypeDefinitionImpl(type, type.FullName, referencedPropertyInfo)
                );
            }
        }
    }
}