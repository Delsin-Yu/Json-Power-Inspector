using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class StringInspector : BasePropertyInspector<StringPropertyInfo>
{
    [Export] private LineEdit _contentControl;
    
    protected override void Bind(ref JsonNode node)
    {
        var jsonValue = node.AsValue();
        _contentControl.Text = jsonValue.GetValue<string>();
        _contentControl.TextChanged += value => ReplaceValue(JsonValue.Create(value));
    }
}