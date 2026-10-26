"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { PageHeader } from "@/components/layout/PageHeader";
import { FormContainer } from "@/components/forms/FormContainer";
import { FormTabs } from "@/components/forms/FormTabs";
import { useSelectedMembership } from "@/lib/membership/selected-membership-context";
import { PersonalProfileForm } from "./personal-profile-form";
import { AppearancePreferencesForm } from "./appearance-preferences-form";
import { ChangePasswordForm } from "./change-password-form";

type ProfileTab = "personal" | "appearance" | "security";

export function ProfilePageContent() {
  const t = useTranslations("myProfile");
  const { currentUser } = useSelectedMembership();
  const [activeTab, setActiveTab] = useState<ProfileTab>("personal");
  const user = currentUser.user;

  if (!user) return null;

  return (
    <FormContainer maxWidth="lg" header={<PageHeader title={t("title")} subtitle={t("subtitle")} />}>
      <div className="flex flex-col gap-6">
        <FormTabs<ProfileTab>
          activeTab={activeTab}
          onChange={setActiveTab}
          ariaLabel={t("title")}
          tabs={[
            { id: "personal", label: t("tabs.personal") },
            { id: "appearance", label: t("tabs.appearance") },
            { id: "security", label: t("tabs.security") },
          ]}
        />
        <div id="tabpanel-personal" role="tabpanel" aria-labelledby="tab-personal" className={activeTab === "personal" ? undefined : "hidden"}>
          <PersonalProfileForm key={user.id} user={user} />
        </div>
        <div id="tabpanel-appearance" role="tabpanel" aria-labelledby="tab-appearance" className={activeTab === "appearance" ? undefined : "hidden"}>
          <AppearancePreferencesForm />
        </div>
        <div id="tabpanel-security" role="tabpanel" aria-labelledby="tab-security" className={activeTab === "security" ? undefined : "hidden"}>
          <ChangePasswordForm />
        </div>
      </div>
    </FormContainer>
  );
}
