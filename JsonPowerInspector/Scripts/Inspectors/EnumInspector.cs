using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class EnumInspector : BasePropertyInspector<EnumPropertyInfo>
{
    [Export] private OptionButton _contentControl;

    protected override void OnInitialize(EnumPropertyInfo propertyInfo)
    {
        _contentControl.Clear();
        
        // TODO: Flag support
        
        foreach (var enumValue in propertyInfo.EnumValues)
        {
            _contentControl.AddItem(enumValue.ValueName);
        }

        _contentControl.Selected = -1;
    }
}