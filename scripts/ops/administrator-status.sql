SELECT m.id AS membership_id, u.display_name
FROM organization.memberships m
JOIN identity_access.users u ON u.id = m.user_id
JOIN organization.organizations o ON o.id = m.organization_id
WHERE m.organization_id = :'organization_id'
  AND m.is_active AND u.is_active AND o.is_active AND u.firebase_uid IS NOT NULL
  AND (m.starts_at_utc IS NULL OR m.starts_at_utc <= now())
  AND (m.expires_at_utc IS NULL OR m.expires_at_utc > now())
  AND EXISTS (
    SELECT 1
    FROM identity_access.membership_roles mr
    JOIN identity_access.roles r ON r.id = mr.role_id AND r.is_active
    JOIN identity_access.role_permissions rp ON rp.role_id = r.id AND rp.scope = 'organization' AND rp.scope_id = m.organization_id
    JOIN identity_access.permissions p ON p.id = rp.permission_id AND p.is_active AND p.key = 'users.manage'
    WHERE mr.membership_id = m.id);
