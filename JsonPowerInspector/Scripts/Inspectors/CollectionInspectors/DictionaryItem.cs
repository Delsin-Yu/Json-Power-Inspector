using Godot;

namespace JsonPowerInspector;

public partial class DictionaryItem : Control
{
    [Export] private Button _removeElement;
    [Export] private Control _keyContainer;
    [Export] private Control _valueContainer;
}