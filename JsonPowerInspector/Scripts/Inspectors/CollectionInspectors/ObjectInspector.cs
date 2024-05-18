using System;
using System.Linq;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class ObjectInspector : CollectionInspector<ObjectPropertyInfo>
{
    [Export] private Button _createOrDeleteBtn;

    protected override bool DisplayChildObjectByDefault => false;


    protected override void OnPostInitialize()
    {
        _createOrDeleteBtn.Pressed += () =>
        {
            if (GetBackingNode() != null)
            {
                SetBackingNode(null);
                CleanChildNode();
                _createOrDeleteBtn.Text = "+";
            }
            else
            {
                var jsonObject = CreateDefaultJsonObject(PropertyInfo);
                SetBackingNode(jsonObject);
                BindObject(jsonObject);
                _createOrDeleteBtn.Text = "X";
            }
            CurrentSession.MarkChanged();
        };
    }

    private JsonObject CreateDefaultJsonObject(ObjectPropertyInfo objectPropertyInfo)
    {
        var definition = CurrentSession.ObjectDefinitionMap[objectPropertyInfo.ObjectTypeName];
        var jsonObject = new JsonObject();
        foreach (var propertyInfo in definition.Properties)
        {
            jsonObject.Add(propertyInfo.Name, Utils.CreateDefaultJsonObjectForProperty(propertyInfo));
        }

        return jsonObject;
    }
    
    protected override void OnInitialPrint(JsonNode node)
    {
        if (node == null)
        {
            _createOrDeleteBtn.Text = "+";
        }
        else
        {
            BindObject(node);
            _createOrDeleteBtn.Text = "X";
        }
    }

    protected override void Bind(ref JsonNode node)
    {
        _createOrDeleteBtn.Text = node != null ? "X" : "+";
    }

    private void BindObject(JsonNode node)
    {
        var objectDefinition = CurrentSession.ObjectDefinitionMap[PropertyInfo.ObjectTypeName];
        var span = objectDefinition.Properties.AsSpan();
        var jsonObject = node.AsObject();
        var jsonProperties = jsonObject.ToArray();
        for (var index = 0; index < span.Length; index++)
        {
            var info = span[index];
            var inspector = Utils.CreateInspectorForProperty(info, CurrentSession.InspectorSpawner, true);
            AddChildNode(inspector, (Control)inspector);
            var propertyName = jsonProperties[index].Key;
            inspector.BindJsonNode(jsonObject, propertyName);
        }
    }
}