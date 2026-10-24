using Microsoft.EntityFrameworkCore;
using TanErp.Application.Surveys.MasterData;
using TanErp.Domain.Items;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence.Surveys;

public class SurveyMasterDataStore : ISurveyMasterDataStore
{
    private readonly AppDbContext _db;

    public SurveyMasterDataStore(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<SurveyDesignStyle>> ListDesignStylesAsync(Guid organizationId, bool onlyActive, CancellationToken cancellationToken)
    {
        var query = _db.SurveyDesignStyles
            .Where(x => x.OrganizationId == organizationId);

        if (onlyActive)
        {
            query = query.Where(x => x.IsActive);
        }

        return await query
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<SurveyDesignStyle?> GetDesignStyleByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        return await _db.SurveyDesignStyles
            .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == id, cancellationToken);
    }

    public async Task<SurveyDesignStyle?> GetDesignStyleByCodeAsync(Guid organizationId, string code, CancellationToken cancellationToken)
    {
        var normalized = code.Trim().ToLowerInvariant();
        return await _db.SurveyDesignStyles
            .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Code == normalized, cancellationToken);
    }

    public async Task AddDesignStyleAsync(SurveyDesignStyle style, CancellationToken cancellationToken)
    {
        _db.SurveyDesignStyles.Add(style);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateDesignStyleAsync(SurveyDesignStyle style, CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SurveyMoodTone>> ListMoodTonesAsync(Guid organizationId, bool onlyActive, CancellationToken cancellationToken)
    {
        var query = _db.SurveyMoodTones
            .Where(x => x.OrganizationId == organizationId);

        if (onlyActive)
        {
            query = query.Where(x => x.IsActive);
        }

        return await query
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<SurveyMoodTone?> GetMoodToneByIdAsync(Guid organizationId, Guid id, CancellationToken cancellationToken)
    {
        return await _db.SurveyMoodTones
            .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == id, cancellationToken);
    }

    public async Task<SurveyMoodTone?> GetMoodToneByCodeAsync(Guid organizationId, string code, CancellationToken cancellationToken)
    {
        var normalized = code.Trim().ToLowerInvariant();
        return await _db.SurveyMoodTones
            .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Code == normalized, cancellationToken);
    }

    public async Task AddMoodToneAsync(SurveyMoodTone moodTone, CancellationToken cancellationToken)
    {
        _db.SurveyMoodTones.Add(moodTone);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateMoodToneAsync(SurveyMoodTone moodTone, CancellationToken cancellationToken)
    {
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task EnsureDefaultMasterDataAsync(Guid organizationId, CancellationToken cancellationToken)
    {
        var hasStyles = await _db.SurveyDesignStyles.AnyAsync(x => x.OrganizationId == organizationId, cancellationToken);
        if (!hasStyles)
        {
            var defaultStyles = new List<SurveyDesignStyle>
            {
                new(Guid.NewGuid(), organizationId, "modern_luxury",
                    LocalizedText.Create("โมเดิร์น ลักชูรี่", "Modern Luxury"),
                    LocalizedText.CreateOptional("หรูหรา โลหะสีทอง/พิงค์โกลด์ หินอ่อน กระจกเงา และไฟซ่อน", "Luxury aesthetic with gold/rose gold accents, marble, mirror finishes, and warm concealed lighting"),
                    1),
                new(Guid.NewGuid(), organizationId, "japandi",
                    LocalizedText.Create("จาแปนดี้ / มูจิ", "Japandi / Muji"),
                    LocalizedText.CreateOptional("เน้นงานไม้สีอ่อน โทนธรรมชาติ ผสมผสานความเรียบง่ายและอบอุ่น", "Blend of Japanese minimalism and Scandinavian warmth with natural light oak"),
                    2),
                new(Guid.NewGuid(), organizationId, "modern_contemporary",
                    LocalizedText.Create("โมเดิร์น ร่วมสมัย", "Modern Contemporary"),
                    LocalizedText.CreateOptional("ทันสมัย เรียบง่าย ฟังก์ชันลงตัว เส้นสายคมชัด", "Clean lines, functional layouts, and contemporary balanced textures"),
                    3),
                new(Guid.NewGuid(), organizationId, "scandinavian",
                    LocalizedText.Create("สแกนดิเนเวียน", "Scandinavian"),
                    LocalizedText.CreateOptional("โปร่งสบาย โทนสีสว่าง ไม้ธรรมชาติ ผ่อนคลาย", "Light-filled spaces, natural wood, airy layouts, and gentle cozy touches"),
                    4),
                new(Guid.NewGuid(), organizationId, "minimalist",
                    LocalizedText.Create("มินิมอล", "Minimalist"),
                    LocalizedText.CreateOptional("เรียบเกลี้ยง ไร้มือจับ ซ่อนฟังก์ชัน จัดเก็บสบายตา", "Handle-less clean cabinetry, concealed storage, and uncluttered visual simplicity"),
                    5),
                new(Guid.NewGuid(), organizationId, "industrial_loft",
                    LocalizedText.Create("อินดัสเทรียล ลอฟท์", "Industrial Loft"),
                    LocalizedText.CreateOptional("โครงเหล็ก ปูนเปลือย อิฐโชว์แนว ไม้สีเข้ม", "Raw concrete, steel framing, exposed brick, and rich dark timber accents"),
                    6),
                new(Guid.NewGuid(), organizationId, "classic_neo_classic",
                    LocalizedText.Create("คลาสสิก / นีโอคลาสสิก", "Classic / Neo Classic"),
                    LocalizedText.CreateOptional("บัวคิ้ว หน้าบานลูกฟัก สมมาตรภูมิฐาน", "Molding profiles, panel doors, refined proportions, and timeless stately elegance"),
                    7),
                new(Guid.NewGuid(), organizationId, "vintage_retro",
                    LocalizedText.Create("วินเทจ / ย้อนยุค", "Vintage / Retro"),
                    LocalizedText.CreateOptional("กลิ่นอายย้อนยุค มีเสน่ห์เฉพาะตัว", "Retro nostalgia, playful mid-century curves, and distinctive vintage charm"),
                    8)
            };

            _db.SurveyDesignStyles.AddRange(defaultStyles);
        }

        var hasMoodTones = await _db.SurveyMoodTones.AnyAsync(x => x.OrganizationId == organizationId, cancellationToken);
        if (!hasMoodTones)
        {
            var defaultMoodTones = new List<SurveyMoodTone>
            {
                new(Guid.NewGuid(), organizationId, "warm_earthy",
                    LocalizedText.Create("วอร์มโทน / อบอุ่น", "Warm & Earthy"),
                    LocalizedText.CreateOptional("โทนสีน้ำตาล ครีม เบจ ไม้ธรรมชาติ อบอุ่น สบายตา", "Warm browns, cream, beige, and natural timber tones"),
                    new[] { "#F5EBE0", "#D6CCC2", "#8D6E63", "#4E342E" },
                    1),
                new(Guid.NewGuid(), organizationId, "cool_monotone",
                    LocalizedText.Create("คูลโทน / ขาว-เทา-ดำ", "Cool & Monotone"),
                    LocalizedText.CreateOptional("เน้นสีขาว เทา ดำ หินอ่อน นิ่งสงบ สุขุม โมเดิร์น", "Crisp whites, neutral grays, black metal, and cool stone"),
                    new[] { "#FFFFFF", "#E0E0E0", "#757575", "#212121" },
                    2),
                new(Guid.NewGuid(), organizationId, "dark_elegant",
                    LocalizedText.Create("ดาร์ก ลักชูรี่", "Dark & Elegant"),
                    LocalizedText.CreateOptional("เน้นสีดำ วอลนัทเข้ม ตัดเส้นสายโลหะสีทอง หรูหราลุ่มลึก", "Moody charcoal, dark walnut, brushed brass, and deep navy"),
                    new[] { "#1A1A1A", "#3E2723", "#C5A059", "#0B3056" },
                    3),
                new(Guid.NewGuid(), organizationId, "light_natural",
                    LocalizedText.Create("สว่างโปร่ง", "Light & Natural"),
                    LocalizedText.CreateOptional("เน้นสีขาวสว่าง ไม้สน/โอ๊คธรรมชาติ โปร่งโล่ง สบายตา", "Off-white, pale pine, light oak, and sage greenery accents"),
                    new[] { "#FAFAFA", "#FFF8E7", "#E2D4C9", "#A3B18A" },
                    4),
                new(Guid.NewGuid(), organizationId, "colorful_pastel",
                    LocalizedText.Create("พาสเทล / คัลเลอร์ฟูล", "Pastel & Colorful"),
                    LocalizedText.CreateOptional("เน้นสีสันสดใส เขียวเซจ น้ำเงินเข้ม พาสเทล มีชีวิตชีวา", "Playful pastels, soft mint, dusty rose, and muted lavender"),
                    new[] { "#C2D8B9", "#8E9AAF", "#CBC0D3", "#EFD3D7" },
                    5)
            };

            _db.SurveyMoodTones.AddRange(defaultMoodTones);
        }

        if (!hasStyles || !hasMoodTones)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
