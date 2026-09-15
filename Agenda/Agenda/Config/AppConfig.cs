using Microsoft.Extensions.Configuration;

namespace Agenda.Config;

public class AppConfig {
    static AppConfig() {
        Config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
    public static IConfiguration Config { get; }
    
    public static string DataFolder => Path.Combine(
        Environment.CurrentDirectory, 
        Config.GetValue<string>("Repository:Directory") ?? "data");
    
    public static bool DropData => Config.GetValue<bool>("Repository:DropData", false);
    
    public static int CacheSize => Config.GetValue("Cache:Size", 5);
}