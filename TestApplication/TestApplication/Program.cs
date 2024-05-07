using System;
using JsonPowerInspector.Template;

var definition = TemplateSerializer.CollectDefinition(typeof(MyComplexModel), "Test Model");


Console.WriteLine(definition);



class MyComplexModel
{
    // Create your model here
}