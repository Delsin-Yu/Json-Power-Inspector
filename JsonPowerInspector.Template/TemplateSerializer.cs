using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace JsonPowerInspector.Template;

public static class TemplateSerializer
{
    public static JsonFileInfo Deserialize(string templateFilePath)
    {
        using var fileStream = File.OpenRead(templateFilePath);
        return JsonSerializer.Deserialize(fileStream, PowerTemplateJsonContext.Default.JsonFileInfo);
    }

    [RequiresUnreferencedCode("CollectDefinition is intended to be used in editor to generate JsonFileInfo")]
    public static ObjectDefinition CollectDefinition(Type objectType, string objectName = null)
    {
        var propertyInfos = objectType.GetProperties(BindingFlags.Instance);

        var properties = new List<ObjectPropertyInfo>();
        
        foreach (var propertyInfo in propertyInfos)
        {
            var propertyType = propertyInfo.PropertyType;
            if (propertyType.IsArray)
            {
                
            }
            
        }
                
        var definition = new ObjectDefinition()
        {
            ObjectTypeName = objectName ?? objectType.Name,
        };
        
        return null;
    }
}