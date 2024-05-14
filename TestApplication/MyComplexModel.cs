using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TMI_RogueLike_DataEditor.Model;

[JsonSerializable(typeof(TestModels))]
[JsonSourceGenerationOptions(UseStringEnumConverter = true, WriteIndented = true)]
public partial class TestApplicationJsonContext : JsonSerializerContext;

public class TestModels
{
    public MySimpleModel MySimpleModel { get; set; }
    public MyComplexModel MyComplexModel { get; set; }
    public MyCollectionModel MyCollectionModel { get; set; }
}

public class MyCollectionModel
{
    public MyDictionaryModel MyDictionaryModel { get; set; }
    public MyArrayModel MyArrayModel { get; set; }
}
public class MyDictionaryModel
{
    public Dictionary<string, int> StrInt { get; set; }
    public Dictionary<int, string> IntStr { get; set; }
    public Dictionary<float, string> FPStr { get; set; }
}

public class MyArrayModel
{
    public string[] StrArr { get; set; }
    public int[] IntArr { get; set; }
    public MySimpleModel[] MySimpleModels { get; set; }
}

public class MySimpleModel
{
    public NationModel NationModel { get; set; }
    public CharacterModel CharacterModel { get; set; }
    public LocationModel LocationModel { get; set; }
    public WeaponModel WeaponModel { get; set; }
    public ItemModel ItemModel { get; set; }
}

public class MyComplexModel
{
    public Dictionary<int, string> Data { get; set; }
    public Dictionary<string, CharacterModel> Data2 { get; set; }
    public List<NationModel> Nations { get; set; }
    public List<CharacterModel> Characters { get; set; }
    public List<LocationModel> Locations { get; set; }
    public List<WeaponModel> Weapons { get; set; }
    public List<ItemModel> Items { get; set; }
    public RogueLikeData RogueLikeData { get; set; }
}

[Flags]
public enum Alignment
{
    Good = 1 << 0,
    Neutral = 1 << 1,
    Evil = 1 << 2
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