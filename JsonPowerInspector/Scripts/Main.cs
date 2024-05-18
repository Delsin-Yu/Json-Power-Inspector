using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Text.Json;
using Godot;
using GodotTask;
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
    [Export] private Label _versionInfo;
    [Export] private Control _nullIndicator;
    
    public const string Extension = ".jsontemplate";
    public const string Data = ".json";

    private InspectionSessionController _currentFocusedSession;
    
    public override void _Ready()
    {
        _versionInfo.Text = Version.Current;
        
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
        var tabBar = _tabContainer.GetTabBar();
        tabBar.TabCloseDisplayPolicy = TabBar.CloseButtonDisplayPolicy.ShowActiveOnly;
        tabBar.TabClosePressed += async tabIndex =>
        {
            var child = _tabContainer.GetChild<InspectionSessionController>((int)tabIndex);
            if (child.Changed)
            {
                var ok = await Dialogs.OpenDataLossDialog();
                if (!ok) return;
            }

            child.QueueFree();
            
            var nullIndicatorVisible = _tabContainer.GetChildCount() - 1 == 0;
            _nullIndicator.Visible = nullIndicatorVisible;
            _tabContainer.Visible = !nullIndicatorVisible;
        };

        window.FilesDropped += async files =>
        {
            var templateFile = TryMatch(files, Extension);
            var dataFile = TryMatch(files, Data);

            if (templateFile != null && dataFile != null)
            {
                await LoadTemplate(templateFile, dataFile);
                return;
            }

            // Data == null
            if (templateFile != null)
            {
                await LoadTemplate(templateFile, null);
                return;
            }
            
            // Template == null
            if (dataFile != null)
            {
                if (_currentFocusedSession == null)
                {
                    await Dialogs.OpenErrorDialog("You are trying to load a Json data, where no template has loaded.", "No Template loaded!");
                    return;
                }
                
                await _currentFocusedSession.LoadFromJsonDataPathAsync(dataFile);
            }

            return;

            static string TryMatch(ReadOnlySpan<string> files, string matchExtension)
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
            
            async GDTask LoadTemplate(string templatePath, string dataPath)
            {
                var sessionController = _sessionPrefab.Instantiate<InspectionSessionController>();
                _tabContainer.AddChild(sessionController);
                
                try
                {
                    sessionController.StartSession(
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
                        templatePath,
                        dataPath
                    );
                }
                catch (SerializationException serializationException)
                {
                    sessionController.QueueFree();
                    await Dialogs.OpenErrorDialog(serializationException.Message);
                    return;
                }
                
                _nullIndicator.Hide();
                _tabContainer.Show();
            }
        };
        
        _nullIndicator.Show();
        _tabContainer.Hide();
        CallDeferred(MethodName.ApplyContentScale);
    }

    public override void _Input(InputEvent inputEvent)
    {       
        if (!InputMap.ActionHasEvent("save", inputEvent)) return;
        if (inputEvent.IsPressed())
        {
            if (_currentFocusedSession == null) return;
            _currentFocusedSession.Save().Forget();
        }
        GetViewport().SetInputAsHandled();
    }

    private void ApplyContentScale()
    {
        var window = GetTree().Root;
        window.Size = UserConfig.Current.Size;
        window.ContentScaleFactor = UserConfig.Current.ScaleFactor;
        _slider.Value = UserConfig.Current.ScaleFactor;
        _displayScale.Value = UserConfig.Current.ScaleFactor;
        window.MoveToCenter();
    }
    
    public override void _Notification(int what)
    {
        if(what != NotificationWMCloseRequest) return;
        UserConfig.SaveConfig();
    }
}