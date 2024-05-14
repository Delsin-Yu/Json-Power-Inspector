using System;
using Godot;

namespace JsonPowerInspector;

public partial class DictionaryItem : Control
{
    [Export] private Button _removeElement;
    [Export] private Label _key;
    [Export] private Control _valueContainer;

    private Action _deleteCurrentElementCall;
    
    public override void _Ready()
    {
        _removeElement.Pressed += RemoveCurrentElement;
    }

    public void Initialize(string keyName, IPropertyInspector inspector, Action deleteCall)
    {
        _deleteCurrentElementCall = deleteCall;
        _key.Text = keyName;
        _valueContainer.AddChild((Control)inspector);
    }

    private void RemoveCurrentElement()
    {
        _deleteCurrentElementCall();
        _deleteCurrentElementCall = null;
    }
}