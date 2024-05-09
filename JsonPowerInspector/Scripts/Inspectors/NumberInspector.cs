using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class NumberInspector : BasePropertyInspector<NumberPropertyInfo>
{
    [Export] private SpinBox _contentControl;

    protected override void OnInitialize(NumberPropertyInfo propertyInfo)
    {
        _contentControl.Step = propertyInfo.NumberKind is NumberPropertyInfo.NumberType.Int ? 1 : 0.001;
        _contentControl.MinValue = propertyInfo.Range.Lower;
        _contentControl.MaxValue = propertyInfo.Range.Upper;
    }
}