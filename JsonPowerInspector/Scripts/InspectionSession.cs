using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public class InspectionSession
{
    public InspectorSpawner InspectorSpawner { get; private set; }

    private readonly Dictionary<string, ObjectDefinition> _objectDefinitionMap;

    private readonly ObjectDefinition _mainObjectDefinition;

    private JsonObject _editingJsonObject;
    private JsonObject _templateJsonObject;

    public InspectionSession(
        PackedObjectDefinition packedObjectDefinition,
        InspectorSpawner inspectorSpawner
    )
    {
        _objectDefinitionMap = packedObjectDefinition.ReferencedObjectDefinition.ToDictionary(x => x.ObjectTypeName, x => x);
        _mainObjectDefinition = packedObjectDefinition.MainObjectDefinition;
        InspectorSpawner = inspectorSpawner;
    }

    public ObjectDefinition LookupObject(string objectName) => _objectDefinitionMap[objectName];

    public void StartSession(Label objectName, Control rootObjectContainer, JsonObject jsonObject)
    {
        objectName.Text = _mainObjectDefinition.ObjectTypeName;

        _templateJsonObject = new();
        foreach (var propertyInfo in _mainObjectDefinition.Properties.AsSpan())
        {
            rootObjectContainer.AddChild(Utils.CreateInspectorForProperty(propertyInfo, InspectorSpawner));
            _templateJsonObject.Add(propertyInfo.Name, Utils.CreateJsonObjectForProperty(propertyInfo, _objectDefinitionMap));
        }

        LoadFromJsonObject(jsonObject ?? _templateJsonObject.DeepClone().AsObject());
    }

    public void LoadFromJsonObject(JsonObject jsonObject)
    {
        if (!ObjectEquals(jsonObject, _templateJsonObject, out var reason))
        {
            GD.PrintErr($"""
                        The provided json object does not equals to template:
                        
                        Reason:
                        {reason}
                        
                        Template:
                        {_templateJsonObject}
                        
                        Data:
                        {jsonObject}
                        """);
            return;
        }
        
        // TODO: Warn User of data loss
        _editingJsonObject = jsonObject;
        
    }

    private static bool ObjectEquals(JsonObject a, JsonObject b, out string reason)
    {
        if (a.Count != b.Count)
        {
            reason = "[JsonObject] Children Count Differs";
            return false;
        }
        foreach (var ((aKey, aValue), (bKey, bValue)) in a.OrderBy(x => x.Key).Zip(b.OrderBy(x => x.Key), (x, y) => (x, y)))
        {
            if (aKey != bKey)
            {
                reason = $"[JsonObject] Key Differs: {aKey} vs {bKey}";
                return false;
            }

            if (aValue!.GetValueKind() != bValue!.GetValueKind())
            {
                reason = $"[JsonObject] ValueKind Differs: {aValue!.GetValueKind()} vs {bValue!.GetValueKind()}";
                return false;
            }
            switch (aValue)
            {
                case JsonObject aJsonObject:
                    var bJsonObject = (JsonObject)bValue;
                    if (!ObjectEquals(aJsonObject, bJsonObject, out reason)) return false;
                    break;
                case JsonArray aJsonArray:
                    var bJsonArray = (JsonArray)bValue;
                    if (!ArrayEquals(aJsonArray, bJsonArray, out reason)) return false;
                    break;
                case JsonValue:
                    break;
                default:
                    reason = $"[JsonObject] Unsupported child type: {aValue.GetType()}";
                    return false;
            }
        }

        reason = null;
        return true;
    }

    private static bool ArrayEquals(JsonArray a, JsonArray b, out string reason)
    {
        if (a.Count == 0 || b.Count == 0)
        {
            reason = null;
            return true;
        }

        var aGroup = a.GroupBy(x => x.GetValueKind()).ToArray();
        var bGroup = b.GroupBy(x => x.GetValueKind()).ToArray();
        if (aGroup.Length != bGroup.Length)
        {
            reason = $"[ArrayObject] Types contains differs, {aGroup.Length} in A where {bGroup.Length} in B";
            return false;
        }

        if (aGroup.Length == 1)
        {
            if (aGroup[0].Key == bGroup[0].Key)
            {
                reason = null;
                return true;
            }

            reason = $"[ArrayObject] Type in array differs, {aGroup[0].Key} in A where {bGroup[0].Key} in B";
            return false;
        }

        var aValueKinds = a.Select(x => x.GetValueKind()).ToArray();
        var bValueKinds = b.Select(x => x.GetValueKind()).ToArray();
        var success = aValueKinds.SequenceEqual(bValueKinds);
        if (!success)
        {
            reason = $"[ArrayObject] Type sequences occurs in array differs: \n" +
                     $"A: {string.Join(", ", aValueKinds)}" +
                     $"B: {string.Join(", ", bValueKinds)}";
            return false;
        }

        reason = null;
        return true;
    }
}