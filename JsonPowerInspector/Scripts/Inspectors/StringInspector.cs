using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class StringInspector : BasePropertyInspector<StringPropertyInfo>
{
    [Export] private LineEdit _contentControl;

    protected override void OnInitialize(StringPropertyInfo propertyInfo)
    {
        
    }
}