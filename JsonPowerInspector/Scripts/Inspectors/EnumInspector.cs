using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class EnumInspector : BasePropertyInspector<EnumPropertyInfo>
{
    [Export] private OptionButton _contentControl;
    [Export] private Button _flagsButton;

    private PopupMenu _menu;
    private readonly List<EnumPropertyInfo.EnumValue> _selections = [];
    
    protected override void OnInitialize(EnumPropertyInfo propertyInfo)
    {
        _selections.Clear();
        _contentControl.Clear();

        if (propertyInfo.IsFlags)
        {
            _contentControl.Hide();
            _flagsButton.Show();

            _menu = new()
            {
                Transient = true,
                TransientToFocused = true, 
                HideOnCheckableItemSelection = false
            };

            foreach (var enumValue in propertyInfo.EnumValues)
            {
                _menu.AddCheckItem(enumValue.DisplayName);
                _selections.Add(enumValue);
            }
            
            AddChild(_menu);

            
            _flagsButton.Pressed += () =>
            {
                var buttonPosition = _flagsButton.GetScreenPosition();
                var buttonRect = _flagsButton.GetGlobalRect();
                var scaleFactor = GetTree().Root.ContentScaleFactor;
                _menu.ContentScaleFactor = scaleFactor;
                _menu.Popup(
                    new Rect2I(
                        (int)buttonPosition.X,
                        (int)(buttonPosition.Y + buttonRect.Size.Y * scaleFactor),
                        0,
                        0
                    )
                );
            };
        }
        else
        {
            _contentControl.Show();
            _flagsButton.Hide();

            foreach (var enumValue in propertyInfo.EnumValues)
            {
                _contentControl.AddItem(enumValue.DisplayName);
                _selections.Add(enumValue);
            }

            _contentControl.Selected = -1;
        }
    }

    protected override void Bind(ref JsonNode node)
    {
        var jsonValue = node.AsValue();

        if (PropertyInfo.IsFlags)
        {
            var calculatedFlag = 0L;
            var str = jsonValue.GetValue<string>();
            var values = str.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            var hasUnknownEntries = false;
            foreach (var value in values)
            {
                var index = _selections.FindIndex(x => x.DeclareName == value);
                if (index == -1)
                {
                    hasUnknownEntries = true;
                    continue;
                }
                var flag = _selections[index].Value;
                calculatedFlag |= flag;
            }
            
            _menu.IndexPressed += index =>
            {
                var intIndex = (int)index;
                var currentFlag = 0L;
                for (var i = 0; i < _selections.Count; i++)
                {
                    if(!_menu.IsItemChecked(i)) continue;
                    currentFlag |= _selections[i].Value;
                }

                var currentValue = _selections[intIndex].Value;
                var isChecked = _menu.IsItemChecked(intIndex);
                if (currentValue == 0)
                {
                    if (!isChecked) currentFlag = 0;
                    else return;
                }
                else
                {
                    if (!isChecked) currentFlag |= currentValue;
                    else currentFlag &= ~currentValue;
                }

                
                RefreshFlags(currentFlag, true);
            };
            
            RefreshFlags(calculatedFlag, hasUnknownEntries);
        }
        else
        {
            _contentControl.Selected = _selections.FindIndex(x => x.DeclareName == jsonValue.GetValue<string>());
            _contentControl.ItemSelected += index => ReplaceValue(JsonValue.Create(_selections[(int)index].DeclareName));   
        }
    }

    private void RefreshFlags(long currentFlag, bool updateBacking)
    {
        var list = new List<(string, string)>();
        for (var i = 0; i < _selections.Count; i++)
        {
            var (displayName, declareName, enumValue) = _selections[i];
            var hasValue = (currentFlag & enumValue) == enumValue;
            if (enumValue == 0 && currentFlag != 0) hasValue = false;
            _menu.SetItemChecked(i, hasValue);
            if (hasValue)
            {
                list.Add((declareName, displayName));
            }
        }

        if(updateBacking) ReplaceValue(JsonValue.Create(string.Join(", ", list.Select(x => x.Item1))));
        _flagsButton.Text = string.Join(", ", list.Select(x => x.Item2));
    }
}