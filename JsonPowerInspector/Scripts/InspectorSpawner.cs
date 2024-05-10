using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public class InspectorSpawner
{
    private readonly PackedScene _dictionaryInspector;
    private readonly PackedScene _enumInspector;
    private readonly PackedScene _numberInspector;
    private readonly PackedScene _objectInspector;
    private readonly PackedScene _stringInspector;
    private readonly PackedScene _arrayInspector;
    private readonly PackedScene _booleanInspector;

    public InspectorSpawner(
        PackedScene dictionaryInspector,
        PackedScene enumInspector,
        PackedScene numberInspector,
        PackedScene objectInspector,
        PackedScene stringInspector,
        PackedScene arrayInspector,
        PackedScene booleanInspector
    )
    {
        _dictionaryInspector = dictionaryInspector;
        _enumInspector = enumInspector;
        _numberInspector = numberInspector;
        _objectInspector = objectInspector;
        _stringInspector = stringInspector;
        _arrayInspector = arrayInspector;
        _booleanInspector = booleanInspector;
    }

    public StringInspector Create(StringPropertyInfo stringPropertyInfo) => Print<StringInspector, StringPropertyInfo>(_stringInspector, stringPropertyInfo);
    public NumberInspector Create(NumberPropertyInfo numberPropertyInfo) => Print<NumberInspector, NumberPropertyInfo>(_numberInspector, numberPropertyInfo);
    public ObjectInspector Create(ObjectPropertyInfo objectPropertyInfo) => Print<ObjectInspector, ObjectPropertyInfo>(_objectInspector, objectPropertyInfo);
    public BooleanInspector Create(BooleanPropertyInfo booleanPropertyInfo) => Print<BooleanInspector, BooleanPropertyInfo>(_booleanInspector, booleanPropertyInfo);
    public ArrayInspector Create(ArrayPropertyInfo arrayPropertyInfo) => Print<ArrayInspector, ArrayPropertyInfo>(_arrayInspector, arrayPropertyInfo);
    public DictionaryInspector Create(DictionaryPropertyInfo dictionaryPropertyInfo) => Print<DictionaryInspector, DictionaryPropertyInfo>(_dictionaryInspector, dictionaryPropertyInfo);
    public EnumInspector Create(EnumPropertyInfo enumPropertyInfo) => Print<EnumInspector, EnumPropertyInfo>(_enumInspector, enumPropertyInfo);
    
    private static TInspector Print<TInspector, TPropertyInfo>(PackedScene inspectorPrefab, TPropertyInfo propertyInfo) 
        where TInspector : BasePropertyInspector<TPropertyInfo> 
        where TPropertyInfo : BaseObjectPropertyInfo
    {
        var instance = inspectorPrefab.Instantiate<TInspector>();
        instance.Initialize(propertyInfo);
        return instance;
    }
}