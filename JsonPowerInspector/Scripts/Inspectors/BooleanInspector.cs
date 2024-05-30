using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class BooleanInspector : BasePropertyInspector<BooleanPropertyInfo>
{
    [Export] private CheckBox _contentControl;
    [Export] private Button _createOrDeleteBtn;

    protected override void OnInitialize(BooleanPropertyInfo propertyInfo)
    {
        _createOrDeleteBtn.Visible = propertyInfo.Nullable;
        if (propertyInfo.Nullable)
        {
            _createOrDeleteBtn.Pressed += () =>
            {
                JsonNode jsonNode = GetBackingNode() == null ? false : null;
                SetBackingNode(jsonNode);
                Bind(ref jsonNode);
                CurrentSession.MarkChanged();
            };
        }
        _contentControl.Toggled += value => ReplaceValue(JsonValue.Create(value));
    }

    protected override void Bind(ref JsonNode node)
    {
        if (node is null && !PropertyInfo.Nullable)
        {
            throw new JsonException("Supplying a null value on not nullable boolean property!");
        }
        
        if (node is null)
        {
            _createOrDeleteBtn.Text = "+";
            _contentControl.Hide();
        }
        else
        {
            _createOrDeleteBtn.Text = "X";
            _contentControl.ButtonPressed = node.AsValue().GetValue<bool>();
            _contentControl.Show();
        }
    }
}