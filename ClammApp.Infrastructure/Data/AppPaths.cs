namespace ClammApp.Infrastructure.Data;

public static class AppPaths
{
    public static string DirectorioDatos { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClammApp");

    public static string RutaBaseDatos { get; } = Path.Combine(DirectorioDatos, "clamm.db");

    public static void AsegurarDirectorio()
    {
        Directory.CreateDirectory(DirectorioDatos);
    }
}
