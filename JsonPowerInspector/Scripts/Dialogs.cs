using Godot;
using GodotTask;

namespace JsonPowerInspector;

public static class Version
{
    public static string Current => _versions[^1];

    private static readonly string[] _versions =
    [
        "0.0.1"
    ];
}
public static class Dialogs
{
    public static GDTask<bool> OpenDataLossDialog() =>
        OpenYesNoDialog(
            "The dialog you are closing contains unsaved data, please confirm.",
            "Potential Data Loss",
            "Discard",
            "Back"
        );

    public static async GDTask<bool> OpenYesNoDialog(string message, string title = "Confirm!", string okText = "Ok", string cancelText = "Cancel")
    {
        var dialog = new ConfirmationDialog()
        {
            DialogText = message,
            Title = title,
            OkButtonText = okText,
            CancelButtonText = cancelText,
        };
        
        dialog.GetLabel().AutowrapMode = TextServer.AutowrapMode.Arbitrary;
        var window = ((SceneTree)Engine.GetMainLoop()).Root;
        // dialog.ContentScaleFactor = window.ContentScaleFactor;

        bool? status = null;

        window.AddChild(dialog);

        dialog.Confirmed += () => status = true;
        dialog.Canceled += () => status = false;
        dialog.PopupCentered();

        await GDTask.WaitUntil(() => status.HasValue);

        return status!.Value;
    }
    public static async GDTask OpenErrorDialog(string message, string title = "Error!")
    {
        var errorDialog = new AcceptDialog
        {
            DialogText = message,
            Title = title,
        };

        errorDialog.GetLabel().AutowrapMode = TextServer.AutowrapMode.Word;

        var window = ((SceneTree)Engine.GetMainLoop()).Root;
        window.AddChild(errorDialog);
        // errorDialog.ContentScaleFactor = window.ContentScaleFactor;


        var closed = false;

        errorDialog.Confirmed += () => closed = true;

        errorDialog.PopupCentered();
        await GDTask.WaitUntil(() => closed);
        
        errorDialog.QueueFree();
    }
    
    public static GDTask<string> OpenSaveFileDialog()
    {
        var fileDialog = new FileDialog
        {
            FileMode = FileDialog.FileModeEnum.SaveFile,
            Access = FileDialog.AccessEnum.Filesystem,
            UseNativeDialog = true,
            Filters = ["*.json;Json Data File"],
            Title = "Specify Path"
        };

        var window = ((SceneTree)Engine.GetMainLoop()).Root;
        window.AddChild(fileDialog);
        // fileDialog.ContentScaleFactor = window.ContentScaleFactor;
        
        fileDialog.PopupCentered();

        var path = fileDialog.CurrentFile;
        
        fileDialog.QueueFree();

        if (string.IsNullOrEmpty(path)) return GDTask.FromResult<string>(null);

        return GDTask.FromResult(path);
    }
}