using BooliBot.Common.Extensions;
using BooliBot.Discord.Services.Abstractions;
using BooliBot.GameServer.Services.Abstractions;
using BooliBot.GameServer.Settings;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BooliBot.Discord.Services
{
    public class GameServerCommandService : IGameServerCommandService
    {
        private readonly ILogger<GameServerCommandService> _logger;
        private readonly GameServerConfig _serverSettings;
        private readonly IPerformanceMetricsService _performanceMetricsService;
        private readonly IGameProcessService _gameProcessService;

        public GameServerCommandService(ILogger<GameServerCommandService> logger, DiscordSocketClient discordClient, 
            GameServerConfig serverSettings, IPerformanceMetricsService performanceMetricsService,
            IGameProcessService gameProcessService)
        {
            _logger = logger;
            _serverSettings = serverSettings;
            _performanceMetricsService = performanceMetricsService;
            _gameProcessService = gameProcessService;
        }

        public async Task HandleServerListCommandAsync(SocketSlashCommand command)
        {
            var gameServers = _serverSettings.GameServers!.Keys;
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(command.User.ToString(), command.User.GetAvatarUrl() ?? command.User.GetDefaultAvatarUrl())
                .WithTitle("Registered game servers")
                .WithDescription(string.Join(",\n", gameServers.Select(gs => gs.FirstCharToUpper())))
                .WithColor(Color.Green)
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embedBuilder.Build());
        }

        public async Task HandlePerformanceCommandAsync(SocketSlashCommand command)
        {
            _logger.LogDebug("Fetching server performance metrics...");

            var metrics = _performanceMetricsService.GetMetrics();
            var lowLoad = (metrics.CpuMetrics!.LoadPercentage < 25) && ((metrics.MemoryMetrics!.Used / metrics.MemoryMetrics!.Total) * 100 < 25);
            var highLoad = (metrics.CpuMetrics!.LoadPercentage < 90) && ((metrics.MemoryMetrics!.Used / metrics.MemoryMetrics!.Total) * 100 < 90);

            var embedBuilder = new EmbedBuilder()
                .WithAuthor(command.User.ToString(), command.User.GetAvatarUrl() ?? command.User.GetDefaultAvatarUrl())
                .WithTitle("Server Metrics")
                .WithDescription("Current server metrics")
                .AddField("CPU Usage", $"{metrics.CpuMetrics!.LoadPercentage}%", true)
                .AddField("RAM Usage", $"{Math.Round(metrics.MemoryMetrics!.Used / metrics.MemoryMetrics!.Total * 100)}%", true)
                .AddField("RAM Used", metrics.MemoryMetrics!.Used)
                .AddField("RAM Free", metrics.MemoryMetrics!.Free)
                .AddField("RAM Total", metrics.MemoryMetrics!.Total)
                .WithColor(GetLoadColor(lowLoad, highLoad))
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embedBuilder.Build());
        }

        public async Task HandleServerGetCommandAsync(SocketSlashCommand command)
        {
            _logger.LogDebug("Fetching game server information...");

            var serverIdentifier = command.Data.Options.FirstOrDefault()?.Value.ToString();
            if (serverIdentifier == null)
            {
                await command.RespondAsync($"No server identifier provided...");
                return;
            }

            var serverFound = _serverSettings.GameServers!.TryGetValue(serverIdentifier.ToLower(), out var gameServer);
            if (!serverFound)
            {
                await command.RespondAsync($"No server found with identifier {serverIdentifier.FirstCharToUpper()}");
                return;
            }

            var gameProcess = _gameProcessService.GetProcessBypath(gameServer!.Location!);
            var gameRunning = gameProcess != null;

            var embedBuilder = new EmbedBuilder()
                .WithAuthor(command.User.ToString(), command.User.GetAvatarUrl() ?? command.User.GetDefaultAvatarUrl())
                .WithTitle($"{serverIdentifier.FirstCharToUpper()} Information")
                .WithDescription("Game server information")
                .AddField("Server Name", gameServer!.ServerName)
                .AddField("Server Status", gameRunning ? "Running" : "Not running")
                .AddField("Server Access", gameServer.Access)
                .AddField("Server Password", string.IsNullOrEmpty(gameServer.Password) ? "No password" : gameServer.Password)
                .WithColor(gameRunning ? Color.Green : Color.Red)
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embedBuilder.Build());
        }

        public async Task HandleServerRestartCommandAsync(SocketSlashCommand command)
        {
            var serverIdentifier = command.Data.Options.FirstOrDefault()?.Value.ToString();
            if (serverIdentifier == null)
            {
                await command.RespondAsync($"No server identifier provided...");
                return;
            }

            _logger.LogDebug($"Performing server restart of identifier {serverIdentifier}");

            var serverFound = _serverSettings.GameServers!.TryGetValue(serverIdentifier.ToLower(), out var gameServer);
            if (!serverFound)
            {
                await command.RespondAsync($"No server found with identifier {serverIdentifier.FirstCharToUpper()}");
                return;
            }

            var gameProcess = _gameProcessService.GetProcessBypath(gameServer!.Location!);
            if (gameProcess == null)
            {
                await command.RespondAsync($"No running server with identifier {serverIdentifier.FirstCharToUpper()}. If you just restarted it then wait.");
                return;
            }

            gameProcess.Kill();

            var embedBuilder = new EmbedBuilder()
                .WithAuthor(command.User.ToString(), command.User.GetAvatarUrl() ?? command.User.GetDefaultAvatarUrl())
                .WithTitle($"{serverIdentifier} restarting")
                .WithDescription("Game server is updating and restarting...")
                .WithColor(Color.Orange)
                .WithCurrentTimestamp();

            await command.RespondAsync(embed: embedBuilder.Build());
        }

        private Color GetLoadColor(bool lowLoad, bool highLoad)
        {
            if (lowLoad)
            {
                return Color.Green;
            }

            if (highLoad)
            {
                return Color.Red;
            }

            return Color.Orange;
        }
    }
}
