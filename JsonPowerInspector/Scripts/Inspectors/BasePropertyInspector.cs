using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public interface IPropertyInspector
{
    string DisplayName { get; }
    void BindJsonNode(ref JsonNode jsonNode);
}

public abstract partial class BasePropertyInspector<TPropertyInfo> : Control, IPropertyInspector where TPropertyInfo : BaseObjectPropertyInfo
{
    [Export] private Label _propertyName;

    public string DisplayName { get; private set; }
    
    protected TPropertyInfo PropertyInfo { get; private set; }
    protected JsonNode BackingNode { get; private set; }

    public void Initialize(TPropertyInfo propertyInfo)
    {
        DisplayName = propertyInfo.Name;
        _propertyName.Text = DisplayName;
        PropertyInfo = propertyInfo;
        OnInitialize(propertyInfo);
    }

    protected virtual void OnInitialize(TPropertyInfo propertyInfo) { }

    public void BindJsonNode(ref JsonNode node)
    {
        Bind(ref node);
        BackingNode = node;
    }
    
    protected virtual void Bind(ref JsonNode node) { }
}