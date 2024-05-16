using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using JsonPowerInspector.Template;
using Tynamix.ObjectFiller;

CreateData(TestApplicationJsonContext.Default.TestModels);
CreateData(TestApplicationJsonContext.Default.MySimpleModel);
CreateData(TestApplicationJsonContext.Default.MyComplexModel);
CreateData(TestApplicationJsonContext.Default.MyCollectionModel);
CreateData(TestApplicationJsonContext.Default.MyDropdownModel);
CreateData(TestApplicationJsonContext.Default.RogueLikeData);
CreateData(TestApplicationJsonContext.Default.Ingredient);
CreateData(TestApplicationJsonContext.Default.NationModel);
CreateData(TestApplicationJsonContext.Default.CharacterModel);
CreateData(TestApplicationJsonContext.Default.LocationModel);
CreateData(TestApplicationJsonContext.Default.WeaponModel);
CreateData(TestApplicationJsonContext.Default.ReligionModel);

static void CreateData<T>(JsonTypeInfo<T> typeInfo) where T : class
{
    if (!Directory.Exists("Data"))
    {
        Directory.CreateDirectory("Data");
    }
    var typeName = typeof(T).Name;
    var definition = TemplateSerializer.CollectTypeDefinition<T>();
    File.WriteAllText($"Data/{typeName}.jsontemplate", TemplateSerializer.Serialize(definition), Encoding.UTF8);
    var filler = new Filler<T>();
    filler
        .Setup()
        .DictionaryItemCount(5, 10)
        .ListItemCount(5, 10);
    
    var model = filler.Create();
    File.WriteAllText($"Data/{typeName}.json", JsonSerializer.Serialize(model, typeInfo), Encoding.UTF8);
}