"use client";

import React, { useState, useEffect, useMemo, useCallback } from "react";
import { useTranslations } from "next-intl";
import { Drawer } from "@/components/ui/Drawer";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { useUpdateDocumentSequence } from "../api/document-sequence-queries";
import { ApiError } from "@/lib/api/api-error";
import type { DocumentSequenceItem, ResetPeriod } from "../types";

export interface DocumentSequenceDrawerProps {
  item: DocumentSequenceItem | null;
  isOpen: boolean;
  onClose: () => void;
}

const TOKEN_DEFINITIONS = [
  { token: "{PREFIX}", key: "prefix" },
  { token: "{BRANCH}", key: "branch" },
  { token: "{BBBB}", key: "bbbb" },
  { token: "{BB}", key: "bb" },
  { token: "{YYYY}", key: "yyyy" },
  { token: "{YY}", key: "yy" },
  { token: "{MM}", key: "mm" },
  { token: "{DD}", key: "dd" },
  { token: "{SEQ:4}", key: "seq4" },
  { token: "{SEQ:5}", key: "seq5" },
] as const;

type YearFormat = "bb" | "bbbb" | "yyyy" | "yy" | "none";
type MonthFormat = "mm" | "mmdd" | "none";
type Separator = "" | "-" | "/";

export function DocumentSequenceDrawer({
  item,
  isOpen,
  onClose,
}: DocumentSequenceDrawerProps) {
  const t = useTranslations("documentNumbering");
  const tCommon = useTranslations("common");

  // Core state
  const [prefix, setPrefix] = useState("");
  const [formatPattern, setFormatPattern] = useState("");
  const [resetPeriod, setResetPeriod] = useState<ResetPeriod>("Yearly");
  const [padding, setPadding] = useState<number>(4);
  const [isBranchSpecific, setIsBranchSpecific] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // UX Builder state
  const [isAdvancedMode, setIsAdvancedMode] = useState(false);
  const [yearFormat, setYearFormat] = useState<YearFormat>("bb");
  const [monthFormat, setMonthFormat] = useState<MonthFormat>("mm");
  const [prefixSeparator, setPrefixSeparator] = useState<Separator>("");
  const [seqSeparator, setSeqSeparator] = useState<Separator>("-");

  const updateMutation = useUpdateDocumentSequence();

  // Helper to construct pattern from visual segments
  const constructPattern = useCallback(
    (
      pfxSep: Separator,
      yr: YearFormat,
      mo: MonthFormat,
      sSep: Separator,
      pad: number,
      branch: boolean
    ) => {
      let p = "{PREFIX}";
      if (pfxSep) p += pfxSep;
      if (branch) p += "{BRANCH}-";

      if (yr !== "none") {
        if (yr === "bb") p += "{BB}";
        else if (yr === "bbbb") p += "{BBBB}";
        else if (yr === "yyyy") p += "{YYYY}";
        else if (yr === "yy") p += "{YY}";
      }

      if (mo !== "none") {
        if (mo === "mm") p += "{MM}";
        else if (mo === "mmdd") p += "{MM}{DD}";
      }

      if (sSep) p += sSep;
      p += `{SEQ:${pad}}`;
      return p;
    },
    []
  );

  // Initialize state when item changes
  useEffect(() => {
    if (item) {
      setPrefix(item.prefix);
      setFormatPattern(item.formatPattern);
      setResetPeriod(item.resetPeriod);
      setPadding(item.padding);
      setIsBranchSpecific(item.isBranchSpecific);
      setError(null);

      // Analyze existing pattern to match Segment Builder
      const pat = item.formatPattern.toUpperCase();
      let matchedYear: YearFormat = "none";
      if (pat.includes("{BBBB}")) matchedYear = "bbbb";
      else if (pat.includes("{BB}")) matchedYear = "bb";
      else if (pat.includes("{YYYY}")) matchedYear = "yyyy";
      else if (pat.includes("{YY}")) matchedYear = "yy";

      let matchedMonth: MonthFormat = "none";
      if (pat.includes("{MM}{DD}")) matchedMonth = "mmdd";
      else if (pat.includes("{MM}")) matchedMonth = "mm";

      // Detect prefix separator
      let matchedPrefixSep: Separator = "";
      if (pat.startsWith("{PREFIX}-")) matchedPrefixSep = "-";
      else if (pat.startsWith("{PREFIX}/")) matchedPrefixSep = "/";

      // Detect seq separator
      let matchedSeqSep: Separator = "";
      if (pat.includes("-{SEQ")) matchedSeqSep = "-";
      else if (pat.includes("/{SEQ")) matchedSeqSep = "/";

      setYearFormat(matchedYear);
      setMonthFormat(matchedMonth);
      setPrefixSeparator(matchedPrefixSep);
      setSeqSeparator(matchedSeqSep);

      // If pattern has complex tokens or doesn't match standard, default to advanced
      const isComplex =
        !pat.includes("{SEQ") ||
        (matchedYear === "none" && matchedMonth === "none" && !pat.endsWith("{SEQ:4}") && !pat.endsWith("{SEQ:5}") && !pat.endsWith("{SEQ:6}"));
      setIsAdvancedMode(Boolean(isComplex && item.id));
    }
  }, [item]);

  // Sync builder changes to formatPattern when in standard mode
  const handleBuilderChange = (updates: {
    yearFormat?: YearFormat;
    monthFormat?: MonthFormat;
    prefixSeparator?: Separator;
    seqSeparator?: Separator;
    padding?: number;
    isBranchSpecific?: boolean;
    resetPeriod?: ResetPeriod;
  }) => {
    const nextYear = updates.yearFormat ?? yearFormat;
    const nextMonth = updates.monthFormat ?? monthFormat;
    const nextPrefixSep = updates.prefixSeparator ?? prefixSeparator;
    const nextSeqSep = updates.seqSeparator ?? seqSeparator;
    const nextPadding = updates.padding ?? padding;
    const nextBranch = updates.isBranchSpecific ?? isBranchSpecific;

    if (updates.yearFormat !== undefined) setYearFormat(updates.yearFormat);
    if (updates.monthFormat !== undefined) setMonthFormat(updates.monthFormat);
    if (updates.prefixSeparator !== undefined) setPrefixSeparator(updates.prefixSeparator);
    if (updates.seqSeparator !== undefined) setSeqSeparator(updates.seqSeparator);
    if (updates.padding !== undefined) setPadding(updates.padding);
    if (updates.isBranchSpecific !== undefined) setIsBranchSpecific(updates.isBranchSpecific);
    if (updates.resetPeriod !== undefined) setResetPeriod(updates.resetPeriod);

    const newPat = constructPattern(
      nextPrefixSep,
      nextYear,
      nextMonth,
      nextSeqSep,
      nextPadding,
      nextBranch
    );
    setFormatPattern(newPat);
  };

  // Preset definitions
  const PRESETS = useMemo(
    () => [
      {
        id: "thaiStandard",
        title: t("presets.thaiStandard"),
        pattern: "{PREFIX}{BB}{MM}-{SEQ:4}",
        resetPeriod: "Monthly" as ResetPeriod,
        padding: 4,
        isBranchSpecific: false,
        yearFormat: "bb" as YearFormat,
        monthFormat: "mm" as MonthFormat,
        prefixSeparator: "" as Separator,
        seqSeparator: "-" as Separator,
        example: `${prefix || "EST"}6909-0001`,
      },
      {
        id: "international",
        title: t("presets.international"),
        pattern: "{PREFIX}-{YYYY}{MM}-{SEQ:5}",
        resetPeriod: "Monthly" as ResetPeriod,
        padding: 5,
        isBranchSpecific: false,
        yearFormat: "yyyy" as YearFormat,
        monthFormat: "mm" as MonthFormat,
        prefixSeparator: "-" as Separator,
        seqSeparator: "-" as Separator,
        example: `${prefix || "EST"}-202609-00001`,
      },
      {
        id: "branch",
        title: t("presets.branch"),
        pattern: "{PREFIX}-{BRANCH}-{BB}{MM}-{SEQ:4}",
        resetPeriod: "Monthly" as ResetPeriod,
        padding: 4,
        isBranchSpecific: true,
        yearFormat: "bb" as YearFormat,
        monthFormat: "mm" as MonthFormat,
        prefixSeparator: "-" as Separator,
        seqSeparator: "-" as Separator,
        example: `${prefix || "EST"}-HQ-6909-0001`,
      },
      {
        id: "continuous",
        title: t("presets.continuous"),
        pattern: "{PREFIX}-{SEQ:6}",
        resetPeriod: "Never" as ResetPeriod,
        padding: 6,
        isBranchSpecific: false,
        yearFormat: "none" as YearFormat,
        monthFormat: "none" as MonthFormat,
        prefixSeparator: "-" as Separator,
        seqSeparator: "-" as Separator,
        example: `${prefix || "EST"}-000001`,
      },
    ],
    [t, prefix]
  );

  const handleSelectPreset = (p: (typeof PRESETS)[number]) => {
    setFormatPattern(p.pattern);
    setResetPeriod(p.resetPeriod);
    setPadding(p.padding);
    setIsBranchSpecific(p.isBranchSpecific);
    setYearFormat(p.yearFormat);
    setMonthFormat(p.monthFormat);
    setPrefixSeparator(p.prefixSeparator);
    setSeqSeparator(p.seqSeparator);
    setIsAdvancedMode(false);
  };

  // Real-time live preview
  const livePreview = useMemo(() => {
    if (!formatPattern) return "";
    const now = new Date();
    const yearAd = now.getFullYear();
    const yearBe = yearAd + 543;
    const shortYearAd = String(yearAd % 100).padStart(2, "0");
    const shortYearBe = String(yearBe % 100).padStart(2, "0");
    const month = String(now.getMonth() + 1).padStart(2, "0");
    const day = String(now.getDate()).padStart(2, "0");

    let res = formatPattern
      .replace(/\{PREFIX\}/gi, prefix || "")
      .replace(/\{BRANCH\}/gi, isBranchSpecific ? "HQ" : "HQ")
      .replace(/\{YYYY\}/gi, String(yearAd))
      .replace(/\{YY\}/gi, shortYearAd)
      .replace(/\{BBBB\}/gi, String(yearBe))
      .replace(/\{BB\}/gi, shortYearBe)
      .replace(/\{MM\}/gi, month)
      .replace(/\{DD\}/gi, day);

    res = res.replace(/\{SEQ(?::(\d+))?\}/gi, (_, p) => {
      const padLen = p ? parseInt(p, 10) : padding || 4;
      return String(1).padStart(padLen, "0");
    });

    return res;
  }, [formatPattern, prefix, isBranchSpecific, padding]);

  if (!item) return null;

  const handleInsertToken = (token: string) => {
    setFormatPattern((prev) => `${prev}${token}`);
  };

  const handleSave = async () => {
    if (!formatPattern.trim()) {
      setError(t("patternRequired"));
      return;
    }

    try {
      await updateMutation.mutateAsync({
        documentType: item.documentType,
        ifMatch: item.rowVersion,
        payload: {
          prefix: prefix.trim(),
          formatPattern: formatPattern.trim(),
          resetPeriod,
          padding: Number(padding) || 4,
          isBranchSpecific,
        },
      });
      onClose();
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        if (err.code === "DOCUMENT_SEQUENCE_VERSION_CONFLICT") {
          setError(t("versionConflict"));
          return;
        }
        if (err.code === "INVALID_FORMAT_PATTERN") {
          setError(t("invalidPattern"));
          return;
        }
        if (err.code === "INVALID_RESET_PERIOD") {
          setError(t("invalidResetPeriod"));
          return;
        }
        if (err.code === "PERMISSION_DENIED") {
          setError(t("permissionDenied"));
          return;
        }
      }
      setError(err instanceof Error ? err.message : t("updateFailed"));
    }
  };

  return (
    <Drawer
      isOpen={isOpen}
      onClose={onClose}
      title={`${t("editTitle")}: ${t(`types.${item.documentType}`)}`}
      description={t("editDescription")}
      size="lg"
      footer={
        <div className="flex justify-end gap-3 w-full">
          <Button variant="outline" size="sm" onClick={onClose} disabled={updateMutation.isPending}>
            {tCommon("actions.cancel")}
          </Button>
          <Button
            variant="primary"
            size="sm"
            onClick={handleSave}
            isLoading={updateMutation.isPending}
            disabled={updateMutation.isPending}
          >
            {tCommon("actions.save")}
          </Button>
        </div>
      }
    >
      <div className="flex flex-col gap-6 py-2">
        {/* Live Preview Card: Atelier Architectural Navy Sharp */}
        <div className="p-4 border-2 border-erp-navy bg-slate-50 dark:bg-slate-900 flex flex-col gap-3">
          <div className="flex items-center justify-between">
            <span className="text-xs uppercase tracking-wider font-bold text-slate-600 dark:text-slate-400">
              {t("livePreview")}
            </span>
            <Badge variant="outline" size="sm">
              {t("previewBadge")}
            </Badge>
          </div>

          <div className="flex items-baseline gap-2">
            <span className="font-mono text-2xl sm:text-3xl font-bold text-erp-navy dark:text-sky-400 tracking-tight">
              {livePreview || "-"}
            </span>
          </div>

          {/* Color-Coded Breakdown Chips */}
          <div className="flex flex-wrap gap-1.5 pt-1 border-t border-slate-200 dark:border-slate-800 text-xs font-mono">
            <span className="px-2 py-0.5 bg-blue-100 text-blue-800 dark:bg-blue-950 dark:text-blue-200 border border-blue-300 dark:border-blue-700">
              {t("prefix")}: {prefix || "-"}
            </span>
            {isBranchSpecific && (
              <span className="px-2 py-0.5 bg-purple-100 text-purple-800 dark:bg-purple-950 dark:text-purple-200 border border-purple-300 dark:border-purple-700">
                {t("badgeBranchSpecific")}: HQ
              </span>
            )}
            {yearFormat !== "none" && (
              <span className="px-2 py-0.5 bg-amber-100 text-amber-800 dark:bg-amber-950 dark:text-amber-200 border border-amber-300 dark:border-amber-700">
                {t(`yearOptions.${yearFormat}`)}
              </span>
            )}
            {monthFormat !== "none" && (
              <span className="px-2 py-0.5 bg-emerald-100 text-emerald-800 dark:bg-emerald-950 dark:text-emerald-200 border border-emerald-300 dark:border-emerald-700">
                {t(`monthOptions.${monthFormat}`)}
              </span>
            )}
            <span className="px-2 py-0.5 bg-slate-200 text-slate-800 dark:bg-slate-800 dark:text-slate-200 border border-slate-400 dark:border-slate-600">
              {padding} {t("digits")}
            </span>
            <span className="px-2 py-0.5 bg-cyan-100 text-cyan-800 dark:bg-cyan-950 dark:text-cyan-200 border border-cyan-300 dark:border-cyan-700">
              {t(`resetOptions.${resetPeriod.toLowerCase()}`)}
            </span>
          </div>
        </div>

        {error && (
          <div className="p-3 border border-red-500 bg-red-50 dark:bg-red-950/40 text-red-700 dark:text-red-300 text-sm">
            {error}
          </div>
        )}

        {/* Section 1: Popular Preset Cards (1-Click selection) */}
        <div className="flex flex-col gap-2.5">
          <div>
            <h3 className="text-xs font-bold uppercase tracking-wider text-slate-900 dark:text-white">
              {t("presetsTitle")}
            </h3>
            <p className="text-xs text-slate-500 dark:text-slate-400 mt-0.5">
              {t("presetsSubtitle")}
            </p>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
            {PRESETS.map((preset) => {
              const isSelected = formatPattern === preset.pattern && isBranchSpecific === preset.isBranchSpecific;
              return (
                <button
                  key={preset.id}
                  type="button"
                  onClick={() => handleSelectPreset(preset)}
                  className={`p-3 text-left border transition-all cursor-pointer flex flex-col justify-between gap-1.5 ${
                    isSelected
                      ? "border-erp-navy bg-sky-50/50 dark:bg-sky-950/30 ring-1 ring-erp-navy"
                      : "border-erp-border bg-white dark:bg-slate-950 hover:border-slate-400"
                  }`}
                >
                  <div className="flex items-center justify-between w-full">
                    <span className="text-xs font-bold text-slate-900 dark:text-white">
                      {preset.title}
                    </span>
                    {isSelected && (
                      <span className="w-2 h-2 rounded-full bg-erp-navy dark:bg-sky-400" />
                    )}
                  </div>
                  <div className="font-mono text-xs font-semibold text-erp-navy dark:text-sky-300">
                    {preset.example}
                  </div>
                </button>
              );
            })}
          </div>
        </div>

        {/* Prefix Input */}
        <div className="flex flex-col gap-1.5">
          <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
            {t("prefix")}
          </label>
          <input
            type="text"
            className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 font-mono text-sm uppercase"
            value={prefix}
            onChange={(e) => setPrefix(e.target.value.toUpperCase())}
            placeholder="EST"
            maxLength={10}
          />
        </div>

        {/* Section 2: Visual Segment Builder (When not in advanced mode) */}
        {!isAdvancedMode ? (
          <div className="flex flex-col gap-4 border border-erp-border p-4 bg-slate-50/60 dark:bg-slate-900/40">
            <div className="flex items-center justify-between border-b border-erp-border pb-2">
              <span className="text-xs font-bold uppercase tracking-wider text-slate-800 dark:text-slate-200">
                {t("builderTitle")}
              </span>
              <span className="text-xs text-slate-500 font-mono">
                {formatPattern}
              </span>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              {/* Year Format */}
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
                  {t("yearFormat")}
                </label>
                <select
                  className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 text-sm"
                  value={yearFormat}
                  onChange={(e) => handleBuilderChange({ yearFormat: e.target.value as YearFormat })}
                >
                  <option value="bb">{t("yearOptions.bb")}</option>
                  <option value="bbbb">{t("yearOptions.bbbb")}</option>
                  <option value="yyyy">{t("yearOptions.yyyy")}</option>
                  <option value="yy">{t("yearOptions.yy")}</option>
                  <option value="none">{t("yearOptions.none")}</option>
                </select>
              </div>

              {/* Month/Day Format */}
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
                  {t("monthFormat")}
                </label>
                <select
                  className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 text-sm"
                  value={monthFormat}
                  onChange={(e) => handleBuilderChange({ monthFormat: e.target.value as MonthFormat })}
                >
                  <option value="mm">{t("monthOptions.mm")}</option>
                  <option value="mmdd">{t("monthOptions.mmdd")}</option>
                  <option value="none">{t("monthOptions.none")}</option>
                </select>
              </div>

              {/* Prefix Separator */}
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
                  {t("separatorPrefix")}
                </label>
                <select
                  className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 text-sm"
                  value={prefixSeparator}
                  onChange={(e) => handleBuilderChange({ prefixSeparator: e.target.value as Separator })}
                >
                  <option value="">{t("separatorOptions.none")}</option>
                  <option value="-">{t("separatorOptions.dash")}</option>
                  <option value="/">{t("separatorOptions.slash")}</option>
                </select>
              </div>

              {/* Sequence Separator */}
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
                  {t("separatorSeq")}
                </label>
                <select
                  className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 text-sm"
                  value={seqSeparator}
                  onChange={(e) => handleBuilderChange({ seqSeparator: e.target.value as Separator })}
                >
                  <option value="-">{t("separatorOptions.dash")}</option>
                  <option value="/">{t("separatorOptions.slash")}</option>
                  <option value="">{t("separatorOptions.none")}</option>
                </select>
              </div>

              {/* Digits / Padding */}
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
                  {t("digits")}
                </label>
                <div className="grid grid-cols-3 gap-2">
                  {[4, 5, 6].map((num) => (
                    <button
                      key={num}
                      type="button"
                      onClick={() => handleBuilderChange({ padding: num })}
                      className={`py-2 px-3 text-xs font-mono font-bold border transition-colors cursor-pointer ${
                        padding === num
                          ? "border-erp-navy bg-erp-navy text-white"
                          : "border-erp-border bg-white dark:bg-slate-950 text-slate-700 dark:text-slate-300 hover:border-slate-400"
                      }`}
                    >
                      {num} {t("digits")}
                    </button>
                  ))}
                </div>
              </div>

              {/* Reset Cycle */}
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
                  {t("resetPeriod")}
                </label>
                <select
                  className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 text-sm"
                  value={resetPeriod}
                  onChange={(e) => handleBuilderChange({ resetPeriod: e.target.value as ResetPeriod })}
                >
                  <option value="Monthly">{t("resetOptions.monthly")}</option>
                  <option value="Yearly">{t("resetOptions.yearly")}</option>
                  <option value="Daily">{t("resetOptions.daily")}</option>
                  <option value="Never">{t("resetOptions.never")}</option>
                </select>
              </div>
            </div>

            {/* Branch Specific Toggle */}
            <label className="flex items-center gap-2 cursor-pointer pt-2 border-t border-erp-border">
              <input
                type="checkbox"
                className="w-4 h-4 border border-erp-border text-erp-navy focus:ring-0"
                checked={isBranchSpecific}
                onChange={(e) => handleBuilderChange({ isBranchSpecific: e.target.checked })}
              />
              <span className="text-sm font-medium text-slate-800 dark:text-slate-200">
                {t("isBranchSpecific")}
              </span>
            </label>
          </div>
        ) : (
          /* Section 3: Advanced Mode (Raw Pattern + Token Pills) */
          <div className="flex flex-col gap-4 border border-amber-300 dark:border-amber-700 p-4 bg-amber-50/40 dark:bg-amber-950/20">
            <div className="flex flex-col gap-2">
              <div className="flex items-center justify-between">
                <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
                  {t("formatPattern")}
                </label>
                <span className="text-xs text-amber-600 dark:text-amber-400 font-medium">
                  {t("advancedModeToggle")}
                </span>
              </div>
              <input
                type="text"
                className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 font-mono text-sm"
                value={formatPattern}
                onChange={(e) => setFormatPattern(e.target.value)}
                placeholder="{PREFIX}-{BRANCH}-{BB}{MM}-{SEQ:4}"
              />

              {/* Token Insertion Pills */}
              <div className="flex flex-col gap-1 mt-1">
                <span className="text-xs text-slate-500">{t("clickToInsertToken")}:</span>
                <div className="flex flex-wrap gap-1.5">
                  {TOKEN_DEFINITIONS.map((tok) => (
                    <button
                      key={tok.token}
                      type="button"
                      onClick={() => handleInsertToken(tok.token)}
                      className="px-2 py-1 text-xs font-mono bg-white dark:bg-slate-800 border border-slate-300 dark:border-slate-700 hover:border-erp-navy hover:text-erp-navy transition-colors cursor-pointer"
                      title={t(`tokens.${tok.key}`)}
                    >
                      {tok.token}
                    </button>
                  ))}
                </div>
              </div>
            </div>

            {/* Reset Period & Padding in Advanced Mode */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
                  {t("resetPeriod")}
                </label>
                <select
                  className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 text-sm"
                  value={resetPeriod}
                  onChange={(e) => setResetPeriod(e.target.value as ResetPeriod)}
                >
                  <option value="Never">{t("resetOptions.never")}</option>
                  <option value="Yearly">{t("resetOptions.yearly")}</option>
                  <option value="Monthly">{t("resetOptions.monthly")}</option>
                  <option value="Daily">{t("resetOptions.daily")}</option>
                </select>
              </div>

              <div className="flex flex-col gap-1.5">
                <label className="text-xs font-semibold uppercase tracking-wider text-slate-700 dark:text-slate-300">
                  {t("padding")}
                </label>
                <input
                  type="number"
                  min={1}
                  max={10}
                  className="erp-input px-3 py-2 border border-erp-border bg-white dark:bg-slate-950 text-sm"
                  value={padding}
                  onChange={(e) => setPadding(parseInt(e.target.value, 10) || 4)}
                />
              </div>
            </div>

            {/* Branch Specific Checkbox */}
            <label className="flex items-center gap-2 cursor-pointer pt-2">
              <input
                type="checkbox"
                className="w-4 h-4 border border-erp-border text-erp-navy focus:ring-0"
                checked={isBranchSpecific}
                onChange={(e) => setIsBranchSpecific(e.target.checked)}
              />
              <span className="text-sm text-slate-800 dark:text-slate-200">{t("isBranchSpecific")}</span>
            </label>
          </div>
        )}

        {/* Advanced Mode Toggle Switch */}
        <div className="flex items-center justify-between pt-2 border-t border-erp-border">
          <div className="flex flex-col">
            <span className="text-xs font-semibold text-slate-800 dark:text-slate-200">
              {t("advancedModeToggle")}
            </span>
            <span className="text-xs text-slate-500">
              {t("advancedModeHint")}
            </span>
          </div>
          <button
            type="button"
            role="switch"
            aria-checked={isAdvancedMode}
            onClick={() => setIsAdvancedMode((prev) => !prev)}
            className={`w-11 h-6 flex items-center p-1 transition-colors cursor-pointer border ${
              isAdvancedMode ? "bg-erp-navy border-erp-navy justify-end" : "bg-slate-200 dark:bg-slate-700 border-slate-300 dark:border-slate-600 justify-start"
            }`}
          >
            <div className="w-4 h-4 bg-white shadow-sm" />
          </button>
        </div>
      </div>
    </Drawer>
  );
}
