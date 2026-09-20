export type ResetPeriod = "Never" | "Yearly" | "Monthly" | "Daily";

export interface DocumentSequenceItem {
  id: string | null;
  documentType: string;
  prefix: string;
  formatPattern: string;
  resetPeriod: ResetPeriod;
  padding: number;
  isBranchSpecific: boolean;
  isActive: boolean;
  samplePreview: string;
}

export interface UpdateDocumentSequencePayload {
  prefix: string;
  formatPattern: string;
  resetPeriod: ResetPeriod;
  padding: number;
  isBranchSpecific: boolean;
}

export interface PreviewDocumentSequencePayload {
  prefix: string;
  formatPattern: string;
  branchCode?: string;
  padding: number;
  sampleSequence: number;
}
