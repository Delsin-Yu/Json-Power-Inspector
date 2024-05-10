using System.IO;
using System.Text;
using System.Text.Json;
using JsonPowerInspector.Template;
using Tynamix.ObjectFiller;

var definition = TemplateSerializer.CollectTypeDefinition<MyComplexModel>();

File.WriteAllText("definition.jsontemplate", TemplateSerializer.Serialize(definition), Encoding.UTF8);

var filler = new Filler<MyComplexModel>();

var myComplexModel = filler.Create();

File.WriteAllText("data.json", JsonSerializer.Serialize(myComplexModel, TestApplicationJsonContext.Default.MyComplexModel), Encoding.UTF8);
