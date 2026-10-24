"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { Controller, type Control, type FieldErrors, type UseFormRegister, type UseFormSetValue } from "react-hook-form";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import { Select } from "@/components/ui/Select";
import { Checkbox } from "@/components/ui/Checkbox";
import { Badge } from "@/components/ui/Badge";
import { Button } from "@/components/ui/Button";
import { Drawer } from "@/components/ui/Drawer";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Alert } from "@/components/ui/Alert";
import { Tooltip } from "@/components/ui/Tooltip";
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
  setValue?: UseFormSetValue<UserAdminFormValues>;
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
  setValue,
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

  // Quick Add Drawer state for Department
  const [showQuickDept, setShowQuickDept] = useState(false);
  const [newDeptCode, setNewDeptCode] = useState("");
  const [newDeptName, setNewDeptName] = useState("");
  const [newDeptNameEn, setNewDeptNameEn] = useState("");
  const [deptError, setDeptError] = useState<string | null>(null);

  // Quick Add Drawer state for Job Title
  const [showQuickJob, setShowQuickJob] = useState(false);
  const [newJobCode, setNewJobCode] = useState("");
  const [newJobName, setNewJobName] = useState("");
  const [newJobNameEn, setNewJobNameEn] = useState("");
  const [jobError, setJobError] = useState<string | null>(null);

  const branches = branchesQuery.data ?? [];
  const roles = rolesQuery.data?.items ?? [];
  const departments = departmentsQuery.data ?? [];
  const jobTitles = jobTitlesQuery.data ?? [];

  const handleCreateDepartment = async () => {
    if (!newDeptCode.trim() || !newDeptName.trim()) return;
    setDeptError(null);
    try {
      const created = await createDeptMutation.mutateAsync({
        code: newDeptCode.trim(),
        name: newDeptName.trim(),
        nameEn: newDeptNameEn.trim() || null,
      });
      if (created?.id && setValue) {
        setValue("departmentId", created.id, { shouldDirty: true, shouldValidate: true });
      }
      setNewDeptCode("");
      setNewDeptName("");
      setNewDeptNameEn("");
      setShowQuickDept(false);
    } catch (err: unknown) {
      setDeptError(err instanceof Error ? err.message : t("errors.GENERIC"));
    }
  };

  const handleCreateJobTitle = async () => {
    if (!newJobCode.trim() || !newJobName.trim()) return;
    setJobError(null);
    try {
      const created = await createJobTitleMutation.mutateAsync({
        code: newJobCode.trim(),
        name: newJobName.trim(),
        nameEn: newJobNameEn.trim() || null,
        departmentId: selectedDepartmentId || null,
      });
      if (created?.id && setValue) {
        setValue("jobTitleId", created.id, { shouldDirty: true, shouldValidate: true });
      }
      setNewJobCode("");
      setNewJobName("");
      setNewJobNameEn("");
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
            <div className="erp-form-group !mb-0">
              <label htmlFor="user-admin-department" className="erp-label">
                {t("fields.department")}
              </label>
              <div className="flex items-center gap-2">
                <div className="flex-1">
                  <Select
                    id="user-admin-department"
                    wrapperClassName="!mb-0"
                    disabled={disabled || departmentsQuery.isLoading}
                    options={departments.map((d) => ({ value: d.id ?? "", label: `${d.code} - ${d.name}` }))}
                    placeholder={t("fields.selectDepartment")}
                    {...register("departmentId")}
                  />
                </div>
                {!disabled && (
                  <Tooltip content={t("masterData.quickAddDepartment")} position="top">
                    <Button
                      type="button"
                      variant="outline"
                      size="icon"
                      className="shrink-0 h-[44px] w-[44px] border-erp-border"
                      onClick={() => setShowQuickDept(true)}
                      aria-label={t("masterData.quickAddDepartment")}
                    >
                      <IconPlus size={18} />
                    </Button>
                  </Tooltip>
                )}
              </div>
              {errors.departmentId?.message && (
                <p className="erp-error-text" role="alert">
                  {errors.departmentId.message}
                </p>
              )}
            </div>

            <div className="erp-form-group !mb-0">
              <label htmlFor="user-admin-job-title" className="erp-label">
                {t("fields.jobTitle")}
              </label>
              <div className="flex items-center gap-2">
                <div className="flex-1">
                  <Select
                    id="user-admin-job-title"
                    wrapperClassName="!mb-0"
                    disabled={disabled || jobTitlesQuery.isLoading}
                    options={jobTitles.map((j) => ({ value: j.id ?? "", label: `${j.code} - ${j.name}` }))}
                    placeholder={t("fields.selectJobTitle")}
                    {...register("jobTitleId")}
                  />
                </div>
                {!disabled && (
                  <Tooltip content={t("masterData.quickAddJobTitle")} position="top">
                    <Button
                      type="button"
                      variant="outline"
                      size="icon"
                      className="shrink-0 h-[44px] w-[44px] border-erp-border"
                      onClick={() => setShowQuickJob(true)}
                      aria-label={t("masterData.quickAddJobTitle")}
                    >
                      <IconPlus size={18} />
                    </Button>
                  </Tooltip>
                )}
              </div>
              {errors.jobTitleId?.message && (
                <p className="erp-error-text" role="alert">
                  {errors.jobTitleId.message}
                </p>
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

      {/* Quick Add Department Drawer */}
      <Drawer
        isOpen={showQuickDept}
        onClose={() => {
          setShowQuickDept(false);
          setDeptError(null);
        }}
        title={t("masterData.newDepartmentTitle")}
        description={t("masterData.newDepartmentDescription")}
        size="md"
        footer={
          <div className="flex w-full items-center justify-end gap-2">
            <Button
              type="button"
              variant="outline"
              size="md"
              onClick={() => {
                setShowQuickDept(false);
                setDeptError(null);
              }}
              disabled={createDeptMutation.isPending}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant="primary"
              size="md"
              isLoading={createDeptMutation.isPending}
              onClick={handleCreateDepartment}
              disabled={!newDeptCode.trim() || !newDeptName.trim()}
            >
              {tCommon("actions.save")}
            </Button>
          </div>
        }
      >
        <div className="space-y-4 py-2">
          {deptError && <Alert variant="danger">{deptError}</Alert>}
          <Input
            label={t("masterData.code")}
            value={newDeptCode}
            placeholder="DEP-01"
            onChange={(e) => setNewDeptCode(e.target.value)}
            required
            autoFocus
          />
          <Input
            label={t("masterData.name")}
            value={newDeptName}
            placeholder={t("masterData.namePlaceholder")}
            onChange={(e) => setNewDeptName(e.target.value)}
            required
          />
          <Input
            label={t("masterData.nameEn")}
            value={newDeptNameEn}
            placeholder={t("masterData.nameEnPlaceholder")}
            onChange={(e) => setNewDeptNameEn(e.target.value)}
          />
        </div>
      </Drawer>

      {/* Quick Add Job Title Drawer */}
      <Drawer
        isOpen={showQuickJob}
        onClose={() => {
          setShowQuickJob(false);
          setJobError(null);
        }}
        title={t("masterData.newJobTitleTitle")}
        description={t("masterData.newJobTitleDescription")}
        size="md"
        footer={
          <div className="flex w-full items-center justify-end gap-2">
            <Button
              type="button"
              variant="outline"
              size="md"
              onClick={() => {
                setShowQuickJob(false);
                setJobError(null);
              }}
              disabled={createJobTitleMutation.isPending}
            >
              {tCommon("actions.cancel")}
            </Button>
            <Button
              type="button"
              variant="primary"
              size="md"
              isLoading={createJobTitleMutation.isPending}
              onClick={handleCreateJobTitle}
              disabled={!newJobCode.trim() || !newJobName.trim()}
            >
              {tCommon("actions.save")}
            </Button>
          </div>
        }
      >
        <div className="space-y-4 py-2">
          {jobError && <Alert variant="danger">{jobError}</Alert>}
          <Input
            label={t("masterData.code")}
            value={newJobCode}
            placeholder="POS-01"
            onChange={(e) => setNewJobCode(e.target.value)}
            required
            autoFocus
          />
          <Input
            label={t("masterData.name")}
            value={newJobName}
            placeholder={t("masterData.namePlaceholder")}
            onChange={(e) => setNewJobName(e.target.value)}
            required
          />
          <Input
            label={t("masterData.nameEn")}
            value={newJobNameEn}
            placeholder={t("masterData.nameEnPlaceholder")}
            onChange={(e) => setNewJobNameEn(e.target.value)}
          />
        </div>
      </Drawer>
    </div>
  );
}
