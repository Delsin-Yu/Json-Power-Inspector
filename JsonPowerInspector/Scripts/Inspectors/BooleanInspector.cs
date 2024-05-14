using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class BooleanInspector : BasePropertyInspector<BooleanPropertyInfo>
{
    [Export] private CheckBox _contentControl;

    protected override void Bind(ref JsonNode node)
    {
        var jsonValue = node.AsValue();
        _contentControl.ButtonPressed = jsonValue.GetValue<bool>();
        _contentControl.Toggled += ReplaceValue;
    }
}