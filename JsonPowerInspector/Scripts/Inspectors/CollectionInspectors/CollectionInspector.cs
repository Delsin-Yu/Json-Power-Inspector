using System.Collections.Generic;
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
    private readonly List<Node> _childrenNodes = [];

    protected IReadOnlyList<Node> ChildrenNodes => _childrenNodes;
    
    protected record struct Node(IPropertyInspector Inspector, BaseObjectPropertyInfo PropertyInfo);

    protected void AddChildNode(IPropertyInspector inspector, Control inspectorControl, BaseObjectPropertyInfo propertyInfo)
    {
        _childrenNodes.Add(new(inspector, propertyInfo));
        _contentControl.AddChild(inspectorControl);
        _emptyIndicator.Hide();
    }

    protected void RemoveNode(int index)
    {
        _childrenNodes.RemoveAt(index);
        if(_childrenNodes.Count == 0) _emptyIndicator.Show();
    }

    protected sealed override void OnInitialize(TPropertyInfo propertyInfo)
    {
        _foldout.Toggled += on =>
        {
            if (!_created)
            {
                _created = true;
                OnInitialPrint(BackingNode);
            }
            _contentPanel.Visible = on;
            OnFoldUpdate(on);
        };
        _foldout.ButtonPressed = false;
        _contentPanel.Visible = false;
        OnFoldUpdate(false);
        OnPostInitialize(propertyInfo);
    }

    protected virtual void OnFoldUpdate(bool shown) { }

    protected abstract void OnInitialPrint(JsonNode node);

    protected virtual void OnPostInitialize(TPropertyInfo propertyInfo) { }
}