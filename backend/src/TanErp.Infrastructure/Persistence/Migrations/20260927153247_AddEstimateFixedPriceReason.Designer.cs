using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TanErp.Infrastructure.Persistence;

namespace TanErp.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260927153247_AddEstimateFixedPriceReason")]
partial class AddEstimateFixedPriceReason
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
    }
}
