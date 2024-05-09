using System.Text.Json.Nodes;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class ArrayInspector : CollectionInspector<ArrayPropertyInfo>
{
    [Export] private Button _addElement;
    
    protected override void OnFoldUpdate(bool shown) => _addElement.Visible = shown;
    
    protected override void OnPostInitialize(ArrayPropertyInfo propertyInfo)
    {
    }

    public override void Bind(JsonNode node)
    {
        
    }
}