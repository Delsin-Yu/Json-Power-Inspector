using System;
using System.Collections.Generic;
using System.Globalization;
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


    protected override void OnPostInitialize(DictionaryPropertyInfo propertyInfo)
    {
        _dictionaryElementCount.Editable = false;
        _addElement.Pressed += async () =>
        {
            var keyInputWindow = _keyInputWindow.Instantiate<KeyInputWindow>();
            var rootContentScaleFactor = GetTree().Root.ContentScaleFactor;
            keyInputWindow.ContentScaleFactor = rootContentScaleFactor;
            AddChild(keyInputWindow);
            string selectedKey;
            var spawner = Main.CurrentSession.InspectorSpawner;
            var jsonObject = GetBackingNode().AsObject();
            switch (PropertyInfo.KeyTypeInfo)
            {
                case NumberPropertyInfo numberPropertyInfo:
                    var numberKeysInDictionary = new HashSet<double>();
                    foreach (var (key, _) in jsonObject) numberKeysInDictionary.Add(double.Parse(key));
                    var (hasValue, numberKey) = await keyInputWindow.ShowAsync(numberPropertyInfo, spawner, numberKeysInDictionary);
                    if(!hasValue) return;
                    selectedKey = numberKey.ToString("N3", CultureInfo.InvariantCulture);
                    break;
                case StringPropertyInfo stringPropertyInfo:
                    var stringKeysInDictionary = new HashSet<string>();
                    foreach (var (key, _) in jsonObject) stringKeysInDictionary.Add(key);
                    (hasValue, var stringKey) = await keyInputWindow.ShowAsync(stringPropertyInfo, spawner, stringKeysInDictionary);
                    if(!hasValue) return;
                    selectedKey = stringKey;
                    break;
                case DropdownPropertyInfo dropdownPropertyInfo:
                    switch (dropdownPropertyInfo.Kind)
                    {
                        case DropdownPropertyInfo.DropdownKind.Int:
                            var intKeysInDictionary = new HashSet<int>();
                            foreach (var (key, _) in jsonObject) intKeysInDictionary.Add(int.Parse(key));
                            (hasValue, var intKey) = await keyInputWindow.ShowAsyncInt(dropdownPropertyInfo, spawner, intKeysInDictionary);
                            if(!hasValue) return;
                            selectedKey = intKey.ToString();
                            break;
                        case DropdownPropertyInfo.DropdownKind.Float:
                            numberKeysInDictionary = [];
                            foreach (var (key, _) in jsonObject) numberKeysInDictionary.Add(double.Parse(key));
                            (hasValue, numberKey) = await keyInputWindow.ShowAsyncFloat(dropdownPropertyInfo, spawner, numberKeysInDictionary);
                            if(!hasValue) return;
                            selectedKey = numberKey.ToString("N3", CultureInfo.InvariantCulture);
                            break;
                        case DropdownPropertyInfo.DropdownKind.String:
                            stringKeysInDictionary = [];
                            foreach (var (key, _) in jsonObject) stringKeysInDictionary.Add(key);
                            (hasValue, stringKey) = await keyInputWindow.ShowAsyncString(dropdownPropertyInfo, spawner, stringKeysInDictionary);
                            if(!hasValue) return;
                            selectedKey = stringKey;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                default:
                    throw new InvalidOperationException(PropertyInfo.KeyTypeInfo.GetType().Name);
            }
            var newNode = Utils.CreateDefaultJsonObjectForProperty(PropertyInfo.ValueTypeInfo);
            jsonObject.Add(selectedKey, newNode);
            BindDictionaryItem(spawner, selectedKey, jsonObject);
            _dictionaryElementCount.Value++;
        };
    }

    protected override void Bind(ref JsonNode node)
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

            BindDictionaryItem(spawner, jsonArrayElement.Key, jsonObject);
        }
    }

    private void BindDictionaryItem(InspectorSpawner spawner, string key, JsonObject jsonObject)
    {
        var inspector = Utils.CreateInspectorForProperty(
            PropertyInfo.ValueTypeInfo,
            spawner
        );
        inspector.BindJsonNode(jsonObject, key);
        var dictionaryItem = _dictionaryElement.Instantiate<DictionaryItem>();
        dictionaryItem.Initialize(
            key,
            inspector,
            () =>
            {
                DeleteChildNode(inspector);
                jsonObject.Remove(key);
                _dictionaryElementCount.Value--;
            }
        );
        AddChildNode(inspector, dictionaryItem);
    }
}