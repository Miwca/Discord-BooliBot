using Discord.WebSocket;

namespace BooliBot.Discord.Services.Abstractions;

public interface ISlashCommandService
{
    Task BuildSlashCommandsAsync();
    Task HandleTestCommandAsync(SocketSlashCommand command);
}