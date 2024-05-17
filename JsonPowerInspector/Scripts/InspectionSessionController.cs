using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Godot;
using GodotTask;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class InspectionSessionController : Control
{
    [Export] private Control _container;
    [Export] private Button _templatePathBtn;
    [Export] private Button _dataPathBtn;
    [Export] private Button _save;
    [Export] private Button _revert;
    [Export] private PanelContainer _dataContainer;
    [Export] private Color _validDataColor;
    [Export] private Color _invalidDataColor;
    
    public InspectorSpawner InspectorSpawner { get; private set; }
    private readonly List<IPropertyInspector> _inspectorRoot = [];
    public string TemplateDirectory { get; set; }
    public IReadOnlyDictionary<string, ObjectDefinition> ObjectDefinitionMap => _objectDefinitionMap;
    public bool Changed { get; private set; }

    private Dictionary<string, ObjectDefinition> _objectDefinitionMap;
    private ObjectDefinition _mainObjectDefinition;
    private JsonObject _editingJsonObject;
    private string _templatePath;
    private string _dataPath;

    private string TemplatePath
    {
        get => _templatePath;
        set
        {
            _templatePath = value;
            _templatePathBtn.Text = value;
        }
    }

    private string DataPath
    {
        get => _dataPath;
        set
        {
            _dataPath = value;
            _dataPathBtn.Text = value ?? "New Data";
            _dataContainer.SelfModulate = value != null ? _validDataColor : _invalidDataColor;
        }
    }

    private async GDTask OpenTemplateFile()
    {
        if (!File.Exists(TemplatePath))
        {
            await Dialogs.OpenErrorDialog(
                "The active template for this dialog no longer belongs to a file anymore, " +
                "try recreate the template file and start a new session with it.",
                "The file no longer exists"
            );
            DataPath = null;
            return;
        }
        OS.ShellOpen(TemplatePath);
    }

    private async GDTask OpenDataFile()
    {
        if (DataPath == null)
        {
            await Dialogs.OpenErrorDialog(
                "The active data for this dialog does not belongs to a file yet, " +
                "please perform a save first.",
                "The file does not exist"
            );
            return;
        }

        if (!File.Exists(DataPath))
        {
            await Dialogs.OpenErrorDialog(
                "The active data for this dialog no longer belongs to a file anymore, " +
                "please perform a save to recreate it.",
                "The file no longer exist"
            );
            DataPath = null;
            return;
        }

        OS.ShellOpen(DataPath);
    }

    public override void _Ready()
    {
        _save.Pressed += () => Save().Forget();
        _revert.Pressed += async () =>
        {
            bool success;
            if (DataPath == null) success = await MountJsonObjectAsync(CreateTemplateJsonObject());
            else success = await LoadFromJsonDataPathAsync(DataPath);
            if(!success) return;
            ResetChanged();
        };
        _templatePathBtn.Pressed += () => OpenTemplateFile().Forget();
        _dataPathBtn.Pressed += () => OpenDataFile().Forget();
    }

    public void MarkChanged()
    {
        if(Changed) return;
        Changed = true;
        Name += " (*)";
    }

    private void ResetChanged()
    {
        Name = _mainObjectDefinition.ObjectTypeName;
        Changed = false;
    }

    public void StartSession(
        InspectorSpawner inspectorSpawner,
        string templatePath,
        string dataPath
    )
    {
        TemplatePath = templatePath;

        PackedObjectDefinition setup;
        try
        {
            using var fileStream = File.OpenRead(templatePath);
            setup = JsonSerializer.Deserialize(fileStream, PowerTemplateJsonContext.Default.PackedObjectDefinition);
        }
        catch (Exception e)
        {
            throw new SerializationException($"{e.GetType().Name} when reading the template file: {templatePath}\n{e}");
        }
        
        _objectDefinitionMap = setup.ReferencedObjectDefinition.ToDictionary(x => x.ObjectTypeName, x => x);
        _mainObjectDefinition = setup.MainObjectDefinition;
        InspectorSpawner = inspectorSpawner;
        TemplateDirectory = Path.GetDirectoryName(templatePath)!;
        Name = setup.MainObjectDefinition.ObjectTypeName;
        
        if (dataPath != null)
        {
            LoadFromJsonDataPathAsync(dataPath);
            return;
        }

        DataPath = null;
        var templateJsonObject = CreateTemplateJsonObject();
        MountJsonObjectAsync(templateJsonObject).Forget();
    }

    private JsonObject CreateTemplateJsonObject()
    {
        JsonObject templateJsonObject = new();
        foreach (var propertyInfo in _mainObjectDefinition.Properties.AsSpan())
        {
            //var propertyPath = new HashSet<BaseObjectPropertyInfo>();
            templateJsonObject.Add(propertyInfo.Name, Utils.CreateDefaultJsonObjectForProperty(propertyInfo));
        }

        return templateJsonObject;
    }

    public GDTask<bool> LoadFromJsonDataPathAsync(string dataPath)
    {
        DataPath = dataPath;
        JsonObject jsonObject;
        {
            using var fileStream = File.OpenRead(dataPath);
            jsonObject = (JsonObject)JsonObject.Parse(fileStream);
        }
        return MountJsonObjectAsync(jsonObject);
    }

    private async GDTask<bool> MountJsonObjectAsync(JsonObject jsonObject)
    {
        if (Changed)
        {
            var discard = await Dialogs.OpenDataLossDialog();
            if(!discard) return false;
        }
        
        ResetControls();

        foreach (var propertyInfo in _mainObjectDefinition.Properties)
        {
            var inspectorForProperty = Utils.CreateInspectorForProperty(propertyInfo, InspectorSpawner);
            _inspectorRoot.Add(inspectorForProperty);
            _container.AddChild((Control)inspectorForProperty);
        }        
        
        _editingJsonObject = jsonObject;
        var objectProperty = _editingJsonObject.ToArray();
        for (var index = 0; index < _inspectorRoot.Count; index++)
        {
            try
            {
                var jsonNode = objectProperty[index].Key;
                var propertyInspector = _inspectorRoot[index];
                propertyInspector.BindJsonNode(_editingJsonObject, jsonNode);
            }
            catch (Exception e)
            {
                await Dialogs.OpenErrorDialog(
                    $"{e.GetType()} when trying to load the specified Json to the active data template, " +
                    $"it's most likely that the template does not belongs to the JSON format.\n{e}"
                );
                ResetControls();
                DataPath = null;
                await MountJsonObjectAsync(CreateTemplateJsonObject());
                return false;
            }
        }

        return true;
    }

    private void ResetControls()
    {
        foreach (var inspector in _inspectorRoot)
        {
            ((Control)inspector).QueueFree();
        }

        _inspectorRoot.Clear();
    }

    private readonly JsonSerializerOptions _options = new() { WriteIndented = true, };
    
    public async GDTask Save()
    {
        PickPath:
        if (DataPath == null)
        {
            var selected = await Dialogs.OpenSaveFileDialog();
            if (selected == null) return;
            DataPath = selected;
        }

        string jsonString;
        try
        {
            if(!_options.IsReadOnly) _options.MakeReadOnly(true);
            jsonString = _editingJsonObject.ToJsonString(_options);
        }
        catch (Exception e)
        {
            await Dialogs.OpenErrorDialog($"{e.GetType().Name} when serializing the editing data:\n{e}");
            return;
        }
        
        try
        {
            File.WriteAllText(DataPath, jsonString);
        }
        catch (Exception e)
        {
            var yes = await Dialogs.OpenYesNoDialog(
                $"Error when saving the data file to the specified path, " +
                $"do you wish to pick another path?\n{e.Message}",
                $"{e.GetType().TypeHandle} when saving.",
                "Yes",
                "No"
            );

            if (yes)
            {
                DataPath = null;
                goto PickPath;
            }
            return;
        }
        ResetChanged();
    }
}