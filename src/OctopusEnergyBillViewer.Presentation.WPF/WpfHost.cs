using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace OctopusEnergyBillViewer.Presentation.WPF;

internal class WpfHost
{
    public static void Run(string[] args, Action<IServiceCollection>? configureServicesAction = null, Action<IHostBuilder>? configureHostBuilderAction = null)
    {
        var builder = Host.CreateDefaultBuilder(args);
        builder.ConfigureServices(
            services =>
            {
                services.AddHostedService<WpfHostedService>();
                services.AddSingleton<Application>();
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainWindowViewModel>();
            });
        if (configureServicesAction is not null)
        {
            builder.ConfigureServices(configureServicesAction);
        }
        configureHostBuilderAction?.Invoke(builder);
        if (!Thread.CurrentThread.TrySetApartmentState(ApartmentState.STA))
        {
            Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
            Thread.CurrentThread.SetApartmentState(ApartmentState.STA);
        }
        builder.Build().Run();
    }

    private class WpfHostedService(IHostApplicationLifetime hostApplicationLifetime, Application application, MainWindow mainWindow) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            application.Run(mainWindow);
            hostApplicationLifetime.StopApplication();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
