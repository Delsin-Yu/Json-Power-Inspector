using System.IO;
using System.Text;
using System.Text.Json;
using JsonPowerInspector.Template;
using TMI_RogueLike_DataEditor.Model;
using Tynamix.ObjectFiller;

var definition = TemplateSerializer.CollectTypeDefinition<TestModels>();

File.WriteAllText("definition.jsontemplate", TemplateSerializer.Serialize(definition), Encoding.UTF8);

var filler = new Filler<TestModels>();

filler
    .Setup()
    .DictionaryItemCount(5, 10)
    .ListItemCount(5, 10);

var model = filler.Create();

File.WriteAllText("data.json", JsonSerializer.Serialize(model, TestApplicationJsonContext.Default.TestModels), Encoding.UTF8);
