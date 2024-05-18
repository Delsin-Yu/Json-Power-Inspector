// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

var programVersion = JsonPowerInspector.Version.Current;

var repoDir = Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "..");

var projectPath = Path.GetFullPath(Path.Combine(repoDir, "JsonPowerInspector"));
var publishDir = Path.GetFullPath(Path.Combine(repoDir, "Build", $"Json Power Inspector v{programVersion}"));
var publishPath = $"../Build/\"Json Power Inspector v{programVersion}\"/\"Json Power Inspector.exe\"";
var godotPath = args[0];

Console.WriteLine(
    $"""
     Publishing version: {programVersion},
     Project Path: {projectPath},
     Publish Path: {publishPath},
     Godot Path: {args[0]}
     """
);

Directory.CreateDirectory(publishDir);


var processStartInfo = new ProcessStartInfo
{
    WorkingDirectory = projectPath,
    Arguments = $"--headless --export-release Win64 {publishPath}",
    FileName = godotPath
};

await Process.Start(processStartInfo)!.WaitForExitAsync();

Console.WriteLine("Publish finish");