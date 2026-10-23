"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { Controller, type Control, type FieldErrors, type UseFormRegister } from "react-hook-form";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Checkbox } from "@/components/ui/Checkbox";
import { Badge } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Alert } from "@/components/ui/Alert";
import { IconPlus } from "@/components/common/Icons";
import { FormSection } from "@/components/forms/FormSection";
import { AvatarUpload } from "./avatar-upload";
import {
  useAdminRoles,
  useCreateDepartment,
  useCreateJobTitle,
  useDepartments,
  useJobTitles,
} from "../api/user-admin-queries";
import { useOrganizationBranches } from "@/features/item-master/api/item-master-queries";

export interface UserAdminFormValues {
  displayName: string;
  email: string;
  phoneNumber: string;
  employeeCode: string;
  departmentId: string;
  jobTitleId: string;
  notes: string;
  branchId: string;
  startsAt: string;
  expiresAt: string;
  roleIds: string[];
}

interface UserAdminFormFieldsProps {
  register: UseFormRegister<UserAdminFormValues>;
  control: Control<UserAdminFormValues>;
  errors: FieldErrors<UserAdminFormValues>;
  mode: "create" | "edit";
  avatarFile: File | null;
  onAvatarFileChange: (file: File | null) => void;
  existingAvatarFileId?: string | null;
  existingAvatarUrl?: string | null;
  currentDisplayName?: string;
  selectedDepartmentId?: string;
  disabled?: boolean;
  hideRoles?: boolean;
  hideMembershipDates?: boolean;
}

export function UserAdminFormFields({
  register,
  control,
  errors,
  mode,
  avatarFile,
  onAvatarFileChange,
  existingAvatarFileId,
  existingAvatarUrl,
  currentDisplayName,
  selectedDepartmentId,
  disabled = false,
  hideRoles = false,
  hideMembershipDates = false,
}: UserAdminFormFieldsProps) {
  const t = useTranslations("userAdmin");
  const tCommon = useTranslations("common");

  const branchesQuery = useOrganizationBranches();
  const rolesQuery = useAdminRoles();
  const departmentsQuery = useDepartments();
  const jobTitlesQuery = useJobTitles(selectedDepartmentId || null);

  const createDeptMutation = useCreateDepartment();
  const createJobTitleMutation = useCreateJobTitle();

  // Quick Add state for Department
  const [showQuickDept, setShowQuickDept] = useState(false);
  const [newDeptCode, setNewDeptCode] = useState("");
  const [newDeptName, setNewDeptName] = useState("");
  const [deptError, setDeptError] = useState<string | null>(null);

  // Quick Add state for Job Title
  const [showQuickJob, setShowQuickJob] = useState(false);
  const [newJobCode, setNewJobCode] = useState("");
  const [newJobName, setNewJobName] = useState("");
  const [jobError, setJobError] = useState<string | null>(null);

  const branches = branchesQuery.data ?? [];
  const roles = rolesQuery.data?.items ?? [];
  const departments = departmentsQuery.data ?? [];
  const jobTitles = jobTitlesQuery.data ?? [];

  const handleCreateDepartment = async () => {
    if (!newDeptCode.trim() || !newDeptName.trim()) return;
    setDeptError(null);
    try {
      await createDeptMutation.mutateAsync({
        code: newDeptCode.trim(),
        name: newDeptName.trim(),
      });
      setNewDeptCode("");
      setNewDeptName("");
      setShowQuickDept(false);
    } catch (err: unknown) {
      setDeptError(err instanceof Error ? err.message : t("errors.GENERIC"));
    }
  };

  const handleCreateJobTitle = async () => {
    if (!newJobCode.trim() || !newJobName.trim()) return;
    setJobError(null);
    try {
      await createJobTitleMutation.mutateAsync({
        code: newJobCode.trim(),
        name: newJobName.trim(),
        departmentId: selectedDepartmentId || null,
      });
      setNewJobCode("");
      setNewJobName("");
      setShowQuickJob(false);
    } catch (err: unknown) {
      setJobError(err instanceof Error ? err.message : t("errors.GENERIC"));
    }
  };

  return (
    <div className="space-y-6">
      <FormSection title={t("sections.identity")} description={t("sections.identityHint")}>
        <div className="space-y-4">
          <AvatarUpload
            file={avatarFile}
            existingFileId={existingAvatarFileId}
            existingUrl={existingAvatarUrl}
            onFileChange={onAvatarFileChange}
            displayName={currentDisplayName}
            disabled={disabled}
          />

          <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
            <Input
              label={t("fields.displayName")}
              required
              disabled={disabled}
              error={errors.displayName?.message}
              placeholder={t("fields.displayNamePlaceholder")}
              {...register("displayName")}
            />
            <Input
              label={t("fields.email")}
              type="email"
              required
              disabled={disabled || mode === "edit"}
              placeholder={t("fields.emailPlaceholder")}
              helperText={mode === "create" ? t("fields.emailHint") : undefined}
              error={errors.email?.message}
              {...register("email")}
            />
            <Input
              label={t("fields.phoneNumber")}
              disabled={disabled}
              error={errors.phoneNumber?.message}
              placeholder={t("fields.phoneNumberPlaceholder")}
              {...register("phoneNumber")}
            />
            <Input
              label={t("fields.employeeCode")}
              disabled={disabled}
              error={errors.employeeCode?.message}
              placeholder={t("fields.employeeCodePlaceholder")}
              {...register("employeeCode")}
            />
          </div>
        </div>
      </FormSection>

      <FormSection title={t("sections.masterData")} description={t("sections.masterDataHint")}>
        <div className="space-y-4">
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
            <div className="space-y-2">
              <div className="flex items-end gap-2">
                <div className="flex-1">
                  <Select
                    label={t("fields.department")}
                    disabled={disabled || departmentsQuery.isLoading}
                    options={departments.map((d) => ({ value: d.id ?? "", label: `${d.code} - ${d.name}` }))}
                    placeholder={t("fields.selectDepartment")}
                    error={errors.departmentId?.message}
                    {...register("departmentId")}
                  />
                </div>
                {!disabled && (
                  <Button
                    type="button"
                    variant="outline"
                    size="md"
                    className="shrink-0 mb-[1px]"
                    icon={<IconPlus size={16} />}
                    onClick={() => setShowQuickDept((v) => !v)}
                    title={t("masterData.quickAddDepartment")}
                  >
                    {t("masterData.add")}
                  </Button>
                )}
              </div>

              {showQuickDept && (
                <div className="border border-erp-border bg-erp-surface-muted p-3 space-y-3">
                  <h4 className="text-xs font-bold text-erp-navy">{t("masterData.newDepartmentTitle")}</h4>
                  {deptError && <Alert variant="danger">{deptError}</Alert>}
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                    <Input
                      label={t("masterData.code")}
                      value={newDeptCode}
                      placeholder="DEP-01"
                      onChange={(e) => setNewDeptCode(e.target.value)}
                    />
                    <Input
                      label={t("masterData.name")}
                      value={newDeptName}
                      placeholder={t("masterData.namePlaceholder")}
                      onChange={(e) => setNewDeptName(e.target.value)}
                    />
                  </div>
                  <div className="flex justify-end gap-2">
                    <Button type="button" size="sm" variant="outline" onClick={() => setShowQuickDept(false)}>
                      {tCommon("actions.cancel")}
                    </Button>
                    <Button
                      type="button"
                      size="sm"
                      variant="primary"
                      isLoading={createDeptMutation.isPending}
                      onClick={handleCreateDepartment}
                    >
                      {tCommon("actions.save")}
                    </Button>
                  </div>
                </div>
              )}
            </div>

            <div className="space-y-2">
              <div className="flex items-end gap-2">
                <div className="flex-1">
                  <Select
                    label={t("fields.jobTitle")}
                    disabled={disabled || jobTitlesQuery.isLoading}
                    options={jobTitles.map((j) => ({ value: j.id ?? "", label: `${j.code} - ${j.name}` }))}
                    placeholder={t("fields.selectJobTitle")}
                    error={errors.jobTitleId?.message}
                    {...register("jobTitleId")}
                  />
                </div>
                {!disabled && (
                  <Button
                    type="button"
                    variant="outline"
                    size="md"
                    className="shrink-0 mb-[1px]"
                    icon={<IconPlus size={16} />}
                    onClick={() => setShowQuickJob((v) => !v)}
                    title={t("masterData.quickAddJobTitle")}
                  >
                    {t("masterData.add")}
                  </Button>
                )}
              </div>

              {showQuickJob && (
                <div className="border border-erp-border bg-erp-surface-muted p-3 space-y-3">
                  <h4 className="text-xs font-bold text-erp-navy">{t("masterData.newJobTitleTitle")}</h4>
                  {jobError && <Alert variant="danger">{jobError}</Alert>}
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                    <Input
                      label={t("masterData.code")}
                      value={newJobCode}
                      placeholder="POS-01"
                      onChange={(e) => setNewJobCode(e.target.value)}
                    />
                    <Input
                      label={t("masterData.name")}
                      value={newJobName}
                      placeholder={t("masterData.namePlaceholder")}
                      onChange={(e) => setNewJobName(e.target.value)}
                    />
                  </div>
                  <div className="flex justify-end gap-2">
                    <Button type="button" size="sm" variant="outline" onClick={() => setShowQuickJob(false)}>
                      {tCommon("actions.cancel")}
                    </Button>
                    <Button
                      type="button"
                      size="sm"
                      variant="primary"
                      isLoading={createJobTitleMutation.isPending}
                      onClick={handleCreateJobTitle}
                    >
                      {tCommon("actions.save")}
                    </Button>
                  </div>
                </div>
              )}
            </div>
          </div>

          <Input
            label={t("fields.notes")}
            disabled={disabled}
            error={errors.notes?.message}
            placeholder={t("fields.notesPlaceholder")}
            {...register("notes")}
          />
        </div>
      </FormSection>

      {!hideMembershipDates && (
        <FormSection title={t("sections.access")} description={t("sections.accessHint")}>
          <div className="space-y-4">
            <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
              <Select
                label={t("fields.branch")}
                disabled={disabled}
                options={branches.map((b) => ({ value: b.id, label: b.name }))}
                placeholder={t("organizationWide")}
                {...register("branchId")}
              />
              <Input
                label={t("fields.startsAt")}
                type="datetime-local"
                disabled={disabled}
                error={errors.startsAt?.message}
                {...register("startsAt")}
              />
              <Input
                label={t("fields.expiresAt")}
                type="datetime-local"
                disabled={disabled}
                error={errors.expiresAt?.message}
                {...register("expiresAt")}
              />
            </div>
          </div>
        </FormSection>
      )}

      {!hideRoles && (
        <FormSection title={t("sections.roles")} description={t("sections.rolesHint")}>
          {rolesQuery.isPending ? (
            <MonoSpinner size="md" label={tCommon("states.loading")} />
          ) : rolesQuery.isError ? (
            <Alert variant="danger">{t("errors.GENERIC")}</Alert>
          ) : (
            <Controller
              control={control}
              name="roleIds"
              render={({ field }) => (
                <fieldset className="flex flex-col gap-2" aria-describedby={errors.roleIds ? "role-ids-error" : undefined}>
                  <legend className="erp-label">
                    {t("fields.roles")}
                    <span className="erp-label-required">*</span>
                  </legend>
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                    {roles.map((role) => {
                      const checked = field.value.includes(role.id ?? "");
                      return (
                        <div key={role.id} className="flex items-center gap-2 border border-erp-border p-2 bg-erp-surface">
                          <Checkbox
                            label={role.name}
                            checked={checked}
                            disabled={disabled || !role.assignable}
                            onChange={(event) =>
                              field.onChange(
                                event.target.checked
                                  ? [...field.value, role.id ?? ""]
                                  : field.value.filter((id) => id !== role.id)
                              )
                            }
                          />
                          {role.requiresApproval ? <Badge variant="warning" size="sm">{t("requiresApproval")}</Badge> : null}
                          {!role.assignable ? <Badge variant="outline" size="sm">{t("notAssignable")}</Badge> : null}
                        </div>
                      );
                    })}
                  </div>
                  {errors.roleIds ? (
                    <p id="role-ids-error" role="alert" className="text-xs text-erp-danger mt-1">
                      {errors.roleIds.message}
                    </p>
                  ) : null}
                </fieldset>
              )}
            />
          )}
        </FormSection>
      )}
    </div>
  );
}
