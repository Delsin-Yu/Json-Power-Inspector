using System;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class ObjectInspector : CollectionInspector<ObjectPropertyInfo>
{
    protected override void OnPostInitialize(ObjectPropertyInfo propertyInfo)
    {
        var objectDefinition = Main.CurrentSession.LookupObject(propertyInfo.ObjectTypeName);
        foreach (var info in objectDefinition.Properties.AsSpan())
        {
            Container.AddChild(CreateInspectorForProperty(info));
        }
    }

    public static Control CreateInspectorForProperty(BaseObjectPropertyInfo propertyInfo)
    {
        var spawner = Main.CurrentSession.InspectorSpawner;
        return propertyInfo switch
        {
            StringPropertyInfo stringPropertyInfo => spawner.Create(stringPropertyInfo),
            NumberPropertyInfo numberPropertyInfo => spawner.Create(numberPropertyInfo),
            ObjectPropertyInfo objectPropertyInfo => spawner.Create(objectPropertyInfo),
            BooleanPropertyInfo booleanPropertyInfo => spawner.Create(booleanPropertyInfo),
            ArrayPropertyInfo arrayPropertyInfo => spawner.Create(arrayPropertyInfo),
            DictionaryPropertyInfo dictionaryPropertyInfo => spawner.Create(dictionaryPropertyInfo),
            EnumPropertyInfo enumPropertyInfo => spawner.Create(enumPropertyInfo),
            _ => throw new InvalidOperationException()
        };
    }
}