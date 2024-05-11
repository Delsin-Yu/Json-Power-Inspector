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

    public static JsonNode CreateDefaultJsonObjectForProperty(BaseObjectPropertyInfo propertyInfo) =>
        propertyInfo switch
        {
            StringPropertyInfo => string.Empty,
            NumberPropertyInfo => 0,
            ObjectPropertyInfo => new JsonObject(),
            BooleanPropertyInfo => false,
            ArrayPropertyInfo => new JsonArray(),
            DictionaryPropertyInfo => new JsonObject(),
            EnumPropertyInfo enumPropertyInfo => enumPropertyInfo.EnumValues.FirstOrDefault().ValueName ?? string.Empty,
            _ => throw new InvalidOperationException()
        };

    public static JsonNode CreateJsonObjectForProperty(BaseObjectPropertyInfo propertyInfo, IReadOnlyDictionary<string, ObjectDefinition> objectLookup, HashSet<BaseObjectPropertyInfo> path)
    {
        var jsonProperty = CreateDefaultJsonObjectForProperty(propertyInfo);
        
        if (propertyInfo is not ObjectPropertyInfo objectPropertyInfo) return jsonProperty;
        
        var subJsonObject = jsonProperty.AsObject();
        var objectDefinition = objectLookup[objectPropertyInfo.ObjectTypeName];
        foreach (var baseObjectPropertyInfo in objectDefinition.Properties.AsSpan())
        {
            if(!path.Add(baseObjectPropertyInfo)) continue;
            subJsonObject.Add(baseObjectPropertyInfo.Name, CreateJsonObjectForProperty(baseObjectPropertyInfo, objectLookup, path));
        }

        return jsonProperty;
    }
}