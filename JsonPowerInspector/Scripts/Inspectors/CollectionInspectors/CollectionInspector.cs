using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public abstract partial class CollectionInspector<TPropertyInfo> : BasePropertyInspector<TPropertyInfo> where TPropertyInfo : BaseObjectPropertyInfo
{
    [Export] private Control _contentControl;
    [Export] private Control _contentPanel;
    [Export] private Label _emptyIndicator;
    [Export] private CheckButton _foldout;
    
    private bool _created;

    protected virtual bool DisplayChildObjectByDefault => true;

    private readonly Dictionary<IPropertyInspector, Control> _controls = [];

    protected void CleanChildNode()
    {
        foreach (var control in _controls.Values)
        {
            control.QueueFree();
        }
        _controls.Clear();
        _emptyIndicator.Show();
    }
    
    protected void AddChildNode(IPropertyInspector inspector, Control inspectorControl)
    {
        _contentControl.AddChild(inspectorControl);
        _emptyIndicator.Hide();
        _controls.Add(inspector, inspectorControl);
        if(inspector is ObjectInspector objectInspector) objectInspector.ToggleFold(DisplayChildObjectByDefault);
    }

    protected void DeleteChildNode(IPropertyInspector inspector)
    {
        _controls.Remove(inspector, out var control);
        control!.QueueFree();
        if (_controls.Count == 0) _emptyIndicator.Show();
    }

    protected sealed override void OnInitialize(TPropertyInfo propertyInfo)
    {
        _foldout.Toggled += on =>
        {
            if (!_created)
            {
                _created = true;
                OnInitialPrint(GetBackingNode());
            }
            _contentPanel.Visible = on;
            OnFoldUpdate(on);
        };
        _foldout.ButtonPressed = false;
        _contentPanel.Visible = false;
        OnFoldUpdate(false);
        OnPostInitialize(propertyInfo);
    }

    public void ToggleFold(bool shown) => _foldout.ButtonPressed = shown;

    protected virtual void OnFoldUpdate(bool shown) { }

    protected abstract void OnInitialPrint(JsonNode node);

    protected virtual void OnPostInitialize(TPropertyInfo propertyInfo) { }
}