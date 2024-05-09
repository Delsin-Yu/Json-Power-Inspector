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

    public StringInspector Create(StringPropertyInfo stringPropertyInfo, string path) => Print<StringInspector, StringPropertyInfo>(_stringInspector, stringPropertyInfo, path);
    public NumberInspector Create(NumberPropertyInfo numberPropertyInfo, string path) => Print<NumberInspector, NumberPropertyInfo>(_numberInspector, numberPropertyInfo, path);
    public ObjectInspector Create(ObjectPropertyInfo objectPropertyInfo, string path) => Print<ObjectInspector, ObjectPropertyInfo>(_objectInspector, objectPropertyInfo, path);
    public BooleanInspector Create(BooleanPropertyInfo booleanPropertyInfo, string path) => Print<BooleanInspector, BooleanPropertyInfo>(_booleanInspector, booleanPropertyInfo, path);
    public ArrayInspector Create(ArrayPropertyInfo arrayPropertyInfo, string path) => Print<ArrayInspector, ArrayPropertyInfo>(_arrayInspector, arrayPropertyInfo, path);
    public DictionaryInspector Create(DictionaryPropertyInfo dictionaryPropertyInfo, string path) => Print<DictionaryInspector, DictionaryPropertyInfo>(_dictionaryInspector, dictionaryPropertyInfo, path);
    public EnumInspector Create(EnumPropertyInfo enumPropertyInfo, string path) => Print<EnumInspector, EnumPropertyInfo>(_enumInspector, enumPropertyInfo, path);
    
    private static TInspector Print<TInspector, TPropertyInfo>(PackedScene inspectorPrefab, TPropertyInfo propertyInfo, string path) 
        where TInspector : BasePropertyInspector<TPropertyInfo> 
        where TPropertyInfo : BaseObjectPropertyInfo
    {
        var instance = inspectorPrefab.Instantiate<TInspector>();
        instance.Initialize(propertyInfo, path);
        return instance;
    }
}