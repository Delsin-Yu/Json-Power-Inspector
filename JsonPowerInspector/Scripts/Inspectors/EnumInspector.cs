using System.Collections.Generic;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class EnumInspector : BasePropertyInspector<EnumPropertyInfo>
{
    [Export] private OptionButton _contentControl;

    private readonly List<string> _selections = [];
    
    protected override void OnInitialize(EnumPropertyInfo propertyInfo)
    {
        _selections.Clear();
        _contentControl.Clear();
        
        // TODO: Flag support
        
        foreach (var enumValue in propertyInfo.EnumValues)
        {
            _contentControl.AddItem(enumValue.ValueName);
            _selections.Add(enumValue.ValueName);
        }

        _contentControl.Selected = -1;
    }

    public override void Bind(JsonNode node)
    {
        _contentControl.Selected = _selections.IndexOf(node.AsValue().GetValue<string>());
    }
}