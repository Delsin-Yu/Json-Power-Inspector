using Godot;
using GodotTask;

namespace JsonPowerInspector;

public static class Dialogs
{
    public static async GDTask<string> OpenSaveFileDialog(string defaultDir)
    {
        var fileDialog = new FileDialog
        {
            FileMode = FileDialog.FileModeEnum.SaveFile,
            Access = FileDialog.AccessEnum.Filesystem,
            UseNativeDialog = true,
            Filters = ["*.json;Json Data File"],
            Title = "Specify Path",
            CurrentDir = defaultDir
        };
        
        ((SceneTree)Engine.GetMainLoop()).Root.AddChild(fileDialog);
        
        fileDialog.PopupCentered();

        var path = fileDialog.CurrentFile;
        
        fileDialog.QueueFree();

        if (string.IsNullOrEmpty(path)) return null;

        return fileDialog.CurrentPath;
    }
}