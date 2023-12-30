using Microsoft.Extensions.DependencyInjection;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using OctopusEnergyBillViewer.Model;
using OctopusEnergyBillViewer.Model.Accounts;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis;
using OctopusEnergyBillViewer.Model.OctopusEnergyApis.Accounts;
using OctopusEnergyBillViewer.Model.Settings;
using OctopusEnergyBillViewer.Presentation.WPF;
using OctopusEnergyBillViewer.Presentation.WPF.Dialogs;
using System.Diagnostics;
using System.IO;

WpfHost.Run(
    args,
    services =>
    {
        services.AddTransient<LoginDialog>();
        services.AddTransient<LoginDialogViewModel>();
        services.AddSingleton<IDialogService<LoginDialog>, DialogService<LoginDialog>>();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();

        services.AddSingleton<ApplicationService>();
        services.AddSingleton<CredentialManager>();
        services.AddSingleton<AccountFetcher>();
        services.AddSingleton<IOctopusEnergyApi, OctopusEnergyApiGraphql>();
        services.AddSingleton<ISettingRepository, SettingRepositoryJson>();
        services.AddLogging(
            loggingBuilder =>
            {
                var processName = Process.GetCurrentProcess().ProcessName;
                var layout = "${longdate} [${level:uppercase=true}] [${logger}] ${message}${onexception:${newline}${exception}}";
                var logFile = new FileTarget("logfile")
                {
                    FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), processName, "Logs", "${shortdate}.log"),
                    Layout = layout,
                };
                var logdebugger = new DebuggerTarget("logdebugger") { Layout = layout };
                var config = new LoggingConfiguration();
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logFile);
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, logdebugger);
                loggingBuilder.AddNLog(config);
            });
    });
