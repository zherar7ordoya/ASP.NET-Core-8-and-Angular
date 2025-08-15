using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.NetworkInformation;

namespace HealthCheck.Server;

public class ICMPHealthCheck : IHealthCheck
{
    private readonly string Host = $"10.0.0.0";
    private readonly int HealthyRoundtripTime = 300;
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
                                                          CancellationToken cancellationToken = default)
    {
        try
        {
            using var ping = new Ping();
            var reply = await ping.SendPingAsync(Host);

            return reply.Status switch
            {
                IPStatus.Success => (reply.RoundtripTime > HealthyRoundtripTime)
                                        ? HealthCheckResult.Degraded()
                                        : HealthCheckResult.Healthy(),
                _ => HealthCheckResult.Unhealthy(),
            };
        }
        catch { return HealthCheckResult.Unhealthy(); }
    }
}