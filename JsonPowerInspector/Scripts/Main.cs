using System;
using System.IO;
using System.Linq;
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
            var matchedFile = files.FirstOrDefault(
                filePath =>
                {
                    var extension = Path.GetExtension(filePath);
                    return string.Equals(extension, Extension, StringComparison.OrdinalIgnoreCase);
                }
            );
            if (matchedFile is null) return;
            TryLoadJson(matchedFile);
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

    private void TryLoadJson(string filePath)
    {
        var setup = TemplateSerializer.Deserialize(filePath);
        CurrentSession = new(setup, _spawner);
        CurrentSession.StartSession(_objectName, _objectContainer);
    }
}