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

    [Fact]
    public void ServiceRequest_WaitCustomer_AndResume_PausesAndAccumulatesTime()
    {
        var actor = Guid.NewGuid();
        var request = new ServiceRequest(Guid.NewGuid(), Org, Guid.NewGuid(), Guid.NewGuid(), null, "SRV-2", "หัวข้อ", "รายละเอียด", ServicePriority.Normal, actor, Now);
        request.Start(actor, Now);
        request.WaitCustomer("รอลูกค้าส่งรูปเพิ่ม", actor, Now);

        Assert.Equal(ServiceRequestStatus.WaitingCustomer, request.Status);
        Assert.Equal(Now, request.PausedAtUtc);
        Assert.Equal(0, request.PausedSeconds);

        var resumeTime = Now.AddHours(3);
        request.ResumeFromWaitCustomer(actor, resumeTime);

        Assert.Equal(ServiceRequestStatus.InProgress, request.Status);
        Assert.Null(request.PausedAtUtc);
        Assert.Equal(3 * 3600, request.PausedSeconds);

        // Resolve also flushes pause if still paused
        request.WaitCustomer("รอลูกค้าตรวจหน้างาน", actor, resumeTime);
        var resolveTime = resumeTime.AddHours(2);
        request.Resolve("เรียบร้อย", actor, resolveTime);

        Assert.Equal(ServiceRequestStatus.Resolved, request.Status);
        Assert.Null(request.PausedAtUtc);
        Assert.Equal((3 + 2) * 3600, request.PausedSeconds);
        Assert.Equal(resolveTime, request.ResolvedAtUtc);
    }

    [Fact]
    public void ServiceSlaCalculator_CalculatesElapsed_DeductsHolidays_AndTracksStatus()
    {
        var actor = Guid.NewGuid();
        var createdAt = new DateTimeOffset(2026, 10, 5, 8, 0, 0, TimeSpan.Zero); // Monday
        var request = new ServiceRequest(Guid.NewGuid(), Org, Guid.NewGuid(), Guid.NewGuid(), null, "SRV-3", "หัวข้อ", "รายละเอียด", ServicePriority.Normal, actor, createdAt);

        // Policy for normal is 8 response, 48 resolve
        var (respH, resH) = ServiceSlaCalculator.GetPolicyHours(ServicePriority.Normal);
        Assert.Equal((8, 48), (respH, resH));

        // 1. Initial on-track: 10 hours elapsed with no holidays or pauses
        var t1 = createdAt.AddHours(10);
        var eval1 = ServiceSlaCalculator.Evaluate(request, t1, new HashSet<DateOnly>());
        Assert.Equal(10.0m, eval1.ElapsedHours);
        Assert.Equal(SlaStatus.OnTrack, eval1.SlaStatus);

        // 2. Holiday deduction: Tuesday 2026-10-06 is holiday
        var holidays = new HashSet<DateOnly> { new DateOnly(2026, 10, 6) };
        var t2 = createdAt.AddDays(2); // Wednesday 48 hours later (covers full Tuesday = 24h holiday)
        var eval2 = ServiceSlaCalculator.Evaluate(request, t2, holidays);
        Assert.Equal(24.0m, eval2.ElapsedHours); // 48h - 24h holiday = 24h
        Assert.Equal(SlaStatus.OnTrack, eval2.SlaStatus);

        // 3. Warning status at >= 80% (48 * 0.8 = 38.4 hours):
        // 39 hours elapsed
        var t3 = createdAt.AddHours(39);
        var eval3 = ServiceSlaCalculator.Evaluate(request, t3, new HashSet<DateOnly>());
        Assert.Equal(39.0m, eval3.ElapsedHours);
        Assert.Equal(SlaStatus.Warning, eval3.SlaStatus);

        // 4. Breached status when > 48 hours:
        var t4 = createdAt.AddHours(50);
        var eval4 = ServiceSlaCalculator.Evaluate(request, t4, new HashSet<DateOnly>());
        Assert.Equal(50.0m, eval4.ElapsedHours);
        Assert.Equal(SlaStatus.Breached, eval4.SlaStatus);

        // 5. Paused status when WaitingCustomer:
        request.Start(actor, t1);
        request.WaitCustomer("รอลูกค้า", actor, t1);
        var evalPaused = ServiceSlaCalculator.Evaluate(request, t4, new HashSet<DateOnly>());
        Assert.Equal(SlaStatus.Paused, evalPaused.SlaStatus);

        // 6. Met status when Resolved within resolve hours:
        request.ResumeFromWaitCustomer(actor, t1.AddHours(2));
        var resolveTime = createdAt.AddHours(30);
        request.Resolve("เสร็จสิ้น", actor, resolveTime);
        var evalMet = ServiceSlaCalculator.Evaluate(request, resolveTime.AddDays(10), new HashSet<DateOnly>());
        Assert.Equal(SlaStatus.Met, evalMet.SlaStatus);
    }

    [Fact]
    public void ServiceRequest_RecordServiceCharge_RejectsInWarranty_AndBilledDuplicates()
    {
        var actor = Guid.NewGuid();
        var billingId = Guid.NewGuid();

        // 1. In warranty cannot be charged
        var inWarrantyRequest = new ServiceRequest(Guid.NewGuid(), Org, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "SRV-W", "หัวข้อ", "รายละเอียด", ServicePriority.Normal, actor, Now);
        Assert.True(inWarrantyRequest.InWarranty);
        var exInWarranty = Assert.Throws<ServiceDomainException>(() =>
            inWarrantyRequest.RecordServiceCharge(1500m, "ค่าบริการ", billingId, "BIL-1", actor, Now));
        Assert.Equal("SERVICE_CHARGE_IN_WARRANTY", exInWarranty.Code);

        // 2. Out of warranty can be charged
        var outWarrantyRequest = new ServiceRequest(Guid.NewGuid(), Org, Guid.NewGuid(), Guid.NewGuid(), null, "SRV-O", "หัวข้อ", "รายละเอียด", ServicePriority.Normal, actor, Now);
        Assert.False(outWarrantyRequest.InWarranty);
        outWarrantyRequest.RecordServiceCharge(1500m, "ค่าบริการตรวจสอบ", billingId, "BIL-1", actor, Now);

        Assert.Equal(1500m, outWarrantyRequest.ServiceChargeAmount);
        Assert.Equal("ค่าบริการตรวจสอบ", outWarrantyRequest.ServiceChargeNote);
        Assert.Equal(billingId, outWarrantyRequest.BillingDocumentId);
        Assert.Equal("BIL-1", outWarrantyRequest.BillingDocumentNumber);

        // 3. Duplicate charge rejected
        var exDuplicate = Assert.Throws<ServiceDomainException>(() =>
            outWarrantyRequest.RecordServiceCharge(2000m, "ค่าบริการซ้ำ", Guid.NewGuid(), "BIL-2", actor, Now));
        Assert.Equal("SERVICE_INVALID_STATE", exDuplicate.Code);

        // 4. Invalid amounts
        var reqInvalid = new ServiceRequest(Guid.NewGuid(), Org, Guid.NewGuid(), Guid.NewGuid(), null, "SRV-INV", "หัวข้อ", "รายละเอียด", ServicePriority.Normal, actor, Now);
        Assert.Equal("SERVICE_FIELD_INVALID", Assert.Throws<ServiceDomainException>(() => reqInvalid.RecordServiceCharge(-10m, "x", billingId, "BIL-1", actor, Now)).Code);
        Assert.Equal("SERVICE_FIELD_INVALID", Assert.Throws<ServiceDomainException>(() => reqInvalid.RecordServiceCharge(10.123m, "x", billingId, "BIL-1", actor, Now)).Code);
    }

    [Fact]
    public void TechnicianAssignment_ValidatesFields_AndTargetExclusivity()
    {
        var actor = Guid.NewGuid();
        var tech = Guid.NewGuid();
        var branch = Guid.NewGuid();
        var date = new DateOnly(2026, 11, 10);
        var installId = Guid.NewGuid();
        var serviceId = Guid.NewGuid();

        // 1. Installation requires install job and no service request
        var installAssign = new TechnicianAssignment(Guid.NewGuid(), Org, branch, tech, date, TechnicianAssignmentKind.Installation, installId, null, "งานติดตั้งงวด 1", actor, Now);
        Assert.Equal(TechnicianAssignmentKind.Installation, installAssign.AssignmentKind);
        Assert.Equal(installId, installAssign.InstallationJobId);
        Assert.Null(installAssign.ServiceRequestId);

        // Missing install job
        Assert.Equal("SERVICE_FIELD_INVALID", Assert.Throws<ServiceDomainException>(() =>
            new TechnicianAssignment(Guid.NewGuid(), Org, branch, tech, date, TechnicianAssignmentKind.Installation, null, null, null, actor, Now)).Code);

        // Both IDs provided
        Assert.Equal("SERVICE_FIELD_INVALID", Assert.Throws<ServiceDomainException>(() =>
            new TechnicianAssignment(Guid.NewGuid(), Org, branch, tech, date, TechnicianAssignmentKind.Installation, installId, serviceId, null, actor, Now)).Code);

        // 2. Service request requires service request ID and no install job
        var serviceAssign = new TechnicianAssignment(Guid.NewGuid(), Org, branch, tech, date, TechnicianAssignmentKind.ServiceRequest, null, serviceId, "งานซ่อมก๊อกน้ำ", actor, Now);
        Assert.Equal(TechnicianAssignmentKind.ServiceRequest, serviceAssign.AssignmentKind);
        Assert.Equal(serviceId, serviceAssign.ServiceRequestId);
        Assert.Null(serviceAssign.InstallationJobId);

        // Missing service request
        Assert.Equal("SERVICE_FIELD_INVALID", Assert.Throws<ServiceDomainException>(() =>
            new TechnicianAssignment(Guid.NewGuid(), Org, branch, tech, date, TechnicianAssignmentKind.ServiceRequest, null, null, null, actor, Now)).Code);
    }
}
