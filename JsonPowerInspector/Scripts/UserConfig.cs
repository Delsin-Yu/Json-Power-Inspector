﻿using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Godot;

namespace JsonPowerInspector;

public static partial class UserConfig
{
    [JsonSerializable(typeof(ConfigData))]
    [JsonSourceGenerationOptions(UseStringEnumConverter = true, WriteIndented = true)]
    private partial class ConfigDataContext : JsonSerializerContext;
    
    public static JsonTypeInfo<ConfigData> Context => ConfigDataContext.Default.ConfigData;
    
    public class ConfigData
    {
        public record struct SerializableVector2(int X, int Y)
        {
            public static implicit operator Vector2I(SerializableVector2 a) => new(a.X, a.Y);
            public static implicit operator SerializableVector2(Vector2I a) => new(a.X, a.Y);
        }
        
        public float ScaleFactor { get; set; } = 1;
        public SerializableVector2 Size { get; set; } = new (500, 500);
    }

    public static ConfigData Current { get; set; }

    private const string _configPath = "user://config.json";
    
    public static void LoadConfig()
    {
        if (!FileAccess.FileExists(_configPath))
        {
            Current = new();
            return;
        }
        using var access = FileAccess.Open(_configPath, FileAccess.ModeFlags.Read);
        var jsonText = access.GetAsText(true);
        Current = JsonSerializer.Deserialize(jsonText, Context);
        Current ??= new();
    }

    public static void SaveConfig()
    {
        var jsonText = JsonSerializer.Serialize(Current, Context);
        using var access = FileAccess.Open(_configPath, FileAccess.ModeFlags.WriteRead);
        access.StoreLine(jsonText);
    }
}