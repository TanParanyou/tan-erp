"use client";

import React, { useMemo, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { useLocale, useTranslations } from "next-intl";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormSection } from "@/components/forms/FormSection";
import { FormActionBar } from "@/components/forms/FormActionBar";
import { Alert } from "@/components/ui/Alert";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Textarea } from "@/components/ui/Textarea";
import { useToast } from "@/hooks/useToast";
import type { RoleDefinitionResponse, RoleGrantRequest } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import {
  useAdminPermissionCatalog,
  useCreateRoleDefinition,
  useRoleDefinition,
  useUpdateRoleDefinition,
} from "../api/role-management-queries";
import { roleAdminErrorKey } from "../role-management-errors";
import { RoleActivationPanel } from "./role-activation-panel";
import { PermissionGrantPicker, type CatalogItem } from "./permission-grant-picker";

const ORGANIZATION_SCOPE = "organization";

interface RoleFormValues {
  name: string;
  description: string;
}

interface ScopedGrant {
  permissionKey: string;
  scope: string;
  branchId: string | null;
  branchName: string | null;
}

const orNull = (value: string): string | null => (value.trim() === "" ? null : value.trim());

/** Branch/own-scope grants cannot be edited here; they are kept and sent back unchanged on save. */
function toScopedGrants(role: RoleDefinitionResponse | undefined): ScopedGrant[] {
  return (role?.permissions ?? []).flatMap((p) =>
    p.permissionKey && p.scope && p.scope !== ORGANIZATION_SCOPE
      ? [{ permissionKey: p.permissionKey, scope: p.scope, branchId: p.branch?.id ?? null, branchName: p.branch?.name ?? null }]
      : []
  );
}

export function RoleDefinitionEditor({ roleId }: { roleId: string }) {
  const isCreate = roleId === "create" || roleId === "add";
  const t = useTranslations("roleAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const router = useRouter();
  const { toast } = useToast();
  const { selectedMembership } = useSelectedMembership();
  const canManage = can(selectedMembership, PERMISSIONS.ROLES_MANAGE);
  const roleQuery = useRoleDefinition(roleId, !isCreate);
  const catalogQuery = useAdminPermissionCatalog();
  const createMutation = useCreateRoleDefinition();
  const updateMutation = useUpdateRoleDefinition();
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [edited, setEdited] = useState<ReadonlySet<string> | null>(null);
  // One key per submit intent: a retry after a network failure replays the same request instead of creating twice.
  const intentRef = useRef<{ fingerprint: string; key: string } | null>(null);

  // Create mode never reads an existing role, even if the query layer still holds data for the route param.
  const role = isCreate ? undefined : roleQuery.data;
  const existing = role?.id && role.rowVersion ? { id: role.id, rowVersion: role.rowVersion } : null;
  const catalog = useMemo<CatalogItem[]>(
    () =>
      (catalogQuery.data?.items ?? []).flatMap((item) =>
        item.key ? [{ key: item.key, description: item.description ?? null, isApprovalPermission: item.isApprovalPermission === true }] : []
      ),
    [catalogQuery.data]
  );
  const baseline = useMemo<ReadonlySet<string>>(
    () => new Set((role?.permissions ?? []).flatMap((p) => (p.permissionKey && p.scope === ORGANIZATION_SCOPE ? [p.permissionKey] : []))),
    [role]
  );
  const scoped = useMemo(() => toScopedGrants(role), [role]);
  const selected = edited ?? baseline;
  const permissionsChanged = edited !== null && (edited.size !== baseline.size || [...edited].some((key) => !baseline.has(key)));

  const schema = useMemo(
    () =>
      z.object({
        name: z.string().trim().min(1, t("editor.validation.nameRequired")).min(3, t("editor.validation.nameLength")).max(100, t("editor.validation.nameLength")),
        description: z.string().trim().max(500),
      }),
    [t]
  );
  const {
    register,
    handleSubmit,
    formState: { errors, isDirty },
  } = useForm<RoleFormValues>({
    resolver: zodResolver(schema),
    defaultValues: { name: "", description: "" },
    values: !isCreate && role ? { name: role.name ?? "", description: role.description ?? "" } : undefined,
  });

  if (!isCreate && roleQuery.isPending) return <MonoSpinner size="lg" label={tCommon("states.loading")} />;
  if (!isCreate && (roleQuery.isError || !existing)) return <Alert variant="danger">{t("errors.RESOURCE_NOT_FOUND")}</Alert>;

  const backHref = `/${locale}/settings/roles`;
  const isSaving = createMutation.isPending || updateMutation.isPending;
  const locked = role?.isSystem === true || role?.pendingChange != null;
  const canEdit = canManage && !locked;
  const addsApproval =
    !isCreate && (role?.memberCount ?? 0) > 0 && catalog.some((c) => c.isApprovalPermission && selected.has(c.key) && !baseline.has(c.key));

  const toggle = (key: string) => {
    const next = new Set(selected);
    if (next.has(key)) next.delete(key);
    else next.add(key);
    setEdited(next);
  };

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);
    const permissions: RoleGrantRequest[] = [
      ...[...selected].sort().map((permissionKey) => ({ permissionKey, scope: ORGANIZATION_SCOPE, branchId: null })),
      ...scoped.map((g) => ({ permissionKey: g.permissionKey, scope: g.scope, branchId: g.branchId })),
    ];
    const payload = { name: values.name.trim(), description: orNull(values.description), permissions };
    try {
      if (isCreate) {
        const fingerprint = JSON.stringify(payload);
        if (intentRef.current?.fingerprint !== fingerprint) intentRef.current = { fingerprint, key: crypto.randomUUID() };
        const created = await createMutation.mutateAsync({ payload, idempotencyKey: intentRef.current.key });
        intentRef.current = null;
        toast.success(t("editor.createSuccess"));
        if (created.id) router.push(`${backHref}/${created.id}`);
      } else if (existing) {
        const result = await updateMutation.mutateAsync({ roleId: existing.id, payload, ifMatch: existing.rowVersion });
        // 202 from the server: the change became a request for a checker and is not in effect yet.
        toast.success(t(result.pendingRequest ? "editor.pendingSubmitted" : "editor.saveSuccess"));
        setEdited(null);
      }
    } catch (error: unknown) {
      setSubmitError(t(`errors.${roleAdminErrorKey(error)}`));
    }
  });

  return (
    <div className="space-y-6">
      <FormContainer
        asForm
        noValidate
        onSubmit={onSubmit}
        maxWidth="lg"
        header={
          <PageHeader
            title={isCreate ? t("editor.createTitle") : t("editor.editTitle")}
            subtitle={isCreate ? t("editor.createSubtitle") : (role?.name ?? undefined)}
            backHref={backHref}
            backLabel={t("editor.backToList")}
          />
        }
        errorBanner={submitError ? <Alert variant="danger">{submitError}</Alert> : undefined}
        actionBar={
          canEdit ? (
            <FormActionBar
              isDirty={isDirty || permissionsChanged}
              isLoading={isSaving}
              isSaveDisabled={isSaving}
              saveText={isCreate ? t("editor.createSubmit") : t("editor.saveSubmit")}
              saveButtonType="submit"
              cancelHref={backHref}
              showCancel
              cancelText={tCommon("actions.cancel")}
            />
          ) : undefined
        }
      >
        {role?.isSystem === true ? <Alert variant="info">{t("editor.systemReadOnly")}</Alert> : null}
        {role?.pendingChange != null ? <Alert variant="warning">{t("editor.pendingBanner")}</Alert> : null}
        <FormSection title={isCreate ? t("editor.createTitle") : t("editor.editTitle")}>
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
            <Input label={t("editor.fields.name")} required disabled={!canEdit} error={errors.name?.message} {...register("name")} />
            <Textarea label={t("editor.fields.description")} rows={2} disabled={!canEdit} error={errors.description?.message} {...register("description")} />
          </div>
        </FormSection>
        <FormSection title={t("editor.permissions.title")}>
          {addsApproval ? <Alert variant="warning">{t("editor.approvalNotice")}</Alert> : null}
          {catalogQuery.isPending ? (
            <MonoSpinner size="md" label={tCommon("states.loading")} />
          ) : catalogQuery.isError ? (
            <Alert variant="danger">{t("errors.GENERIC")}</Alert>
          ) : (
            <PermissionGrantPicker catalog={catalog} selected={selected} disabled={!canEdit} onToggle={toggle} />
          )}
          {scoped.length > 0 ? (
            <div className="space-y-1">
              <p className="text-sm font-semibold text-erp-text-main">{t("editor.permissions.scopedTitle")}</p>
              <p className="text-xs text-erp-text-muted">{t("editor.permissions.scopedHint")}</p>
              <ul className="list-disc pl-5 text-sm">
                {scoped.map((g) => (
                  <li key={`${g.permissionKey}-${g.scope}-${g.branchId ?? "own"}`}>
                    {g.permissionKey} · {t(`editor.permissions.scope.${g.scope === "branch" ? "branch" : "own"}`)}
                    {g.branchName ? ` ${g.branchName}` : ""}
                  </li>
                ))}
              </ul>
            </div>
          ) : null}
        </FormSection>
      </FormContainer>
      {!isCreate && role ? <RoleActivationPanel role={role} canManage={canManage} /> : null}
    </div>
  );
}
