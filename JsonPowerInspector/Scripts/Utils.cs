using System;
using System.Linq;
using System.Text.Json.Nodes;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public static class Utils
{
    public static IPropertyInspector CreateInspectorForProperty(BaseObjectPropertyInfo propertyInfo, InspectorSpawner spawner, bool affectMainObject)
    {
        return propertyInfo switch
        {
            StringPropertyInfo stringPropertyInfo => spawner.Create(stringPropertyInfo, affectMainObject),
            NumberPropertyInfo numberPropertyInfo => spawner.Create(numberPropertyInfo, affectMainObject),
            ObjectPropertyInfo objectPropertyInfo => spawner.Create(objectPropertyInfo, affectMainObject),
            BooleanPropertyInfo booleanPropertyInfo => spawner.Create(booleanPropertyInfo, affectMainObject),
            ArrayPropertyInfo arrayPropertyInfo => spawner.Create(arrayPropertyInfo, affectMainObject),
            DictionaryPropertyInfo dictionaryPropertyInfo => spawner.Create(dictionaryPropertyInfo, affectMainObject),
            EnumPropertyInfo enumPropertyInfo => spawner.Create(enumPropertyInfo, affectMainObject),
            DropdownPropertyInfo dropdownPropertyInfo => spawner.Create(dropdownPropertyInfo, affectMainObject),
            _ => throw new InvalidOperationException()
        };
    }

    public static JsonNode CreateDefaultJsonObjectForProperty(BaseObjectPropertyInfo propertyInfo) =>
        propertyInfo switch
        {
            StringPropertyInfo => string.Empty,
            NumberPropertyInfo => 0,
            ObjectPropertyInfo => null,
            BooleanPropertyInfo => false,
            ArrayPropertyInfo => new JsonArray(),
            DictionaryPropertyInfo => new JsonObject(),
            EnumPropertyInfo enumPropertyInfo => enumPropertyInfo.EnumValues.FirstOrDefault().DeclareName ?? string.Empty,
            DropdownPropertyInfo dropdownPropertyInfo => dropdownPropertyInfo.Kind switch
            {
                DropdownPropertyInfo.DropdownKind.Int => 0,
                DropdownPropertyInfo.DropdownKind.Float => 0.0,
                DropdownPropertyInfo.DropdownKind.String => "",
                _ => throw new InvalidOperationException()
            },
            _ => throw new InvalidOperationException()
        };
}