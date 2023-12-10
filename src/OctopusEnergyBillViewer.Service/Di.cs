using Microsoft.Extensions.DependencyInjection;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using OctopusEnergyBillViewer.Model;
using OctopusEnergyBillViewer.Model.Accounts;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts;
using OctopusEnergyBillViewer.Model.Settings;
using System.Diagnostics;

namespace OctopusEnergyBillViewer.Service;

public static class Di
{
    public static ApplicationService ApplicationService => provider_.GetRequiredService<ApplicationService>();

    private static readonly string processName_ = Process.GetCurrentProcess().ProcessName;
    private static readonly IServiceProvider provider_;

    static Di()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ApplicationService>();
        services.AddSingleton<CredentialManager>();
        services.AddSingleton<AccountFetcher>();
        services.AddSingleton<IOctpusEnergyApi, OctpusEnergyApiGraphql>();
        services.AddSingleton<ISettingRepository, SettingRepositoryJson>();
        services.AddLogging(
            loggingBuilder =>
            {
                var layout = "${longdate} [${level:uppercase=true}] [${logger}] ${message}${onexception:${newline}${exception}}";
                var logFile = new FileTarget("logfile")
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), processName_, "Logs", "${shortdate}.log"),
                    Layout = layout,
                };
                var logdebugger = new DebuggerTarget("logdebugger") { Layout = layout };
                var config = new LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logFile);
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logdebugger);
                loggingBuilder.AddNLog(config);
            }
        );

        provider_ = services.BuildServiceProvider();
    }
}