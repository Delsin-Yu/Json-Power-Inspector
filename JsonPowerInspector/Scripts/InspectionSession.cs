using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public class InspectionSession
{
    public InspectorSpawner InspectorSpawner { get; private set; }

    private readonly Dictionary<string, ObjectDefinition> _objectDefinitionMap;

    private readonly ObjectDefinition _mainObjectDefinition;
    
    public InspectionSession(
        PackedObjectDefinition packedObjectDefinition,
        InspectorSpawner inspectorSpawner
    )
    {
        _objectDefinitionMap = packedObjectDefinition.ReferencedObjectDefinition.ToDictionary(x => x.ObjectTypeName, x => x);
        _mainObjectDefinition = packedObjectDefinition.MainObjectDefinition;
        InspectorSpawner = inspectorSpawner;
    }

    public ObjectDefinition LookupObject(string objectName) => 
        _objectDefinitionMap[objectName];

    public void StartSession(Label objectName, Control rootObjectContainer)
    {
        objectName.Text = _mainObjectDefinition.ObjectTypeName;
        foreach (var propertyInfo in _mainObjectDefinition.Properties.AsSpan())
        {
            rootObjectContainer.AddChild(ObjectInspector.CreateInspectorForProperty(propertyInfo));
        }
    }
}