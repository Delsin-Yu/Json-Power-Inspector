using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using JsonPowerInspector.Template;

namespace JsonPowerInspector;

public partial class Main : Control
{
	public const string Extension = ".json";
	
	public override void _Ready()
	{
		GetTree().Root.FilesDropped += files =>
		{
			var matchedFile = files.FirstOrDefault(filePath =>
			{
				var extension = Path.GetExtension(filePath);
				return string.Equals(extension, Extension, StringComparison.OrdinalIgnoreCase);
			});
			if(matchedFile is null) return;
			TryLoadJson(matchedFile);
		};
	}

	private void TryLoadJson(string filePath)
	{
		var fileStream = File.OpenRead(filePath);
		var jsonFileInfo = JsonSerializer.Deserialize(fileStream, PowerTemplateJsonContext.Default.JsonFileInfo);
	}
}