using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.NetworkInformation;

namespace HealthCheck.Server;

public class ICMPHealthCheck(string host, int healthyRoundtripTime) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(host);
            switch (reply.Status)
            {
                case IPStatus.Success:
                    var msg = $"ICMP to {host} took {reply.RoundtripTime} ms.";
                    return (reply.RoundtripTime > healthyRoundtripTime)
                        ? HealthCheckResult.Degraded(msg)
                        : HealthCheckResult.Healthy(msg);
                default:
                    var err = $"ICMP to {host} failed: {reply.Status}";
                    return HealthCheckResult.Unhealthy(err);
            }
        }
        catch (Exception e)
        {
            var err = $"ICMP failed: {e.Message}";
            return HealthCheckResult.Unhealthy(err);
        }
    }
}