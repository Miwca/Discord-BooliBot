using BooliBot.Bully.Services.Abstractions;
using BooliBot.Discord.Services.Abstractions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace BooliBot.Discord.Services;

public class BullyCommandService : IBullyCommandService
{
    private readonly ILogger<BullyCommandService> _logger;
    private readonly IBullyService _bullyService;

    public BullyCommandService(ILogger<BullyCommandService> logger, IBullyService bullyService)
    {
        _logger = logger;
        _bullyService = bullyService;
    }

    public async Task HandleRandomBullyQuoteCommandAsync(SocketSlashCommand command)
    {
        var guildUser = (SocketGuildUser?)command.Data.Options.FirstOrDefault()?.Value;
        if (guildUser == null)
        {
            await command.RespondAsync($"{command.User.Mention}, {_bullyService.GetRandomBurn()}");
            return;
        }

        await command.RespondAsync($"{guildUser.Mention}, {_bullyService.GetRandomBurn()}");
    }
}