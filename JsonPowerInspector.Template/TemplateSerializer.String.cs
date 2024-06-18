using System;
using System.Linq;

namespace JsonPowerInspector.Template;

public static partial class TemplateSerializer
{
    private static void SerializeStringProperty(string name, out BaseObjectPropertyInfo baseObjectPropertyInfo,
        Attribute[] attributesArray, string displayName)
    {
        var dropdown = attributesArray.OfType<DropdownAttribute>().FirstOrDefault();
        if (dropdown != null)
        {
            baseObjectPropertyInfo = new DropdownPropertyInfo(
                name,
                displayName,
                DropdownPropertyInfo.DropdownKind.String,
                dropdown.DataPath,
                dropdown.Regex
            );
        }
        else
        {
            var defaultValue = attributesArray.OfType<StringDefaultValueAttribute>().FirstOrDefault();
            var originValue = defaultValue == null ? string.Empty : defaultValue.DefaultValue;
            baseObjectPropertyInfo = new StringPropertyInfo(name, displayName, originValue);
        }
    }
}