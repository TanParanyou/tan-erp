using TanErp.Domain.Service;
using Xunit;

namespace TanErp.UnitTests.Service;

public class ServiceDomainTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 4, 0, 0, 0, TimeSpan.Zero);
    private static readonly Guid Org = Guid.NewGuid();

    private static DateOnly D(string value) => DateOnly.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

    [Theory]
    [InlineData("2026-10-04", 12, "2027-10-03")]
    [InlineData("2026-01-15", 1, "2026-02-14")]
    [InlineData("2026-01-31", 1, "2026-02-28")]
    [InlineData("2026-01-30", 1, "2026-02-28")]
    [InlineData("2024-02-29", 12, "2025-02-28")]
    public void Warranty_EndsTheDayBeforeTheAnniversary_OrOnTheLastDayOfAShortMonth(string start, int months, string expectedEnd)
    {
        var warranty = new Warranty(Guid.NewGuid(), Org, Guid.NewGuid(), Guid.NewGuid(), "WAR-1", D(start), months, Now);
        Assert.Equal(D(expectedEnd), warranty.EndDate);
        Assert.True(warranty.Covers(D(start)));
        Assert.True(warranty.Covers(warranty.EndDate));
        Assert.False(warranty.Covers(warranty.EndDate.AddDays(1)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(121)]
    public void Warranty_RejectsOutOfRangeTerms(int months)
    {
        Assert.Equal("INSTALLATION_FIELD_INVALID", Assert.Throws<ServiceDomainException>(() =>
            new Warranty(Guid.NewGuid(), Org, Guid.NewGuid(), Guid.NewGuid(), "WAR-1", new DateOnly(2026, 10, 4), months, Now)).Code);
    }

    private static InstallationJob NewJob()
    {
        var job = new InstallationJob(Guid.NewGuid(), Org, Guid.NewGuid(), Guid.NewGuid(), "INS-1", new DateOnly(2026, 11, 1), new DateOnly(2026, 11, 3), null, null, Guid.NewGuid(), Now);
        job.AddChecklistItem(new InstallationChecklistItem(Guid.NewGuid(), Org, job.Id, 1, "ตรวจงาน", true));
        return job;
    }

    [Fact]
    public void DefectVerification_NeedsADifferentPerson_AndAReopenedDefectBlocksReady()
    {
        var job = NewJob();
        var fixer = Guid.NewGuid();
        var checker = Guid.NewGuid();
        job.Start(Now);
        job.SetChecklistDone(job.Checklist.Single().Id, true, fixer, Now);
        var defect = job.ReportDefect("รอย", DefectSeverity.Minor, fixer, Now);
        job.ResolveDefect(defect.Id, "ซ่อม", fixer, Now);
        Assert.Equal("INSTALLATION_SELF_VERIFICATION", Assert.Throws<ServiceDomainException>(() => job.VerifyDefect(defect.Id, fixer, Now)).Code);
        job.VerifyDefect(defect.Id, checker, Now);
        job.MarkReady(Now);
        Assert.Equal(InstallationStatus.ReadyForHandover, job.Status);

        job.ReopenDefect(defect.Id, "กลับมา", checker, Now);
        Assert.Equal(InstallationStatus.InProgress, job.Status);
        Assert.Equal("INSTALLATION_DEFECTS_OPEN", Assert.Throws<ServiceDomainException>(() => job.MarkReady(Now)).Code);
    }

    [Fact]
    public void Handover_RequiresReady_ExplicitWarranty_AndNoFutureDate()
    {
        var job = NewJob();
        var today = new DateOnly(2026, 10, 4);
        Assert.Equal("INSTALLATION_INVALID_STATE", Assert.Throws<ServiceDomainException>(() => job.Handover(HandoverOutcome.Accepted, "คุณสมชาย", null, 12, today, today, Now)).Code);
        job.Start(Now);
        job.SetChecklistDone(job.Checklist.Single().Id, true, Guid.NewGuid(), Now);
        job.MarkReady(Now);
        Assert.Equal("INSTALLATION_FIELD_INVALID", Assert.Throws<ServiceDomainException>(() => job.Handover(HandoverOutcome.Accepted, "คุณสมชาย", null, null, today, today, Now)).Code);
        Assert.Equal("INSTALLATION_FIELD_INVALID", Assert.Throws<ServiceDomainException>(() => job.Handover(HandoverOutcome.Accepted, "คุณสมชาย", null, 12, today.AddDays(1), today, Now)).Code);
        job.Handover(HandoverOutcome.Accepted, "คุณสมชาย", null, 0, today, today, Now);
        Assert.Equal(InstallationStatus.HandedOver, job.Status);
    }

    [Fact]
    public void ServiceRequest_FollowsItsFlow_AndKeepsAnEventPerStep()
    {
        var actor = Guid.NewGuid();
        var request = new ServiceRequest(Guid.NewGuid(), Org, Guid.NewGuid(), Guid.NewGuid(), null, "SRV-1", "หัวข้อ", "รายละเอียด", ServicePriority.Normal, actor, Now);
        Assert.False(request.InWarranty);
        Assert.Equal("SERVICE_INVALID_STATE", Assert.Throws<ServiceDomainException>(() => request.Close(actor, Now)).Code);
        request.Start(actor, Now);
        request.Resolve("เสร็จ", actor, Now);
        request.Close(actor, Now);
        request.Reopen("ซ้ำ", actor, Now);
        Assert.Equal(new[] { "open", "in_progress", "resolved", "closed", "in_progress" }, request.Events.Select(e => e.ToStatus).ToArray());
        Assert.Equal(1, request.ReopenCount);
    }
}
