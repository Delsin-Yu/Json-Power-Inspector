using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public interface IPropertyInspector
{
    string DisplayName { set; }
    string BackingPropertyName { set; }
    void BindJsonNode(JsonNode parent, string propertyName);
    event Action<JsonValue> ValueChanged;
}

public abstract partial class BasePropertyInspector<TPropertyInfo> : Control, IPropertyInspector where TPropertyInfo : BaseObjectPropertyInfo
{
    [Export] private Label _propertyName;

    public string DisplayName
    {
        set
        {
            _displayName = value;
            _propertyName.Text = value;
        }
    }

    public string BackingPropertyName
    {
        set => _jsonPropertyName = value;
    }

    public event Action<JsonValue> ValueChanged;
    protected TPropertyInfo PropertyInfo { get; private set; }
    protected InspectionSessionController CurrentSession { get; private set; }
    
    private JsonNode _parent;
    private string _jsonPropertyName;
    private string _displayName;
    private bool _affectMainObject;

    public void Initialize(TPropertyInfo propertyInfo, InspectionSessionController currentSession, bool affectMainObject)
    {
        CurrentSession = currentSession;
        DisplayName = propertyInfo.DisplayName;
        PropertyInfo = propertyInfo;
        _affectMainObject = affectMainObject;
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
        SetBackingNode(newNode);
    }
    
    protected JsonNode GetBackingNode() =>
        _parent switch
        {
            JsonArray jsonArray => jsonArray[int.Parse(_jsonPropertyName)],
            JsonObject jsonObject => jsonObject[_jsonPropertyName],
            _ => throw new InvalidOperationException(_parent.GetType().Name)
        };

    protected void SetBackingNode(JsonNode jsonNode)
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
        if(_affectMainObject) CurrentSession.MarkChanged();
    }

    protected void ReplaceValue(JsonValue value)
    {
        var node = GetBackingNode();
        if (node is not JsonValue) throw new InvalidOperationException($"{node.GetValueKind()} is not JsonValue!");
        switch (node.Parent)
        {
            case JsonObject jsonObject:
                jsonObject[node.GetPropertyName()] = value;
                break;
            case JsonArray jsonArray:
                jsonArray[node.GetElementIndex()] = value;
                break;
            default:
                throw new InvalidOperationException(node.Parent?.GetType().Name);
        }
        if(_affectMainObject) CurrentSession.MarkChanged();
        ValueChanged?.Invoke(value);
    }
    
    protected virtual void Bind(ref JsonNode node) { }
}