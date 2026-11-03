"use client";

import React, { useState } from "react";
import { useTranslations } from "next-intl";
import { useSpeechRecognition } from "@/hooks/useSpeechRecognition";
import { Alert } from "@/components/ui/Alert";
import { Button } from "@/components/ui/Button";
import { IconMic } from "@/components/common/Icons";
import {
  hasParsedDimensions,
  parseVoiceMeasurement,
  type ParsedVoiceMeasurement,
} from "../../schemas/field-survey-voice";

const VOICE_ERROR_KEYS: Readonly<Record<string, string>> = {
  "not-allowed": "micDenied",
  "service-not-allowed": "micDenied",
  "no-speech": "noSpeech",
  "language-not-supported": "languageUnsupported",
  network: "network",
};

interface PendingVoiceResult {
  transcript: string;
  parsed: ParsedVoiceMeasurement;
}

export interface FieldSurveyVoiceInputProps {
  /** BCP 47 tag passed to the recogniser, e.g. "th-TH". */
  lang: string;
  /** Only offer the spoken title when the item has none yet, so typed names are never overwritten. */
  canSetTitle: boolean;
  disabled?: boolean;
  onApply: (parsed: ParsedVoiceMeasurement) => void;
}

/** Speak an item's name and dimensions; the parsed values are shown for confirmation before use. */
export function FieldSurveyVoiceInput({ lang, canSetTitle, disabled, onApply }: FieldSurveyVoiceInputProps) {
  const t = useTranslations("fieldSurvey.voice");
  const [pending, setPending] = useState<PendingVoiceResult | null>(null);
  const [errorKey, setErrorKey] = useState<string | null>(null);

  const { isSupported, isListening, start, stop } = useSpeechRecognition({
    lang,
    onFinalTranscript: (transcript) => {
      const parsed = parseVoiceMeasurement(transcript);
      if (!hasParsedDimensions(parsed) && !(canSetTitle && parsed.title)) {
        setErrorKey("noMatch");
        return;
      }
      setPending({ transcript, parsed });
    },
    onError: (code) => {
      if (code === "aborted") return;
      setErrorKey(VOICE_ERROR_KEYS[code] ?? "generic");
    },
  });

  if (!isSupported) return null;

  const previewParts: string[] = [];
  if (pending) {
    const { title, widthMm, depthMm, heightMm } = pending.parsed;
    if (canSetTitle && title) previewParts.push(t("titleLabel", { value: title }));
    if (widthMm !== null) previewParts.push(t("width", { value: widthMm }));
    if (depthMm !== null) previewParts.push(t("depth", { value: depthMm }));
    if (heightMm !== null) previewParts.push(t("height", { value: heightMm }));
  }

  return (
    <div className="space-y-2">
      <div className="flex flex-wrap items-center gap-2">
        <Button
          type="button"
          variant={isListening ? "primary" : "outline"}
          size="sm"
          disabled={disabled}
          onClick={() => {
            setErrorKey(null);
            setPending(null);
            if (isListening) stop();
            else start();
          }}
          icon={<IconMic size={16} />}
        >
          {isListening ? t("listening") : t("start")}
        </Button>
        <span className="text-[11px] text-erp-text-muted">{t("privacyNote")}</span>
      </div>

      {errorKey && (
        <Alert variant="warning" onClose={() => setErrorKey(null)}>
          {t(`errors.${errorKey}`)}
        </Alert>
      )}

      {pending && (
        <Alert variant="info" title={t("confirmTitle")}>
          <div className="space-y-3">
            <div className="text-xs text-erp-text-muted">&ldquo;{pending.transcript}&rdquo;</div>
            <div className="text-sm font-semibold text-erp-navy">{previewParts.join(" · ")}</div>
            <div className="flex flex-wrap gap-2">
              <Button
                type="button"
                variant="primary"
                size="sm"
                onClick={() => {
                  onApply(pending.parsed);
                  setPending(null);
                }}
              >
                {t("apply")}
              </Button>
              <Button type="button" variant="outline" size="sm" onClick={() => setPending(null)}>
                {t("cancel")}
              </Button>
            </div>
          </div>
        </Alert>
      )}
    </div>
  );
}
