using System.IO;
using System.Text;
using System.Text.Json;
using JsonPowerInspector.Template;
using TMI_RogueLike_DataEditor.Model;
using Tynamix.ObjectFiller;

var definition = TemplateSerializer.CollectTypeDefinition<MyDictionaryModel>();

File.WriteAllText("definition.jsontemplate", TemplateSerializer.Serialize(definition), Encoding.UTF8);

var filler = new Filler<MyDictionaryModel>();

var myComplexModel = filler.Create();

File.WriteAllText("data.json", JsonSerializer.Serialize(myComplexModel, TestApplicationJsonContext.Default.MyDictionaryModel), Encoding.UTF8);
