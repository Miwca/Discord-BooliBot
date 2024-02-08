using System.Diagnostics;
using BooliBot.GameServer.Dtos;
using BooliBot.GameServer.Services.Abstractions;

namespace BooliBot.GameServer.Services
{
    public class PerformanceMetricsService : IPerformanceMetricsService
    {
        public ServerMetricsDto GetMetrics()
        {
            var memoryMetrics = GetMemoryMetrics();
            var cpuMetrics = GetCpuMetrics();

            return new ServerMetricsDto()
            {
                MemoryMetrics = memoryMetrics,
                CpuMetrics = cpuMetrics
            };
        }

        private CpuMetricsDto GetCpuMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo();
            info.FileName = "wmic";
            info.Arguments = "CPU GET LoadPercentage";
            info.RedirectStandardOutput = true;

            using (var process = Process.Start(info))
            {
                output = process!.StandardOutput.ReadToEnd();
            }

            var lines = output.Trim().Split("\n");
            var cpuLoadPercentageStr = lines[1];
            var cpuLoadPercentage = double.Parse(cpuLoadPercentageStr);

            return new CpuMetricsDto
            {
                LoadPercentage = cpuLoadPercentage,
                UnusedPercentage = 100 - cpuLoadPercentage
            };
        }

        private MemoryMetricsDto GetMemoryMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo();
            info.FileName = "wmic";
            info.Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value";
            info.RedirectStandardOutput = true;

            using (var process = Process.Start(info))
            {
                output = process!.StandardOutput.ReadToEnd();
            }

            var lines = output.Trim().Split("\n");
            var freeMemoryParts = lines[0].Split("=", StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1].Split("=", StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetricsDto();
            metrics.Total = Math.Round(double.Parse(totalMemoryParts[1]) / 1024, 0);
            metrics.Free = Math.Round(double.Parse(freeMemoryParts[1]) / 1024, 0);
            metrics.Used = metrics.Total - metrics.Free;

            return metrics;
        }

        private MemoryMetricsDto GetUnixMetrics()
        {
            var output = "";

            var info = new ProcessStartInfo("free -m");
            info.FileName = "/bin/bash";
            info.Arguments = "-c \"free -m\"";
            info.RedirectStandardOutput = true;

            using (var process = Process.Start(info))
            {
                output = process!.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
            }

            var lines = output.Split("\n");
            var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var metrics = new MemoryMetricsDto();
            metrics.Total = double.Parse(memory[1]);
            metrics.Used = double.Parse(memory[2]);
            metrics.Free = double.Parse(memory[3]);

            return metrics;
        }
    }
}
