using System;
using System.Linq;
using System.Text.Json.Nodes;
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

    public override void Bind(JsonNode node)
    {
        var o = node.AsObject().ToArray();
        for (var index = 0; index < o.Length; index++)
        {
            Nodes[index].Inspector.Bind(o[index].Value!);
        }
    }
}