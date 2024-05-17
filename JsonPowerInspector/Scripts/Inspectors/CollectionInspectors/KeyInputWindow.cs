using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Godot;
using GodotTask;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class KeyInputWindow : Window
{
    [Export] private Control _container;
    [Export] private Button _addBtn;
    [Export] private Button _cancelBtn;
    [Export] private Label _cantAddHelp;

    private bool _addClicked;
    private bool _removeClicked;
    
    public override void _Ready()
    {
        _addBtn.Pressed += () => _addClicked = true;
        _cancelBtn.Pressed += () => _removeClicked = true;
    }

    public GDTask<(bool, double)> ShowAsync(NumberPropertyInfo numberPropertyInfo, InspectorSpawner inspectorSpawner, HashSet<double> bannedKey) =>
        ShowAsync(
            numberPropertyInfo,
            static (spawner, info) => spawner.Create(info),
            inspectorSpawner,
            bannedKey,
            0.0
        );

    public GDTask<(bool, string)> ShowAsync(StringPropertyInfo stringPropertyInfo, InspectorSpawner inspectorSpawner, HashSet<string> bannedKey) =>
        ShowAsync(
            stringPropertyInfo,
            static (spawner, info) => spawner.Create(info),
            inspectorSpawner,
            bannedKey,
            ""
        );
    
    public GDTask<(bool, int)> ShowAsyncInt(DropdownPropertyInfo dropdownPropertyInfo, InspectorSpawner inspectorSpawner, HashSet<int> bannedKey) =>
        ShowAsync(
            dropdownPropertyInfo,
            static (spawner, info) => spawner.Create(info),
            inspectorSpawner,
            bannedKey,
            0
        );
    
    public GDTask<(bool, double)> ShowAsyncFloat(DropdownPropertyInfo dropdownPropertyInfo, InspectorSpawner inspectorSpawner, HashSet<double> bannedKey) =>
        ShowAsync(
            dropdownPropertyInfo,
            static (spawner, info) => spawner.Create(info),
            inspectorSpawner,
            bannedKey,
            0.0
        );

    public GDTask<(bool, string)> ShowAsyncString(DropdownPropertyInfo dropdownPropertyInfo, InspectorSpawner inspectorSpawner, HashSet<string> bannedKey) =>
        ShowAsync(
            dropdownPropertyInfo,
            static (spawner, info) => spawner.Create(info),
            inspectorSpawner,
            bannedKey,
            ""
        );

    public override void _Notification(int what)
    {
        if (what != NotificationWMCloseRequest) return;
        _removeClicked = true;
    }

    [SuppressMessage("Warning", "IL3050")]
    [SuppressMessage("Warning", "IL2026")]
    private async GDTask<(bool, TValue)> ShowAsync<TPropertyInfo, TValue>(
        TPropertyInfo propertyInfo,
        Func<InspectorSpawner, TPropertyInfo, IPropertyInspector> inspectorFactory,
        InspectorSpawner spawner,
        IReadOnlySet<TValue> bannedKey,
        TValue defaultValue
    ) where TValue : notnull
    {
        var inspector = inspectorFactory(spawner, propertyInfo);
        inspector.ValueChanged += value =>
        {
            var typedValue = (TValue)value;
            var addBtnDisabled = bannedKey.Contains(typedValue);
            UpdateAddButton(addBtnDisabled);
        };
        UpdateAddButton(bannedKey.Contains(defaultValue));
        var emptyJsonObject = new JsonObject();
        JsonNode jsonValueNode = JsonValue.Create(defaultValue);
        const string name = "TempJsonNode";
        emptyJsonObject.Add(name, jsonValueNode);
        _container.AddChild((Control)inspector);
        inspector.BindJsonNode(emptyJsonObject, name);
        Show();
        await GDTask.WaitUntil(() => _addClicked || _removeClicked);
        QueueFree();
        if (_addClicked)
        {
            emptyJsonObject[name]!.AsValue().TryGetValue<TValue>(out var returnValue);
            return (true, returnValue);
        }
        return (false, default);
    }

    private void UpdateAddButton(bool addBtnDisabled)
    {
        _addBtn.Disabled = addBtnDisabled;
        _cantAddHelp.Visible = addBtnDisabled;
    }
}