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

    private readonly List<IPropertyInspector> _inspectorRoot = [];

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
            var inspectorForProperty = Utils.CreateInspectorForProperty(propertyInfo, InspectorSpawner);
            _inspectorRoot.Add(inspectorForProperty);
            rootObjectContainer.AddChild((Control)inspectorForProperty);
            var propertyPath = new HashSet<BaseObjectPropertyInfo>();
            _templateJsonObject.Add(propertyInfo.Name, Utils.CreateJsonObjectForProperty(propertyInfo, _objectDefinitionMap, propertyPath));
        }

        LoadFromJsonObject(jsonObject ?? _templateJsonObject.DeepClone().AsObject());
    }
    
    public void LoadFromJsonObject(JsonObject jsonObject)
    {
        // TODO: Warn User of data loss
        _editingJsonObject = jsonObject;
        var objectProperty = _editingJsonObject.ToArray();
        for (var index = 0; index < _inspectorRoot.Count; index++)
        {
            var jsonNode = objectProperty[index].Value;
            var newNode = jsonNode;
            var propertyInspector = _inspectorRoot[index];
            propertyInspector.BindJsonNode(ref newNode);
            if (newNode != jsonNode)
            {
                _editingJsonObject[index] = newNode;
            }
        }
    }
}