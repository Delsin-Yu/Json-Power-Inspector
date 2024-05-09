using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class DictionaryInspector : CollectionInspector<DictionaryPropertyInfo>
{
    [Export] private Button _addElement;

    protected override void OnFoldUpdate(bool shown) => _addElement.Visible = shown;

    protected override void OnPostInitialize(DictionaryPropertyInfo propertyInfo)
    {
    }
}