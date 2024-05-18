using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public class InspectorSpawner
{
    private readonly PackedScene _dictionaryInspector;
    private readonly PackedScene _enumInspector;
    private readonly PackedScene _dropdownInspector;
    private readonly PackedScene _numberInspector;
    private readonly PackedScene _objectInspector;
    private readonly PackedScene _stringInspector;
    private readonly PackedScene _arrayInspector;
    private readonly PackedScene _booleanInspector;
    private readonly InspectionSessionController _currentSession;

    public InspectorSpawner(
        PackedScene dictionaryInspector,
        PackedScene enumInspector,
        PackedScene dropdownInspector,
        PackedScene numberInspector,
        PackedScene objectInspector,
        PackedScene stringInspector,
        PackedScene arrayInspector,
        PackedScene booleanInspector,
        InspectionSessionController currentSession
    )
    {
        _dictionaryInspector = dictionaryInspector;
        _enumInspector = enumInspector;
        _numberInspector = numberInspector;
        _objectInspector = objectInspector;
        _stringInspector = stringInspector;
        _arrayInspector = arrayInspector;
        _booleanInspector = booleanInspector;
        _currentSession = currentSession;
        _dropdownInspector = dropdownInspector;
    }

    public StringInspector Create(StringPropertyInfo stringPropertyInfo, bool affectMainObject) => Print<StringInspector, StringPropertyInfo>(_stringInspector, stringPropertyInfo, affectMainObject);
    public NumberInspector Create(NumberPropertyInfo numberPropertyInfo, bool affectMainObject) => Print<NumberInspector, NumberPropertyInfo>(_numberInspector, numberPropertyInfo, affectMainObject);
    public ObjectInspector Create(ObjectPropertyInfo objectPropertyInfo, bool affectMainObject) => Print<ObjectInspector, ObjectPropertyInfo>(_objectInspector, objectPropertyInfo, affectMainObject);
    public BooleanInspector Create(BooleanPropertyInfo booleanPropertyInfo, bool affectMainObject) => Print<BooleanInspector, BooleanPropertyInfo>(_booleanInspector, booleanPropertyInfo, affectMainObject);
    public ArrayInspector Create(ArrayPropertyInfo arrayPropertyInfo, bool affectMainObject) => Print<ArrayInspector, ArrayPropertyInfo>(_arrayInspector, arrayPropertyInfo, affectMainObject);
    public DictionaryInspector Create(DictionaryPropertyInfo dictionaryPropertyInfo, bool affectMainObject) => Print<DictionaryInspector, DictionaryPropertyInfo>(_dictionaryInspector, dictionaryPropertyInfo, affectMainObject);
    public EnumInspector Create(EnumPropertyInfo enumPropertyInfo, bool affectMainObject) => Print<EnumInspector, EnumPropertyInfo>(_enumInspector, enumPropertyInfo, affectMainObject);
    public DropdownInspector Create(DropdownPropertyInfo dropdownPropertyInfo, bool affectMainObject) => Print<DropdownInspector, DropdownPropertyInfo>(_dropdownInspector, dropdownPropertyInfo, affectMainObject);

    private TInspector Print<TInspector, TPropertyInfo>(
        PackedScene inspectorPrefab,
        TPropertyInfo propertyInfo,
        bool affectMainObject
    )
        where TInspector : BasePropertyInspector<TPropertyInfo>
        where TPropertyInfo : BaseObjectPropertyInfo
    {
        var instance = inspectorPrefab.Instantiate<TInspector>();
        instance.Initialize(propertyInfo, _currentSession, affectMainObject);
        return instance;
    }
}