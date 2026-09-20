"use client";

import React from "react";
import { DocumentSequenceList } from "@/features/settings/document-numbering/components/document-sequence-list";

export default function DocumentNumberingSettingsPage() {
  return (
    <div className="container mx-auto p-4 sm:p-6 lg:p-8 max-w-7xl">
      <DocumentSequenceList />
    </div>
  );
}
