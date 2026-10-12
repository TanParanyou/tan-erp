using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using TanErp.Api;
using TanErp.Api.Background;
using TanErp.Application.Notifications;
using Xunit;

namespace TanErp.IntegrationTests.Api;

public class MembershipExpiryReminderWorkerTests
{
    private sealed class FakeService : IMembershipExpiryReminderService
    {
        public int Calls { get; private set; }
        public bool Throw { get; set; }

        public Task<MembershipExpiryReminderRunResult> RunOnceAsync(CancellationToken ct = default)
        {
            Calls++;
            if (Throw) throw new InvalidOperationException("boom");
            return Task.FromResult(new MembershipExpiryReminderRunResult(1, 1, 0, false));
        }
    }

    private static (MembershipExpiryReminderWorker Worker, FakeService Service) Create()
    {
        var service = new FakeService();
        var provider = new ServiceCollection().AddScoped<IMembershipExpiryReminderService>(_ => service).BuildServiceProvider();
        var worker = new MembershipExpiryReminderWorker(
            provider.GetRequiredService<IServiceScopeFactory>(), new MembershipExpiryReminderOptions(), NullLogger<MembershipExpiryReminderWorker>.Instance);
        return (worker, service);
    }

    [Fact]
    public async Task ATick_RunsTheServiceOnce()
    {
        var (worker, service) = Create();
        await worker.RunTickAsync(CancellationToken.None);
        Assert.Equal(1, service.Calls);
    }

    [Fact]
    public async Task AFailingRun_IsSwallowed_SoTheScheduleKeepsGoing()
    {
        var (worker, service) = Create();
        service.Throw = true;
        await worker.RunTickAsync(CancellationToken.None);
        service.Throw = false;
        await worker.RunTickAsync(CancellationToken.None);
        Assert.Equal(2, service.Calls);
    }

    [Fact]
    public async Task ACancelledTick_DoesNotThrow()
    {
        var (worker, _) = Create();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        await worker.RunTickAsync(cts.Token);
    }

    [Fact]
    public void TheTestEnvironment_NeverRegistersTheWorker_ButStillResolvesTheService()
    {
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Database"] = "Host=localhost;Database=unused;Username=unused;Password=unused"
            }));
        });

        Assert.DoesNotContain(factory.Services.GetServices<IHostedService>(), s => s is MembershipExpiryReminderWorker);
        using var scope = factory.Services.CreateScope();
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IMembershipExpiryReminderService>());
    }
}
