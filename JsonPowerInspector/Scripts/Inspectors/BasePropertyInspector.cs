using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public abstract partial class BasePropertyInspector<TPropertyInfo> : Control where TPropertyInfo : BaseObjectPropertyInfo
{
    [Export] private Label _propertyName;

    public void Initialize(TPropertyInfo propertyInfo)
    {
        _propertyName.Text = propertyInfo.Name;
        OnInitialize(propertyInfo);
    }

    protected abstract void OnInitialize(TPropertyInfo propertyInfo);
}