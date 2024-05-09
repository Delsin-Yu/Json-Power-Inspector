using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public interface IPropertyInspector
{
    string PropertyPath { get; }
    string DisplayName { get; }
    void Bind(JsonNode jsonNode);
}

public abstract partial class BasePropertyInspector<TPropertyInfo> : Control, IPropertyInspector where TPropertyInfo : BaseObjectPropertyInfo
{
    [Export] private Label _propertyName;

    public string PropertyPath { get; set; }
    public string DisplayName { get; private set; }
    
    protected TPropertyInfo PropertyInfo { get; private set; }

    public void Initialize(TPropertyInfo propertyInfo, string propertyPath)
    {
        DisplayName = propertyInfo.Name;
        _propertyName.Text = DisplayName;
        PropertyPath = propertyPath;
        PropertyInfo = propertyInfo;
        OnInitialize(propertyInfo);
    }
    
    protected abstract void OnInitialize(TPropertyInfo propertyInfo);

    public abstract void Bind(JsonNode node);
}