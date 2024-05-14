using System;
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

    private bool _addClicked;
    private bool _removeClicked;
    
    public override void _Ready()
    {
        _addBtn.Pressed += () => _addClicked = true;
        _cancelBtn.Pressed += () => _removeClicked = true;
    }

    public GDTask<(bool, double)> ShowAsync(NumberPropertyInfo numberPropertyInfo, InspectorSpawner inspectorSpawner) =>
        ShowAsync(
            numberPropertyInfo,
            static (spawner, info) => spawner.Create(info),
            inspectorSpawner,
            0.0
        );

    public GDTask<(bool, string)> ShowAsync(StringPropertyInfo stringPropertyInfo, InspectorSpawner inspectorSpawner) =>
        ShowAsync(
            stringPropertyInfo,
            static (spawner, info) => spawner.Create(info),
            inspectorSpawner,
            ""
        );

    private async GDTask<(bool, TValue)> ShowAsync<TPropertyInfo, TValue>(
        TPropertyInfo propertyInfo,
        Func<InspectorSpawner, TPropertyInfo, IPropertyInspector> inspectorFactory,
        InspectorSpawner spawner,
        TValue defaultValue
    ) where TValue : notnull
    {
        var inspector = inspectorFactory(spawner, propertyInfo);
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
}