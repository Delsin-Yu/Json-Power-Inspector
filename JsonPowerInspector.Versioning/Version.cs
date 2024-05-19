namespace JsonPowerInspector;

public static class Version
{
    public static string Current => _versions[^1];

    private static readonly string[] _versions =
    [
        "0.0.1",
        "0.0.2",
    ];
}