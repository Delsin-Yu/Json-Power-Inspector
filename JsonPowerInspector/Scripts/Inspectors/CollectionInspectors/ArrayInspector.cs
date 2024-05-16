using System;
using System.Collections.Generic;
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

    private Action<IPropertyInspector, int> _removeCall;
    
    private readonly List<ArrayItem> _arrayItems = [];

    protected override void OnPostInitialize()
    {
        _removeCall = RemoveArrayElement;
        _arrayElementCount.Editable = false;

        _addElement.Pressed += () =>
        {
            var jsonArray = GetBackingNode().AsArray();
            var newNode = Utils.CreateDefaultJsonObjectForProperty(PropertyInfo.ArrayElementTypeInfo);
            jsonArray.Add(newNode);
            BindArrayItem(CurrentSession.InspectorSpawner, jsonArray.Count - 1, jsonArray);
            _arrayElementCount.Value++;
            CurrentSession.MarkChanged();
        };
    }

    protected override void Bind(ref JsonNode node)
    {
        node ??= new JsonArray();
        _arrayElementCount.Value = node.AsArray().Count;
    }

    protected override void OnInitialPrint(JsonNode node)
    {
        var jsonArray = node.AsArray();
        var spawner = CurrentSession.InspectorSpawner;
        for (var index = 0; index < jsonArray.Count; index++)
        {
            BindArrayItem(spawner, index, node);
        }
    }

    private void BindArrayItem(InspectorSpawner spawner, int index, JsonNode node)
    {
        var inspector = Utils.CreateInspectorForProperty(
            PropertyInfo.ArrayElementTypeInfo,
            spawner
        );
        inspector.BindJsonNode(node, index.ToString());
        var arrayItem = _arrayElement.Instantiate<ArrayItem>();
        arrayItem.Initialize(inspector, _removeCall);
        arrayItem.Index = index;
        AddChildNode(inspector, arrayItem);
        _arrayItems.Add(arrayItem);
    }

    private void RemoveArrayElement(IPropertyInspector inspector, int index)
    {
        DeleteChildNode(inspector);
        _arrayItems.RemoveAt(index);
        GetBackingNode().AsArray().RemoveAt(index);
        _arrayElementCount.Value--;
        CurrentSession.MarkChanged();
        for (var i = 0; i < _arrayItems.Count; i++)
        {
            _arrayItems[i].Index = i;
        }
    }
}