using BooliBot.Discord.Constants;
using BooliBot.Discord.Services.Abstractions;
using BooliBot.Discord.Settings;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using DiscordConfig = BooliBot.Discord.Settings.DiscordConfig;

namespace BooliBot.Discord.Handlers;

public class SlashCommandHandler
{
    private readonly ILogger<SlashCommandHandler> _logger;
    private readonly DiscordConfig _discordSettings;
    private readonly ISlashCommandService _commandService;
    private readonly IGameServerCommandService _gameServerCommandService;
    private readonly IBullyCommandService _bullyCommandService;

    public SlashCommandHandler(ILogger<SlashCommandHandler> logger, DiscordConfig discordSettings, 
        ISlashCommandService commandService, IGameServerCommandService gameServerCommandService, 
        IBullyCommandService bullyCommandService)
    {
        _logger = logger;
        _discordSettings = discordSettings;
        _commandService = commandService;
        _gameServerCommandService = gameServerCommandService;
        _bullyCommandService = bullyCommandService;
    }

    public async Task SlashCommandExecutedAsync(SocketSlashCommand command)
    {
        try
        {
            switch (command.Data.Name)
            {
                case SlashCommandConstants.Booli:
                    await _bullyCommandService.HandleRandomBullyQuoteCommandAsync(command);
                    break;
                case SlashCommandConstants.BooliTest:
                    await _commandService.HandleTestCommandAsync(command);
                    break;
                case SlashCommandConstants.BooliList:
                    await _gameServerCommandService.HandleServerListCommandAsync(command);
                    break;
                case SlashCommandConstants.BooliPerformance:
                    await _gameServerCommandService.HandlePerformanceCommandAsync(command);
                    break;
                case SlashCommandConstants.BooliGet:
                    await _gameServerCommandService.HandleServerGetCommandAsync(command);
                    break;
                case SlashCommandConstants.BooliRestart:
                    if (!await IsUserAuthorized(command)) return;
                    await _gameServerCommandService.HandleServerRestartCommandAsync(command);
                    break;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred executing slash command.");
            await SendErrorResponseAsync(command, e);
        }
    }

    private async Task<bool> IsUserAuthorized(SocketSlashCommand command)
    {
        var isAdmin = _discordSettings.Admins!.Contains(command.User.Id);
        if (!isAdmin)
        {
            await command.RespondAsync($"You're not authorized to perform this action...", ephemeral: true);
        }

        return isAdmin;
    }

    private async Task SendErrorResponseAsync(SocketSlashCommand command, Exception e)
    {
        var embedBuilder = new EmbedBuilder()
            .WithAuthor(command.User.ToString(), command.User.GetAvatarUrl() ?? command.User.GetDefaultAvatarUrl())
            .WithTitle($"Exception occurred")
            .WithDescription("I encountered an error while performing my task")
            .AddField("Command", command.Data.Name)
            .AddField("Message", e.Message)
            .WithColor(Color.Red)
            .WithCurrentTimestamp();

        await command.RespondAsync(embed: embedBuilder.Build());
    }
}