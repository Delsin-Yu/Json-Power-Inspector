using System;
using JsonPowerInspector.Template;

var definition = TemplateSerializer.CollectDefinition(typeof(MyComplexModel), "Test Model");


Console.WriteLine(definition);



public class MyComplexModel
{
    // Create your ultra complex model here
}