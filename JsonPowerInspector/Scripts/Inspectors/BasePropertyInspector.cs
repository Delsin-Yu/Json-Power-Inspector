using System;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public interface IPropertyInspector
{
    string DisplayName { get; }
    void BindJsonNode(JsonNode parent, string propertyName);
}

public abstract partial class BasePropertyInspector<TPropertyInfo> : Control, IPropertyInspector where TPropertyInfo : BaseObjectPropertyInfo
{
    [Export] private Label _propertyName;

    public string DisplayName { get; private set; }
    protected TPropertyInfo PropertyInfo { get; private set; }

    private JsonNode _parent;
    private string _jsonPropertyName;
    
    public void Initialize(TPropertyInfo propertyInfo)
    {
        DisplayName = propertyInfo.Name;
        _propertyName.Text = DisplayName;
        PropertyInfo = propertyInfo;
        OnInitialize(propertyInfo);
    }

    protected virtual void OnInitialize(TPropertyInfo propertyInfo) { }

    public void BindJsonNode(JsonNode parent, string propertyName)
    {
        _parent = parent;
        _jsonPropertyName = propertyName;
        var node = GetBackingNode();
        var newNode = node;
        Bind(ref newNode);
        if (newNode == node) return;
        ReplaceBacking(newNode);
    }

    protected JsonNode GetBackingNode() =>
        _parent switch
        {
            JsonArray jsonArray => jsonArray[int.Parse(_jsonPropertyName)],
            JsonObject jsonObject => jsonObject[_jsonPropertyName],
            _ => throw new InvalidOperationException(_parent.GetType().Name)
        };

    private void ReplaceBacking(JsonNode jsonNode)
    {
        switch (_parent)
        {
            case JsonArray jsonArray:
                jsonArray[int.Parse(_jsonPropertyName)] = jsonNode;
                break;
            case JsonObject jsonObject:
                jsonObject[_jsonPropertyName] = jsonNode;
                break;
            default:
                throw new InvalidOperationException(_parent.GetType().Name);
        }
    }
    
    protected void ReplaceValue<TValue>(TValue value)
    {
        var node = GetBackingNode();
        if (node is not JsonValue jsonValue) throw new InvalidOperationException($"{node.GetValueKind()} is not JsonValue!");
        jsonValue.ReplaceWith(value);
    }
    
    protected virtual void Bind(ref JsonNode node) { }
}