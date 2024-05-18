using System.Text.Json.Serialization;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public static partial class Serialization
{
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(byte))]
    [JsonSerializable(typeof(ushort))]
    [JsonSerializable(typeof(uint))]
    [JsonSerializable(typeof(ulong))]
    [JsonSerializable(typeof(sbyte))]
    [JsonSerializable(typeof(short))]
    [JsonSerializable(typeof(int))]
    [JsonSerializable(typeof(long))]
    [JsonSerializable(typeof(float))]
    [JsonSerializable(typeof(double))]
    [JsonSerializable(typeof(UserConfig.ConfigData))]
    [JsonSourceGenerationOptions(UseStringEnumConverter = true, WriteIndented = true)]
    [JsonSerializable(typeof(PackedObjectDefinition))]
    public partial class ApplicationContext : JsonSerializerContext;
    
    public static ApplicationContext Default => ApplicationContext.Default;
}