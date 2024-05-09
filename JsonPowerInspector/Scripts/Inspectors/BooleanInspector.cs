using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class BooleanInspector : BasePropertyInspector<BooleanPropertyInfo>
{
    [Export] private CheckBox _contentControl;

    protected override void OnInitialize(BooleanPropertyInfo propertyInfo)
    {
    }

    public override void Bind(JsonNode node)
    {
        _contentControl.ButtonPressed = node.AsValue().GetValue<bool>();
    }
}