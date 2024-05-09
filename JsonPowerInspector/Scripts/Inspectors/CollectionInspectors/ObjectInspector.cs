using System;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class ObjectInspector : CollectionInspector<ObjectPropertyInfo>
{
    protected override void OnPostInitialize(ObjectPropertyInfo propertyInfo)
    {
        var objectDefinition = Main.CurrentSession.LookupObject(propertyInfo.ObjectTypeName);
        foreach (var info in objectDefinition.Properties.AsSpan())
        {
            CreateInspector(info);
        }
    }
}