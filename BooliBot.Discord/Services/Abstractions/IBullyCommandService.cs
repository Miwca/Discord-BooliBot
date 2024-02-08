using Discord.WebSocket;

namespace BooliBot.Discord.Services.Abstractions;

public interface IBullyCommandService
{
    Task HandleRandomBullyQuoteCommandAsync(SocketSlashCommand command);
}