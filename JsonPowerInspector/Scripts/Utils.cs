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

    public static JsonNode CreateDefaultJsonNodeForProperty(BaseObjectPropertyInfo propertyInfo)
    {
        switch (propertyInfo)
        {
            case StringPropertyInfo stringPropertyInfo:
                return stringPropertyInfo.DefaultValue ?? string.Empty;
            case NumberPropertyInfo numberProperty:
                if (numberProperty.Nullable)
                {
                    return null;
                }

                switch (numberProperty.NumberKind)
                {
                    case NumberPropertyInfo.NumberType.Int:
                        return int.TryParse(numberProperty.DefaultValue, out var intResult) ? intResult : 0;
                    case NumberPropertyInfo.NumberType.Float:
                        return double.TryParse(numberProperty.DefaultValue, out var floatResult) ? floatResult : 0;
                    default:
                        return 0;
                }
            case ObjectPropertyInfo:
                return null;
            case BooleanPropertyInfo boolProperty:
                return boolProperty.Nullable ? null : false;
            case ArrayPropertyInfo:
                return new JsonArray();
            case DictionaryPropertyInfo:
                return new JsonObject();
            case EnumPropertyInfo enumPropertyInfo:
                if (enumPropertyInfo.EnumValues.Length == 0)
                {
                    return string.Empty;
                }
                var targetEnum = enumPropertyInfo.EnumValues.FirstOrDefault(x => x.DeclareName == enumPropertyInfo.DefaultValue);
                if (targetEnum.Equals(default))
                {
                    return enumPropertyInfo.EnumValues.FirstOrDefault().DeclareName;
                }
                else
                {
                    return targetEnum.DeclareName;
                }
            case DropdownPropertyInfo dropdownPropertyInfo:
                return dropdownPropertyInfo.Kind switch
                {
                    DropdownPropertyInfo.DropdownKind.Int => 0,
                    DropdownPropertyInfo.DropdownKind.Float => 0.0,
                    DropdownPropertyInfo.DropdownKind.String => "",
                    _ => throw new InvalidOperationException()
                };
            default:
                throw new InvalidOperationException();
        }
    }
}