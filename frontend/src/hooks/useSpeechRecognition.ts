"use client";

import { useCallback, useEffect, useRef, useState, useSyncExternalStore } from "react";

interface SpeechRecognitionResultLike {
  readonly isFinal: boolean;
  readonly [index: number]: { readonly transcript: string };
}

interface SpeechRecognitionEventLike {
  readonly results: ArrayLike<SpeechRecognitionResultLike>;
}

interface SpeechRecognitionErrorEventLike {
  readonly error: string;
}

interface SpeechRecognitionLike {
  lang: string;
  continuous: boolean;
  interimResults: boolean;
  onresult: ((event: SpeechRecognitionEventLike) => void) | null;
  onerror: ((event: SpeechRecognitionErrorEventLike) => void) | null;
  onend: (() => void) | null;
  start(): void;
  stop(): void;
  abort(): void;
}

type SpeechRecognitionConstructor = new () => SpeechRecognitionLike;

interface SpeechWindow {
  SpeechRecognition?: SpeechRecognitionConstructor;
  webkitSpeechRecognition?: SpeechRecognitionConstructor;
}

function getSpeechRecognition(): SpeechRecognitionConstructor | null {
  if (typeof window === "undefined") return null;
  const speechWindow = window as unknown as SpeechWindow;
  return speechWindow.SpeechRecognition ?? speechWindow.webkitSpeechRecognition ?? null;
}

const subscribeNever = () => () => undefined;

export interface UseSpeechRecognitionOptions {
  lang: string;
  onFinalTranscript: (transcript: string) => void;
  /** Receives the browser's error code (e.g. "not-allowed", "no-speech"). */
  onError: (code: string) => void;
}

/**
 * Browser Web Speech API wrapper (free, no key). Support varies by browser and language, and the
 * audio is processed by the browser vendor's service, so callers must disclose that to users.
 */
export function useSpeechRecognition({ lang, onFinalTranscript, onError }: UseSpeechRecognitionOptions) {
  const isSupported = useSyncExternalStore(subscribeNever, () => getSpeechRecognition() !== null, () => false);
  const [isListening, setIsListening] = useState(false);
  const recognitionRef = useRef<SpeechRecognitionLike | null>(null);
  const callbacksRef = useRef({ onFinalTranscript, onError });

  useEffect(() => {
    callbacksRef.current = { onFinalTranscript, onError };
  });

  useEffect(
    () => () => {
      recognitionRef.current?.abort();
    },
    []
  );

  const start = useCallback(() => {
    const Recognition = getSpeechRecognition();
    if (!Recognition) return;

    const recognition = new Recognition();
    recognition.lang = lang;
    recognition.continuous = false;
    recognition.interimResults = false;
    recognition.onresult = (event) => {
      const result = event.results[event.results.length - 1];
      if (result?.isFinal) callbacksRef.current.onFinalTranscript(result[0].transcript);
    };
    recognition.onerror = (event) => callbacksRef.current.onError(event.error);
    recognition.onend = () => setIsListening(false);
    recognitionRef.current = recognition;

    try {
      recognition.start();
      setIsListening(true);
    } catch {
      callbacksRef.current.onError("start-failed");
    }
  }, [lang]);

  const stop = useCallback(() => {
    recognitionRef.current?.stop();
  }, []);

  return { isSupported, isListening, start, stop };
}
