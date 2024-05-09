using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public abstract partial class CollectionInspector<TPropertyInfo> : BasePropertyInspector<TPropertyInfo> where TPropertyInfo : BaseObjectPropertyInfo
{
    [Export] private Control _contentControl;
    [Export] private Control _contentPanel;
    [Export] private CheckButton _foldout;

    protected Control Container => _contentControl;

    protected override void OnInitialize(TPropertyInfo propertyInfo)
    {
        _foldout.Toggled += on => _contentPanel.Visible = on;
        _foldout.ButtonPressed = false;
        _contentPanel.Visible = false;
        OnPostInitialize(propertyInfo);
    }

    protected abstract void OnPostInitialize(TPropertyInfo propertyInfo);
}