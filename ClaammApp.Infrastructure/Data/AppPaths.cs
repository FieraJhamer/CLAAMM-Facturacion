namespace ClaammApp.Infrastructure.Data;

public static class AppPaths
{
    public static string DirectorioDatos { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClaammApp");

    public static string RutaBaseDatos { get; } = Path.Combine(DirectorioDatos, "claamm.db");

    public static void AsegurarDirectorio()
    {
        Directory.CreateDirectory(DirectorioDatos);
    }
}
