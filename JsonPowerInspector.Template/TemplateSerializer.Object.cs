using System;
using System.Collections.Generic;

namespace JsonPowerInspector.Template;

public static partial class TemplateSerializer
{
    private static void SerializeObjectProperty(string name, Type propertyType,
        Dictionary<string, ObjectDefinition> referencedPropertyInfo,
        out BaseObjectPropertyInfo baseObjectPropertyInfo, string displayName)
    {
        EnsureTypeExists(propertyType);
        baseObjectPropertyInfo = new ObjectPropertyInfo(name, displayName, GetTypeName(propertyType));

        void EnsureTypeExists(Type type)
        {
            if (type.IsPrimitive || type == typeof(string) || type.IsEnum) return;
            var typeName = GetTypeName(type);
            if (!referencedPropertyInfo.TryAdd(typeName, TempObjectProperty)) return;
            var typeDefinition = CollectTypeDefinitionImpl(type, referencedPropertyInfo);
            referencedPropertyInfo[typeName] = typeDefinition;
        }
    }
}