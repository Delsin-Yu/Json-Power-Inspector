using Godot;

namespace JsonPowerInspector;

public partial class DictionaryItem : Control
{
    [Export] private Button _removeElement;
    [Export] private Label _key;
    [Export] private Control _valueContainer;
    
    public string KeyName
    {
        set => _key.Text = value;
    }

    public Control Container => _valueContainer;
}