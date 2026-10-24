export interface HeaderConfig {
  showLogo: boolean;
  logoPosition: "left" | "right";
  logoHeight: string;
  companyNameTh?: string | null;
  companyNameEn?: string | null;
  taxId?: string | null;
  address?: string | null;
  phone?: string | null;
  email?: string | null;
  documentTitle: string;
}

export interface ColumnsConfig {
  showSku: boolean;
  showSpecs: boolean;
  showThumbnail: boolean;
  showLineDiscount: boolean;
}

export interface SummaryConfig {
  showVat: boolean;
  showWithholdingTax: boolean;
  showBahtText: boolean;
}

export interface TermsConfig {
  validityPeriodText?: string | null;
  commercialTermsText?: string | null;
  showBankDetails: boolean;
  bankName?: string | null;
  accountNo?: string | null;
  accountName?: string | null;
  footerNote?: string | null;
}

export interface SignaturesConfig {
  signatureCount: number;
  box1Label: string;
  box2Label?: string | null;
  box3Label?: string | null;
}

export interface DocumentTemplateLayoutConfig {
  header: HeaderConfig;
  columns: ColumnsConfig;
  summary: SummaryConfig;
  terms: TermsConfig;
  signatures: SignaturesConfig;
}

export interface DocumentTemplateItem {
  id: string;
  organizationId: string;
  branchId?: string | null;
  documentType: string;
  name: string;
  isDefault: boolean;
  layoutConfigJson: string;
  logoFileId?: string | null;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc: string;
  rowVersion: string;
}

export interface CreateDocumentTemplatePayload {
  documentType: string;
  name: string;
  isDefault: boolean;
  layoutConfigJson: string;
  logoFileId?: string | null;
  branchId?: string | null;
}

export interface UpdateDocumentTemplatePayload {
  name: string;
  isDefault: boolean;
  layoutConfigJson: string;
  logoFileId?: string | null;
}
