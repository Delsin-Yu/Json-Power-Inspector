using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class DropdownInspector : BasePropertyInspector<DropdownPropertyInfo>
{
    [Export] private OptionButton _contentControl;

    private readonly List<string> _values = [];

    [GeneratedRegex(@"(?<Value>.+?)\t(?<Display>.+)")]
    private partial Regex GetDefaultLineMatchingRegex();
    
    protected override void OnInitialize(DropdownPropertyInfo propertyInfo)
    {
        var fileLines = File.ReadAllLines(Path.Combine(Main.CurrentSession.TemplateDirectory, propertyInfo.DataSourcePath));

        Regex lineMatchingRegex;
        if (string.IsNullOrWhiteSpace(propertyInfo.ValueDisplayRegex)) lineMatchingRegex = GetDefaultLineMatchingRegex();
        else lineMatchingRegex = new(propertyInfo.ValueDisplayRegex, RegexOptions.Compiled);

        _contentControl.Clear();
        _values.Clear();
        
        foreach (var fileLine in fileLines.Skip(1))
        {
            var match = lineMatchingRegex.Match(fileLine);
            if(!match.Success) continue;
            var display = match.Groups["Display"].Value;
            var value = match.Groups["Value"].Value;
            _values.Add(value);
            _contentControl.AddItem(display);
        }
        
        _contentControl.Selected = -1;
    }

    protected override void Bind(ref JsonNode node)
    {
        var jsonValue = node.AsValue();
        var rawValue = GetValue(jsonValue);
        _contentControl.Selected = _values.IndexOf(rawValue);
        _contentControl.ItemSelected += ReplaceDropdownValue;
    }

    private string GetValue(JsonValue value)
    {
        switch (PropertyInfo.Kind)
        {
            case DropdownPropertyInfo.DropdownKind.Int:
            case DropdownPropertyInfo.DropdownKind.Float:
                if (value.TryGetValue<int>(out var intValue)) return intValue.ToString();
                return value.GetValue<double>().ToString(CultureInfo.InvariantCulture);
            case DropdownPropertyInfo.DropdownKind.String:
                return value.GetValue<string>();
            default:
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    private void ReplaceDropdownValue(long index)
    {
        var stringValue = _values[(int)index];
        switch (PropertyInfo.Kind)
        {
            case DropdownPropertyInfo.DropdownKind.Int:
                ReplaceValue(int.Parse(stringValue));
                break;
            case DropdownPropertyInfo.DropdownKind.Float:
                ReplaceValue(double.Parse(stringValue));
                break;
            case DropdownPropertyInfo.DropdownKind.String:
                ReplaceValue(stringValue);
                break;
            default:
                throw new InvalidOperationException();
        }
    }
}