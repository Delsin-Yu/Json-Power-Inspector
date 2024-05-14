using System;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class Main : Control
{
    public static InspectionSession CurrentSession { get; private set; }
    
    [Export] private PackedScene _dictionaryInspector;
    [Export] private PackedScene _enumInspector;
    [Export] private PackedScene _numberInspector;
    [Export] private PackedScene _objectInspector;
    [Export] private PackedScene _stringInspector;
    [Export] private PackedScene _arrayInspector;
    [Export] private PackedScene _booleanInspector;
    [Export] private Label _objectName;
    [Export] private Control _objectContainer;
    [Export] private Slider _slider;
    [Export] private SpinBox _displayScale;
    
    public const string Extension = ".jsontemplate";
    public const string Data = ".json";

    private InspectorSpawner _spawner;
    
    public override void _Ready()
    {
        UserConfig.LoadConfig();
        
        var window = GetTree().Root;

        window.WrapControls = true;
        window.SizeChanged += () => UserConfig.Current.Size = window.Size;

        _slider.MinValue = 0.5f;
        _slider.MaxValue = 2.5f;
        _slider.Step = 0.1f;
        _displayScale.MinValue = 0.5f;
        _displayScale.MaxValue = 2.5f;
        _displayScale.Step = 0.1f;
        
        _slider.DragEnded += changed =>
        {
            if(!changed) return;
            var sliderValue = (float)_slider.Value;
            window.ContentScaleFactor = sliderValue;
            _displayScale.Value = sliderValue;
            UserConfig.Current.ScaleFactor = sliderValue;
        };
        _displayScale.ValueChanged += value =>
        {
            var floatValue = (float)value;
            window.ContentScaleFactor = floatValue;
            _slider.Value = floatValue;
            UserConfig.Current.ScaleFactor = floatValue; 
        };

        window.FilesDropped += files =>
        {
            var templateFile = TryMatch(files, Extension);
            var dataFile = TryMatch(files, Data);

            if (templateFile != null && dataFile != null)
            {
                LoadBoth(templateFile, dataFile);
                return;
            }

            // Data == null
            if (templateFile != null)
            {
                LoadTemplate(templateFile, null);
                return;
            }
            
            // Template == null
            if (dataFile != null)
            {
                if (CurrentSession == null)
                {
                    // TODO: Error Handling
                    return;
                }
                
                CurrentSession.LoadFromJsonObject(LoadData(dataFile));
            }
        };

        _spawner = new(
            _dictionaryInspector,
            _enumInspector,
            _numberInspector,
            _objectInspector,
            _stringInspector,
            _arrayInspector,
            _booleanInspector
        );

        CallDeferred(MethodName.ApplyContentScale);
    }

    private void ApplyContentScale()
    {
        var window = GetTree().Root;
        window.Size = UserConfig.Current.Size;
        window.ContentScaleFactor = UserConfig.Current.ScaleFactor;
        _slider.Value = UserConfig.Current.ScaleFactor;
        _displayScale.Value = UserConfig.Current.ScaleFactor;
    }
    
    public override void _Notification(int what)
    {
        if(what != NotificationWMCloseRequest) return;
        UserConfig.SaveConfig();
    }

    private static string TryMatch(ReadOnlySpan<string> files, string matchExtension)
    {
        foreach (var file in files)
        {
            var extension = Path.GetExtension(file);
            if (string.Equals(extension, matchExtension, StringComparison.OrdinalIgnoreCase))
            {
                return file;
            }
        }

        return null;
    }

    private void LoadBoth(string filePath, string dataPath)
    {
        LoadTemplate(filePath, LoadData(dataPath));
    }
    
    private void LoadTemplate(string filePath, JsonObject data)
    {
        // TODO: Serialization Exception Handling
        var setup = TemplateSerializer.Deserialize(filePath);
        
        if (CurrentSession != null)
        {
            // TODO: Warn data loss
            foreach (var child in _objectContainer.GetChildren().ToArray().AsSpan())
            {
                child.QueueFree();
            }

            CurrentSession = null;
        }
        
        CurrentSession = new(setup, _spawner);
        CurrentSession.StartSession(_objectName, _objectContainer, data);
    }
    
    private static JsonObject LoadData(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);
        return (JsonObject)JsonObject.Parse(fileStream);
    }
    
}