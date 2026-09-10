"use client";

import React from "react";
import Link from "next/link";
import { useTranslations, useLocale } from "next-intl";
import { useCustomerDetail } from "../api/customer-queries";
import { DuplicateCandidateCard } from "./duplicate-candidate-card";
import { MonoSpinner } from "@/components/ui/MonoSpinner";
import { Button } from "@/components/ui/Button";
import { IconChevronLeft, IconAlertCircle, IconFileText } from "@/components/common/Icons";
import { getContactChannelLabelKey, getCustomerLeadSourceLabelKey, getCustomerStatusLabelKey, getCustomerTypeLabelKey } from "../customer-labels";

import { ConfirmationModal } from "@/components/ui/ConfirmationModal";
import { SiteList } from "@/features/sites/components/site-list";
import { EntityDetailHeader } from "@/components/ui/EntityDetailHeader";
import { Badge } from "@/components/ui/Badge";
import { Avatar } from "@/components/ui/Avatar";
import { can } from "@/lib/permissions/can";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { apiClient } from "@/lib/api/api-client";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useQueryClient } from "@tanstack/react-query";
import { useToast } from "@/hooks/useToast";
import { ApiError, isAuthenticationRequiredError, isMembershipRequiredError } from "@/lib/api/api-error";

interface ActivationIntent {
  signature: string;
  key: string;
}

interface CustomerDetailProps {
  customerId: string;
}

export function CustomerDetail({ customerId }: CustomerDetailProps) {
  const t = useTranslations("customers");
  const tCommon = useTranslations("common");
  const locale = useLocale();
  const queryClient = useQueryClient();
  const { selectedMembership } = useSelectedMembership();
  const { toast } = useToast();

  const [isActivating, setIsActivating] = React.useState(false);
  const [showActivateModal, setShowActivateModal] = React.useState(false);
  const activationIntentRef = React.useRef<ActivationIntent | null>(null);

  const resolveCustomerTypeLabel = (value: string | null | undefined): string => {
    const key = getCustomerTypeLabelKey(value);
    return key ? t(key) : "-";
  };

  const resolveCustomerStatusLabel = (value: string | null | undefined): string => {
    const key = getCustomerStatusLabelKey(value);
    return key ? tCommon(`status.${key}`) : "-";
  };

  const resolveDetailErrorMessage = (error: Error | null): string => {
    if (isAuthenticationRequiredError(error)) {
      return t("errors.authenticationRequired");
    }
    if (isMembershipRequiredError(error)) {
      return t("errors.membershipRequired");
    }
    return t("errors.loadDetail");
  };

  const { data: customer, isLoading, isError, error, refetch } = useCustomerDetail(customerId);

  const handleActivate = async () => {
    if (!customer) return;

    const token = await getAuthToken();
    if (!token) {
      toast.error(t("errors.authenticationRequired"));
      return;
    }

    if (!customer?.id) {
      return;
    }

    const membershipId = selectedMembership?.id;
    if (!membershipId) {
      toast.error(t("errors.membershipRequired"));
      return;
    }

    setIsActivating(true);
    try {
      const signature = `${customer.id}|${customer.rowVersion ?? ""}|activate`;
      if (activationIntentRef.current?.signature !== signature) {
        activationIntentRef.current = { signature, key: crypto.randomUUID() };
      }
      const idempotencyKey = activationIntentRef.current.key;

      await apiClient.activateCustomer(customer.id, {
        token,
        membershipId,
        idempotencyKey,
        locale: locale === "en" ? "en" : "th",
        ifMatch: customer.rowVersion ?? "",
      });

      activationIntentRef.current = null;
      toast.success(t("activateSuccess"));
      setShowActivateModal(false);
      await queryClient.invalidateQueries({ queryKey: ["business"] });
      refetch();
    } catch (err: unknown) {
      if (
        err instanceof ApiError &&
        err.status === 409 &&
        err.code === "CUSTOMER_VERSION_CONFLICT"
      ) {
        toast.error(t("errors.activateConflict"));
      } else {
        toast.error(err instanceof Error ? err.message : t("errors.saveUnexpected"));
      }
    } finally {
      setIsActivating(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-[300px] bg-erp-surface border border-erp-border">
        <MonoSpinner size="lg" label={tCommon("states.loading")} aria-busy="true" />
      </div>
    );
  }

  if (isError || !customer) {
    return (
      <div
        role="alert"
        aria-live="polite"
        className="erp-card p-8 border-erp-danger-border bg-erp-danger-bg text-center flex flex-col items-center gap-4"
      >
        <IconAlertCircle size={32} className="text-erp-danger" />
        <div>
          <h2 className="text-lg font-bold text-erp-danger mb-2">
            {resolveDetailErrorMessage(error)}
          </h2>
        </div>
        <div className="flex gap-4">
          <Button variant="outline" size="md" onClick={() => refetch()} className="min-h-[44px]">
            {tCommon("actions.retry")}
          </Button>
          <Button href={`/${locale}/customers`} variant="primary" size="md" className="min-h-[44px]">
            {t("backToList")}
          </Button>
        </div>
      </div>
    );
  }

  const displayName =
    locale === "en" && customer.displayNameEn
      ? customer.displayNameEn
      : customer.displayNameTh || customer.displayNameEn || "-";

  const contact = customer.primaryContact;
  const canActivate = customer.status === "draft" && can(selectedMembership, "customers.activate");

  return (
    <div className="w-full flex flex-col gap-6">
      {/* Entity Detail Hero Header */}
      <EntityDetailHeader
        backLabel={t("backToList")}
        backHref={`/${locale}/customers`}
        code={customer.code}
        title={displayName}
        subtitle={
          customer.displayNameEn && customer.displayNameTh && locale === "th"
            ? customer.displayNameEn
            : undefined
        }
        avatar={
          <Avatar
            initial={customer.displayNameTh || customer.displayNameEn || undefined}
            variant={customer.customerType === "organization" ? "navy" : "muted"}
            size="lg"
            title={resolveCustomerTypeLabel(customer.customerType)}
          />
        }
        statusBadge={
          <Badge
            variant={customer.status === "active" ? "success" : "neutral"}
            size="md"
          >
            {resolveCustomerStatusLabel(customer.status)}
          </Badge>
        }
        badges={[
          <Badge key="type" variant="outline" size="sm">
            {resolveCustomerTypeLabel(customer.customerType)}
          </Badge>,
        ]}
        metrics={[
          {
            label: t("primaryContact"),
            value: contact?.name || "-",
          },
          {
            label: t("phone"),
            value: contact?.phone || "-",
            isMono: true,
          },
          {
            label: t("email"),
            value: contact?.email || "-",
          },
        ]}
        actions={
          canActivate ? (
            <Button
              variant="primary"
              size="md"
              onClick={() => setShowActivateModal(true)}
              className="min-h-[40px] font-semibold"
            >
              {t("activateAction")}
            </Button>
          ) : undefined
        }
      />

      <ConfirmationModal
        isOpen={showActivateModal}
        onClose={() => setShowActivateModal(false)}
        onConfirm={handleActivate}
        title={t("activateModalTitle")}
        message={t("activateModalDesc")}
        confirmText={tCommon("actions.confirm")}
        cancelText={tCommon("actions.cancel")}
        variant="info"
        isLoading={isActivating}
      />

      {/* Duplicate Candidates reuse shared card; masked values only */}
      {customer.duplicateCandidates && customer.duplicateCandidates.length > 0 && (
        <DuplicateCandidateCard candidates={customer.duplicateCandidates} />
      )}

      {/* Detail Content */}
      <div className="erp-card p-6 flex flex-col gap-5">
        <h2 className="text-lg font-bold text-erp-navy m-0 border-b border-erp-border-subtle pb-3">
          {t("title")}
        </h2>

        <dl className="erp-dl">
          <dt>{t("code")}:</dt>
          <dd className="font-mono font-semibold">{customer.code || "-"}</dd>

          <dt>{t("customerType")}:</dt>
          <dd>
            <span className="erp-badge erp-badge-neutral">
              {resolveCustomerTypeLabel(customer.customerType)}
            </span>
          </dd>

          <dt>{t("displayNameTh")}:</dt>
          <dd className="font-semibold">{customer.displayNameTh || "-"}</dd>

          <dt>{t("displayNameEn")}:</dt>
          <dd>{customer.displayNameEn || "-"}</dd>

          <dt>{t("preferredLocale")}:</dt>
          <dd>{customer.preferredLocale === "en" ? t("localeEnglish") : t("localeThai")}</dd>

          <dt>{t("leadSource")}:</dt>
          <dd>
            {customer.leadSource ? (
              <span className="erp-badge erp-badge-neutral">
                {(() => {
                  const key = getCustomerLeadSourceLabelKey(customer.leadSource);
                  return key ? t(key) : customer.leadSource;
                })()}
              </span>
            ) : (
              "-"
            )}
          </dd>
        </dl>
      </div>

      {/* Primary Contact Section */}
      <div className="erp-card p-6 flex flex-col gap-5">
        <h2 className="text-lg font-bold text-erp-navy m-0 border-b border-erp-border-subtle pb-3">
          {t("primaryContact")}
        </h2>

        {contact ? (
          <dl className="erp-dl">
            <dt>{t("contactName")}:</dt>
            <dd className="font-semibold">{contact.name || "-"}</dd>

            <dt>{t("roleTitle")}:</dt>
            <dd>{contact.roleTitle || "-"}</dd>

            <dt>{t("phone")}:</dt>
            <dd className="font-mono">{contact.phone || "-"}</dd>

            <dt>{t("email")}:</dt>
            <dd>{contact.email || "-"}</dd>

            <dt>{t("lineId")}:</dt>
            <dd className="font-mono">{contact.lineId || "-"}</dd>

            <dt>{t("preferredChannel")}:</dt>
            <dd>
              <span className="erp-badge erp-badge-neutral">
                {(() => {
                  const key = getContactChannelLabelKey(contact.preferredChannel);
                  return key ? t(key) : "-";
                })()}
              </span>
            </dd>
          </dl>
        ) : (
          <p className="text-erp-text-muted text-sm m-0">
            {t("noPrimaryContact")}
          </p>
        )}
      </div>

      {/* Sites Section */}
      <SiteList
        customerId={customerId}
        isCustomerActive={customer.status === "active"}
      />
    </div>
  );
}
