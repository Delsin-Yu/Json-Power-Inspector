using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class BooleanInspector : BasePropertyInspector<BooleanPropertyInfo>
{
    [Export] private CheckBox _contentControl;

    protected override void OnInitialize(BooleanPropertyInfo propertyInfo)
    {
    }
}