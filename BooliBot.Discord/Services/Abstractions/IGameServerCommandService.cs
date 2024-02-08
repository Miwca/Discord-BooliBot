using Discord.WebSocket;

namespace BooliBot.Discord.Services.Abstractions
{
    public interface IGameServerCommandService
    {
        Task HandleServerListCommandAsync(SocketSlashCommand command);
        Task HandlePerformanceCommandAsync(SocketSlashCommand command);
        Task HandleServerGetCommandAsync(SocketSlashCommand command);
        Task HandleServerRestartCommandAsync(SocketSlashCommand command);
    }
}
