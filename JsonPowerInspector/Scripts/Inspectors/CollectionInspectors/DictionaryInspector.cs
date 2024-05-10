﻿using System;
using System.Linq;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class DictionaryInspector : CollectionInspector<DictionaryPropertyInfo>
{
    [Export] private Button _addElement;
    [Export] private SpinBox _dictionaryElementCount;
    [Export] private PackedScene _dictionaryElement;

    protected override void OnFoldUpdate(bool shown) => _addElement.Visible = shown;
    
    protected override void OnBind(ref JsonNode node)
    {
        node ??= new JsonObject();
        _dictionaryElementCount.Value = node.AsObject().Count;
    }
    
    protected override void OnInitialPrint(JsonNode node)
    {
        var jsonObject = node.AsObject();
        var jsonObjectArray = jsonObject.ToArray().AsSpan();
        var spawner = Main.CurrentSession.InspectorSpawner;
        for (var index = 0; index < jsonObjectArray.Length; index++)
        {
            var jsonArrayElement = jsonObjectArray[index];

            var inspector = Utils.CreateInspectorForProperty(
                PropertyInfo.ValueTypeInfo,
                spawner,
                $"{PropertyPath}.{PropertyInfo.Name}.{index}"
            );
            var newElement = jsonArrayElement.Value;
            inspector.Bind(ref newElement);
            if (newElement != jsonArrayElement.Value)
            {
                jsonObject[jsonArrayElement.Key] = newElement;
            }
            var dictionaryItem = _dictionaryElement.Instantiate<DictionaryItem>();
            dictionaryItem.KeyName = jsonArrayElement.Key;
            dictionaryItem.Container.AddChild((Control)inspector);
            AddChildNode(inspector, dictionaryItem, PropertyInfo.KeyTypeInfo);
        }
    }
}