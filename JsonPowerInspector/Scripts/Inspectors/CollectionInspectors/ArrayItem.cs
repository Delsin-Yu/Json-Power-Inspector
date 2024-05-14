using System;
using Godot;

namespace JsonPowerInspector;

public partial class ArrayItem : Control
{
    [Export] private Button _removeElement;
    [Export] private Control _elementContainer;

    private Action<IPropertyInspector, int> _deleteCurrentElementCall;
    private IPropertyInspector _inspector;

    public int Index { get; set; }
    
    public override void _Ready()
    {
        _removeElement.Pressed += RemoveCurrentElement;
    }

    public void Initialize(IPropertyInspector inspector, Action<IPropertyInspector, int> deleteCall)
    {
        _inspector = inspector;
        _deleteCurrentElementCall = deleteCall;
        _elementContainer.AddChild((Control)inspector);
    }

    private void RemoveCurrentElement()
    {
        _deleteCurrentElementCall(_inspector, Index);
        _deleteCurrentElementCall = null;
    }
}