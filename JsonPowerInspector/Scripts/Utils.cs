using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public static class Utils
{
    public static IPropertyInspector CreateInspectorForProperty(BaseObjectPropertyInfo propertyInfo, InspectorSpawner spawner, string path)
    {
        return propertyInfo switch
        {
            StringPropertyInfo stringPropertyInfo => spawner.Create(stringPropertyInfo, path),
            NumberPropertyInfo numberPropertyInfo => spawner.Create(numberPropertyInfo, path),
            ObjectPropertyInfo objectPropertyInfo => spawner.Create(objectPropertyInfo, path),
            BooleanPropertyInfo booleanPropertyInfo => spawner.Create(booleanPropertyInfo, path),
            ArrayPropertyInfo arrayPropertyInfo => spawner.Create(arrayPropertyInfo, path),
            DictionaryPropertyInfo dictionaryPropertyInfo => spawner.Create(dictionaryPropertyInfo, path),
            EnumPropertyInfo enumPropertyInfo => spawner.Create(enumPropertyInfo, path),
            _ => throw new InvalidOperationException()
        };
    }

    public static JsonNode CreateJsonObjectForProperty(BaseObjectPropertyInfo propertyInfo, IReadOnlyDictionary<string, ObjectDefinition> objectLookup)
    {
        switch (propertyInfo)
        {
            case StringPropertyInfo:
                return string.Empty;
            case NumberPropertyInfo:
                return 0;
            case ObjectPropertyInfo:
                var objectDefinition = objectLookup[propertyInfo.Name];
                var subJsonObject = new JsonObject();
                foreach (var objectPropertyInfo in objectDefinition.Properties.AsSpan())
                {
                    subJsonObject.Add(objectPropertyInfo.Name, CreateJsonObjectForProperty(objectPropertyInfo, objectLookup));
                }
                return subJsonObject;
            case BooleanPropertyInfo:
                return false;
            case ArrayPropertyInfo:
                return new JsonArray();
            case DictionaryPropertyInfo:
                return new JsonObject();
            case EnumPropertyInfo enumPropertyInfo:
                return enumPropertyInfo.EnumValues.FirstOrDefault().ValueName ?? string.Empty;
            default: throw new InvalidOperationException();
        }
    }
}