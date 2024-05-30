using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class NumberInspector : BasePropertyInspector<NumberPropertyInfo>
{
    [Export] private SpinBox _contentControl;
    [Export] private Button _createOrDeleteBtn;

    protected override void OnInitialize(NumberPropertyInfo propertyInfo)
    {
        _contentControl.Step = propertyInfo.NumberKind is NumberPropertyInfo.NumberType.Int ? 1 : 0.001;

        if (propertyInfo.Range.HasValue)
        {
            _contentControl.MinValue = propertyInfo.Range.Value.Lower;
            _contentControl.MaxValue = propertyInfo.Range.Value.Upper;
            _contentControl.AllowLesser = false;
            _contentControl.AllowGreater = false;
        }
        else
        {
            _contentControl.MinValue = -1_0000_0000;
            _contentControl.MaxValue = 1_0000_0000;
            _contentControl.AllowLesser = true;
            _contentControl.AllowGreater = true;
        }
        
        _createOrDeleteBtn.Visible = propertyInfo.Nullable;

        if (propertyInfo.Nullable)
        {
            _createOrDeleteBtn.Pressed += () =>
            {
                JsonNode jsonNode = GetBackingNode() == null ? 0 : null;
                SetBackingNode(jsonNode);
                Bind(ref jsonNode);
                CurrentSession.MarkChanged();
            };
        }
        
        _contentControl.ValueChanged += value => ReplaceValue(JsonValue.Create(value));
    }

    protected override void Bind(ref JsonNode node)
    {
        if (node is null && !PropertyInfo.Nullable)
        {
            throw new JsonException("Supplying a null value on not nullable number property!");
        }
        
        if (node is null)
        {
            _createOrDeleteBtn.Text = "+";
            _contentControl.Hide();
        }
        else
        {
            _createOrDeleteBtn.Text = "X";
            _contentControl.Show();
        
            var jsonValue = node.AsValue();
            switch (PropertyInfo.NumberKind)
            {
                case NumberPropertyInfo.NumberType.Int:
                    if (jsonValue.TryGetValue<int>(out var intValue))
                    {
                        _contentControl.Value = intValue;
                    }
                    else
                    {
                        var contentControlValue = jsonValue.GetValue<double>();
                        _contentControl.Value = Math.Round(contentControlValue);
                    }
                    break;
                case NumberPropertyInfo.NumberType.Float:
                    if (jsonValue.TryGetValue<double>(out var floatValue))
                    {
                        _contentControl.Value = floatValue;
                    }
                    else
                    {
                        var contentControlValue = jsonValue.GetValue<int>();
                        _contentControl.Value = contentControlValue;
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}