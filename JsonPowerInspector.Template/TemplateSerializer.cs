using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

[assembly: InternalsVisibleTo("JsonPowerInspector")]

namespace JsonPowerInspector.Template;

/// <summary>
/// Contains helpers to collecting and creating jsontemplate for a user type for JsonPowerInspector to use. 
/// </summary>
public static partial class TemplateSerializer
{
    /// <summary>
    /// Serialize the specified <see cref="PackedObjectDefinition"/> data model to the jsontemplate string.
    /// The developer should save the result into a .jsontemplate file for JsonPowerInspector usage.
    /// </summary>
    /// <param name="packedObjectDefinition">The data model to serialize from.</param>
    /// <returns>A JSON string that should be saved into a .jsontemplate file for JsonPowerInspector usage.</returns>
    public static string Serialize(PackedObjectDefinition packedObjectDefinition) => 
        JsonSerializer.Serialize(packedObjectDefinition, PowerTemplateJsonContext.Default.PackedObjectDefinition);

    /// <summary>
    /// Collect the required serialization info for the specified type for JsonPowerInspector to use.
    /// </summary>
    /// <typeparam name="T">The type for collecting the info from.</typeparam>
    /// <returns>A data model for serializing to .jsontemplate file.</returns>
    public static PackedObjectDefinition CollectTypeDefinition<T>() => CollectTypeDefinition(typeof(T));

    /// <summary>
    /// Collect the required serialization info for the specified type for JsonPowerInspector to use.
    /// </summary>
    /// <param name="objectType">The type for collecting the info from.</param>
    /// <returns>A data model for serializing to .jsontemplate file.</returns>
    [RequiresUnreferencedCode("CollectDefinition is intended to be used in editor to generate JsonFileInfo only.")]
    public static PackedObjectDefinition CollectTypeDefinition(Type objectType)
    {
        var referencedPropertyInfo = new Dictionary<string, ObjectDefinition>();
        var mainObjectDefinition = CollectTypeDefinitionImpl(objectType, referencedPropertyInfo);
        return new(mainObjectDefinition, referencedPropertyInfo.Values.ToArray());
    }
    
    private static ObjectDefinition CollectTypeDefinitionImpl(Type objectType, Dictionary<string, ObjectDefinition> referencedPropertyInfo)
    {
        var propertyInfos = 
            objectType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x is { CanRead: true, CanWrite: true })
                .ToArray();
        
        var properties = new List<BaseObjectPropertyInfo>();
        foreach (var propertyInfo in propertyInfos)
        {
            if (TryParseProperty(
                    propertyInfo.Name,
                    propertyInfo.PropertyType,
                    referencedPropertyInfo,
                    propertyInfo.GetCustomAttributes(),
                    out var parsed
                ))
            {
                properties.Add(parsed);
            }
        }

        var definition = new ObjectDefinition(GetTypeName(objectType), properties.ToArray());
        
        return definition;
    }

    private static string GetTypeName(Type type) => type.Name;

    private static readonly HashSet<Type> SerializeToStringTypes =
    [
        typeof(DateTime)
    ];

    private static readonly ObjectDefinition TempObjectProperty = new("TEMP", Array.Empty<BaseObjectPropertyInfo>());

    private static bool TryParseProperty(
        string name,
        Type propertyType,
        Dictionary<string, ObjectDefinition> referencedPropertyInfo,
        IEnumerable<Attribute> attributes,
        [NotNullWhen(true)] out BaseObjectPropertyInfo? baseObjectPropertyInfo
    )
    {
        var attributesArray = attributes as Attribute[] ?? attributes.ToArray();
        var inspectorName = attributesArray.OfType<InspectorNameAttribute>().FirstOrDefault();
        var displayName = inspectorName != null ? inspectorName.DisplayName : name;

        baseObjectPropertyInfo = null;

        if (SerializeToStringTypes.Contains(propertyType))
        {
            baseObjectPropertyInfo = new StringPropertyInfo(name, displayName);
        }
        else if (propertyType.IsArray)
        {
            if (!SerializeArrayProperty(name, propertyType, referencedPropertyInfo, ref baseObjectPropertyInfo,
                    attributesArray, displayName)) return false;
        }
        else if (propertyType.IsGenericType)
        {
            if (!SerializeGenericProperty(name, propertyType, referencedPropertyInfo, ref baseObjectPropertyInfo,
                    attributesArray, displayName)) return false;
        }
        else if (propertyType.IsPrimitive)
        {
            if (!SerializePrimitiveProperty(name, propertyType, ref baseObjectPropertyInfo, displayName,
                    attributesArray, false)) return false;
        }
        else if (propertyType == typeof(string))
        {
            SerializeStringProperty(name, out baseObjectPropertyInfo, attributesArray, displayName);
        }
        else if (propertyType.IsEnum)
        {
            SerializeEnumProperty(name, propertyType, out baseObjectPropertyInfo, displayName);
        }
        else if (!propertyType.IsGenericType)
        {
            SerializeObjectProperty(name, propertyType, referencedPropertyInfo, out baseObjectPropertyInfo,
                displayName);
        }
        else
        {
            return false;
        }

        return true;
    }
}