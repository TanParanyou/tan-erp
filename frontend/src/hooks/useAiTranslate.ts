"use client";

import { useMutation } from "@tanstack/react-query";
import { useToast } from "./useToast";
import { useTranslations } from "next-intl";

export interface AiTranslateRequest {
  text: string;
  sourceLang?: string;
  targetLang: string;
}

export interface AiTranslateResponse {
  translatedText: string;
}

export function useAiTranslate() {
  const { toast } = useToast();
  const t = useTranslations("common");

  return useMutation({
    mutationFn: async (req: AiTranslateRequest): Promise<AiTranslateResponse> => {
      // Stub or connect to ERP translation endpoint
      return { translatedText: req.text };
    },
    onError: () => {
      toast.error(
        t("error") || "Error",
        t("translationFailed") || "AI Translation failed"
      );
    },
  });
}
