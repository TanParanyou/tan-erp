using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TanErp.Infrastructure.Persistence;

namespace TanErp.IntegrationTests.Support;

/// <summary>Adjusts the org-wide limit row that <c>TestOnlyDataSeeder</c> gave a role, so tests can exercise the approval authority.</summary>
public static class ApprovalLimitTestSupport
{
    public static async Task SetLimitAsync(
        IServiceProvider services, Guid organizationId, string roleName, string documentType,
        bool isUnlimited, decimal? maxAmount, bool isActive = true)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleId = await db.Roles
            .Where(r => r.OrganizationId == organizationId && r.Name == roleName)
            .Select(r => r.Id)
            .SingleAsync();
        var limit = await db.ApprovalLimits.SingleAsync(l =>
            l.OrganizationId == organizationId && l.RoleId == roleId && l.DocumentType == documentType && l.BranchId == null);
        limit.Update(isUnlimited, maxAmount, isActive, DateTimeOffset.UtcNow);
        await db.SaveChangesAsync();
    }
}
