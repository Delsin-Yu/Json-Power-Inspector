using Godot;

namespace JsonPowerInspector;

public partial class ArrayItem : Control
{
    [Export] private Button _removeElement;
    [Export] private Control _elementContainer;

    public Control Container => _elementContainer;
}