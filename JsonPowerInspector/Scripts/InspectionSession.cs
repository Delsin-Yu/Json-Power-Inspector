using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public class InspectionSession
{
    public InspectorSpawner InspectorSpawner { get; private set; }
    private readonly Dictionary<string, ObjectDefinition> _objectDefinitionMap;

    private readonly ObjectDefinition _mainObjectDefinition;

    private readonly Control _rootObjectContainer;
    private JsonObject _editingJsonObject;

    private readonly List<IPropertyInspector> _inspectorRoot = [];
    public string TemplateDirectory { get; set; }
    public IReadOnlyDictionary<string, ObjectDefinition> ObjectDefinitionMap => _objectDefinitionMap;

    public InspectionSession(
        PackedObjectDefinition packedObjectDefinition,
        InspectorSpawner inspectorSpawner,
        string templatePath,
        Label objectName,
        Control rootObjectContainer
    )
    {
        _objectDefinitionMap = packedObjectDefinition.ReferencedObjectDefinition.ToDictionary(x => x.ObjectTypeName, x => x);
        _mainObjectDefinition = packedObjectDefinition.MainObjectDefinition;
        InspectorSpawner = inspectorSpawner;
        TemplateDirectory = Path.GetDirectoryName(templatePath)!;
        objectName.Text = _mainObjectDefinition.ObjectTypeName;
        _rootObjectContainer = rootObjectContainer;

    }

    public void StartSession(JsonObject jsonObject)
    {
        
        if (jsonObject != null)
        {
            LoadFromJsonObject(jsonObject);
            return;
        }

        JsonObject templateJsonObject = new();
        foreach (var propertyInfo in _mainObjectDefinition.Properties.AsSpan())
        {
            var propertyPath = new HashSet<BaseObjectPropertyInfo>();
            templateJsonObject.Add(propertyInfo.Name, Utils.CreateJsonObjectForProperty(propertyInfo, _objectDefinitionMap, propertyPath));
        }

        LoadFromJsonObject(templateJsonObject);
    }


    public void LoadFromJsonObject(JsonObject jsonObject)
    {
        foreach (var inspector in CollectionsMarshal.AsSpan(_inspectorRoot))
        {
            ((Control)inspector).QueueFree();
        }
        
        _inspectorRoot.Clear();
        
        foreach (var propertyInfo in _mainObjectDefinition.Properties.AsSpan())
        {
            var inspectorForProperty = Utils.CreateInspectorForProperty(propertyInfo, InspectorSpawner);
            _inspectorRoot.Add(inspectorForProperty);
            _rootObjectContainer.AddChild((Control)inspectorForProperty);
        }
        
        // TODO: Warn User of data loss
        _editingJsonObject = jsonObject;
        var objectProperty = _editingJsonObject.ToArray();
        for (var index = 0; index < _inspectorRoot.Count; index++)
        {
            var jsonNode = objectProperty[index].Key;
            var propertyInspector = _inspectorRoot[index];
            propertyInspector.BindJsonNode(_editingJsonObject, jsonNode);
        }
    }
}