"use client";

import React, { useRef, useState } from "react";
import { useLocale, useTranslations } from "next-intl";
import { useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { PageHeader } from "@/components/layout/PageHeader";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { Input } from "@/components/ui/Input";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Select } from "@/components/ui/Select";
import { StatusBadge, type BadgeVariant } from "@/components/ui/StatusBadge";
import { Avatar } from "@/components/ui/Avatar";
import { useToast } from "@/hooks/useToast";
import { useDeferredFileUpload } from "@/hooks/useDeferredFileUpload";
import { useApiRequestContext } from "@/lib/api/use-api-request-context";
import { getAuthToken } from "@/lib/auth/auth-session";
import { formatDateTime } from "@/lib/formatters/formatters";
import type { AdminMembershipResponse, AdminUserResponse } from "@/lib/api/api-client";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { can, PERMISSIONS } from "@/lib/permissions/can";
import { useOrganizationBranches } from "@/features/item-master/api/item-master-queries";
import {
  useAdminRoles,
  useAdminUser,
  useAssignAdminRole,
  useDecideAdminRoleRequest,
  useRevokeAdminRole,
  useSetAdminMembershipActive,
  useSetAdminUserActive,
  useUpdateAdminMembership,
  useUpdateAdminUserProfile,
} from "../api/user-admin-queries";
import { adminErrorKey } from "../user-admin-errors";
import { UserAdminFormFields, type UserAdminFormValues } from "./user-admin-form-fields";

const STATUS_VARIANTS: Record<string, BadgeVariant> = {
  active: "success",
  pending: "warning",
  inactive: "neutral",
};

type PendingAction =
  | { kind: "user-active"; active: boolean }
  | { kind: "membership-active"; membership: AdminMembershipResponse; active: boolean }
  | { kind: "revoke-role"; membership: AdminMembershipResponse; roleId: string; roleName: string }
  | { kind: "cancel-request"; requestId: string; rowVersion: string; roleName: string };

interface UserAdminDetailProps {
  userId: string;
}

function toInputValue(iso: string | null | undefined): string {
  if (!iso) return "";
  const date = new Date(iso);
  if (Number.isNaN(date.getTime())) return "";
  const offsetMs = date.getTimezoneOffset() * 60_000;
  return new Date(date.getTime() - offsetMs).toISOString().slice(0, 16);
}

function fromInputValue(value: string): string | null {
  if (!value) return null;
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? null : date.toISOString();
}

export function UserAdminDetail({ userId }: UserAdminDetailProps) {
  const t = useTranslations("userAdmin");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const { toast } = useToast();
  const context = useApiRequestContext();
  const { selectedMembership } = useSelectedMembership();

  const canManageUsers = can(selectedMembership, PERMISSIONS.USERS_MANAGE);
  const canManageMemberships = can(selectedMembership, PERMISSIONS.MEMBERSHIPS_MANAGE);
  const canAssignRoles = can(selectedMembership, PERMISSIONS.ROLES_ASSIGN);

  const userQuery = useAdminUser(userId, true);
  const rolesQuery = useAdminRoles(canAssignRoles);
  const branchesQuery = useOrganizationBranches();

  const updateProfileMutation = useUpdateAdminUserProfile();
  const userActiveMutation = useSetAdminUserActive();
  const membershipActiveMutation = useSetAdminMembershipActive();
  const updateMembershipMutation = useUpdateAdminMembership();
  const assignMutation = useAssignAdminRole();
  const revokeMutation = useRevokeAdminRole();
  const decideMutation = useDecideAdminRoleRequest();

  const { uploadSingleFile, isUploading: isAvatarUploading } = useDeferredFileUpload({
    parentType: "user",
    parentId: userId,
  });

  const assignIntentRef = useRef<{ fingerprint: string; key: string } | null>(null);
  const [pendingAction, setPendingAction] = useState<PendingAction | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isEditingProfile, setIsEditingProfile] = useState(false);
  const [avatarFile, setAvatarFile] = useState<File | null>(null);

  const [roleChoice, setRoleChoice] = useState<Record<string, string>>({});
  const [membershipDraft, setMembershipDraft] = useState<
    Record<string, { branchId: string; startsAt: string; expiresAt: string }>
  >({});

  const user: AdminUserResponse | undefined = userQuery.data;

  const profileSchema = z.object({
    displayName: z.string().trim().min(1, t("validation.displayNameRequired")).max(255, t("validation.displayNameTooLong")),
    email: z.string(),
    phoneNumber: z.string().max(50, t("validation.phoneTooLong")),
    employeeCode: z.string().max(50, t("validation.codeTooLong")),
    departmentId: z.string(),
    jobTitleId: z.string(),
    notes: z.string().max(1000, t("validation.notesTooLong")),
    branchId: z.string(),
    startsAt: z.string(),
    expiresAt: z.string(),
    roleIds: z.array(z.string()),
  });

  const {
    register,
    control,
    handleSubmit,
    reset,
    setValue,
    formState: { errors, isDirty },
  } = useForm<UserAdminFormValues>({
    resolver: zodResolver(profileSchema),
    values: {
      displayName: user?.displayName ?? "",
      email: user?.email ?? "",
      phoneNumber: user?.phoneNumber ?? "",
      employeeCode: user?.employeeCode ?? "",
      departmentId: user?.department?.id ?? "",
      jobTitleId: user?.jobTitle?.id ?? "",
      notes: user?.notes ?? "",
      branchId: "",
      startsAt: "",
      expiresAt: "",
      roleIds: [],
    },
  });

  const watchedDisplayName = useWatch({ control, name: "displayName" });
  const watchedDepartmentId = useWatch({ control, name: "departmentId" });

  const fail = (error: unknown) => setErrorMessage(t(`errors.${adminErrorKey(error)}`));

  const isBusy =
    updateProfileMutation.isPending ||
    isAvatarUploading ||
    userActiveMutation.isPending ||
    membershipActiveMutation.isPending ||
    updateMembershipMutation.isPending ||
    assignMutation.isPending ||
    revokeMutation.isPending ||
    decideMutation.isPending;

  if (userQuery.isPending) {
    return <MonoSpinner size="lg" label={tCommon("states.loading")} />;
  }

  if (userQuery.isError || !user || !user.id || !user.rowVersion) {
    return <Alert variant="danger">{t("errors.RESOURCE_NOT_FOUND")}</Alert>;
  }

  const userRowVersion = user.rowVersion;
  const status = user.status ?? "inactive";

  const onSaveProfile = handleSubmit(async (values) => {
    setErrorMessage(null);
    let avatarFileId = user.avatar?.id ?? null;

    if (avatarFile) {
      try {
        const token = await getAuthToken();
        const uploadRes = await uploadSingleFile(avatarFile, {
          token: token ?? "",
          membershipId: context.membershipId,
          locale,
        });
        avatarFileId = uploadRes?.fileId ?? null;
      } catch (err: unknown) {
        fail(err);
        return;
      }
    }

    try {
      await updateProfileMutation.mutateAsync({
        userId,
        ifMatch: userRowVersion,
        payload: {
          displayName: values.displayName.trim(),
          phoneNumber: values.phoneNumber.trim() || null,
          employeeCode: values.employeeCode.trim() || null,
          departmentId: values.departmentId || null,
          jobTitleId: values.jobTitleId || null,
          avatarFileId,
          notes: values.notes.trim() || null,
        },
      });
      setAvatarFile(null);
      setIsEditingProfile(false);
      toast.success(t("saved"));
    } catch (err: unknown) {
      fail(err);
    }
  });

  const confirmAction = async () => {
    if (!pendingAction) return;
    setErrorMessage(null);
    try {
      if (pendingAction.kind === "user-active") {
        await userActiveMutation.mutateAsync({ userId, active: pendingAction.active, ifMatch: userRowVersion });
      } else if (pendingAction.kind === "membership-active") {
        const { membership, active } = pendingAction;
        if (!membership.id || !membership.rowVersion) return;
        await membershipActiveMutation.mutateAsync({ membershipId: membership.id, active, ifMatch: membership.rowVersion });
      } else if (pendingAction.kind === "revoke-role") {
        if (!pendingAction.membership.id) return;
        await revokeMutation.mutateAsync({ membershipId: pendingAction.membership.id, roleId: pendingAction.roleId });
      } else {
        await decideMutation.mutateAsync({
          requestId: pendingAction.requestId,
          decision: "cancel",
          ifMatch: pendingAction.rowVersion,
        });
      }
      toast.success(t("saved"));
      setPendingAction(null);
    } catch (error: unknown) {
      fail(error);
      setPendingAction(null);
    }
  };

  const assignRole = async (membership: AdminMembershipResponse) => {
    const roleId = roleChoice[membership.id ?? ""];
    if (!membership.id || !roleId) return;
    setErrorMessage(null);
    const fingerprint = `${membership.id}|${roleId}`;
    if (assignIntentRef.current?.fingerprint !== fingerprint) {
      assignIntentRef.current = { fingerprint, key: crypto.randomUUID() };
    }
    try {
      const outcome = await assignMutation.mutateAsync({
        membershipId: membership.id,
        roleId,
        idempotencyKey: assignIntentRef.current.key,
      });
      assignIntentRef.current = null;
      setRoleChoice((current) => ({ ...current, [membership.id ?? ""]: "" }));
      toast.success(outcome.pendingRequest ? t("assignPendingApproval") : t("assigned"));
    } catch (error: unknown) {
      fail(error);
    }
  };

  const saveMembership = async (membership: AdminMembershipResponse) => {
    const draft = membershipDraft[membership.id ?? ""];
    if (!membership.id || !membership.rowVersion || !draft) return;
    setErrorMessage(null);
    try {
      await updateMembershipMutation.mutateAsync({
        membershipId: membership.id,
        ifMatch: membership.rowVersion,
        payload: {
          branchId: draft.branchId || null,
          startsAtUtc: fromInputValue(draft.startsAt),
          expiresAtUtc: fromInputValue(draft.expiresAt),
        },
      });
      setMembershipDraft((current) => {
        const next = { ...current };
        delete next[membership.id ?? ""];
        return next;
      });
      toast.success(t("saved"));
    } catch (error: unknown) {
      fail(error);
    }
  };

  const modalCopy = (() => {
    if (!pendingAction) return null;
    switch (pendingAction.kind) {
      case "user-active":
        return pendingAction.active
          ? { title: t("confirm.activateUserTitle"), message: t("confirm.activateUserMessage"), variant: "info" as const }
          : { title: t("confirm.deactivateUserTitle"), message: t("confirm.deactivateUserMessage"), variant: "danger" as const };
      case "membership-active":
        return pendingAction.active
          ? { title: t("confirm.activateMembershipTitle"), message: t("confirm.activateMembershipMessage"), variant: "info" as const }
          : { title: t("confirm.deactivateMembershipTitle"), message: t("confirm.deactivateMembershipMessage"), variant: "danger" as const };
      case "revoke-role":
        return {
          title: t("confirm.revokeRoleTitle"),
          message: t("confirm.revokeRoleMessage", { role: pendingAction.roleName }),
          variant: "danger" as const,
        };
      default:
        return {
          title: t("confirm.cancelRequestTitle"),
          message: t("confirm.cancelRequestMessage", { role: pendingAction.roleName }),
          variant: "danger" as const,
        };
    }
  })();

  const availableRoles = (membership: AdminMembershipResponse) => {
    const assigned = new Set((membership.roles ?? []).map((role) => role.id));
    const requested = new Set((membership.pendingRoleRequests ?? []).map((request) => request.role?.id));
    return (rolesQuery.data?.items ?? []).filter((role) => role.assignable && !assigned.has(role.id) && !requested.has(role.id));
  };

  return (
    <div className="space-y-6">
      <PageHeader
        title={user.displayName ?? "-"}
        subtitle={user.email ?? undefined}
        backHref={`/${locale}/settings/users`}
        backLabel={t("backToList")}
        actions={
          <div className="flex items-center gap-3">
            <StatusBadge label={t(`status.${status}`)} variant={STATUS_VARIANTS[status]} />
            {canManageUsers ? (
              <Button
                variant={status === "inactive" ? "primary" : "outline"}
                size="md"
                disabled={isBusy}
                onClick={() => setPendingAction({ kind: "user-active", active: status === "inactive" })}
              >
                {status === "inactive" ? t("activateUser") : t("deactivateUser")}
              </Button>
            ) : null}
          </div>
        }
      />

      {errorMessage ? <Alert variant="danger">{errorMessage}</Alert> : null}
      {status === "pending" ? <Alert variant="info">{t("pendingNotice")}</Alert> : null}

      {/* User Information & Master Data Summary Card */}
      <section className="border border-erp-border bg-erp-surface p-4 space-y-4" aria-labelledby="user-profile-heading">
        <div className="flex flex-wrap items-center justify-between gap-3 border-b border-erp-border pb-3">
          <h2 id="user-profile-heading" className="text-base font-bold text-erp-text-main">
            {t("sections.identity")}
          </h2>
          {canManageUsers && !isEditingProfile && (
            <Button
              variant="outline"
              size="sm"
              disabled={isBusy}
              onClick={() => setIsEditingProfile(true)}
            >
              {tCommon("actions.edit")}
            </Button>
          )}
        </div>

        {isEditingProfile ? (
          <form onSubmit={onSaveProfile} className="space-y-4">
            <UserAdminFormFields
              mode="edit"
              register={register}
              control={control}
              errors={errors}
              setValue={setValue}
              avatarFile={avatarFile}
              onAvatarFileChange={setAvatarFile}
              existingAvatarFileId={user.avatar?.id}
              existingAvatarUrl={user.avatar?.servingUrl}
              currentDisplayName={watchedDisplayName}
              selectedDepartmentId={watchedDepartmentId}
              hideRoles={true}
              hideMembershipDates={true}
              disabled={isBusy}
            />
            <div className="flex justify-end gap-2 pt-2 border-t border-erp-border">
              <Button
                type="button"
                variant="outline"
                size="md"
                disabled={isBusy}
                onClick={() => {
                  reset();
                  setAvatarFile(null);
                  setIsEditingProfile(false);
                }}
              >
                {tCommon("actions.cancel")}
              </Button>
              <Button
                type="submit"
                variant="primary"
                size="md"
                isLoading={updateProfileMutation.isPending || isAvatarUploading}
                disabled={isBusy || (!isDirty && avatarFile === null)}
              >
                {tCommon("actions.save")}
              </Button>
            </div>
          </form>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 text-sm">
            <div>
              <span className="text-xs text-erp-text-muted block">{t("fields.phoneNumber")}</span>
              <span className="font-medium text-erp-text-main">{user.phoneNumber || "-"}</span>
            </div>
            <div>
              <span className="text-xs text-erp-text-muted block">{t("fields.employeeCode")}</span>
              <span className="font-medium text-erp-text-main">{user.employeeCode || "-"}</span>
            </div>
            <div>
              <span className="text-xs text-erp-text-muted block">{t("fields.department")}</span>
              <span className="font-medium text-erp-text-main">
                {user.department ? `${user.department.code} - ${user.department.name}` : "-"}
              </span>
            </div>
            <div>
              <span className="text-xs text-erp-text-muted block">{t("fields.jobTitle")}</span>
              <span className="font-medium text-erp-text-main">
                {user.jobTitle ? `${user.jobTitle.code} - ${user.jobTitle.name}` : "-"}
              </span>
            </div>
            {user.notes && (
              <div className="col-span-full">
                <span className="text-xs text-erp-text-muted block">{t("fields.notes")}</span>
                <span className="text-erp-text-main">{user.notes}</span>
              </div>
            )}
          </div>
        )}
      </section>

      {/* Memberships & Roles */}
      {(user.memberships ?? []).map((membership) => {
        const membershipId = membership.id ?? "";
        const draft = membershipDraft[membershipId];
        const roleOptions = availableRoles(membership);
        return (
          <section
            key={membershipId}
            className="space-y-4 border border-erp-border bg-erp-surface p-4"
            aria-label={t("membershipHeading", { branch: membership.branch?.name ?? t("organizationWide") })}
          >
            <div className="flex flex-wrap items-center justify-between gap-3">
              <div>
                <h2 className="text-base font-bold text-erp-text-main">
                  {t("membershipHeading", { branch: membership.branch?.name ?? t("organizationWide") })}
                </h2>
                <p className="text-xs text-erp-text-muted">
                  {t("period", {
                    start: membership.startsAtUtc ? formatDateTime(membership.startsAtUtc, locale) : "-",
                    end: membership.expiresAtUtc ? formatDateTime(membership.expiresAtUtc, locale) : "-",
                  })}
                </p>
              </div>
              <div className="flex items-center gap-3">
                <StatusBadge
                  label={membership.isActive ? t("membershipActive") : t("membershipInactive")}
                  variant={membership.isActive ? "success" : "neutral"}
                />
                {canManageMemberships ? (
                  <Button
                    variant="outline"
                    size="sm"
                    disabled={isBusy}
                    onClick={() => setPendingAction({ kind: "membership-active", membership, active: !membership.isActive })}
                  >
                    {membership.isActive ? t("deactivateMembership") : t("activateMembership")}
                  </Button>
                ) : null}
              </div>
            </div>

            <div>
              <h3 className="mb-2 text-sm font-bold text-erp-text-main">{t("rolesHeading")}</h3>
              {(membership.roles ?? []).length === 0 ? (
                <p className="text-sm text-erp-text-muted">{t("noRoles")}</p>
              ) : (
                <ul className="divide-y divide-erp-border border border-erp-border">
                  {(membership.roles ?? []).map((role) => (
                    <li key={role.id} className="flex items-center justify-between gap-3 px-3 py-2">
                      <span className="text-sm text-erp-text-main">{role.name}</span>
                      {canAssignRoles && role.id ? (
                        <Button
                          variant="outline"
                          size="sm"
                          disabled={isBusy}
                          onClick={() =>
                            setPendingAction({ kind: "revoke-role", membership, roleId: role.id ?? "", roleName: role.name ?? "-" })
                          }
                        >
                          {t("revokeRole")}
                        </Button>
                      ) : null}
                    </li>
                  ))}
                </ul>
              )}
            </div>

            {(membership.pendingRoleRequests ?? []).length > 0 ? (
              <div>
                <h3 className="mb-2 text-sm font-bold text-erp-text-main">{t("pendingRequestsHeading")}</h3>
                <ul className="divide-y divide-erp-border border border-erp-border">
                  {(membership.pendingRoleRequests ?? []).map((request) => (
                    <li key={request.id} className="flex flex-wrap items-center justify-between gap-3 px-3 py-2">
                      <div className="text-sm">
                        <span className="font-medium text-erp-text-main">{request.role?.name}</span>
                        <span className="ml-2 text-xs text-erp-text-muted">
                          {t("requestedBy", { name: request.requestedBy?.name ?? "-" })}
                        </span>
                      </div>
                      {canAssignRoles && request.id && request.rowVersion ? (
                        <Button
                          variant="outline"
                          size="sm"
                          disabled={isBusy}
                          onClick={() =>
                            setPendingAction({
                              kind: "cancel-request",
                              requestId: request.id ?? "",
                              rowVersion: request.rowVersion ?? "",
                              roleName: request.role?.name ?? "-",
                            })
                          }
                        >
                          {t("cancelRequest")}
                        </Button>
                      ) : null}
                    </li>
                  ))}
                </ul>
              </div>
            ) : null}

            {canAssignRoles ? (
              <div className="flex flex-wrap items-end gap-3">
                <Select
                  wrapperClassName="min-w-[16rem]"
                  label={t("assignRoleLabel")}
                  placeholder={t("selectRole")}
                  value={roleChoice[membershipId] ?? ""}
                  options={roleOptions.map((role) => ({
                    value: role.id ?? "",
                    label: role.requiresApproval ? `${role.name} (${t("requiresApproval")})` : role.name ?? "-",
                  }))}
                  onChange={(event) => setRoleChoice((current) => ({ ...current, [membershipId]: event.target.value }))}
                />
                <Button
                  variant="primary"
                  size="md"
                  isLoading={assignMutation.isPending}
                  disabled={isBusy || !roleChoice[membershipId]}
                  onClick={() => assignRole(membership)}
                >
                  {t("assignRole")}
                </Button>
              </div>
            ) : null}

            {canManageMemberships ? (
              <details className="border border-erp-border p-3">
                <summary className="cursor-pointer text-sm font-bold text-erp-text-main">{t("editMembership")}</summary>
                <div className="mt-3 grid grid-cols-1 gap-3 md:grid-cols-3">
                  <Select
                    label={t("fields.branch")}
                    placeholder={t("organizationWide")}
                    value={draft?.branchId ?? membership.branch?.id ?? ""}
                    options={(branchesQuery.data ?? []).map((branch) => ({ value: branch.id, label: branch.name }))}
                    onChange={(event) =>
                      setMembershipDraft((current) => ({
                        ...current,
                        [membershipId]: {
                          branchId: event.target.value,
                          startsAt: draft?.startsAt ?? toInputValue(membership.startsAtUtc),
                          expiresAt: draft?.expiresAt ?? toInputValue(membership.expiresAtUtc),
                        },
                      }))
                    }
                  />
                  <Input
                    label={t("fields.startsAt")}
                    type="datetime-local"
                    placeholder={t("fields.startsAtPlaceholder")}
                    value={draft?.startsAt ?? toInputValue(membership.startsAtUtc)}
                    onChange={(event) =>
                      setMembershipDraft((current) => ({
                        ...current,
                        [membershipId]: {
                          branchId: draft?.branchId ?? membership.branch?.id ?? "",
                          startsAt: event.target.value,
                          expiresAt: draft?.expiresAt ?? toInputValue(membership.expiresAtUtc),
                        },
                      }))
                    }
                  />
                  <Input
                    label={t("fields.expiresAt")}
                    type="datetime-local"
                    placeholder={t("fields.expiresAtPlaceholder")}
                    value={draft?.expiresAt ?? toInputValue(membership.expiresAtUtc)}
                    onChange={(event) =>
                      setMembershipDraft((current) => ({
                        ...current,
                        [membershipId]: {
                          branchId: draft?.branchId ?? membership.branch?.id ?? "",
                          startsAt: draft?.startsAt ?? toInputValue(membership.startsAtUtc),
                          expiresAt: event.target.value,
                        },
                      }))
                    }
                  />
                </div>
                <div className="mt-3 flex justify-end">
                  <Button
                    variant="primary"
                    size="md"
                    isLoading={updateMembershipMutation.isPending}
                    disabled={isBusy || !draft}
                    onClick={() => saveMembership(membership)}
                  >
                    {tCommon("actions.save")}
                  </Button>
                </div>
              </details>
            ) : null}
          </section>
        );
      })}

      {pendingAction ? (
        <ConfirmationModal
          isOpen={true}
          title={modalCopy?.title ?? ""}
          message={modalCopy?.message ?? ""}
          variant={modalCopy?.variant ?? "info"}
          confirmText={tCommon("actions.confirm")}
          cancelText={tCommon("actions.cancel")}
          isLoading={isBusy}
          onConfirm={confirmAction}
          onClose={() => setPendingAction(null)}
        />
      ) : null}
    </div>
  );
}
