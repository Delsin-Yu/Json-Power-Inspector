using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public static class Utils
{
    public static IPropertyInspector CreateInspectorForProperty(BaseObjectPropertyInfo propertyInfo, InspectorSpawner spawner)
    {
        return propertyInfo switch
        {
            StringPropertyInfo stringPropertyInfo => spawner.Create(stringPropertyInfo),
            NumberPropertyInfo numberPropertyInfo => spawner.Create(numberPropertyInfo),
            ObjectPropertyInfo objectPropertyInfo => spawner.Create(objectPropertyInfo),
            BooleanPropertyInfo booleanPropertyInfo => spawner.Create(booleanPropertyInfo),
            ArrayPropertyInfo arrayPropertyInfo => spawner.Create(arrayPropertyInfo),
            DictionaryPropertyInfo dictionaryPropertyInfo => spawner.Create(dictionaryPropertyInfo),
            EnumPropertyInfo enumPropertyInfo => spawner.Create(enumPropertyInfo),
            _ => throw new InvalidOperationException()
        };
    }

    public static JsonNode CreateJsonObjectForProperty(BaseObjectPropertyInfo propertyInfo, IReadOnlyDictionary<string, ObjectDefinition> objectLookup, HashSet<BaseObjectPropertyInfo> path)
    {
        switch (propertyInfo)
        {
            case StringPropertyInfo:
                return string.Empty;
            case NumberPropertyInfo:
                return 0;
            case ObjectPropertyInfo objectPropertyInfo:
                var objectDefinition = objectLookup[objectPropertyInfo.ObjectTypeName];
                var subJsonObject = new JsonObject();
                foreach (var baseObjectPropertyInfo in objectDefinition.Properties.AsSpan())
                {
                    if(!path.Add(baseObjectPropertyInfo)) continue;
                    subJsonObject.Add(baseObjectPropertyInfo.Name, CreateJsonObjectForProperty(baseObjectPropertyInfo, objectLookup, path));
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