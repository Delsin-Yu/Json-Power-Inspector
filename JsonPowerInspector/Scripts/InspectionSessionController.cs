using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class InspectionSessionController : Control
{
    [Export] private Control _container;
    [Export] private Label _info;
    
    public InspectorSpawner InspectorSpawner { get; private set; }
    private readonly List<IPropertyInspector> _inspectorRoot = [];
    public string TemplateDirectory { get; set; }
    public IReadOnlyDictionary<string, ObjectDefinition> ObjectDefinitionMap => _objectDefinitionMap;

    private Dictionary<string, ObjectDefinition> _objectDefinitionMap;
    private ObjectDefinition _mainObjectDefinition;
    private JsonObject _editingJsonObject;
    private string _templatePath;
    private string _dataPath;

    public void StartSession(
        InspectorSpawner inspectorSpawner,
        string templatePath,
        string dataPath
    )
    {
        _templatePath = templatePath;
        
        // TODO: Serialization Exception Handling
        var setup = TemplateSerializer.Deserialize(templatePath);
        
        _objectDefinitionMap = setup.ReferencedObjectDefinition.ToDictionary(x => x.ObjectTypeName, x => x);
        _mainObjectDefinition = setup.MainObjectDefinition;
        InspectorSpawner = inspectorSpawner;
        TemplateDirectory = Path.GetDirectoryName(templatePath)!;
        Name = setup.MainObjectDefinition.ObjectTypeName;
        
        if (dataPath != null)
        {
            LoadFromJsonObject(dataPath);
            return;
        }

        JsonObject templateJsonObject = new();
        foreach (var propertyInfo in _mainObjectDefinition.Properties.AsSpan())
        {
            var propertyPath = new HashSet<BaseObjectPropertyInfo>();
            templateJsonObject.Add(propertyInfo.Name, Utils.CreateJsonObjectForProperty(propertyInfo, _objectDefinitionMap, propertyPath));
        }

        MountJsonObject(templateJsonObject);
    }

    public void LoadFromJsonObject(string dataPath)
    {
        _dataPath = dataPath;
        JsonObject jsonObject;
        {
            using var fileStream = File.OpenRead(dataPath);
            jsonObject = (JsonObject)JsonObject.Parse(fileStream);
        }
        MountJsonObject(jsonObject);
    }

    private void MountJsonObject(JsonObject jsonObject)
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
            _container.AddChild((Control)inspectorForProperty);
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

        _info.Text =
            $"""
             Current Template:
             "{_templatePath}"
             Current Data:
             "{_dataPath ?? "New Data"}"
             """;
    }
}