using System.IO;
using System.Text;
using System.Text.Json;
using JsonPowerInspector.Template;
using Tynamix.ObjectFiller;

var definition = TemplateSerializer.CollectTypeDefinition<MyCollectionModel>();

File.WriteAllText("definition.jsontemplate", TemplateSerializer.Serialize(definition), Encoding.UTF8);

var filler = new Filler<MyCollectionModel>();

var model = filler.Create();

File.WriteAllText("data.json", JsonSerializer.Serialize(model, TestApplicationJsonContext.Default.MyCollectionModel), Encoding.UTF8);
