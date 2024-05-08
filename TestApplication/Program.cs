using System.IO;
using System.Text;
using JsonPowerInspector.Template;

var definition = TemplateSerializer.CollectTypeDefinition<MyComplexModel>();

File.WriteAllText("definition.jsontemplate", TemplateSerializer.Serialize(definition), Encoding.UTF8);