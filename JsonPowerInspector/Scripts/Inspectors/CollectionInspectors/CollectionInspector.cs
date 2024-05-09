using System.Collections.Generic;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public abstract partial class CollectionInspector<TPropertyInfo> : BasePropertyInspector<TPropertyInfo> where TPropertyInfo : BaseObjectPropertyInfo
{
    [Export] private Control _contentControl;
    [Export] private Control _contentPanel;
    [Export] private CheckButton _foldout;

    private readonly List<Node> _nodes = [];
    
    private record struct Node(Control Inspector, BaseObjectPropertyInfo PropertyInfo);
    
    protected void CreateInspector(BaseObjectPropertyInfo propertyInfo)
    {
        var inspector = Utils.CreateInspectorForProperty(propertyInfo, Main.CurrentSession.InspectorSpawner);
        _nodes.Add(new(inspector, propertyInfo));
        _contentControl.AddChild(inspector);
    }
    
    protected override void OnInitialize(TPropertyInfo propertyInfo)
    {
        _foldout.Toggled += on =>
        {
            _contentPanel.Visible = on;
            OnFoldUpdate(on);
        };
        _foldout.ButtonPressed = false;
        _contentPanel.Visible = false;
        OnFoldUpdate(false);
        OnPostInitialize(propertyInfo);
    }

    protected virtual void OnFoldUpdate(bool shown) { }

    protected abstract void OnPostInitialize(TPropertyInfo propertyInfo);
}