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

    public async GDTask<double?> ShowAsync(NumberPropertyInfo numberPropertyInfo, InspectorSpawner inspectorSpawner)
    {
        var inspector = inspectorSpawner.Create(numberPropertyInfo);
        JsonNode jsonValueNode = JsonValue.Create(0);
        _container.AddChild(inspector);
        inspector.BindJsonNode(ref jsonValueNode);
        Show();
        await GDTask.WaitUntil(() => _addClicked || _removeClicked);
        QueueFree();
        if (_addClicked)
        {
            ((JsonValue)jsonValueNode).TryGetValue<double>(out var returnValue);
            return returnValue;
        };
        return null;
    }
    
    public async GDTask<string> ShowAsync(StringPropertyInfo stringPropertyInfo, InspectorSpawner inspectorSpawner)
    {
        var inspector = inspectorSpawner.Create(stringPropertyInfo);
        JsonNode jsonValueNode = JsonValue.Create("");
        _container.AddChild(inspector);
        inspector.BindJsonNode(ref jsonValueNode);
        Show();
        await GDTask.WaitUntil(() => _addClicked || _removeClicked);
        QueueFree();
        if (_addClicked)
        {
            ((JsonValue)jsonValueNode).TryGetValue<string>(out var returnValue);
            return returnValue;
        };
        return null;
    }
}