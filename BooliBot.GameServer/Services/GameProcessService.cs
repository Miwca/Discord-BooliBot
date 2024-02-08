using System.Diagnostics;
using BooliBot.GameServer.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace BooliBot.GameServer.Services;

public class GameProcessService : IGameProcessService
{
    private readonly ILogger<GameProcessService> _logger;

    public GameProcessService(ILogger<GameProcessService> logger)
    {
        _logger = logger;
    }

    public Process? GetProcessBypath(string path)
    {
        var output = "";

        var info = new ProcessStartInfo
        {
            FileName = "wmic",
            Arguments = $"process where \"ExecutablePath='{path}'\" get ProcessID /Value",
            RedirectStandardOutput = true
        };

        using (var query = Process.Start(info))
        {
            output = query!.StandardOutput.ReadToEnd();
        }

        var lines = output.Trim().Split("\n");
        var responseArr = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
        if (responseArr.Length != 2)
        {
            _logger.LogDebug($"No process found running on path {path}");
            return null;
        }

        var processId = Convert.ToInt32(responseArr[1]);
        var process = Process.GetProcessById(processId);
        return process;
    }
}