using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TanErp.Api.Contracts.Crm.Sites;
using TanErp.Api.Contracts.Surveys;
using TanErp.Api.ErrorHandling;
using TanErp.Api.RequestContext;
using TanErp.Application.Surveys;
using TanErp.Application.Surveys.CloneSurveyRevision;
using TanErp.Application.Surveys.CreateSiteSurvey;
using TanErp.Application.Surveys.GetSiteSurvey;
using TanErp.Application.Surveys.MarkSurveyReady;
using TanErp.Application.Surveys.UpdateSurveyDraft;
using TanErp.Application.Surveys.VoidSurveyRevision;
using TanErp.Domain.Surveys;

namespace TanErp.Api.Controllers;

[ApiController]
[Route("api/v1/opportunities/{opportunityId:guid}/surveys")]
[Authorize]
public class SiteSurveysController : ControllerBase
{
    private readonly CreateSiteSurveyHandler _createHandler;
    private readonly GetSiteSurveyHandler _getHandler;
    private readonly UpdateSurveyDraftHandler _updateDraftHandler;
    private readonly MarkSurveyReadyHandler _markReadyHandler;
    private readonly CloneSurveyRevisionHandler _cloneRevisionHandler;
    private readonly VoidSurveyRevisionHandler _voidRevisionHandler;

    public SiteSurveysController(
        CreateSiteSurveyHandler createHandler,
        GetSiteSurveyHandler getHandler,
        UpdateSurveyDraftHandler updateDraftHandler,
        MarkSurveyReadyHandler markReadyHandler,
        CloneSurveyRevisionHandler cloneRevisionHandler,
        VoidSurveyRevisionHandler voidRevisionHandler)
    {
        _createHandler = createHandler;
        _getHandler = getHandler;
        _updateDraftHandler = updateDraftHandler;
        _markReadyHandler = markReadyHandler;
        _cloneRevisionHandler = cloneRevisionHandler;
        _voidRevisionHandler = voidRevisionHandler;
    }

    [HttpGet("/api/v1/surveys/reference-data")]
    [ProducesResponseType<SurveyReferenceDataResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReferenceData(
        [FromServices] TanErp.Application.Surveys.MasterData.ISurveyMasterDataStore masterStore,
        CancellationToken cancellationToken)
    {
        var categories = new List<SurveyCategoryOptionResponse>
        {
            new("built_in", new SurveyLocalizedLabel("เฟอร์นิเจอร์ Built-in", "Built-in Furniture"), "Custom built-in furniture and cabinetry", 1),
            new("loose", new SurveyLocalizedLabel("เฟอร์นิเจอร์ลอยตัว Loose", "Loose Furniture"), "Movable and standalone furniture pieces", 2),
            new("flooring", new SurveyLocalizedLabel("งานพื้นและกระเบื้อง", "Flooring & Tiles"), "SPC, tiles, engineered wood, or parquet flooring", 3),
            new("wall_ceiling", new SurveyLocalizedLabel("งานผนังและฝ้าเพดาน", "Walls & Ceilings"), "Wall panels, wallpaper, drop ceilings, or acoustic walls", 4),
            new("curtain", new SurveyLocalizedLabel("งานม่านและตกแต่งอ่อน", "Curtains & Soft Furnishings"), "Curtains, blinds, and soft decorative treatments", 5),
            new("mep", new SurveyLocalizedLabel("งานระบบไฟฟ้า/ประปา", "MEP (Electrical & Plumbing)"), "Wiring, outlets, switches, water pipes, and sanitary points", 6)
        };

        var photoTags = new List<SurveyPhotoTagOptionResponse>
        {
            new(EvidenceKind.SitePhoto, new SurveyLocalizedLabel("ภาพตำแหน่งหน้างาน", "Site Location"), "Overview context of installation site", 1),
            new(EvidenceKind.MeasurementSketch, new SurveyLocalizedLabel("ภาพสเก็ตช์ / เลเซอร์", "Sketch / Laser Display"), "Measurement sketches and laser meter readings", 2),
            new(EvidenceKind.MaterialSwatch, new SurveyLocalizedLabel("ภาพตัวอย่างวัสดุ / ลวดลาย", "Material / Swatch"), "Existing finishes, wood tones, and stone patterns", 3),
            new(EvidenceKind.ConstraintDefect, new SurveyLocalizedLabel("ภาพอุปสรรค / จุดชำรุด", "Constraint / Defect"), "Existing defects, electrical points, beams, or pipe obstacles", 4),
            new(EvidenceKind.MoodTone, new SurveyLocalizedLabel("ภาพตัวอย่าง Mood & Tone", "Mood & Tone / Reference Photo"), "Customer preferred design style and mood tone sample", 5),
            new(EvidenceKind.Other, new SurveyLocalizedLabel("อื่นๆ", "Other"), "Miscellaneous reference photo", 6)
        };

        var measurementTypes = new List<SurveyMeasurementTypeOptionResponse>
        {
            new("box", new SurveyLocalizedLabel("กล่อง 3 มิติ (กว้าง × ลึก × สูง)", "3D Box (W × D × H)"), "mm", 1),
            new("area", new SurveyLocalizedLabel("พื้นที่ผิว (ตร.ม. พร้อมหักช่องเปิด)", "Surface Area (sqm)"), "sqm", 2),
            new("custom", new SurveyLocalizedLabel("จุดวัดเฉพาะ (หลืบม่าน/ระยะเสา/ระยะฝ้า)", "Custom Measurement Points"), "mm", 3)
        };

        var designStyles = new List<SurveyDesignStyleOptionResponse>
        {
            new("modern_luxury", new SurveyLocalizedLabel("โมเดิร์น ลักชูรี่ (Modern Luxury)", "Modern Luxury"), "หรูหรา โลหะสีทอง/พิงค์โกลด์ หินอ่อน กระจกเงา และไฟซ่อน", 1),
            new("japandi", new SurveyLocalizedLabel("จาแปนดี้ / มูจิ (Japandi / Muji)", "Japandi / Minimal Wood"), "เน้นงานไม้สีอ่อน โทนธรรมชาติ ผสมผสานความเรียบง่ายและอบอุ่น", 2),
            new("modern_contemporary", new SurveyLocalizedLabel("โมเดิร์น ร่วมสมัย (Modern Contemporary)", "Modern Contemporary"), "ทันสมัย เรียบง่าย ฟังก์ชันลงตัว เส้นสายคมชัด", 3),
            new("scandinavian", new SurveyLocalizedLabel("สแกนดิเนเวียน (Scandinavian)", "Scandinavian"), "โปร่งสบาย โทนสีสว่าง ไม้ธรรมชาติ ผ่อนคลาย", 4),
            new("minimalist", new SurveyLocalizedLabel("มินิมอล (Minimalist)", "Minimalist"), "เรียบเกลี้ยง ไร้มือจับ ซ่อนฟังก์ชัน เก็บของเป็นระเบียบ", 5),
            new("industrial_loft", new SurveyLocalizedLabel("อินดัสเทรียล ลอฟท์ (Industrial Loft)", "Industrial Loft"), "โครงเหล็ก ปูนเปลือย อิฐโชว์แนว และไม้สีเข้ม โชว์พื้นผิวจริง", 6),
            new("classic_neo_classic", new SurveyLocalizedLabel("คลาสสิก / นีโอคลาสสิก (Classic / Neo Classic)", "Classic / Neo Classic"), "บัวคิ้ว หน้าบานลูกฟัก ลวดลายวิจิตร และสมมาตรภูมิฐาน", 7),
            new("vintage_retro", new SurveyLocalizedLabel("วินเทจ / ย้อนยุค (Vintage / Retro)", "Vintage / Retro"), "กลิ่นอายย้อนยุค สีสันเฉพาะตัว และเสน่ห์งานคลาสสิก", 8)
        };

        var moodTones = new List<SurveyMoodToneOptionResponse>
        {
            new("warm_earthy", new SurveyLocalizedLabel("วอร์มโทน / อบอุ่น (Warm & Earthy)", "Warm & Earthy"), "โทนสีน้ำตาล ครีม เบจ ไม้ธรรมชาติ อบอุ่น สบายตา", new[] { "#F5EBE0", "#D6CCC2", "#8D6E63", "#4E342E" }, 1),
            new("cool_monotone", new SurveyLocalizedLabel("คูลโทน / ขาว-เทา-ดำ (Cool & Monotone)", "Cool & Monotone"), "เน้นสีขาว เทา ดำ หินอ่อน นิ่งสงบ สุขุม โมเดิร์น", new[] { "#FFFFFF", "#E0E0E0", "#757575", "#212121" }, 2),
            new("dark_elegant", new SurveyLocalizedLabel("ดาร์ก ลักชูรี่ (Dark & Elegant)", "Dark & Elegant"), "เน้นสีดำ วอลนัทเข้ม ตัดเส้นสายโลหะสีทอง หรูหราลุ่มลึก", new[] { "#1A1A1A", "#3E2723", "#C5A059", "#0B3056" }, 3),
            new("light_natural", new SurveyLocalizedLabel("สว่างโปร่ง (Light & Natural)", "Light & Natural"), "เน้นสีขาวสว่าง ไม้สน/โอ๊คธรรมชาติ โปร่งโล่ง สบายตา", new[] { "#FAFAFA", "#FFF8E7", "#E2D4C9", "#A3B18A" }, 4),
            new("colorful_pastel", new SurveyLocalizedLabel("พาสเทล / คัลเลอร์ฟูล (Pastel & Colorful)", "Pastel & Colorful"), "เน้นสีสันสดใส เขียวเซจ น้ำเงินเข้ม ส้มอิฐ หรือพาสเทล มีชีวิตชีวา", new[] { "#C2D8B9", "#8E9AAF", "#CBC0D3", "#EFD3D7" }, 5)
        };

        if (HttpContext.Request.Headers.TryGetValue("X-Organization-Id", out var orgHeader) && Guid.TryParse(orgHeader, out var orgId))
        {
            await masterStore.EnsureDefaultMasterDataAsync(orgId, cancellationToken);
            var dbStyles = await masterStore.ListDesignStylesAsync(orgId, onlyActive: true, cancellationToken);
            var dbTones = await masterStore.ListMoodTonesAsync(orgId, onlyActive: true, cancellationToken);

            if (dbStyles.Count > 0)
            {
                designStyles = dbStyles.Select(s => new SurveyDesignStyleOptionResponse(
                    s.Code,
                    new SurveyLocalizedLabel(s.Name.Thai, s.Name.English ?? s.Code),
                    s.Description?.Thai ?? s.Description?.English,
                    s.SortOrder)).ToList();
            }

            if (dbTones.Count > 0)
            {
                moodTones = dbTones.Select(t => new SurveyMoodToneOptionResponse(
                    t.Code,
                    new SurveyLocalizedLabel(t.Name.Thai, t.Name.English ?? t.Code),
                    t.Description?.Thai ?? t.Description?.English,
                    t.ColorPalette,
                    t.SortOrder)).ToList();
            }
        }

        Response.Headers.CacheControl = "public, max-age=3600";

        return Ok(new SurveyReferenceDataResponse(categories, photoTags, measurementTypes, designStyles, moodTones));
    }

    [HttpPost]
    [ProducesResponseType<SiteSurveyResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid opportunityId,
        [FromBody] CreateSiteSurveyRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new CreateSiteSurveyCommand(
            auth.FirebaseUid,
            auth.MembershipId,
            opportunityId,
            request.SiteId,
            request.AssignedSurveyorId,
            request.ScheduledStartUtc,
            request.ScheduledEndUtc,
            request.ExpectedOpportunityVersion,
            auth.IdempotencyKey,
            traceId);

        var result = await _createHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var survey = result.Value!;
        Response.Headers.ETag = $"\"{survey.RowVersion}\"";

        return CreatedAtAction(nameof(Get), new { opportunityId = survey.OpportunityId }, ToResponse(survey));
    }

    [HttpGet]
    [ProducesResponseType<SiteSurveyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromRoute] Guid opportunityId,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var query = new GetSiteSurveyQuery(
            auth.FirebaseUid,
            auth.MembershipId,
            opportunityId);

        var result = await _getHandler.Handle(query, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        if (result.Value == null)
        {
            return NoContent();
        }

        var survey = result.Value;
        Response.Headers.ETag = $"\"{survey.RowVersion}\"";

        return Ok(ToResponse(survey));
    }

    [HttpPut("{surveyId:guid}/revisions/{revisionId:guid}/draft")]
    [ProducesResponseType<SiteSurveyRevisionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> UpdateDraft(
        [FromRoute] Guid opportunityId,
        [FromRoute] Guid surveyId,
        [FromRoute] Guid revisionId,
        [FromBody] UpdateSurveyDraftRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadConditionalAuthenticatedRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var expectedVersion = auth.IfMatchRowVersion;
        var traceId = HttpContext.TraceIdentifier;

        var areas = request.Areas?.Select(a => new AreaInput(
            a.Id,
            a.Code,
            a.Name,
            a.Description,
            a.SortOrder,
            a.Measurements.Select(m => new MeasurementInput(
                m.Id,
                m.MeasurementType,
                m.Value,
                m.UnitCode,
                m.CaptureMethod,
                m.Notes,
                m.SortOrder)).ToList())).ToList() ?? new List<AreaInput>();

        var command = new UpdateSurveyDraftCommand(
            surveyId,
            revisionId,
            expectedVersion,
            request.VisitedAtUtc,
            request.ScopeSummary,
            request.Assumptions ?? new List<string>(),
            request.Constraints ?? new List<string>(),
            request.MissingDetails ?? new List<string>(),
            areas,
            request.Checklist?.Select(c => new ChecklistInput(c.ItemCode, c.Result, c.Note)).ToList(),
            request.Evidence?.Select(e => new EvidenceInput(e.FileId, e.Kind, e.Caption, e.SortOrder)).ToList(),
            auth.FirebaseUid,
            auth.MembershipId,
            traceId);

        var result = await _updateDraftHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var revision = result.Value!;
        Response.Headers.ETag = $"\"{revision.RowVersion}\"";

        return Ok(ToRevisionResponse(revision));
    }

    [HttpPost("{surveyId:guid}/revisions/{revisionId:guid}/mark-ready")]
    [ProducesResponseType<SiteSurveyRevisionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> MarkReady(
        [FromRoute] Guid opportunityId,
        [FromRoute] Guid surveyId,
        [FromRoute] Guid revisionId,
        [FromBody] MarkSurveyReadyRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;
        var traceId = HttpContext.TraceIdentifier;

        var command = new MarkSurveyReadyCommand(
            surveyId,
            revisionId,
            request.ExpectedRevisionVersion,
            request.ExpectedOpportunityVersion,
            auth.FirebaseUid,
            auth.MembershipId,
            auth.IdempotencyKey,
            traceId);

        var result = await _markReadyHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var revision = result.Value!;
        Response.Headers.ETag = $"\"{revision.RowVersion}\"";

        return Ok(ToRevisionResponse(revision));
    }

    [HttpPost("{surveyId:guid}/revisions")]
    [ProducesResponseType<SiteSurveyRevisionResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CloneRevision(
        [FromRoute] Guid opportunityId,
        [FromRoute] Guid surveyId,
        [FromBody] CloneSurveyRevisionRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;

        var command = new CloneSurveyRevisionCommand(
            opportunityId,
            surveyId,
            request.SourceRevisionId,
            request.Reason,
            auth.FirebaseUid,
            auth.MembershipId,
            auth.IdempotencyKey,
            HttpContext.TraceIdentifier);

        var result = await _cloneRevisionHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var revision = result.Value!;
        Response.Headers.ETag = $"\"{revision.RowVersion}\"";

        return StatusCode(StatusCodes.Status201Created, ToRevisionResponse(revision));
    }

    [HttpPost("{surveyId:guid}/revisions/{revisionId:guid}/void")]
    [ProducesResponseType<SiteSurveyRevisionResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ApiProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> VoidRevision(
        [FromRoute] Guid opportunityId,
        [FromRoute] Guid surveyId,
        [FromRoute] Guid revisionId,
        [FromBody] VoidSurveyRevisionRequest request,
        CancellationToken cancellationToken)
    {
        var contextResult = RequestContextReader.ReadIdempotentRequest(HttpContext);
        if (contextResult.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(contextResult.Error.Code, HttpContext);
        }

        var auth = contextResult.Value!;

        var command = new VoidSurveyRevisionCommand(
            opportunityId,
            surveyId,
            revisionId,
            request.ExpectedRevisionVersion,
            request.Reason,
            auth.FirebaseUid,
            auth.MembershipId,
            auth.IdempotencyKey,
            HttpContext.TraceIdentifier);

        var result = await _voidRevisionHandler.Handle(command, cancellationToken);
        if (result.IsFailure)
        {
            return ProblemDetailsMapper.CreateProblemResult(result.Error.Code, HttpContext);
        }

        var revision = result.Value!;
        Response.Headers.ETag = $"\"{revision.RowVersion}\"";

        return Ok(ToRevisionResponse(revision));
    }

    private static SiteSurveyRevisionResponse ToRevisionResponse(SiteSurveyRevisionProjection r)
    {
        var areas = r.Areas?.Select(a => new SiteSurveyAreaResponse(
            a.Id,
            a.SiteSurveyRevisionId,
            a.Code,
            a.Name,
            a.Description,
            a.SortOrder,
            a.Measurements.Select(m => new SiteSurveyMeasurementResponse(
                m.Id,
                m.SiteSurveyAreaId,
                m.MeasurementType,
                m.Value,
                m.UnitCode,
                m.CaptureMethod,
                m.Notes,
                m.SortOrder)).ToList())).ToList();

        return new SiteSurveyRevisionResponse(
            r.Id,
            r.SiteSurveyId,
            r.RevisionNumber,
            r.SurveyTemplateVersion,
            r.VisitedAtUtc,
            r.ScopeSummary,
            r.Assumptions,
            r.Constraints,
            r.MissingDetails,
            r.Readiness,
            r.Status,
            r.ReadyAtUtc,
            r.ReadyByUserId,
            r.SnapshotHash,
            r.RowVersion,
            r.CreatedAtUtc,
            r.CreatedByUserId,
            areas,
            r.Checklist?.Select(c => new SiteSurveyChecklistResultResponse(c.Id, c.ItemCode, c.Result, c.Note)).ToList(),
            r.Evidence?.Select(e => new SiteSurveyEvidenceResponse(e.Id, e.FileId, e.Kind, e.Caption, e.SortOrder)).ToList());
    }

    private static SiteSurveyResponse ToResponse(SiteSurveyProjection s)
    {
        SiteSurveyRevisionResponse? currentRev = null;
        if (s.CurrentRevision != null)
        {
            currentRev = ToRevisionResponse(s.CurrentRevision);
        }

        SurveyorSummaryResponse? assignedSurveyor = s.AssignedSurveyor != null
            ? new SurveyorSummaryResponse(s.AssignedSurveyor.Id, s.AssignedSurveyor.DisplayName, s.AssignedSurveyor.Email)
            : null;

        SiteSummaryResponse? site = s.Site != null
            ? new SiteSummaryResponse(s.Site.Id, s.Site.Label, s.Site.AddressLine1)
            : null;

        return new SiteSurveyResponse(
            s.Id,
            s.OrganizationId,
            s.BranchId,
            s.OpportunityId,
            s.SiteId,
            s.SurveyNumber,
            s.AssignedSurveyorId,
            s.ScheduledStartUtc,
            s.ScheduledEndUtc,
            s.Status,
            s.RowVersion,
            s.CreatedAtUtc,
            s.CreatedByUserId,
            currentRev,
            assignedSurveyor,
            site,
            s.LatestReadyRevision != null
                ? new SiteSurveyReadyRevisionResponse(s.LatestReadyRevision.Id, s.LatestReadyRevision.RevisionNumber, s.LatestReadyRevision.SnapshotHash)
                : null);
    }
}
