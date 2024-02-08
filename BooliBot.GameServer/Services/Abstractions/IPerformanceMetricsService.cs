using BooliBot.GameServer.Dtos;

namespace BooliBot.GameServer.Services.Abstractions;

public interface IPerformanceMetricsService
{
    ServerMetricsDto GetMetrics();
}