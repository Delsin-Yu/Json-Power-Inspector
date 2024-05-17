using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using JsonPowerInspector.Template;
using TMI_RogueLike_DataEditor.Model;

[JsonSerializable(typeof(TestModels))]
[JsonSourceGenerationOptions(UseStringEnumConverter = true, WriteIndented = true)]
public partial class TestApplicationJsonContext : JsonSerializerContext;

public class TestModels
{
    [InspectorName("简单数据")] public MySimpleModel MySimpleModel { get; set; }
    [InspectorName("复杂数据")] public MyComplexModel MyComplexModel { get; set; }
    [InspectorName("集合数据")] public MyCollectionModel MyCollectionModel { get; set; }
    [InspectorName("可选数据")] public MyDropdownModel MyDropdownModel { get; set; }
}

public class MyDropdownModel
{
    [InspectorDropdown("StringSelection.tsv"), InspectorName("字符串可选数据")] public string StringData { get; set; }
    [InspectorDropdown("FloatSelection.tsv"), InspectorName("浮点数可选数据")] public float FloatData { get; set; }
    [InspectorDropdown("IntSelection.tsv"), InspectorName("整数可选数据")] public int IntData { get; set; }
    [InspectorKeyDropdown("IntSelection.tsv"), InspectorName("字典（键可选）")] public Dictionary<int, float> DictionaryKeyData { get; set; }
    [InspectorValueDropdown("FloatSelection.tsv"), InspectorName("字典（值可选）")] public Dictionary<int, float> DictionaryValueData { get; set; }
    [InspectorKeyDropdown("IntSelection.tsv"), InspectorName("字典（全可选）"), InspectorValueDropdown("FloatSelection.tsv")] public Dictionary<int, float> DictionaryData { get; set; }
}

public class MyCollectionModel
{
    [InspectorName("字典类型")] public MyDictionaryModel MyDictionaryModel { get; set; }
    [InspectorName("数组类型")] public MyArrayModel MyArrayModel { get; set; }
}

public class MyDictionaryModel
{
    [InspectorName("字符串 - 整数 字典"), NumberRangeValue(0, 5)] public Dictionary<string, int> StrInt { get; set; }
    [InspectorName("整数 - 字符串 字典"), NumberRangeKey(0, 5)] public Dictionary<int, string> IntStr { get; set; }
    [InspectorName("浮点数 - 字符串 字典"), NumberRangeKey(0.2, 5)] public Dictionary<float, string> FPStr { get; set; }
}

public class MyArrayModel
{
    [InspectorName("字符串数组")] public string[] StrArr { get; set; }
    [InspectorName("整数数组")] public int[] IntArr { get; set; }
    [InspectorName("对象数组")] public MySimpleModel[] MySimpleModels { get; set; }
}

public class MySimpleModel
{
    [InspectorName("国家数据")] public NationModel NationModel { get; set; }
    [InspectorName("角色数据")] public CharacterModel CharacterModel { get; set; }
    [InspectorName("位置数据")] public LocationModel LocationModel { get; set; }
    [InspectorName("武器数据")] public WeaponModel WeaponModel { get; set; }
    [InspectorName("物品数据")] public ItemModel ItemModel { get; set; }
}

public class MyComplexModel
{
    [InspectorName("数据")] public Dictionary<int, string> Data { get; set; }
    [InspectorName("数据2")] public Dictionary<string, CharacterModel> Data2 { get; set; }
    [InspectorName("国家列表")] public List<NationModel> Nations { get; set; }
    [InspectorName("角色列表")] public List<CharacterModel> Characters { get; set; }
    [InspectorName("位置列表")] public List<LocationModel> Locations { get; set; }
    [InspectorName("武器列表")] public List<WeaponModel> Weapons { get; set; }
    [InspectorName("物品列表")] public List<ItemModel> Items { get; set; }
    [InspectorName("Rogue数据")] public RogueLikeData RogueLikeData { get; set; }
}

[Flags]
public enum Alignment
{
    [InspectorName("好")] Good = 1 << 0,
    [InspectorName("普通")] Neutral = 1 << 1,
    [InspectorName("坏")] Evil = 1 << 2
}

public enum Gender
{
    Male,
    Female,
    Other
}

public class NationModel
{
    public string Name { get; set; }
    public List<CharacterModel> Characters { get; set; }
    public List<LocationModel> Locations { get; set; }
}

public class CharacterModel
{
    public string Name { get; set; }
    public int[] Age { get; set; }
    public int[] Level { get; set; }
    public int[] Health { get; set; }
    public int[] Mana { get; set; }
    public Dictionary<string, ItemModel> Inventory { get; set; }
    public List<CharacterModel> Family { get; set; }
    public DateTime BirthDate { get; set; }
    public RaceModel Race { get; set; }
    public Alignment Alignment { get; set; }
    public List<WeaponModel> Weapons { get; set; }
    public Gender Gender { get; set; }
    public string Description { get; set; }
    public string Background { get; set; }
    public List<HistoryModel> Histories { get; set; }
    public bool IsAlive { get; set; }
    public ReligionModel Religion { get; set; }
}

public class ReligionModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<CharacterModel> Characters { get; set; }
    public List<LocationModel> Locations { get; set; }
}

public class HistoryModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<CharacterModel> Characters { get; set; }
    public List<LocationModel> Locations { get; set; }
}

public class LocationModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public List<CharacterModel> Characters { get; set; }
    public List<LocationModel> SubLocations { get; set; }
}

public class WeaponModel
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int Durability { get; set; }
    public int Weight { get; set; }
    public int Value { get; set; }
    public int Damage { get; set; }
    public int Range { get; set; }
}

public class ItemModel
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int Durability { get; set; }
    public int Weight { get; set; }
    public int Value { get; set; }
}

public class RaceModel
{
    public string Name { get; set; }
    public RaceModel ParentRace { get; set; }
    public List<RaceModel> SubRaces { get; set; }
}