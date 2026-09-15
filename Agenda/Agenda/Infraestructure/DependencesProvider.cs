using Agenda.Config;
using Agenda.Entity;
using Agenda.Models;
using Agenda.Repository;
using Agenda.Service;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Vehiculos.Cache;

namespace Agenda.Infraestructure;

public class DependencesProvider {
    public static IServiceProvider BuildServiceProvider(Action<IServiceCollection>? configureAdditional = null) {
        var service = new ServiceCollection();
        RegisterCache(service);
        RegisterRepository(service);
        RegisterService(service);
        configureAdditional?.Invoke(service);
        return service.BuildServiceProvider();
    }
    private static void RegisterRepository(IServiceCollection services) {
        services.AddSingleton<IContactoRepository>(CreateRepository(AppConfig.DropData));
    }
    private static ContactoRepository CreateRepository(bool dropData) {
        var dataFolder = AppConfig.DataFolder;
        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);
        
        var dbPath = Path.Combine(dataFolder, "itv");
        var context = new AppDbContext($"Data Source={dbPath}");
        
        return new ContactoRepository(context, dropData);
    }
    private static void RegisterCache(IServiceCollection services) {
        services.AddSingleton<ICached<int, Contacto>>(sp =>
            new CacheLru<int, Contacto>(AppConfig.CacheSize));
    }
    private static void RegisterService(IServiceCollection services) {
        services.AddScoped<IContactoService, ContactoService>(sp =>
            new ContactoService(
                sp.GetRequiredService<IContactoRepository>(),
                sp.GetRequiredService<ICached<int, Contacto>>()
            )
        );
    }
}