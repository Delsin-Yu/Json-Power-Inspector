using System;
using System.IO;
using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class Main : Control
{
    [Export] private PackedScene _dictionaryInspector;
    [Export] private PackedScene _enumInspector;
    [Export] private PackedScene _dropdownInspector;
    [Export] private PackedScene _numberInspector;
    [Export] private PackedScene _objectInspector;
    [Export] private PackedScene _stringInspector;
    [Export] private PackedScene _arrayInspector;
    [Export] private PackedScene _booleanInspector;
    [Export] private PackedScene _sessionPrefab;
    [Export] private TabContainer _tabContainer;
    [Export] private Slider _slider;
    [Export] private SpinBox _displayScale;
    
    public const string Extension = ".jsontemplate";
    public const string Data = ".json";

    private InspectionSessionController _currentFocusedSession;
    
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

        _tabContainer.TabChanged += tabIndex => _currentFocusedSession = _tabContainer.GetChild<InspectionSessionController>((int)tabIndex);

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
                if (_currentFocusedSession == null)
                {
                    // TODO: Error Handling
                    return;
                }
                
                _currentFocusedSession.LoadFromJsonObject(LoadData(dataFile));
            }
        };
        
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

    private static JsonObject LoadData(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);
        return (JsonObject)JsonObject.Parse(fileStream);
    }
    
    private void LoadBoth(string filePath, string dataPath)
    {
        LoadTemplate(filePath, LoadData(dataPath));
    }

    private void LoadTemplate(string filePath, JsonObject data)
    {
        // TODO: Serialization Exception Handling
        var setup = TemplateSerializer.Deserialize(filePath);

        var sessionController = _sessionPrefab.Instantiate<InspectionSessionController>();
        sessionController.Name = setup.MainObjectDefinition.ObjectTypeName;
        _tabContainer.AddChild(sessionController);
        sessionController.StartSession(
            setup,
            new(
                _dictionaryInspector,
                _enumInspector,
                _dropdownInspector,
                _numberInspector,
                _objectInspector,
                _stringInspector,
                _arrayInspector,
                _booleanInspector,
                sessionController
            ),
            filePath,
            data
        );
    }
}