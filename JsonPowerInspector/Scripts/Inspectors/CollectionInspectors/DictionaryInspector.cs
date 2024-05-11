using System;
using System.Collections.Generic;
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
    [Export] private PackedScene _keyInputWindow;

    protected override void OnFoldUpdate(bool shown) => _addElement.Visible = shown;
    
    protected override void Bind(ref JsonNode node)
    {
        node ??= new JsonObject();
        _dictionaryElementCount.Value = node.AsObject().Count;
        
        _addElement.Pressed += async () =>
        {
            var keyInputWindow = _keyInputWindow.Instantiate<KeyInputWindow>();
            AddChild(keyInputWindow);
            string selectedKey;
            var spawner = Main.CurrentSession.InspectorSpawner;
            switch (PropertyInfo.KeyTypeInfo)
            {
                case NumberPropertyInfo numberPropertyInfo:
                    var numberKey = await keyInputWindow.ShowAsync(numberPropertyInfo, spawner);
                    if(numberKey is null) return;
                    selectedKey = numberKey.ToString();
                    break;
                case StringPropertyInfo stringPropertyInfo:
                    var stringKey = await keyInputWindow.ShowAsync(stringPropertyInfo, spawner);
                    if(stringKey is null) return;
                    selectedKey = stringKey;
                    break;
                default:
                    throw new InvalidOperationException(PropertyInfo.KeyTypeInfo.GetType().Name);
            }
            var jsonObject = (JsonObject)BackingNode;
            var newNode = Utils.CreateDefaultJsonObjectForProperty(PropertyInfo.ValueTypeInfo);
            jsonObject.Add(selectedKey, newNode);
            BindDictionaryItem(spawner, selectedKey, newNode, jsonObject);
        };
    }
    
    protected override void OnInitialPrint(JsonNode node)
    {
        var jsonObject = node.AsObject();
        var jsonObjectArray = jsonObject.ToArray().AsSpan();
        var spawner = Main.CurrentSession.InspectorSpawner;
        for (var index = 0; index < jsonObjectArray.Length; index++)
        {
            var jsonArrayElement = jsonObjectArray[index];

            BindDictionaryItem(spawner, jsonArrayElement.Key, jsonArrayElement.Value, jsonObject);
        }

    }

    private void BindDictionaryItem(InspectorSpawner spawner, string key, JsonNode value, JsonObject jsonObject)
    {
        var inspector = Utils.CreateInspectorForProperty(
            PropertyInfo.ValueTypeInfo,
            spawner
        );
        var newElement = value;
        inspector.BindJsonNode(ref newElement);
        if (newElement != value)
        {
            jsonObject[key] = newElement;
        }
        var dictionaryItem = _dictionaryElement.Instantiate<DictionaryItem>();
        dictionaryItem.KeyName = key;
        dictionaryItem.Container.AddChild((Control)inspector);
        AddChildNode(inspector, dictionaryItem, PropertyInfo.KeyTypeInfo);
    }
}