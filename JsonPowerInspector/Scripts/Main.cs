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
    
    public const string Extension = ".jsontemplate";
    public const string Data = ".json";

    private InspectorSpawner _spawner;
    
    public override void _Ready()
    {
        var window = GetTree().Root;

        _slider.MinValue = 0.5f;
        _slider.MaxValue = 2.5f;
        _slider.DragEnded += changed =>
        {
            if(!changed) return;
            window.ContentScaleFactor = (float)_slider.Value;
        };
        _slider.Value = window.ContentScaleFactor;
        
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