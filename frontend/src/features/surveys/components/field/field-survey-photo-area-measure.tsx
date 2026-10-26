"use client";

import React, { useEffect, useMemo, useRef, useState } from "react";
import { useTranslations } from "next-intl";
import { Modal } from "@/components/ui/Modal";
import { Button } from "@/components/ui/Button";
import { Tooltip } from "@/components/ui/Tooltip";
import { Select } from "@/components/ui/Select";
import { Input } from "@/components/ui/Input";
import { Alert } from "@/components/ui/Alert";
import { IconCamera, IconCheck, IconRotateCcw, IconTrash } from "@/components/common/Icons";
import { optimizeImageToWebP } from "@/lib/media/image-optimization";
import {
  isSimplePolygon,
  measurePlanePolygon,
  type Point2D,
} from "@/lib/geometry/plane-measurement";
import type {
  FieldSurveyPhoto,
  PhotoMeasurement,
  PhotoReferenceKind,
} from "../../schemas/field-survey-schema";

export interface PhotoAreaMeasureResult {
  photoMeasurement: PhotoMeasurement;
  grossWidthMm: number;
  grossLengthMm: number;
  annotatedPhoto: FieldSurveyPhoto;
}

export interface FieldSurveyPhotoAreaMeasureProps {
  onClose: () => void;
  onApply: (result: PhotoAreaMeasureResult) => void;
}

type PointKind = "reference" | "area";

interface DragState {
  kind: PointKind;
  index: number;
  displayWidth: number;
}

const REFERENCE_PRESETS: Record<Exclude<PhotoReferenceKind, "custom">, { widthMm: number; heightMm: number }> = {
  a4: { widthMm: 210, heightMm: 297 },
  a3: { widthMm: 297, heightMm: 420 },
};
const MAX_IMAGE_DIMENSION = 2048;
const LOUPE_SIZE = 112;
const LOUPE_ZOOM = 2.5;

function toPolygonPoints(points: readonly Point2D[]): string {
  return points.map((p) => `${p.x},${p.y}`).join(" ");
}

function canvasToJpegBlob(canvas: HTMLCanvasElement): Promise<Blob | null> {
  return new Promise((resolve) => canvas.toBlob(resolve, "image/jpeg", 0.85));
}

function loadImage(url: string): Promise<HTMLImageElement> {
  return new Promise((resolve, reject) => {
    const img = new Image();
    img.onload = () => resolve(img);
    img.onerror = () => reject(new Error("image-load-failed"));
    img.src = url;
  });
}

export function FieldSurveyPhotoAreaMeasure({ onClose, onApply }: FieldSurveyPhotoAreaMeasureProps) {
  const t = useTranslations("fieldSurvey.photoMeasure");
  const tDim = useTranslations("fieldSurvey.dimensions");

  const fileInputRef = useRef<HTMLInputElement>(null);
  const stageRef = useRef<HTMLDivElement>(null);
  const imageUrlRef = useRef<string | null>(null);

  const [imageUrl, setImageUrl] = useState<string | null>(null);
  const [imageSize, setImageSize] = useState<{ width: number; height: number } | null>(null);
  const [isPreparing, setIsPreparing] = useState(false);
  const [loadFailed, setLoadFailed] = useState(false);
  const [isRendering, setIsRendering] = useState(false);

  const [referenceKind, setReferenceKind] = useState<PhotoReferenceKind>("a4");
  const [customWidth, setCustomWidth] = useState("");
  const [customHeight, setCustomHeight] = useState("");
  const [referencePoints, setReferencePoints] = useState<Point2D[]>([]);
  const [areaPoints, setAreaPoints] = useState<Point2D[]>([]);
  const [isClosed, setIsClosed] = useState(false);
  const [drag, setDrag] = useState<DragState | null>(null);

  // Revoke the working photo URL when the modal unmounts.
  useEffect(() => {
    return () => {
      if (imageUrlRef.current) URL.revokeObjectURL(imageUrlRef.current);
    };
  }, []);

  const referenceSize = useMemo(() => {
    if (referenceKind !== "custom") return REFERENCE_PRESETS[referenceKind];
    const widthMm = Number(customWidth);
    const heightMm = Number(customHeight);
    return widthMm > 0 && heightMm > 0 ? { widthMm, heightMm } : null;
  }, [referenceKind, customWidth, customHeight]);

  const referenceValid = useMemo(
    () =>
      referencePoints.length === 4 && referenceSize
        ? measurePlanePolygon({
            referenceQuadPx: referencePoints,
            referenceSizeMm: referenceSize,
            polygonPx: referencePoints,
          }) !== null
        : null,
    [referencePoints, referenceSize]
  );

  const isSelfIntersecting = areaPoints.length >= 4 && !isSimplePolygon(areaPoints);

  const result = useMemo(
    () =>
      referenceValid && referenceSize && areaPoints.length >= 3
        ? measurePlanePolygon({
            referenceQuadPx: referencePoints,
            referenceSizeMm: referenceSize,
            polygonPx: areaPoints,
          })
        : null,
    [referenceValid, referenceSize, referencePoints, areaPoints]
  );

  const step = !imageUrl ? 1 : referencePoints.length < 4 ? 2 : 3;
  const canConfirm = Boolean(result) && !isSelfIntersecting && !isRendering;

  const handleFileSelected = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    e.target.value = "";
    if (!file) return;

    setIsPreparing(true);
    setLoadFailed(false);
    const optimized = await optimizeImageToWebP(file, { maxDimension: MAX_IMAGE_DIMENSION });
    if (imageUrlRef.current) URL.revokeObjectURL(imageUrlRef.current);
    const url = URL.createObjectURL(optimized.file);
    imageUrlRef.current = url;
    setImageUrl(url);
    setImageSize(null);
    setReferencePoints([]);
    setAreaPoints([]);
    setIsClosed(false);
    setIsPreparing(false);
  };

  const toImagePoint = (clientX: number, clientY: number): Point2D | null => {
    const rect = stageRef.current?.getBoundingClientRect();
    if (!rect || !imageSize || rect.width === 0 || rect.height === 0) return null;
    const clamp = (value: number, max: number) => Math.min(max, Math.max(0, value));
    return {
      x: clamp(((clientX - rect.left) / rect.width) * imageSize.width, imageSize.width),
      y: clamp(((clientY - rect.top) / rect.height) * imageSize.height, imageSize.height),
    };
  };

  const startDrag = (e: React.PointerEvent<HTMLDivElement>, kind: PointKind, index: number) => {
    const stage = stageRef.current;
    if (!stage) return;
    stage.setPointerCapture(e.pointerId);
    setDrag({ kind, index, displayWidth: stage.getBoundingClientRect().width });
  };

  const handleStagePointerDown = (e: React.PointerEvent<HTMLDivElement>) => {
    const point = toImagePoint(e.clientX, e.clientY);
    if (!point) return;
    if (referencePoints.length < 4) {
      setReferencePoints((prev) => [...prev, point]);
      startDrag(e, "reference", referencePoints.length);
    } else if (!isClosed) {
      setAreaPoints((prev) => [...prev, point]);
      startDrag(e, "area", areaPoints.length);
    }
  };

  const handleHandlePointerDown = (
    e: React.PointerEvent<HTMLDivElement>,
    kind: PointKind,
    index: number
  ) => {
    e.stopPropagation();
    startDrag(e, kind, index);
  };

  const handleStagePointerMove = (e: React.PointerEvent<HTMLDivElement>) => {
    if (!drag) return;
    const point = toImagePoint(e.clientX, e.clientY);
    if (!point) return;
    const replace = (prev: Point2D[]) => prev.map((p, i) => (i === drag.index ? point : p));
    if (drag.kind === "reference") setReferencePoints(replace);
    else setAreaPoints(replace);
  };

  const handleStagePointerUp = () => setDrag(null);

  const handleUndo = () => {
    if (areaPoints.length > 0) {
      setAreaPoints((prev) => prev.slice(0, -1));
      setIsClosed(false);
    } else {
      setReferencePoints((prev) => prev.slice(0, -1));
    }
  };

  const handleClearArea = () => {
    setAreaPoints([]);
    setIsClosed(false);
  };

  const renderAnnotatedPhoto = async (
    measured: NonNullable<typeof result>,
    size: NonNullable<typeof referenceSize>
  ): Promise<PhotoAreaMeasureResult | null> => {
    if (!imageUrl) return null;
    const img = await loadImage(imageUrl);
    const canvas = document.createElement("canvas");
    canvas.width = img.naturalWidth;
    canvas.height = img.naturalHeight;
    const ctx = canvas.getContext("2d");
    if (!ctx) return null;

    const styles = getComputedStyle(document.documentElement);
    const navy = styles.getPropertyValue("--erp-navy").trim();
    const warning = styles.getPropertyValue("--erp-warning").trim();
    const lineWidth = Math.max(2, canvas.width / 400);

    ctx.drawImage(img, 0, 0);
    ctx.lineJoin = "miter";
    ctx.lineWidth = lineWidth;

    const trace = (points: readonly Point2D[]) => {
      ctx.beginPath();
      points.forEach((p, i) => (i === 0 ? ctx.moveTo(p.x, p.y) : ctx.lineTo(p.x, p.y)));
      ctx.closePath();
    };

    ctx.strokeStyle = warning;
    trace(referencePoints);
    ctx.stroke();

    trace(areaPoints);
    ctx.globalAlpha = 0.25;
    ctx.fillStyle = navy;
    ctx.fill();
    ctx.globalAlpha = 1;
    ctx.strokeStyle = navy;
    ctx.stroke();

    const label = `${measured.areaSqm.toFixed(2)} ${tDim("unitSqm")}`;
    const fontSize = Math.max(16, Math.round(canvas.width / 28));
    ctx.font = `bold ${fontSize}px sans-serif`;
    const pad = fontSize / 3;
    const labelWidth = ctx.measureText(label).width + pad * 2;
    ctx.fillStyle = navy;
    ctx.fillRect(pad, pad, labelWidth, fontSize + pad * 2);
    ctx.fillStyle = "#ffffff";
    ctx.textBaseline = "top";
    ctx.fillText(label, pad * 2, pad * 2);

    const blob = await canvasToJpegBlob(canvas);
    if (!blob) return null;

    const annotatedFile = new File([blob], `photo-area-${Date.now()}.jpg`, { type: "image/jpeg" });

    return {
      photoMeasurement: {
        grossAreaSqm: measured.areaSqm,
        vertexCount: measured.vertexCount,
        referenceKind,
        referenceWidthMm: size.widthMm,
        referenceHeightMm: size.heightMm,
      },
      grossWidthMm: measured.widthMm,
      grossLengthMm: measured.lengthMm,
      annotatedPhoto: {
        id: `photo-${Date.now()}-${Math.random().toString(36).substring(2, 7)}`,
        url: URL.createObjectURL(annotatedFile),
        fileName: annotatedFile.name,
        file: annotatedFile,
        tag: "measurement_sketch",
        caption: t("annotatedCaption", { area: measured.areaSqm.toFixed(2) }),
      },
    };
  };

  const handleConfirm = async () => {
    if (!result || !referenceSize || isRendering) return;
    setIsRendering(true);
    try {
      const applied = await renderAnnotatedPhoto(result, referenceSize);
      if (applied) {
        onApply(applied);
        onClose();
      }
    } finally {
      setIsRendering(false);
    }
  };

  const loupe = (() => {
    if (!drag || !imageUrl || !imageSize) return null;
    const point = (drag.kind === "reference" ? referencePoints : areaPoints)[drag.index];
    if (!point) return null;
    const zoomedWidth = drag.displayWidth * LOUPE_ZOOM;
    const zoomedHeight = (zoomedWidth * imageSize.height) / imageSize.width;
    return {
      onRight: point.x < imageSize.width / 2,
      style: {
        width: LOUPE_SIZE,
        height: LOUPE_SIZE,
        backgroundImage: `url(${imageUrl})`,
        backgroundSize: `${zoomedWidth}px ${zoomedHeight}px`,
        backgroundPosition: `${LOUPE_SIZE / 2 - (point.x / imageSize.width) * zoomedWidth}px ${
          LOUPE_SIZE / 2 - (point.y / imageSize.height) * zoomedHeight
        }px`,
      } satisfies React.CSSProperties,
    };
  })();

  const stepLabels = [t("steps.photo"), t("steps.reference"), t("steps.area")];

  const renderHandles = (points: readonly Point2D[], kind: PointKind) =>
    imageSize
      ? points.map((p, index) => (
          <div
            key={`${kind}-${index}`}
            onPointerDown={(e) => handleHandlePointerDown(e, kind, index)}
            className="absolute flex h-11 w-11 -translate-x-1/2 -translate-y-1/2 cursor-grab items-center justify-center"
            style={{
              left: `${(p.x / imageSize.width) * 100}%`,
              top: `${(p.y / imageSize.height) * 100}%`,
            }}
          >
            <span
              className={`flex h-4 w-4 items-center justify-center border border-white text-[9px] font-bold text-white ${
                kind === "reference" ? "bg-erp-warning" : "bg-erp-navy"
              }`}
            >
              {index + 1}
            </span>
          </div>
        ))
      : null;

  return (
    <Modal
      isOpen
      onClose={onClose}
      size="full"
      title={t("title")}
      description={t("subtitle")}
      closeDisabled={isRendering}
      closeOnOverlayClick={false}
      footer={
        <>
          <Button type="button" variant="outline" onClick={onClose} disabled={isRendering}>
            {t("cancel")}
          </Button>
          <Button
            type="button"
            variant="primary"
            onClick={handleConfirm}
            disabled={!canConfirm}
            isLoading={isRendering}
            icon={<IconCheck size={16} />}
          >
            {t("confirm")}
          </Button>
        </>
      }
    >
      <div className="space-y-4">
        {/* Step indicator */}
        <ol className="grid grid-cols-3 gap-1" aria-label={t("stepsLabel")}>
          {stepLabels.map((label, i) => (
            <li
              key={label}
              aria-current={step === i + 1 ? "step" : undefined}
              className={`border px-2 py-2 text-center text-xs font-semibold ${
                step === i + 1
                  ? "border-erp-navy bg-erp-navy text-white"
                  : step > i + 1
                    ? "border-erp-navy bg-erp-surface-subtle text-erp-navy"
                    : "border-erp-border bg-white text-erp-text-muted"
              }`}
            >
              {i + 1}. {label}
            </li>
          ))}
        </ol>

        <Alert variant="info">{t("limitsHint")}</Alert>

        {/* Step 1: photo */}
        <div className="flex flex-wrap items-center gap-3">
          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            capture="environment"
            onChange={handleFileSelected}
            className="hidden"
          />
          <Button
            type="button"
            variant={imageUrl ? "outline" : "primary"}
            onClick={() => fileInputRef.current?.click()}
            isLoading={isPreparing}
            disabled={isRendering}
            icon={<IconCamera size={16} />}
          >
            {imageUrl ? t("changePhoto") : t("choosePhoto")}
          </Button>
          {!imageUrl && <p className="text-xs text-erp-text-muted">{t("photoHint")}</p>}
        </div>

        {loadFailed && <Alert variant="danger">{t("errors.imageLoadFailed")}</Alert>}

        {imageUrl && (
          <>
            {/* Step 2: reference settings */}
            <div className="grid grid-cols-1 gap-3 sm:grid-cols-3">
              <Select
                label={t("referenceLabel")}
                value={referenceKind}
                onChange={(e) => setReferenceKind(e.target.value as PhotoReferenceKind)}
                disabled={isRendering}
                wrapperClassName="!mb-0"
                options={[
                  { value: "a4", label: t("referenceKinds.a4") },
                  { value: "a3", label: t("referenceKinds.a3") },
                  { value: "custom", label: t("referenceKinds.custom") },
                ]}
              />
              {referenceKind === "custom" && (
                <>
                  <Input
                    label={`${t("customWidth")} (${tDim("unitMm")})`}
                    type="number"
                    inputMode="decimal"
                    value={customWidth}
                    onChange={(e) => setCustomWidth(e.target.value)}
                    placeholder={t("customWidthPlaceholder")}
                    disabled={isRendering}
                    wrapperClassName="!mb-0"
                  />
                  <Input
                    label={`${t("customHeight")} (${tDim("unitMm")})`}
                    type="number"
                    inputMode="decimal"
                    value={customHeight}
                    onChange={(e) => setCustomHeight(e.target.value)}
                    placeholder={t("customHeightPlaceholder")}
                    disabled={isRendering}
                    wrapperClassName="!mb-0"
                  />
                </>
              )}
            </div>

            <p className="text-xs font-semibold text-erp-navy" aria-live="polite">
              {step === 2
                ? t("instructions.reference", { count: referencePoints.length })
                : isClosed
                  ? t("instructions.closed")
                  : t("instructions.area", { count: areaPoints.length })}
            </p>

            {/* Annotation stage */}
            <div
              ref={stageRef}
              onPointerDown={handleStagePointerDown}
              onPointerMove={handleStagePointerMove}
              onPointerUp={handleStagePointerUp}
              onPointerCancel={handleStagePointerUp}
              className="relative mx-auto w-fit max-w-full cursor-crosshair select-none border border-erp-border"
              style={{ touchAction: "none" }}
            >
              {/* eslint-disable-next-line @next/next/no-img-element */}
              <img
                src={imageUrl}
                alt={t("photoAlt")}
                draggable={false}
                onLoad={(e) =>
                  setImageSize({
                    width: e.currentTarget.naturalWidth,
                    height: e.currentTarget.naturalHeight,
                  })
                }
                onError={() => setLoadFailed(true)}
                className="block max-h-[60vh] max-w-full"
              />
              {imageSize && (
                <svg
                  viewBox={`0 0 ${imageSize.width} ${imageSize.height}`}
                  className="pointer-events-none absolute inset-0 h-full w-full"
                  aria-hidden="true"
                >
                  {referencePoints.length >= 2 && (
                    <polygon
                      points={toPolygonPoints(referencePoints)}
                      className="fill-none stroke-erp-warning"
                      strokeWidth={2}
                      vectorEffect="non-scaling-stroke"
                    />
                  )}
                  {areaPoints.length >= 2 && (
                    <polygon
                      points={toPolygonPoints(areaPoints)}
                      className="fill-erp-navy stroke-erp-navy"
                      fillOpacity={0.25}
                      strokeWidth={2}
                      vectorEffect="non-scaling-stroke"
                    />
                  )}
                </svg>
              )}
              {renderHandles(referencePoints, "reference")}
              {renderHandles(areaPoints, "area")}
              {loupe && (
                <div
                  className={`pointer-events-none absolute top-1 border-2 border-erp-navy bg-white ${
                    loupe.onRight ? "right-1" : "left-1"
                  }`}
                  style={loupe.style}
                  aria-hidden="true"
                >
                  <span className="absolute left-1/2 top-0 h-full w-px bg-erp-danger" />
                  <span className="absolute left-0 top-1/2 h-px w-full bg-erp-danger" />
                </div>
              )}
            </div>

            {/* Tools */}
            <div className="flex flex-wrap items-center gap-2">
              <Tooltip content={t("undo")} position="top">
                <Button
                  type="button"
                  variant="outline"
                  size="icon"
                  onClick={handleUndo}
                  disabled={isRendering || referencePoints.length + areaPoints.length === 0}
                  aria-label={t("undo")}
                >
                  <IconRotateCcw size={16} />
                </Button>
              </Tooltip>
              <Tooltip content={t("clearArea")} position="top">
                <Button
                  type="button"
                  variant="outline"
                  size="icon"
                  onClick={handleClearArea}
                  disabled={isRendering || areaPoints.length === 0}
                  aria-label={t("clearArea")}
                >
                  <IconTrash size={16} />
                </Button>
              </Tooltip>
              <Button
                type="button"
                variant="primary"
                onClick={() => setIsClosed(true)}
                disabled={isRendering || isClosed || areaPoints.length < 3}
              >
                {t("closePolygon")}
              </Button>
            </div>

            {/* Validation */}
            {referencePoints.length === 4 && !referenceSize && (
              <Alert variant="warning">{t("errors.referenceSizeInvalid")}</Alert>
            )}
            {referenceValid === false && (
              <Alert variant="danger">{t("errors.referenceDegenerate")}</Alert>
            )}
            {isSelfIntersecting && (
              <Alert variant="danger">{t("errors.selfIntersecting")}</Alert>
            )}

            {/* Live result */}
            {result && (
              <div
                className="flex flex-wrap items-center justify-between gap-2 border-l-4 border-erp-navy bg-erp-surface-subtle p-3 text-xs"
                aria-live="polite"
              >
                <div>
                  <span className="text-erp-text-muted">{t("result.area")}: </span>
                  <span className="text-sm font-bold text-erp-navy">
                    {result.areaSqm.toFixed(2)} {tDim("unitSqm")}
                  </span>
                </div>
                <div>
                  <span className="text-erp-text-muted">{t("result.size")}: </span>
                  <span className="font-semibold text-erp-text-main">
                    {result.widthMm} × {result.lengthMm} {tDim("unitMm")}
                  </span>
                </div>
                <div>
                  <span className="text-erp-text-muted">{t("result.vertices")}: </span>
                  <span className="font-semibold text-erp-text-main">{result.vertexCount}</span>
                </div>
              </div>
            )}
          </>
        )}
      </div>
    </Modal>
  );
}
