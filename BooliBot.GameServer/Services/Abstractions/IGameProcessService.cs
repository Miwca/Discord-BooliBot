using System.Diagnostics;

namespace BooliBot.GameServer.Services.Abstractions;

public interface IGameProcessService
{
    Process? GetProcessBypath(string path);
}