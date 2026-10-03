using Microsoft.EntityFrameworkCore;
using TanErp.Domain.IdentityAccess;
using TanErp.Domain.Organization;
using TanErp.Domain.Items;
using TanErp.Domain.Estimates;
using TanErp.Domain.Crm.Customers;
using TanErp.Domain.Crm.Opportunities;
using TanErp.Domain.Crm.Sites;
using TanErp.Domain.Surveys;

namespace TanErp.Infrastructure.Persistence;

public static class TestOnlyDataSeeder
{
    public static readonly Guid TestUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a10");
    public static readonly Guid TestMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a11");
    public static readonly Guid TestOrgId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a12");
    public static readonly Guid TestBranchId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a13");
    public static readonly Guid TestCostSourceId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4a14");
    public const string TestFirebaseUid = "foundation-user-test-only";
    public const string TestUserEmail = "foundation-user@example.test";
    public const string TestUserDisplayName = "ผู้ใช้ TEST_ONLY";

    public static readonly Guid TestUserIdB = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b20");
    public static readonly Guid TestMembershipBId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b21");
    public static readonly Guid TestOrgBId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b22");
    public static readonly Guid TestBranchBId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b23");
    public static readonly Guid TestCostReviewerMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b24");
    public static readonly Guid TestEstimateReviewerUserId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b17");
    public static readonly Guid TestEstimateReviewerMembershipId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b18");
    private static readonly Guid TestEstimateReviewerRoleId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b28");
    private static readonly Guid TestEstimateCalculationPolicyId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b26");
    private static readonly Guid TestEstimateTaxPolicyId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b27");
    public static readonly Guid TestEstimateDemoCustomerId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c01");
    public static readonly Guid TestEstimateDemoSiteId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c02");
    public static readonly Guid TestEstimateDemoOpportunityId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c03");
    public static readonly Guid TestEstimateDemoId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c04");
    public static readonly Guid TestItemCatalogCategoryId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d01");
    public static readonly Guid TestItemCatalogBrandId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d02");
    public static readonly Guid TestItemCatalogTaxCategoryId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d03");
    public static readonly Guid TestItemCatalogSheetUnitId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d11");
    public static readonly Guid TestItemCatalogMeterUnitId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d12");
    public static readonly Guid TestItemCatalogPlywoodId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d21");
    public static readonly Guid TestItemCatalogLaminateId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d22");
    public static readonly Guid TestItemCatalogEdgebandId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d23");
    private static readonly Guid TestItemCatalogPlywoodCostId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d31");
    private static readonly Guid TestItemCatalogLaminateCostId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d32");
    private static readonly Guid TestItemCatalogEdgebandCostId = Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4d33");
    public const string TestFirebaseUidB = "foundation-user-b-test-only";
    public const string TestUserEmailB = "foundation-user-b@example.test";
    public const string TestEstimateReviewerFirebaseUid = "foundation-estimate-reviewer-test-only";
    public const string TestEstimateReviewerEmail = "foundation-estimate-reviewer@example.test";

    public static async Task SeedAsync(
        AppDbContext db,
        string environmentName,
        bool seedTestData,
        bool seedDedicatedEstimateReviewer = false,
        bool seedEstimateDemoData = false,
        bool seedItemCatalogDemoData = false)
    {
        // Environment-lock: strictly only run in Test environment when SeedTestData is true
        if (!string.Equals(environmentName, "Test", StringComparison.OrdinalIgnoreCase) || !seedTestData)
        {
            return;
        }

        // Ensure database schema is migrated
        await db.Database.MigrateAsync();

        // NOTE: no early return here; every block below is find-or-create so
        // restarts backfill permission keys/links added after the first seed.

        // Seed Organization
        var org = await db.Organizations.FindAsync(TestOrgId);
        if (org == null)
        {
            org = new Organization(TestOrgId, "TEST_ONLY Project ERP");
            db.Organizations.Add(org);
        }

        var testCostSource = await db.CostSources.FirstOrDefaultAsync(source => source.Id == TestCostSourceId);
        if (testCostSource is null)
        {
            db.CostSources.Add(new CostSource(TestCostSourceId, TestOrgId, "TEST-MANUAL",
                new LocalizedText("แหล่งราคาทดสอบ", "Test Manual Source"), 1, true, DateTimeOffset.UtcNow));
        }

        // Seed Branch
        var branch = await db.Branches.FindAsync(TestBranchId);
        if (branch == null)
        {
            branch = new Branch(TestBranchId, TestOrgId, "B01", "สาขาทดสอบ");
            db.Branches.Add(branch);
        }

        // Seed User
        var user = await db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == TestFirebaseUid);
        if (user == null)
        {
            user = new User(TestUserId, TestFirebaseUid, TestUserDisplayName, TestUserEmail, isActive: true);
            db.Users.Add(user);
        }

        // Seed Membership
        var membership = await db.Memberships.FindAsync(TestMembershipId);
        if (membership == null)
        {
            membership = new Membership(TestMembershipId, TestOrgId, TestBranchId, TestUserId, isActive: true);
            db.Memberships.Add(membership);
        }

        // Seed Permissions (Identity + CRM)
        var permKeys = new[]
        {
            ("organizations.read", "Read Organization"),
            ("customers.read", "Read Customers"),
            ("customers.create", "Create Customers"),
            ("customers.update", "Update Customers"),
            ("customers.deactivate", "Deactivate Customers"),
            ("customers.credit.read", "Read Customer Credit Terms"),
            ("customers.credit.manage", "Manage Customer Credit Terms"),
            ("customer-contacts.manage", "Manage Customer Contacts"),
            ("customers.activate", "Activate Customers"),
            ("sites.read", "Read Sites"),
            ("sites.manage", "Manage Sites"),
            ("opportunities.read", "Read Opportunities"),
            ("opportunities.create", "Create Opportunities"),
            ("opportunities.update", "Update Opportunities"),
            ("opportunities.transition", "Transition Opportunities"),
            ("surveys.read", "Read Site Surveys"),
            ("surveys.create", "Create Site Surveys"),
            ("surveys.update", "Update Site Surveys"),
            ("surveys.mark-ready", "Mark Site Surveys Ready"),
            ("estimates.read", "Read Estimates"),
            ("estimates.create", "Create Estimates"),
            ("estimates.update", "Update Estimates"),
            ("estimates.override-price", "Override Estimate Selling Price"),
            ("estimates.submit", "Submit Estimates for Approval"),
            ("estimates.cancel", "Cancel Estimates"),
            ("estimates.revise", "Create Estimate Revisions"),
            ("estimates.approve", "Review and Approve Estimates"),
            ("users.read", "Read Users and Memberships"),
            ("users.manage", "Manage Users"),
            ("memberships.manage", "Manage Memberships"),
            ("roles.assign", "Assign Roles"),
            ("roles.assign-approval", "Approve Role Assignments"),
            ("quotations.read", "Read Quotation Documents"),
            ("quotations.issue", "Issue Quotations"),
            ("quotations.accept", "Accept Quotations"),
            ("document-sequences.read", "Read Document Sequences"),
            ("document-sequences.manage", "Manage Document Sequences"),
            ("document-sequences.manual-override", "Manual Override Document Numbers"),
            ("items.read", "Read Items and Estimate Catalog"),
            ("items.create", "Create Items"),
            ("items.update", "Update Items"),
            ("items.activate", "Activate Items"),
            ("items.deactivate", "Deactivate Items"),
            ("items.manage-taxonomy", "Manage Categories and Brands"),
            ("items.manage-branches", "Manage Item Branch Availability"),
            ("items.manage-images", "Manage Item Images"),
            ("items.manage-barcodes", "Manage Item Barcodes"),
            ("cost-records.read", "Read Cost Records"),
            ("cost-sources.read", "Read Cost Sources"),
            ("cost-sources.manage", "Manage Cost Sources"),
            ("cost-records.create", "Create Cost Records"),
            ("cost-records.submit", "Submit Cost Records"),
            ("cost-records.approve", "Approve Cost Records"),
            ("cost-records.publish", "Publish Cost Records"),
            ("cost-records.disable", "Disable Cost Records"),
            ("units.read", "Read Units"),
            ("units.manage", "Manage Units")
        };


        var seededPerms = new List<Permission>();
        foreach (var (key, desc) in permKeys)
        {
            var perm = await db.Permissions.FirstOrDefaultAsync(p => p.Key == key);
            if (perm == null)
            {
                perm = Permission.Create(key, desc);
                db.Permissions.Add(perm);
            }
            seededPerms.Add(perm);
        }

        // Seed Role for Org A (backfill new permission links for roles seeded before CRM keys existed)
        var role = await db.Roles.FirstOrDefaultAsync(r => r.OrganizationId == TestOrgId && r.Name == "Test Admin");
        if (role == null)
        {
            role = new Role(Guid.NewGuid(), TestOrgId, "Test Admin", "Test Admin Role", isActive: true);
            db.Roles.Add(role);

            var membershipRole = new MembershipRole(TestMembershipId, role.Id, TestOrgId);
            db.MembershipRoles.Add(membershipRole);
        }

        foreach (var perm in seededPerms)
        {
            var exists = await db.RolePermissions.AnyAsync(rp =>
                rp.RoleId == role.Id && rp.PermissionId == perm.Id
                && rp.Scope == PermissionScope.Organization && rp.ScopeId == TestOrgId);
            if (!exists)
            {
                db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), role.Id, TestOrgId, perm.Id, PermissionScope.Organization, TestOrgId));
            }
        }

        // Seed a low-privilege role so user administration can be exercised without approval permissions.
        var readOnlyRole = await db.Roles.FirstOrDefaultAsync(r => r.OrganizationId == TestOrgId && r.Name == "Test Read Only");
        if (readOnlyRole == null)
        {
            readOnlyRole = new Role(Guid.NewGuid(), TestOrgId, "Test Read Only", "Read-only opportunities and customers", isActive: true);
            db.Roles.Add(readOnlyRole);
        }

        foreach (var perm in seededPerms.Where(p => p.Key is "opportunities.read" or "customers.read"))
        {
            var exists = await db.RolePermissions.AnyAsync(rp =>
                rp.RoleId == readOnlyRole.Id && rp.PermissionId == perm.Id
                && rp.Scope == PermissionScope.Organization && rp.ScopeId == TestOrgId);
            if (!exists)
            {
                db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), readOnlyRole.Id, TestOrgId, perm.Id, PermissionScope.Organization, TestOrgId));
            }
        }

        // Seed Deterministic Org B Fixture
        var orgB = await db.Organizations.FindAsync(TestOrgBId);
        if (orgB == null)
        {
            orgB = new Organization(TestOrgBId, "TEST_ONLY Organization B");
            db.Organizations.Add(orgB);
        }

        // Seed Branch B
        var branchB = await db.Branches.FindAsync(TestBranchBId);
        if (branchB == null)
        {
            branchB = new Branch(TestBranchBId, TestOrgBId, "B01-B", "สาขาทดสอบ B");
            db.Branches.Add(branchB);
        }

        var userB = await db.Users.FirstOrDefaultAsync(u => u.FirebaseUid == TestFirebaseUidB);
        if (userB == null)
        {
            userB = new User(TestUserIdB, TestFirebaseUidB, "ผู้ใช้ TEST_ONLY B", TestUserEmailB, isActive: true);
            db.Users.Add(userB);
        }

        var membershipB = await db.Memberships.FindAsync(TestMembershipBId);
        if (membershipB == null)
        {
            membershipB = new Membership(TestMembershipBId, TestOrgBId, TestBranchBId, userB.Id, isActive: true);
            db.Memberships.Add(membershipB);
        }

        var roleB = await db.Roles.FirstOrDefaultAsync(r => r.OrganizationId == TestOrgBId && r.Name == "Test Admin B");
        if (roleB == null)
        {
            roleB = new Role(Guid.NewGuid(), TestOrgBId, "Test Admin B", "Test Admin Role B", isActive: true);
            db.Roles.Add(roleB);

            var membershipRoleB = new MembershipRole(TestMembershipBId, roleB.Id, TestOrgBId);
            db.MembershipRoles.Add(membershipRoleB);
        }

        foreach (var perm in seededPerms)
        {
            var existsB = await db.RolePermissions.AnyAsync(rp =>
                rp.RoleId == roleB.Id && rp.PermissionId == perm.Id
                && rp.Scope == PermissionScope.Organization && rp.ScopeId == TestOrgBId);
            if (!existsB)
            {
                db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), roleB.Id, TestOrgBId, perm.Id, PermissionScope.Organization, TestOrgBId));
            }
        }

        // Seed a separate same-organization checker identity for priced Item → BOQ acceptance.
        var costReviewerMembership = await db.Memberships.FindAsync(TestCostReviewerMembershipId);
        if (costReviewerMembership is null)
        {
            costReviewerMembership = new Membership(TestCostReviewerMembershipId, TestOrgId, TestBranchId, userB.Id, isActive: true);
            db.Memberships.Add(costReviewerMembership);
        }

        var costReviewerRole = await db.Roles.FirstOrDefaultAsync(r => r.OrganizationId == TestOrgId && r.Name == "Test Cost Reviewer");
        if (costReviewerRole is null)
        {
            costReviewerRole = new Role(Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4b25"), TestOrgId, "Test Cost Reviewer", "TEST_ONLY independent cost reviewer", isActive: true);
            db.Roles.Add(costReviewerRole);
        }

        var reviewerAssignmentExists = await db.MembershipRoles.AnyAsync(mr =>
            mr.MembershipId == TestCostReviewerMembershipId && mr.RoleId == costReviewerRole.Id && mr.OrganizationId == TestOrgId);
        if (!reviewerAssignmentExists)
        {
            db.MembershipRoles.Add(new MembershipRole(TestCostReviewerMembershipId, costReviewerRole.Id, TestOrgId));
        }

        var costReviewerPermissionKeys = new[]
        {
            "cost-records.read", "cost-records.approve", "cost-records.publish", "estimates.approve", "estimates.cancel"
        };
        foreach (var permissionKey in costReviewerPermissionKeys.Where(key =>
                     !seedDedicatedEstimateReviewer || key != "estimates.approve"))
        {
            var permission = seededPerms.Single(seed => seed.Key == permissionKey);
            var existsReviewerPermission = await db.RolePermissions.AnyAsync(rp =>
                rp.RoleId == costReviewerRole.Id && rp.PermissionId == permission.Id
                && rp.Scope == PermissionScope.Organization && rp.ScopeId == TestOrgId);
            if (!existsReviewerPermission)
            {
                db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), costReviewerRole.Id, TestOrgId, permission.Id, PermissionScope.Organization, TestOrgId));
            }
        }

        if (seedDedicatedEstimateReviewer)
        {
            var estimateReviewerUser = await db.Users.FirstOrDefaultAsync(user =>
                user.FirebaseUid == TestEstimateReviewerFirebaseUid);
            if (estimateReviewerUser is null)
            {
                estimateReviewerUser = new User(TestEstimateReviewerUserId, TestEstimateReviewerFirebaseUid,
                    "ผู้ตรวจประเมิน TEST_ONLY", TestEstimateReviewerEmail, isActive: true);
                db.Users.Add(estimateReviewerUser);
            }

            var estimateReviewerMembership = await db.Memberships.FindAsync(TestEstimateReviewerMembershipId);
            if (estimateReviewerMembership is null)
            {
                estimateReviewerMembership = new Membership(TestEstimateReviewerMembershipId, TestOrgId, TestBranchId,
                    estimateReviewerUser.Id, isActive: true);
                db.Memberships.Add(estimateReviewerMembership);
            }

            var estimateReviewerRole = await db.Roles.FirstOrDefaultAsync(role =>
                role.Id == TestEstimateReviewerRoleId && role.OrganizationId == TestOrgId);
            if (estimateReviewerRole is null)
            {
                estimateReviewerRole = new Role(TestEstimateReviewerRoleId, TestOrgId, "Test Estimate Reviewer",
                    "TEST_ONLY independent estimate reviewer", isActive: true);
                db.Roles.Add(estimateReviewerRole);
            }

            var estimateReviewerAssignmentExists = await db.MembershipRoles.AnyAsync(membershipRole =>
                membershipRole.MembershipId == TestEstimateReviewerMembershipId &&
                membershipRole.RoleId == estimateReviewerRole.Id &&
                membershipRole.OrganizationId == TestOrgId);
            if (!estimateReviewerAssignmentExists)
            {
                db.MembershipRoles.Add(new MembershipRole(TestEstimateReviewerMembershipId, estimateReviewerRole.Id, TestOrgId));
            }

            foreach (var permissionKey in new[] { "opportunities.read", "estimates.read", "estimates.approve" })
            {
                var estimateReviewerPermission = seededPerms.Single(permission => permission.Key == permissionKey);
                var reviewerPermissionExists = await db.RolePermissions.AnyAsync(rolePermission =>
                    rolePermission.RoleId == estimateReviewerRole.Id &&
                    rolePermission.PermissionId == estimateReviewerPermission.Id &&
                    rolePermission.Scope == PermissionScope.Organization &&
                    rolePermission.ScopeId == TestOrgId);
                if (!reviewerPermissionExists)
                {
                    db.RolePermissions.Add(new RolePermission(Guid.NewGuid(), estimateReviewerRole.Id, TestOrgId,
                        estimateReviewerPermission.Id, PermissionScope.Organization, TestOrgId));
                }
            }
        }

        if (!await db.CalculationPolicyVersions.AnyAsync(policy => policy.Id == TestEstimateCalculationPolicyId))
        {
            var calculationPolicy = new CalculationPolicyVersion(
                TestEstimateCalculationPolicyId, TestOrgId, TestBranchId, "TEST_ONLY_CALC", 1,
                "none", 0m, "away-from-zero", new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero), null, TestUserId);
            calculationPolicy.Publish(TestUserIdB, new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero));
            db.CalculationPolicyVersions.Add(calculationPolicy);
        }

        if (!await db.TaxPolicyVersions.AnyAsync(policy => policy.Id == TestEstimateTaxPolicyId))
        {
            var taxPolicy = new TaxPolicyVersion(
                TestEstimateTaxPolicyId, TestOrgId, TestBranchId, "TEST_ONLY_TAX", 1,
                "exclusive", 0.07m, "TEST_ONLY_VAT", new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero), null, TestUserId);
            taxPolicy.Publish(TestUserIdB, new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero));
            db.TaxPolicyVersions.Add(taxPolicy);
        }

        if (seedEstimateDemoData)
        {
            await SeedEstimateDemoDataAsync(db);
        }

        if (seedItemCatalogDemoData)
        {
            await SeedItemCatalogDemoDataAsync(db);
        }

        await db.SaveChangesAsync();
    }

    private static async Task SeedEstimateDemoDataAsync(AppDbContext db)
    {
        if (await db.Estimates.AnyAsync(estimate => estimate.Id == TestEstimateDemoId))
        {
            // Databases seeded before the billing address fixture existed still need it to issue a quotation.
            await EnsureEstimateDemoBillingAddressAsync(db, DateTimeOffset.UtcNow);
            await EnsureEstimateDemoWorkItemReasonsAsync(db);
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var customer = Customer.CreateDraft(
            TestEstimateDemoCustomerId,
            TestOrgId,
            TestUserId,
            CustomerType.Person,
            "ลูกค้าทดลองประเมินราคา TEST_ONLY",
            "Estimate Demo Customer TEST_ONLY",
            "th",
            new PrimaryContactInput("ผู้ติดต่อทดสอบ", null, "0800000000", null, "phone"),
            now,
            customerCode: "TEST-EST-CUSTOMER");
        customer.Activate(customer.RowVersion);
        db.Customers.Add(customer);
        await EnsureEstimateDemoBillingAddressAsync(db, now);

        var address = new SiteAddressInput("1 ถนนสุขุมวิท", "คลองเตย", "คลองเตย", "กรุงเทพมหานคร", "10110", "TH");
        var site = Site.CreateActive(
            TestEstimateDemoSiteId,
            TestOrgId,
            customer.Id,
            TestUserId,
            "บ้านตัวอย่างสำหรับทดสอบ",
            address,
            13.7m,
            100.5m,
            null,
            now);
        db.Sites.Add(site);

        var opportunity = Opportunity.CreateDraft(
            TestEstimateDemoOpportunityId,
            TestOrgId,
            TestBranchId,
            customer.Id,
            site.Id,
            TestUserId,
            TestUserId,
            "TEST_ONLY งานตัวอย่างประเมินราคาบิลท์อิน",
            "ข้อมูลและราคาสมมติสำหรับทดสอบหน้าประเมินราคาเท่านั้น",
            ["built-in"],
            null,
            250000m,
            "THB",
            new DateOnly(2026, 12, 31),
            now.AddDays(2),
            "ทดสอบ Estimate workspace",
            now);
        opportunity.Qualify(opportunity.RowVersion);
        opportunity.EnterSurveying(opportunity.RowVersion, site.Id);
        opportunity.EnterEstimating(opportunity.RowVersion);
        db.Opportunities.Add(opportunity);
        db.OpportunityStageHistories.AddRange(
            new OpportunityStageHistory(Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c11"), TestOrgId,
                opportunity.Id, OpportunityStage.Draft, OpportunityStage.Qualified, "TEST_ONLY_SEED", "Demo estimate fixture", TestUserId,
                now.AddMinutes(-3), "TEST_ONLY_SEED_V1", "test-estimate-seed-qualified"),
            new OpportunityStageHistory(Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c12"), TestOrgId,
                opportunity.Id, OpportunityStage.Qualified, OpportunityStage.Surveying, "TEST_ONLY_SEED", "Demo estimate fixture", TestUserId,
                now.AddMinutes(-2), "TEST_ONLY_SEED_V1", "test-estimate-seed-surveying"),
            new OpportunityStageHistory(Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c13"), TestOrgId,
                opportunity.Id, OpportunityStage.Surveying, OpportunityStage.Estimating, "TEST_ONLY_SEED", "Demo estimate fixture", TestUserId,
                now.AddMinutes(-1), "TEST_ONLY_SEED_V1", "test-estimate-seed-estimating"));

        var survey = SiteSurvey.CreateAppointment(
            TestOrgId,
            TestBranchId,
            opportunity.Id,
            site.Id,
            TestUserId,
            TestUserId,
            now,
            now.AddHours(2),
            now);
        var surveyRevision = SiteSurveyRevision.CreateBaseline(TestOrgId, survey.Id, TestUserId, now);
        var area = new SiteSurveyArea(Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c21"),
            TestOrgId, surveyRevision.Id, "BEDROOM", "ห้องนอน", "Bedroom", 1);
        area.AddMeasurement(new SiteSurveyMeasurement(Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c22"),
            TestOrgId, area.Id, "width", 3.2m, "m", "measured", "TEST_ONLY", 1));
        area.AddMeasurement(new SiteSurveyMeasurement(Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c23"),
            TestOrgId, area.Id, "length", 2.4m, "m", "measured", "TEST_ONLY", 2));
        surveyRevision.AddArea(area);
        surveyRevision.UpdateDraft(now, "ตู้เสื้อผ้าบิลท์อินและชั้นวางตัวอย่าง", ["ขนาดเป็นข้อมูลสมมติ"], null, null);
        surveyRevision.MarkReady(TestUserId, now, "TEST_ONLY_DEMO_SURVEY_SNAPSHOT");
        db.SiteSurveys.Add(survey);
        db.SiteSurveyRevisions.Add(surveyRevision);

        var estimate = Estimate.CreateDraft(
            TestEstimateDemoId,
            TestOrgId,
            TestBranchId,
            customer.Id,
            opportunity.Id,
            "TEST-ONLY-ESTIMATE-0001",
            surveyRevision.Id,
            surveyRevision.SnapshotHash,
            "THB");
        var revision = estimate.CurrentRevision!;
        var section = new EstimateSection(Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c31"),
            TestOrgId, revision.Id, "BUILT-IN", "งานบิลท์อินตัวอย่าง", "Sample Built-ins", 1);
        AddDemoWorkItem(section, "WARDROBE", "ตู้เสื้อผ้าบิลท์อิน", "Built-in wardrobe", 1,
            (CostComponentType.Material, "วัสดุตู้เสื้อผ้า TEST_ONLY", 28500m),
            (CostComponentType.Labor, "ค่าแรงติดตั้ง TEST_ONLY", 9500m));
        AddDemoWorkItem(section, "SHELVES", "ชั้นวางของ", "Wall shelving", 2,
            (CostComponentType.Material, "วัสดุชั้นวาง TEST_ONLY", 4200m),
            (CostComponentType.Labor, "ค่าแรงติดตั้ง TEST_ONLY", 1800m));
        revision.AddSection(section);
        db.Estimates.Add(estimate);
    }

    private const string DemoCustomWorkItemReasonCode = "TEST_ONLY_DEMO_CUSTOM_WORK";
    private const string DemoCustomWorkItemReason = "รายการสมมติสำหรับทดสอบเท่านั้น";

    private static async Task EnsureEstimateDemoWorkItemReasonsAsync(AppDbContext db)
    {
        // Demo work items are custom (no Item Master link); older seeds lack the reason required to submit.
        var draftWorkItems = await db.EstimateWorkItems
            .Where(item => item.OverrideReasonCode == null &&
                           db.EstimateSections.Any(section => section.Id == item.EstimateSectionId &&
                               db.EstimateRevisions.Any(revision => revision.Id == section.EstimateRevisionId &&
                                   revision.EstimateId == TestEstimateDemoId &&
                                   (revision.Status == EstimateRevisionStatus.Draft || revision.Status == EstimateRevisionStatus.Returned))))
            .ToListAsync();
        foreach (var item in draftWorkItems)
        {
            item.SetCustomWorkItemReason(DemoCustomWorkItemReasonCode, DemoCustomWorkItemReason);
        }
    }

    private static async Task EnsureEstimateDemoBillingAddressAsync(AppDbContext db, DateTimeOffset now)
    {
        var hasBillingAddress = await db.CustomerAddresses.AnyAsync(address =>
            address.CustomerId == TestEstimateDemoCustomerId && address.AddressType == "billing" && address.IsPrimary);
        if (hasBillingAddress)
        {
            return;
        }

        // TEST_ONLY billing address so the demo estimate can be taken through quotation issuance.
        db.CustomerAddresses.Add(new CustomerAddress(
            Guid.Parse("019a3cf8-96f0-7c9f-b207-93aa818f4c41"),
            TestEstimateDemoCustomerId,
            TestOrgId,
            TestUserId,
            "billing",
            "ที่อยู่เรียกเก็บเงิน TEST_ONLY",
            "1 ถนนสุขุมวิท",
            "คลองเตย",
            "คลองเตย",
            "กรุงเทพมหานคร",
            "10110",
            "TH",
            true,
            now));
    }

    private static async Task SeedItemCatalogDemoDataAsync(AppDbContext db)
    {
        var now = DateTimeOffset.UtcNow;

        if (!await db.ItemCategories.AnyAsync(row => row.Id == TestItemCatalogCategoryId))
        {
            db.ItemCategories.Add(new ItemCategory(
                TestItemCatalogCategoryId,
                TestOrgId,
                "TEST-MAT-BOARD",
                LocalizedText.Create("แผ่นวัสดุ TEST_ONLY", "TEST_ONLY Sheet Materials"),
                LocalizedText.Create("หมวดวัสดุตัวอย่างสำหรับทดสอบ", "Sample material category for testing"),
                null,
                [ItemType.Material],
                1,
                TestUserId,
                now));
        }

        if (!await db.ItemBrands.AnyAsync(row => row.Id == TestItemCatalogBrandId))
        {
            db.ItemBrands.Add(new ItemBrand(
                TestItemCatalogBrandId,
                TestOrgId,
                "TEST-MATERIALS",
                LocalizedText.Create("แบรนด์ทดสอบ", "Test Materials"),
                LocalizedText.Create("แบรนด์สมมติ ห้ามใช้อ้างอิงจัดซื้อ", "Fictional brand; not for procurement"),
                1,
                TestUserId,
                now));
        }

        if (!await db.ItemTaxCategories.AnyAsync(row => row.Id == TestItemCatalogTaxCategoryId))
        {
            db.ItemTaxCategories.Add(new ItemTaxCategory(
                TestItemCatalogTaxCategoryId,
                TestOrgId,
                "TEST-MATERIAL",
                LocalizedText.Create("วัสดุทดสอบ", "Test Material"),
                1,
                TestUserId,
                now));
        }

        if (!await db.Units.AnyAsync(row => row.Id == TestItemCatalogSheetUnitId))
        {
            db.Units.Add(new UnitOfMeasure(
                TestItemCatalogSheetUnitId,
                TestOrgId,
                "TEST-SHEET",
                LocalizedText.Create("แผ่นทดสอบ", "Test Sheet"),
                "sheet",
                "count",
                0,
                "half_up",
                TestUserId,
                now));
        }

        if (!await db.Units.AnyAsync(row => row.Id == TestItemCatalogMeterUnitId))
        {
            db.Units.Add(new UnitOfMeasure(
                TestItemCatalogMeterUnitId,
                TestOrgId,
                "TEST-METER",
                LocalizedText.Create("เมตรทดสอบ", "Test Meter"),
                "m",
                "length",
                3,
                "half_up",
                TestUserId,
                now));
        }

        await AddCatalogItemAsync(db, new CatalogDemoItem(
            TestItemCatalogPlywoodId,
            TestItemCatalogPlywoodCostId,
            "TEST-MAT-PLY-MR18",
            LocalizedText.Create("ไม้อัดกันชื้น TEST_ONLY 18 มม.", "TEST_ONLY Moisture-Resistant Plywood 18 mm"),
            LocalizedText.Create("ไม้อัดเกรด MR ขนาด 1,220 × 2,440 มม.", "MR grade plywood, 1,220 × 2,440 mm"),
            TestItemCatalogSheetUnitId,
            1_250m,
            new Dictionary<string, string>
            {
                ["material"] = "plywood",
                ["grade"] = "MR",
                ["thickness_mm"] = "18",
                ["width_mm"] = "1220",
                ["length_mm"] = "2440",
                ["surface"] = "raw"
            },
            "TEST-PLY-MR18"));

        await AddCatalogItemAsync(db, new CatalogDemoItem(
            TestItemCatalogLaminateId,
            TestItemCatalogLaminateCostId,
            "TEST-MAT-LAM-WHITE",
            LocalizedText.Create("ลามิเนตสีขาวด้าน TEST_ONLY", "TEST_ONLY White Matte Laminate"),
            LocalizedText.Create("แผ่นลามิเนตผิวด้านสำหรับงานภายใน", "Matte laminate sheet for interior use"),
            TestItemCatalogSheetUnitId,
            580m,
            new Dictionary<string, string>
            {
                ["material"] = "high-pressure laminate",
                ["thickness_mm"] = "0.8",
                ["width_mm"] = "1220",
                ["length_mm"] = "2440",
                ["color"] = "white",
                ["finish"] = "matte"
            },
            "TEST-LAM-WHITE"));

        await AddCatalogItemAsync(db, new CatalogDemoItem(
            TestItemCatalogEdgebandId,
            TestItemCatalogEdgebandCostId,
            "TEST-MAT-EDGE-PVC22",
            LocalizedText.Create("ขอบปิด PVC สีขาว TEST_ONLY 22 มม.", "TEST_ONLY White PVC Edge Band 22 mm"),
            LocalizedText.Create("แถบปิดขอบเฟอร์นิเจอร์ หนา 1 มม.", "Furniture edge band, 1 mm thick"),
            TestItemCatalogMeterUnitId,
            18m,
            new Dictionary<string, string>
            {
                ["material"] = "PVC",
                ["thickness_mm"] = "1",
                ["width_mm"] = "22",
                ["color"] = "white",
                ["finish"] = "matte"
            },
            "TEST-EDGE-PVC22"));

        await SeedCatalogFilterFixturesAsync(db, now);
    }

    private static async Task SeedCatalogFilterFixturesAsync(AppDbContext db, DateTimeOffset now)
    {
        var brandIds = Enumerable.Range(1, 3)
            .Select(index => DemoFixtureId(0x20, index, 1))
            .ToArray();
        var brands = new[]
        {
            (Code: "TEST-FILTER-BRAND-A", Thai: "แบรนด์กรอง A", English: "Filter Brand A"),
            (Code: "TEST-FILTER-BRAND-B", Thai: "แบรนด์กรอง B", English: "Filter Brand B"),
            (Code: "TEST-FILTER-BRAND-C", Thai: "แบรนด์กรอง C", English: "Filter Brand C")
        };
        for (var index = 0; index < brands.Length; index++)
        {
            if (!await db.ItemBrands.AnyAsync(row => row.Id == brandIds[index]))
            {
                var brand = brands[index];
                db.ItemBrands.Add(new ItemBrand(brandIds[index], TestOrgId, brand.Code,
                    LocalizedText.Create(brand.Thai, brand.English),
                    LocalizedText.Create("ข้อมูลทดสอบสำหรับ filter เท่านั้น", "Test data for filters only"),
                    index + 2, TestUserId, now));
            }
        }

        var serviceTaxCategoryId = DemoFixtureId(0x30, 2, 1);
        if (!await db.ItemTaxCategories.AnyAsync(row => row.Id == serviceTaxCategoryId))
        {
            db.ItemTaxCategories.Add(new ItemTaxCategory(serviceTaxCategoryId, TestOrgId, "TEST-SERVICE",
                LocalizedText.Create("บริการทดสอบ", "Test Service"), 2, TestUserId, now));
        }

        var hourUnitId = DemoFixtureId(0x40, 1, 1);
        var jobUnitId = DemoFixtureId(0x40, 2, 1);
        var eachUnitId = DemoFixtureId(0x40, 3, 1);
        var dayUnitId = DemoFixtureId(0x40, 4, 1);
        var extraUnits = new[]
        {
            (Id: hourUnitId, Code: "TEST-HOUR", Thai: "ชั่วโมงทดสอบ", English: "Test Hour", Symbol: "hr", Dimension: "time", Scale: 2),
            (Id: jobUnitId, Code: "TEST-JOB", Thai: "งานทดสอบ", English: "Test Job", Symbol: "job", Dimension: "count", Scale: 0),
            (Id: eachUnitId, Code: "TEST-EACH", Thai: "ชิ้นทดสอบ", English: "Test Each", Symbol: "ea", Dimension: "count", Scale: 0),
            (Id: dayUnitId, Code: "TEST-DAY", Thai: "วันทดสอบ", English: "Test Day", Symbol: "day", Dimension: "time", Scale: 2)
        };
        foreach (var unit in extraUnits)
        {
            if (!await db.Units.AnyAsync(row => row.Id == unit.Id))
            {
                db.Units.Add(new UnitOfMeasure(unit.Id, TestOrgId, unit.Code,
                    LocalizedText.Create(unit.Thai, unit.English), unit.Symbol, unit.Dimension,
                    unit.Scale, "half_up", TestUserId, now));
            }
        }

        var typeFixtures = new[]
        {
            new CatalogFilterType(1, ItemType.Material, "MAT", "วัสดุ", "Material", TestItemCatalogSheetUnitId, "TEST-MATERIAL", ItemCapabilities.DefaultMaterial),
            new CatalogFilterType(2, ItemType.Labor, "LAB", "ค่าแรง", "Labor", hourUnitId, "TEST-SERVICE", ItemCapabilities.DefaultService),
            new CatalogFilterType(3, ItemType.Service, "SVC", "บริการ", "Service", jobUnitId, "TEST-SERVICE", ItemCapabilities.DefaultService),
            new CatalogFilterType(4, ItemType.Product, "PRD", "สินค้า", "Product", eachUnitId, "TEST-MATERIAL", new ItemCapabilities(true, true, true, true, false)),
            new CatalogFilterType(5, ItemType.Subcontract, "SUB", "งานจ้างช่วง", "Subcontract", dayUnitId, "TEST-SERVICE", new ItemCapabilities(false, true, true, false, false)),
            new CatalogFilterType(6, ItemType.Other, "OTH", "รายการอื่น", "Other", eachUnitId, "TEST-SERVICE", new ItemCapabilities(true, true, false, false, false))
        };

        foreach (var type in typeFixtures)
        {
            var categoryId = DemoFixtureId(0x10, type.Index, 1);
            if (!await db.ItemCategories.AnyAsync(row => row.Id == categoryId))
            {
                db.ItemCategories.Add(new ItemCategory(categoryId, TestOrgId, $"TEST-CAT-{type.Code}",
                    LocalizedText.Create($"หมวด{type.Thai}กรอง TEST_ONLY", $"TEST_ONLY {type.English} Filter Category"),
                    LocalizedText.Create("หมวดตัวอย่างสำหรับการกรองรายการ", "Sample category for item filtering"),
                    null, [type.ItemType], type.Index + 1, TestUserId, now));
            }

            for (var itemNumber = 1; itemNumber <= 12; itemNumber++)
            {
                var status = ((itemNumber - 1) % 3) switch
                {
                    0 => ItemStatus.Active,
                    1 => ItemStatus.Draft,
                    _ => ItemStatus.Inactive
                };
                var code = $"TEST-{type.Code}-FILTER-{itemNumber:D2}";
                var attributes = new Dictionary<string, string>
                {
                    ["fixture"] = "item_catalog_filter_test_only",
                    ["item_type"] = type.ItemType,
                    ["variant"] = $"{type.Code}-{itemNumber:D2}"
                };
                if (type.ItemType == ItemType.Material)
                {
                    attributes["material"] = itemNumber % 2 == 0 ? "MDF" : "plywood";
                    attributes["thickness_mm"] = (9 + itemNumber).ToString();
                    attributes["width_mm"] = "1220";
                    attributes["length_mm"] = "2440";
                    attributes["finish"] = itemNumber % 2 == 0 ? "white_matte" : "natural";
                }

                var itemId = DemoFixtureId(0x80 + type.Index, itemNumber, 1);
                var demoItem = new CatalogFilterItem(
                    itemId,
                    DemoFixtureId(0x80 + type.Index, itemNumber, 2),
                    DemoFixtureId(0x80 + type.Index, itemNumber, 3),
                    DemoFixtureId(0x10, type.Index, 1),
                    brandIds[(itemNumber - 1) % brandIds.Length],
                    DemoFixtureId(0x90 + type.Index, itemNumber, 4),
                    type,
                    code,
                    LocalizedText.Create($"{type.Thai}กรอง TEST_ONLY {itemNumber:D2}", $"TEST_ONLY {type.English} Filter Item {itemNumber:D2}"),
                    LocalizedText.Create("ข้อมูลตัวอย่างสำหรับค้นหาและกรอง", "Sample specification for search and filters"),
                    attributes,
                    status,
                    type.ItemType == ItemType.Material ? 500m + itemNumber * 25m : 250m + type.Index * 50m + itemNumber * 10m,
                    $"TST.{type.Code}.{itemNumber:D2}");
                await AddCatalogFilterItemAsync(db, demoItem, now);
            }
        }
    }

    private static async Task AddCatalogFilterItemAsync(AppDbContext db, CatalogFilterItem demo, DateTimeOffset now)
    {
        var item = await db.Items.FirstOrDefaultAsync(row => row.Id == demo.ItemId);
        if (item is null)
        {
            item = Item.CreateDraft(demo.ItemId, TestOrgId, demo.Code, demo.Type.ItemType,
                demo.CategoryId, demo.BrandId, demo.Name, demo.Description, demo.Type.UnitId,
                ItemAvailabilityMode.AllBranches, demo.Type.Capabilities, demo.Attributes, 1,
                TestUserId, now, demo.Type.TaxCategoryCode);
            if (demo.Status != ItemStatus.Draft)
            {
                item.Activate(TestUserId, now, false, true, true, true);
            }
            if (demo.Status == ItemStatus.Inactive)
            {
                item.Deactivate(TestUserId, now, "TEST_ONLY_FILTER_FIXTURE", "Fixture row for inactive status filter");
            }
            db.Items.Add(item);
        }

        if (!await db.ItemBarcodes.AnyAsync(row => row.Id == demo.BarcodeId))
        {
            db.ItemBarcodes.Add(new ItemBarcode(demo.BarcodeId, TestOrgId, demo.ItemId,
                "internal", demo.Barcode, demo.Type.UnitId, 1m, "each", true, TestUserId, now));
        }

        if (demo.Status != ItemStatus.Active || await db.CostRecords.AnyAsync(row => row.Id == demo.CostRecordId))
        {
            return;
        }

        var costRecord = CostRecord.CreateDraft(demo.CostRecordId, TestOrgId, demo.ItemId,
            CostScopeType.Organization, null, demo.Type.UnitId, "THB", demo.CostAmount, 0m, null,
            now.AddDays(-30), null, 1, TestCostSourceId,
            $"TEST_ONLY-FILTER-SOURCE-{demo.Code}", "ราคาสมมติสำหรับทดสอบ filter เท่านั้น", null,
            TestUserId, now);
        costRecord.Submit(TestUserId, now);
        costRecord.Approve(TestUserIdB, now);
        costRecord.Publish(TestUserIdB, now);
        db.CostRecords.Add(costRecord);
        db.CostRecordReviews.Add(new CostRecordReview(demo.ReviewId, TestOrgId, costRecord.Id,
            "approved", "TEST_ONLY filter fixture review", TestUserIdB,
            $"user:{TestUserIdB},membership:{TestCostReviewerMembershipId}", now));
    }

    private static Guid DemoFixtureId(int group, int ordinal, int kind) =>
        Guid.Parse($"019a3cf8-96f0-7c9f-b207-81a1{group:x2}{ordinal:x4}{kind:x2}");

    private static async Task AddCatalogItemAsync(AppDbContext db, CatalogDemoItem demo)
    {
        var now = DateTimeOffset.UtcNow;
        var item = await db.Items.FirstOrDefaultAsync(row => row.Id == demo.ItemId);
        if (item is null)
        {
            item = Item.CreateDraft(
                demo.ItemId,
                TestOrgId,
                demo.Code,
                ItemType.Material,
                TestItemCatalogCategoryId,
                TestItemCatalogBrandId,
                demo.Name,
                demo.Description,
                demo.UnitId,
                ItemAvailabilityMode.AllBranches,
                ItemCapabilities.DefaultMaterial,
                demo.Attributes,
                1,
                TestUserId,
                now,
                "TEST-MATERIAL");
            item.Activate(TestUserId, now, false, true, true, true);
            db.Items.Add(item);
        }

        if (!await db.ItemBarcodes.AnyAsync(row => row.ItemId == demo.ItemId && row.Value == demo.Barcode))
        {
            db.ItemBarcodes.Add(new ItemBarcode(
                Guid.NewGuid(),
                TestOrgId,
                demo.ItemId,
                "internal",
                demo.Barcode,
                demo.UnitId,
                1m,
                "each",
                true,
                TestUserId,
                now));
        }

        if (await db.CostRecords.AnyAsync(row => row.Id == demo.CostRecordId))
        {
            return;
        }

        var costRecord = CostRecord.CreateDraft(
            demo.CostRecordId,
            TestOrgId,
            demo.ItemId,
            CostScopeType.Organization,
            null,
            demo.UnitId,
            "THB",
            demo.CostAmount,
            0m,
            null,
            now.AddDays(-30),
            null,
            1,
            TestCostSourceId,
            $"TEST_ONLY-SUPPLIER-QUOTE-{demo.Code}",
            "ราคาสมมติสำหรับทดสอบ Item Catalog เท่านั้น",
            null,
            TestUserId,
            now);
        costRecord.Submit(TestUserId, now);
        costRecord.Approve(TestUserIdB, now);
        costRecord.Publish(TestUserIdB, now);
        db.CostRecords.Add(costRecord);
        db.CostRecordReviews.Add(new CostRecordReview(
            Guid.NewGuid(),
            TestOrgId,
            costRecord.Id,
            "approved",
            "TEST_ONLY independent checker seed",
            TestUserIdB,
            $"user:{TestUserIdB},membership:{TestCostReviewerMembershipId}",
            now));
    }

    private sealed record CatalogDemoItem(
        Guid ItemId,
        Guid CostRecordId,
        string Code,
        LocalizedText Name,
        LocalizedText Description,
        Guid UnitId,
        decimal CostAmount,
        Dictionary<string, string> Attributes,
        string Barcode);

    private sealed record CatalogFilterType(
        int Index,
        string ItemType,
        string Code,
        string Thai,
        string English,
        Guid UnitId,
        string TaxCategoryCode,
        ItemCapabilities Capabilities);

    private sealed record CatalogFilterItem(
        Guid ItemId,
        Guid CostRecordId,
        Guid BarcodeId,
        Guid CategoryId,
        Guid BrandId,
        Guid ReviewId,
        CatalogFilterType Type,
        string Code,
        LocalizedText Name,
        LocalizedText Description,
        Dictionary<string, string> Attributes,
        string Status,
        decimal CostAmount,
        string Barcode);

    private static void AddDemoWorkItem(
        EstimateSection section,
        string code,
        string descriptionTh,
        string descriptionEn,
        decimal quantity,
        params (string Type, string Description, decimal UnitCost)[] costs)
    {
        var sortOrder = section.WorkItems.Count + 1;
        var itemId = Guid.NewGuid();
        var workItem = new EstimateWorkItem(itemId, TestOrgId, section.Id, code,
            descriptionTh, descriptionEn, quantity, "งาน", SellingRuleType.Margin, 0.30m, sortOrder);
        var componentOrder = 1;
        foreach (var cost in costs)
        {
            var component = new EstimateCostComponent(Guid.NewGuid(), TestOrgId, itemId,
                cost.Type, cost.Description, quantity, "งาน", cost.UnitCost, "THB", componentOrder++);
            component.SetProvisionalReason("TEST_ONLY_DEMO_COST", "ราคาสมมติสำหรับทดสอบเท่านั้น");
            workItem.AddCostComponent(component);
        }

        workItem.SetCustomWorkItemReason(DemoCustomWorkItemReasonCode, DemoCustomWorkItemReason);
        section.AddWorkItem(workItem);
    }
}
