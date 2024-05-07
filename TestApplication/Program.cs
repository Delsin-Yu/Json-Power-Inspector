using System;
using System.Collections.Generic;
using JsonPowerInspector.Template;
using TMI_RogueLike_DataEditor.Model;

var definition = TemplateSerializer.CollectTypeDefinition(typeof(RogueLikeData), out var referencedPropertyInfo);


Console.WriteLine(definition);

Console.WriteLine("Referenced: \n");

foreach (var value in referencedPropertyInfo.Values)
{
    Console.WriteLine(value);
}


public class MyComplexModel
{
    // Create your ultra complex model here
    public List<NationModel> Nations;
    public List<CharacterModel> Characters;
    public List<LocationModel> Locations;
    public List<WeaponModel> Weapons;
    public List<ItemModel> Items;
}

public enum Alignment
{
    Good,
    Neutral,
    Evil
}

public enum Gender
{
    Male,
    Female,
    Other
}
public class NationModel
{
    public string Name;
    public List<CharacterModel> Characters;
    public List<LocationModel> Locations;
}
public class CharacterModel
{
    public string Name;
    public int Age;
    public int Level;
    public int Health;
    public int Mana;
    public Dictionary<string,ItemModel> Inventory;
    public List<CharacterModel> Family;
    public DateTime BirthDate;
    public RaceModel Race;
    public Alignment Alignment;
    public List<WeaponModel>  Weapons;
    public Gender Gender;
    public string Description;
    public string Background;
    public List<HistoryModel> Histories;
    public bool IsAlive;
    public ReligionModel Religion;
    
}
public class ReligionModel
{
    public string Name;
    public string Description;
    public List<CharacterModel> Characters;
    public List<LocationModel> Locations;
}
public class HistoryModel
{
    public string Name;
    public string Description;
    public List<CharacterModel> Characters;
    public List<LocationModel> Locations;
}
public class LocationModel
{
    public string Name;
    public string Description;
    public List<CharacterModel> Characters;
    public List<LocationModel> SubLocations;
}
public class WeaponModel
{
    public string Name;
    public int Level;
    public int Durability;
    public int Weight;
    public int Value;
    public int Damage;
    public int Range;
}
public class ItemModel
{
    public string Name;
    public int Level;
    public int Durability;
    public int Weight;
    public int Value;
}
public class RaceModel
{
    public string Name;
    public RaceModel ParentRace;
    public List<RaceModel> SubRaces;
}