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
        // TODO: Warn User of data loss
        _editingJsonObject = jsonObject;
        
    }
}