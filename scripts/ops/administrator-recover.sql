\set ON_ERROR_STOP on
BEGIN;

-- 1. The target must be a real, signed-in, active user that belongs to this organization.
--    Any other target raises an error, which stops the script and leaves nothing committed.
SELECT EXISTS (
  SELECT 1 FROM organization.memberships m
  JOIN identity_access.users u ON u.id = m.user_id
  WHERE m.id = :'target_membership_id' AND m.organization_id = :'organization_id'
    AND u.is_active AND u.firebase_uid IS NOT NULL) AS target_ok \gset
-- The message comes from a subquery so PostgreSQL cannot fold the untaken branch into a plan-time error.
SELECT CASE WHEN :'target_ok'::boolean THEN 1 ELSE CAST(g.message AS integer) END AS guard
FROM (SELECT 'target membership is not an active, signed-in user of this organization' AS message) g;

-- 2. Re-enable the membership and clear a lapsed window so it is usable now.
UPDATE organization.memberships
SET is_active = true,
    starts_at_utc = CASE WHEN starts_at_utc > now() THEN NULL ELSE starts_at_utc END,
    expires_at_utc = CASE WHEN expires_at_utc <= now() THEN NULL ELSE expires_at_utc END,
    row_version = gen_random_uuid()
WHERE id = :'target_membership_id' AND organization_id = :'organization_id';

-- 3. Grant the administrator role (no-op when already granted).
INSERT INTO identity_access.membership_roles (membership_id, role_id, organization_id, assigned_at_utc)
SELECT :'target_membership_id', :'admin_role_id', :'organization_id', now()
WHERE NOT EXISTS (
  SELECT 1 FROM identity_access.membership_roles
  WHERE membership_id = :'target_membership_id' AND role_id = :'admin_role_id');

-- 4. Audit the manual grant. actor_user_id must be an existing users.id (the operator's user, or the target when none).
INSERT INTO audit.audit_events (id, organization_id, actor_user_id, action, resource_type, resource_id,
                                occurred_at_utc, trace_id, changes, reason)
VALUES (gen_random_uuid(), :'organization_id', :'actor_user_id', 'users.recovery-grant', 'Membership',
        :'target_membership_id', now(), 'manual-recovery',
        jsonb_build_object('roleId', :'admin_role_id'), :'reason');

-- 5. Verify before committing: at least one administrator must now exist (run status.sql, then COMMIT or ROLLBACK).
