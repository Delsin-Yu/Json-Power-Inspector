using System;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class ArrayInspector : CollectionInspector<ArrayPropertyInfo>
{
    [Export] private Button _addElement;
    [Export] private SpinBox _arrayElementCount;
    [Export] private PackedScene _arrayElement;
    
    protected override void OnFoldUpdate(bool shown) => _addElement.Visible = shown;

    protected override void Bind(ref JsonNode node)
    {
        node ??= new JsonArray();
        _arrayElementCount.Value = node.AsArray().Count;
    }

    protected override void OnInitialPrint(JsonNode node)
    {
        var jsonArray = node.AsArray();
        var spawner = Main.CurrentSession.InspectorSpawner;
        for (var index = 0; index < jsonArray.Count; index++)
        {
            var jsonArrayElement = jsonArray[index];
            var inspector = Utils.CreateInspectorForProperty(
                PropertyInfo.ArrayElementTypeInfo,
                spawner
            );
            inspector.BindJsonNode(ref jsonArrayElement);
            var arrayItem = _arrayElement.Instantiate<ArrayItem>();
            arrayItem.Container.AddChild((Control)inspector);
            AddChildNode(inspector, arrayItem, PropertyInfo.ArrayElementTypeInfo);
        }
    }
}