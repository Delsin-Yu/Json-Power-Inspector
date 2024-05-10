using System;
using System.Linq;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class ObjectInspector : CollectionInspector<ObjectPropertyInfo>
{
    [Export] private Button _deleteBtn;

    protected override void OnFoldUpdate(bool shown)
    {

    }

    protected override void OnInitialPrint(JsonNode node)
    {
        if (node == null) return;
        
        var objectDefinition = Main.CurrentSession.LookupObject(PropertyInfo.ObjectTypeName);
        var span = objectDefinition.Properties.AsSpan();
        var jsonObject = node.AsObject();
        var jsonProperties = jsonObject.ToArray();
        for (var index = 0; index < span.Length; index++)
        {
            var info = span[index];
            var inspector = Utils.CreateInspectorForProperty(info, Main.CurrentSession.InspectorSpawner, $"{PropertyPath}.{info.Name}");
            AddChildNode(inspector, info);
            var jsonNode = jsonProperties[index].Value;
            var newNode = jsonNode;
            inspector.Bind(ref newNode);
            if (newNode != jsonNode)
            {
                jsonObject[info.Name] = newNode;
            }
        }
    }
}