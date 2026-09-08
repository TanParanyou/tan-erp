export type CustomerTypeLabelKey = "organization" | "person";
export type CustomerStatusLabelKey = "draft" | "active" | "inactive";
export type ContactChannelLabelKey = "channelPhone" | "channelEmail" | "channelLine" | "channelOther";

export function getCustomerTypeLabelKey(value: string | null | undefined): CustomerTypeLabelKey | null {
  if (value === "organization" || value === "person") {
    return value;
  }

  return null;
}

export function getCustomerStatusLabelKey(value: string | null | undefined): CustomerStatusLabelKey | null {
  if (value === "draft" || value === "active" || value === "inactive") {
    return value;
  }

  return null;
}

export function getContactChannelLabelKey(value: string | null | undefined): ContactChannelLabelKey | null {
  switch (value) {
    case "phone":
      return "channelPhone";
    case "email":
      return "channelEmail";
    case "line":
      return "channelLine";
    case "other":
      return "channelOther";
    default:
      return null;
  }
}

export type CustomerLeadSourceKey =
  | "leadSourceWalkIn"
  | "leadSourceFacebookAds"
  | "leadSourceReferral"
  | "leadSourceProjectDeveloper"
  | "leadSourceWebsite"
  | "leadSourceOther";

export function getCustomerLeadSourceLabelKey(value: string | null | undefined): CustomerLeadSourceKey | null {
  switch (value) {
    case "walk_in":
      return "leadSourceWalkIn";
    case "facebook_ads":
      return "leadSourceFacebookAds";
    case "referral":
      return "leadSourceReferral";
    case "project_developer":
      return "leadSourceProjectDeveloper";
    case "website":
      return "leadSourceWebsite";
    case "other":
      return "leadSourceOther";
    default:
      return null;
  }
}
