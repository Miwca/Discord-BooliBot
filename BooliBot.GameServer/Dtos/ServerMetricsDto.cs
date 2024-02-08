namespace BooliBot.GameServer.Dtos;

public class ServerMetricsDto
{
    public CpuMetricsDto? CpuMetrics { get; set; }
    public MemoryMetricsDto? MemoryMetrics { get; set; }
}