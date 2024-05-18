using System.Collections.Generic;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class EnumInspector : BasePropertyInspector<EnumPropertyInfo>
{
    [Export] private OptionButton _contentControl;

    private readonly List<EnumPropertyInfo.EnumValue> _selections = [];
    
    protected override void OnInitialize(EnumPropertyInfo propertyInfo)
    {
        _selections.Clear();
        _contentControl.Clear();
        
        // TODO: Flag support
        
        foreach (var enumValue in propertyInfo.EnumValues)
        {
            _contentControl.AddItem(enumValue.DisplayName);
            _selections.Add(enumValue);
        }

        _contentControl.Selected = -1;
    }

    protected override void Bind(ref JsonNode node)
    {
        var jsonValue = node.AsValue();
        _contentControl.Selected = _selections.FindIndex(x => x.DeclareName == jsonValue.GetValue<string>());
        _contentControl.ItemSelected += index => ReplaceValue(JsonValue.Create(_selections[(int)index].DeclareName));
    }
}