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
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { Textarea } from "@/components/ui/Textarea";
import { IconTrash } from "@/components/common/Icons";
import { useToast } from "@/hooks/useToast";
import type { RoleDefinitionResponse, RoleGrantRequest } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useAdminBranches } from "@/features/settings/organization/api/organization-admin-queries";
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

/** Scoped (branch/own) grants can be added, viewed and removed; they are sent on save. */
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
  const branchesQuery = useAdminBranches("active");
  const branches = branchesQuery.data ?? [];
  const createMutation = useCreateRoleDefinition();
  const updateMutation = useUpdateRoleDefinition();
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [edited, setEdited] = useState<ReadonlySet<string> | null>(null);

  // Scoped permission grants (branch / own)
  const [newPermissionKey, setNewPermissionKey] = useState("");
  const [newScope, setNewScope] = useState<"branch" | "own">("branch");
  const [newBranchId, setNewBranchId] = useState("");

  // One key per submit intent: a retry after a network failure replays the same request instead of creating twice.
  const intentRef = useRef<{ fingerprint: string; key: string } | null>(null);

  // Create mode never reads an existing role, even if the query layer still holds data for the route param.
  const role = isCreate ? undefined : roleQuery.data;
  const existing = role?.id && role.rowVersion ? { id: role.id, rowVersion: role.rowVersion } : null;
  const catalog = useMemo<CatalogItem[]>(
    () =>
      (catalogQuery.data?.items ?? []).flatMap((item) =>
        item.key
          ? [
              {
                key: item.key,
                description: item.description ?? null,
                module: item.module ?? null,
                requiresMakerChecker: item.requiresMakerChecker ?? item.isApprovalPermission === true,
                isDocumentApproval: item.isDocumentApproval === true,
                isApprovalPermission: item.isApprovalPermission === true,
              },
            ]
          : []
      ),
    [catalogQuery.data]
  );
  const baseline = useMemo<ReadonlySet<string>>(
    () => new Set((role?.permissions ?? []).flatMap((p) => (p.permissionKey && p.scope === ORGANIZATION_SCOPE ? [p.permissionKey] : []))),
    [role]
  );
  const baselineScoped = useMemo(() => toScopedGrants(role), [role]);
  const [editedScoped, setEditedScoped] = useState<ScopedGrant[] | null>(null);
  const scopedGrants = editedScoped ?? baselineScoped;

  const scopedGrantsChanged = useMemo(() => {
    if (editedScoped === null) return false;
    if (editedScoped.length !== baselineScoped.length) return true;
    const baselineKeys = new Set(
      baselineScoped.map((g) => `${g.permissionKey}:${g.scope}:${g.branchId ?? ""}`)
    );
    return editedScoped.some(
      (g) => !baselineKeys.has(`${g.permissionKey}:${g.scope}:${g.branchId ?? ""}`)
    );
  }, [editedScoped, baselineScoped]);

  const selected = edited ?? baseline;
  const orgPermissionsChanged = edited !== null && (edited.size !== baseline.size || [...edited].some((key) => !baseline.has(key)));
  const permissionsChanged = orgPermissionsChanged || scopedGrantsChanged;

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
  const hasDocumentApproval = catalog.some((c) => c.isDocumentApproval && selected.has(c.key));

  const toggle = (key: string) => {
    const next = new Set(selected);
    if (next.has(key)) next.delete(key);
    else next.add(key);
    setEdited(next);
  };

  const batchToggle = (keys: string[], select: boolean) => {
    const next = new Set(selected);
    for (const key of keys) {
      if (select) next.add(key);
      else next.delete(key);
    }
    setEdited(next);
  };

  const handleAddScoped = () => {
    if (!newPermissionKey) return;
    if (newScope === "branch" && !newBranchId) return;
    const targetBranchId = newScope === "branch" ? newBranchId : null;
    const duplicate = scopedGrants.some(
      (g) => g.permissionKey === newPermissionKey && g.scope === newScope && g.branchId === targetBranchId
    );
    if (duplicate) {
      toast.error(t("editor.permissions.duplicateScoped"));
      return;
    }
    const branchObj = newScope === "branch" ? branches.find((b) => b.id === newBranchId) : null;
    const nextGrant: ScopedGrant = {
      permissionKey: newPermissionKey,
      scope: newScope,
      branchId: targetBranchId,
      branchName: branchObj?.name ?? branchObj?.code ?? targetBranchId,
    };
    setEditedScoped([...scopedGrants, nextGrant]);
    setNewPermissionKey("");
    setNewBranchId("");
  };

  const handleRemoveScoped = (index: number) => {
    setEditedScoped(scopedGrants.filter((_, i) => i !== index));
  };

  const onSubmit = handleSubmit(async (values) => {
    setSubmitError(null);
    const permissions: RoleGrantRequest[] = [
      ...[...selected].sort().map((permissionKey) => ({ permissionKey, scope: ORGANIZATION_SCOPE, branchId: null })),
      ...scopedGrants.map((g) => ({ permissionKey: g.permissionKey, scope: g.scope, branchId: g.branchId })),
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
        setEditedScoped(null);
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
            <Input
              label={t("editor.fields.name")}
              placeholder={t("editor.fields.namePlaceholder")}
              required
              disabled={!canEdit}
              error={errors.name?.message}
              {...register("name")}
            />
            <Textarea
              label={t("editor.fields.description")}
              placeholder={t("editor.fields.descriptionPlaceholder")}
              rows={2}
              disabled={!canEdit}
              error={errors.description?.message}
              {...register("description")}
            />
          </div>
        </FormSection>
        <FormSection title={t("editor.permissions.title")}>
          {addsApproval ? <Alert variant="warning">{t("editor.approvalNotice")}</Alert> : null}
          {hasDocumentApproval ? (
            <div className="border border-blue-200 bg-blue-50/70 p-3 text-xs text-blue-900">
              💡 {t("editor.permissions.approvalNoticeHint")}
            </div>
          ) : null}
          {catalogQuery.isPending ? (
            <MonoSpinner size="md" label={tCommon("states.loading")} />
          ) : catalogQuery.isError ? (
            <Alert variant="danger">{t("errors.GENERIC")}</Alert>
          ) : (
            <PermissionGrantPicker
              catalog={catalog}
              selected={selected}
              disabled={!canEdit}
              onToggle={toggle}
              onBatchToggle={batchToggle}
            />
          )}
          <div className="space-y-3 pt-4 border-t border-erp-border">
            <div>
              <p className="text-sm font-semibold text-erp-text-main">{t("editor.permissions.scopedTitle")}</p>
              <p className="text-xs text-erp-text-muted">{t("editor.permissions.scopedHint")}</p>
            </div>
            {canEdit && (
              <div className="flex flex-wrap items-end gap-2 border border-erp-border bg-erp-surface-subtle p-3">
                <div className="min-w-52 flex-1">
                  <Select
                    label={t("editor.permissions.selectPermission")}
                    value={newPermissionKey}
                    onChange={(e) => setNewPermissionKey(e.target.value)}
                    options={[
                      { value: "", label: t("editor.permissions.selectPermission") },
                      ...catalog.map((c) => ({ value: c.key, label: c.key })),
                    ]}
                    wrapperClassName="!mb-0"
                  />
                </div>
                <div className="w-36">
                  <Select
                    label={t("editor.permissions.selectScope")}
                    value={newScope}
                    onChange={(e) => {
                      const val = e.target.value as "branch" | "own";
                      setNewScope(val);
                      if (val !== "branch") setNewBranchId("");
                    }}
                    options={[
                      { value: "branch", label: t("editor.permissions.scope.branch") },
                      { value: "own", label: t("editor.permissions.scope.own") },
                    ]}
                    wrapperClassName="!mb-0"
                  />
                </div>
                {newScope === "branch" && (
                  <div className="min-w-44 flex-1 sm:flex-none">
                    <Select
                      label={t("editor.permissions.selectBranch")}
                      value={newBranchId}
                      onChange={(e) => setNewBranchId(e.target.value)}
                      disabled={branchesQuery.isLoading || branches.length === 0}
                      options={[
                        {
                          value: "",
                          label: branchesQuery.isLoading
                            ? tCommon("states.loading")
                            : branches.length === 0
                            ? t("editor.permissions.noBranches")
                            : t("editor.permissions.selectBranch"),
                        },
                        ...branches.map((b) => ({ value: b.id ?? "", label: b.name ?? b.code ?? b.id ?? "" })),
                      ]}
                      wrapperClassName="!mb-0"
                    />
                  </div>
                )}
                <Button
                  type="button"
                  variant="primary"
                  className="h-11 min-h-[44px]"
                  disabled={!newPermissionKey || (newScope === "branch" && !newBranchId)}
                  onClick={handleAddScoped}
                >
                  {t("editor.permissions.addScoped")}
                </Button>
              </div>
            )}
            {scopedGrants.length > 0 ? (
              <ul className="divide-y divide-erp-border border border-erp-border">
                {scopedGrants.map((g, index) => (
                  <li
                    key={`${g.permissionKey}-${g.scope}-${g.branchId ?? "own"}`}
                    className="flex items-center justify-between p-2.5 text-sm"
                  >
                    <div>
                      <span className="font-mono font-medium text-erp-navy">{g.permissionKey}</span>
                      <span className="mx-2 text-erp-text-muted">·</span>
                      <span className="text-erp-text-main">
                        {t(`editor.permissions.scope.${g.scope === "branch" ? "branch" : "own"}`)}
                      </span>
                      {g.scope === "branch" && (
                        <span className="ml-1 text-erp-text-muted">
                          {g.branchName ? ` ${g.branchName}` : branches.find((b) => b.id === g.branchId)?.name ? ` ${branches.find((b) => b.id === g.branchId)?.name}` : g.branchId ? ` (${g.branchId})` : ""}
                        </span>
                      )}
                    </div>
                    {canEdit && (
                      <Button
                        type="button"
                        size="icon"
                        variant="ghost"
                        className="h-9 w-9 text-erp-text-muted hover:text-erp-danger"
                        aria-label={t("editor.permissions.removeScoped")}
                        onClick={() => handleRemoveScoped(index)}
                      >
                        <IconTrash size={16} />
                      </Button>
                    )}
                  </li>
                ))}
              </ul>
            ) : (
              <p className="text-xs text-erp-text-muted italic">{t("editor.permissions.noScopedGrants")}</p>
            )}
          </div>
        </FormSection>
      </FormContainer>
      {!isCreate && role ? <RoleActivationPanel role={role} canManage={canManage} /> : null}
    </div>
  );
}
