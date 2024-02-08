using BooliBot.Bully.Services;
using BooliBot.Bully.Services.Abstractions;
using BooliBot.Bully.Settings;
using BooliBot.Discord;
using BooliBot.Discord.Handlers;
using BooliBot.Discord.Services;
using BooliBot.Discord.Services.Abstractions;
using BooliBot.Discord.Settings;
using BooliBot.GameServer.Services;
using BooliBot.GameServer.Services.Abstractions;
using BooliBot.GameServer.Settings;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging.EventLog;
using DiscordConfig = BooliBot.Discord.Settings.DiscordConfig;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(options =>
    {
        if (OperatingSystem.IsWindows())
        {
            options.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Information);
        }
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        var discordSettings = configuration.GetSection("Discord").Get<DiscordConfig>();
        var gameServerSettings = configuration.GetSection("GameServer").Get<GameServerConfig>();
        var bullySettings = configuration.GetSection("Bully").Get<BullyConfig>();
        services.AddSingleton(_ => discordSettings);
        services.AddSingleton(_ => gameServerSettings);
        services.AddSingleton(_ => bullySettings);

        var discordConfig = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.None
        };

        services.AddSingleton(_ => discordConfig);
        services.AddSingleton<DiscordSocketClient>();
        services.AddSingleton<SlashCommandHandler>();
        services.AddSingleton<ISlashCommandService, SlashCommandService>();
        services.AddSingleton<IGameServerCommandService, GameServerCommandService>();
        services.AddSingleton<IBullyCommandService, BullyCommandService>();
        services.AddSingleton<IPerformanceMetricsService, PerformanceMetricsService>();
        services.AddSingleton<IGameProcessService, GameProcessService>();
        services.AddSingleton<IBullyService, BullyService>();

        services.AddHostedService<DiscordClient>();

        if (OperatingSystem.IsWindows())
        {
            services.Configure<EventLogSettings>(config =>
            {
                if (OperatingSystem.IsWindows())
                {
                    config.LogName = "BooliBot";
                    config.SourceName = "BooliBot Source";
                }
            });
        }
    })
    .UseWindowsService()
    .Build();

await host.RunAsync();
