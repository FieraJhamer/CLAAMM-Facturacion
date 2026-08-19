namespace ClaammApp.Infrastructure.Data;

public static class AppPaths
{
    public static string DirectorioDatos { get; } = AppContext.BaseDirectory;

    public static string RutaBaseDatos { get; } = Path.Combine(DirectorioDatos, "claamm.db");

    public static void AsegurarDirectorio()
    {
        Directory.CreateDirectory(DirectorioDatos);
    }
}
