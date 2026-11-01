"use client";

import React, { useMemo, useState } from "react";
import { useTranslations } from "next-intl";
import { Button } from "@/components/ui/Button";
import { Badge } from "@/components/ui/Badge";
import { DataTable, type Column } from "@/components/ui/DataTable";
import { Drawer } from "@/components/ui/Drawer";
import { Input } from "@/components/ui/Input";
import { Checkbox } from "@/components/ui/Checkbox";
import { MultiLangInput, type MultiLangText } from "@/components/forms/MultiLangInput";
import {
  IconPlus,
  IconEdit,
  IconRefresh,
  IconClose,
  IconSparkles,
  IconPalette,
  IconFileText,
  IconUpload,
  IconTrash,
  IconEye,
  IconCheckCircle,
} from "@/components/common/Icons";
import { SurveyTemplateManager } from "./survey-template-manager";
import { DesignStyleThumbnail } from "./design-style-thumbnail";
import { DesignStyleIllustration } from "./design-style-illustration";
import { AuthenticatedFileImage } from "@/components/common/AuthenticatedFileImage";
import { GalleryLightboxModal, type GalleryItemMetadata } from "@/components/common/GalleryLightboxModal";
import { useDeferredFileUpload } from "@/hooks/useDeferredFileUpload";
import { getAuthToken } from "@/lib/auth/auth-session";
import { useOptionalSelectedMembership } from "@/lib/membership/selected-membership-context";
import { useToast } from "@/hooks/useToast";
import {
  useSurveyDesignStyles,
  useCreateSurveyDesignStyle,
  useUpdateSurveyDesignStyle,
  useSurveyMoodTones,
  useCreateSurveyMoodTone,
  useUpdateSurveyMoodTone,
} from "../../api/survey-queries";
import type {
  SurveyDesignStyleResponse,
  SurveyMoodToneResponse,
} from "@/lib/api/api-client";

type MasterDataTab = "design-styles" | "mood-tones" | "templates";

interface PendingImageItem {
  id: string;
  file: File;
  previewUrl: string;
}

interface ItemEditorState {
  id?: string;
  code: string;
  nameTh: string;
  nameEn: string;
  descriptionTh: string;
  descriptionEn: string;
  colorPalette: string[];
  sortOrder: number;
  isActive: boolean;
  imageFileId?: string | null;
  imageFileIds: string[];
}

const initialEditorState: ItemEditorState = {
  code: "",
  nameTh: "",
  nameEn: "",
  descriptionTh: "",
  descriptionEn: "",
  colorPalette: [],
  sortOrder: 0,
  isActive: true,
  imageFileId: null,
  imageFileIds: [],
};

export function SurveyMasterDataManager() {
  const t = useTranslations("surveySettings");
  const tCommon = useTranslations("common");
  const { toast } = useToast();

  const [activeTab, setActiveTab] = useState<MasterDataTab>("design-styles");
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);
  const [editorState, setEditorState] = useState<ItemEditorState>(initialEditorState);
  // eslint-disable-next-line no-restricted-syntax -- token-exempt: default value of a colour picker input (user data, not theme)
  const [newColorInput, setNewColorInput] = useState("#3B82F6");
  const [validationError, setValidationError] = useState<string | null>(null);

  // File upload & gallery state for design style illustration(s)
  const fileInputRef = React.useRef<HTMLInputElement>(null);
  const [pendingImages, setPendingImages] = useState<PendingImageItem[]>([]);
  const [existingImageFileIds, setExistingImageFileIds] = useState<string[]>([]);
  
  // Drawer Gallery Lightbox state
  const [isDrawerLightboxOpen, setIsDrawerLightboxOpen] = useState(false);
  const [drawerLightboxIndex, setDrawerLightboxIndex] = useState(0);

  const membershipContext = useOptionalSelectedMembership();
  const { uploadFiles, isUploading: isFileUploading } = useDeferredFileUpload({
    parentType: "survey-design-style",
    parentId: editorState.id ?? null,
  });

  // Queries
  const {
    data: designStyles,
    isLoading: isLoadingStyles,
    isError: isErrorStyles,
    refetch: refetchStyles,
  } = useSurveyDesignStyles();

  const {
    data: moodTones,
    isLoading: isLoadingTones,
    isError: isErrorTones,
    refetch: refetchTones,
  } = useSurveyMoodTones();

  // Mutations
  const createStyleMutation = useCreateSurveyDesignStyle();
  const updateStyleMutation = useUpdateSurveyDesignStyle();
  const createToneMutation = useCreateSurveyMoodTone();
  const updateToneMutation = useUpdateSurveyMoodTone();

  const isSaving =
    createStyleMutation.isPending ||
    updateStyleMutation.isPending ||
    createToneMutation.isPending ||
    updateToneMutation.isPending ||
    isFileUploading;

  const cleanupPendingPreviews = () => {
    for (const item of pendingImages) {
      URL.revokeObjectURL(item.previewUrl);
    }
    setPendingImages([]);
  };

  // Open drawer for creating
  const handleAddNew = () => {
    cleanupPendingPreviews();
    setExistingImageFileIds([]);
    setEditorState(initialEditorState);
    setValidationError(null);
    setIsDrawerOpen(true);
  };

  // Open drawer for editing design style
  const handleEditStyle = (item: SurveyDesignStyleResponse) => {
    cleanupPendingPreviews();
    // Resolve existing image IDs
    const resolvedIds: string[] = [];
    if (item.imageFileIds && Array.isArray(item.imageFileIds)) {
      for (const id of item.imageFileIds) {
        if (id && !resolvedIds.includes(id)) {
          resolvedIds.push(id);
        }
      }
    }
    if (item.imageFileId && !resolvedIds.includes(item.imageFileId)) {
      resolvedIds.unshift(item.imageFileId);
    }

    setExistingImageFileIds(resolvedIds);
    setEditorState({
      id: item.id,
      code: item.code ?? "",
      nameTh: item.name?.thai || "",
      nameEn: item.name?.english || "",
      descriptionTh: item.description?.thai || "",
      descriptionEn: item.description?.english || "",
      colorPalette: [],
      sortOrder: item.sortOrder ?? 0,
      isActive: item.isActive ?? true,
      imageFileId: item.imageFileId ?? (resolvedIds[0] || null),
      imageFileIds: resolvedIds,
    });
    setValidationError(null);
    setIsDrawerOpen(true);
  };

  // Open drawer for editing mood tone
  const handleEditTone = (item: SurveyMoodToneResponse) => {
    cleanupPendingPreviews();
    setExistingImageFileIds([]);
    setEditorState({
      id: item.id,
      code: item.code ?? "",
      nameTh: item.name?.thai || "",
      nameEn: item.name?.english || "",
      descriptionTh: item.description?.thai || "",
      descriptionEn: item.description?.english || "",
      colorPalette: item.colorPalette ? [...item.colorPalette] : [],
      sortOrder: item.sortOrder ?? 0,
      isActive: item.isActive ?? true,
      imageFileId: null,
      imageFileIds: [],
    });
    setValidationError(null);
    setIsDrawerOpen(true);
  };

  const handleCloseDrawer = () => {
    if (isSaving) return;
    cleanupPendingPreviews();
    setExistingImageFileIds([]);
    setIsDrawerOpen(false);
    setEditorState(initialEditorState);
    setValidationError(null);
  };

  // Multi-image selection handler
  const handleFilesSelect = (files: FileList | null) => {
    if (!files || files.length === 0) return;
    const newItems: PendingImageItem[] = [];
    for (let i = 0; i < files.length; i++) {
      const file = files[i];
      if (!file) continue;
      newItems.push({
        id: `pending-${crypto.randomUUID()}`,
        file,
        previewUrl: URL.createObjectURL(file),
      });
    }
    setPendingImages((prev) => [...prev, ...newItems]);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleRemoveExistingImage = (fileIdToRemove: string) => {
    setExistingImageFileIds((prev) => prev.filter((id) => id !== fileIdToRemove));
  };

  const handleRemovePendingImage = (pendingIdToRemove: string) => {
    setPendingImages((prev) => {
      const found = prev.find((p) => p.id === pendingIdToRemove);
      if (found) {
        URL.revokeObjectURL(found.previewUrl);
      }
      return prev.filter((p) => p.id !== pendingIdToRemove);
    });
  };

  const handleRemoveAllImages = () => {
    cleanupPendingPreviews();
    setExistingImageFileIds([]);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  // Set cover (move selected image to index 0)
  const handleSetCoverExisting = (fileId: string) => {
    setExistingImageFileIds((prev) => [fileId, ...prev.filter((id) => id !== fileId)]);
  };

  const handleSetCoverPending = (pendingId: string) => {
    const selected = pendingImages.find((p) => p.id === pendingId);
    if (!selected) return;
    setPendingImages((prev) => [selected, ...prev.filter((p) => p.id !== pendingId)]);
  };

  // Build combined items for Drawer Lightbox
  const totalImageCount = existingImageFileIds.length + pendingImages.length;
  const drawerLightboxItems: GalleryItemMetadata[] = [
    ...existingImageFileIds.map((fId, idx) => ({
      id: `existing-${fId}`,
      fileId: fId,
      caption: `${editorState.nameTh || editorState.code} (${idx + 1}/${totalImageCount})`,
    })),
    ...pendingImages.map((p, idx) => ({
      id: p.id,
      imageUrl: p.previewUrl,
      caption: `${editorState.nameTh || editorState.code} (${existingImageFileIds.length + idx + 1}/${totalImageCount})`,
    })),
  ];

  const openDrawerLightboxAtIndex = (index: number) => {
    setDrawerLightboxIndex(index);
    setIsDrawerLightboxOpen(true);
  };

  // Color palette operations for mood & tone
  const handleAddColor = () => {
    const trimmed = newColorInput.trim();
    if (!trimmed) return;
    if (editorState.colorPalette.includes(trimmed)) return;
    setEditorState((prev) => ({
      ...prev,
      colorPalette: [...prev.colorPalette, trimmed],
    }));
  };

  const handleRemoveColor = (indexToRemove: number) => {
    setEditorState((prev) => ({
      ...prev,
      colorPalette: prev.colorPalette.filter((_, idx) => idx !== indexToRemove),
    }));
  };

  // Submit handler
  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editorState.code.trim()) {
      setValidationError(t("messages.codeRequired"));
      return;
    }
    if (!editorState.nameTh.trim()) {
      setValidationError(t("messages.nameRequired"));
      return;
    }
    setValidationError(null);

    const namePayload = {
      thai: editorState.nameTh.trim(),
      english: editorState.nameEn.trim() || undefined,
    };

    const descriptionPayload =
      editorState.descriptionTh.trim() || editorState.descriptionEn.trim()
        ? {
            thai: editorState.descriptionTh.trim() || undefined,
            english: editorState.descriptionEn.trim() || undefined,
          }
        : undefined;

    try {
      if (activeTab === "design-styles") {
        // Upload any newly selected pending files
        const newlyUploadedIds: string[] = [];
        if (pendingImages.length > 0) {
          const token = await getAuthToken();
          const uploadResult = await uploadFiles(
            pendingImages.map((p) => p.file),
            {
              token: token ?? "",
              membershipId: membershipContext?.selectedMembership?.id,
              locale: "th",
            }
          );
          for (const f of uploadResult.files) {
            newlyUploadedIds.push(f.fileId);
          }
        }

        // Combine remaining existing IDs and new IDs
        const combinedIds = [...existingImageFileIds, ...newlyUploadedIds];
        const primaryCoverId = combinedIds[0] ?? null;

        if (editorState.id) {
          await updateStyleMutation.mutateAsync({
            id: editorState.id,
            payload: {
              name: namePayload,
              description: descriptionPayload,
              sortOrder: editorState.sortOrder,
              isActive: editorState.isActive,
              imageFileId: primaryCoverId,
              imageFileIds: combinedIds,
            },
          });
          toast.success(t("messages.updateSuccess"));
        } else {
          await createStyleMutation.mutateAsync({
            code: editorState.code.trim(),
            name: namePayload,
            description: descriptionPayload,
            sortOrder: editorState.sortOrder,
            imageFileId: primaryCoverId,
            imageFileIds: combinedIds,
          });
          toast.success(t("messages.createSuccess"));
        }
      } else {
        if (editorState.id) {
          await updateToneMutation.mutateAsync({
            id: editorState.id,
            payload: {
              name: namePayload,
              description: descriptionPayload,
              colorPalette: editorState.colorPalette,
              sortOrder: editorState.sortOrder,
              isActive: editorState.isActive,
            },
          });
          toast.success(t("messages.updateSuccess"));
        } else {
          await createToneMutation.mutateAsync({
            code: editorState.code.trim(),
            name: namePayload,
            description: descriptionPayload,
            colorPalette: editorState.colorPalette,
            sortOrder: editorState.sortOrder,
          });
          toast.success(t("messages.createSuccess"));
        }
      }
      handleCloseDrawer();
    } catch {
      toast.error(t("messages.saveFailed"));
    }
  };

  // Columns for Design Styles Table
  const styleColumns = useMemo<Column<SurveyDesignStyleResponse>[]>(
    () => [
      {
        id: "illustration",
        header: t("table.illustration"),
        className: "w-24 text-center",
        cell: (_val, item) => (
          <div className="flex items-center justify-center">
            <DesignStyleThumbnail
              code={item.code ?? ""}
              imageFileId={item.imageFileId}
              imageFileIds={item.imageFileIds}
              name={item.name?.thai || item.name?.english || item.code || ""}
              size="md"
            />
          </div>
        ),
      },
      {
        id: "code",
        header: t("table.code"),
        className: "min-w-[120px]",
        cell: (_val, item) => (
          <span className="font-mono font-semibold text-erp-navy">
            {item.code}
          </span>
        ),
      },
      {
        id: "name",
        header: t("table.name"),
        className: "min-w-[200px]",
        cell: (_val, item) => (
          <div className="flex flex-col">
            <span className="font-medium text-erp-text-main">
              {item.name?.thai || "-"}
            </span>
            {item.name?.english && (
              <span className="text-xs text-erp-text-muted">
                {item.name.english}
              </span>
            )}
          </div>
        ),
      },
      {
        id: "description",
        header: t("table.description"),
        className: "min-w-[240px]",
        cell: (_val, item) => (
          <span className="text-xs text-erp-text-body line-clamp-2">
            {item.description?.thai || item.description?.english || "-"}
          </span>
        ),
      },
      {
        id: "sortOrder",
        header: t("table.sortOrder"),
        className: "w-20 text-center",
        cell: (_val, item) => (
          <span className="text-xs text-erp-text-muted font-mono">
            {item.sortOrder}
          </span>
        ),
      },
      {
        id: "isActive",
        header: t("table.status"),
        className: "w-28 text-center",
        cell: (_val, item) => (
          <Badge
            variant={item.isActive ? "success" : "neutral"}
            size="sm"
          >
            {item.isActive ? t("status.active") : t("status.inactive")}
          </Badge>
        ),
      },
      {
        id: "actions",
        header: t("table.actions"),
        isAction: true,
        className: "w-24 text-right",
        cell: (_val, item) => (
          <Button
            variant="outline"
            size="sm"
            onClick={() => handleEditStyle(item)}
            className="inline-flex items-center gap-1.5"
            icon={<IconEdit size={14} />}
          >
            <span>{tCommon("actions.edit")}</span>
          </Button>
        ),
      },
    ],
    [t, tCommon]
  );

  // Columns for Mood & Tones Table
  const toneColumns = useMemo<Column<SurveyMoodToneResponse>[]>(
    () => [
      {
        id: "code",
        header: t("table.code"),
        className: "min-w-[120px]",
        cell: (_val, item) => (
          <span className="font-mono font-semibold text-erp-navy">
            {item.code}
          </span>
        ),
      },
      {
        id: "name",
        header: t("table.name"),
        className: "min-w-[200px]",
        cell: (_val, item) => (
          <div className="flex flex-col">
            <span className="font-medium text-erp-text-main">
              {item.name?.thai || "-"}
            </span>
            {item.name?.english && (
              <span className="text-xs text-erp-text-muted">
                {item.name.english}
              </span>
            )}
          </div>
        ),
      },
      {
        id: "palette",
        header: t("table.palette"),
        className: "min-w-[160px]",
        cell: (_val, item) => {
          const colors = item.colorPalette ?? [];
          if (colors.length === 0) return <span className="text-xs text-erp-text-muted">-</span>;
          return (
            <div className="flex items-center gap-1.5 flex-wrap">
              {colors.map((color, idx) => (
                <span
                  key={`${item.id}-${color}-${idx}`}
                  className="w-5 h-5 rounded-none border border-erp-border shadow-xs shrink-0 inline-block"
                  style={{ backgroundColor: color }}
                  title={color}
                />
              ))}
            </div>
          );
        },
      },
      {
        id: "description",
        header: t("table.description"),
        className: "min-w-[220px]",
        cell: (_val, item) => (
          <span className="text-xs text-erp-text-body line-clamp-2">
            {item.description?.thai || item.description?.english || "-"}
          </span>
        ),
      },
      {
        id: "sortOrder",
        header: t("table.sortOrder"),
        className: "w-20 text-center",
        cell: (_val, item) => (
          <span className="text-xs text-erp-text-muted font-mono">
            {item.sortOrder}
          </span>
        ),
      },
      {
        id: "isActive",
        header: t("table.status"),
        className: "w-28 text-center",
        cell: (_val, item) => (
          <Badge
            variant={item.isActive ? "success" : "neutral"}
            size="sm"
          >
            {item.isActive ? t("status.active") : t("status.inactive")}
          </Badge>
        ),
      },
      {
        id: "actions",
        header: t("table.actions"),
        isAction: true,
        className: "w-24 text-right",
        cell: (_val, item) => (
          <Button
            variant="outline"
            size="sm"
            onClick={() => handleEditTone(item)}
            className="inline-flex items-center gap-1.5"
            icon={<IconEdit size={14} />}
          >
            <span>{tCommon("actions.edit")}</span>
          </Button>
        ),
      },
    ],
    [t, tCommon]
  );

  return (
    <div className="flex flex-col gap-6">
      {/* 2-Tier Architectural Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 border-b border-erp-border pb-4">
        <div>
          <h1 className="text-2xl font-bold text-erp-text-main tracking-tight">
            {t("title")}
          </h1>
          <p className="text-sm text-erp-text-muted mt-1">
            {t("description")}
          </p>
        </div>
        {activeTab !== "templates" && (
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => (activeTab === "design-styles" ? refetchStyles() : refetchTones())}
              title={tCommon("actions.refresh")}
            >
              <IconRefresh size={16} />
              <span>{tCommon("actions.refresh")}</span>
            </Button>
            <Button
              variant="primary"
              size="sm"
              onClick={handleAddNew}
              icon={<IconPlus size={16} />}
            >
              <span>
                {activeTab === "design-styles" ? t("actions.addStyle") : t("actions.addMoodTone")}
              </span>
            </Button>
          </div>
        )}
      </div>

      {/* Tabs */}
      <div className="border-b border-erp-border">
        <nav className="flex space-x-6" aria-label="Tabs">
          <button
            type="button"
            onClick={() => setActiveTab("design-styles")}
            className={`flex items-center gap-2 py-3 px-1 border-b-2 font-medium text-sm transition-colors ${
              activeTab === "design-styles"
                ? "border-erp-navy text-erp-navy font-semibold"
                : "border-transparent text-erp-text-muted hover:text-erp-text-main hover:border-erp-border"
            }`}
          >
            <IconSparkles size={16} />
            <span>{t("tabs.designStyles")}</span>
            {designStyles && (
              <span className="ml-1 px-1.5 py-0.5 text-xs bg-erp-surface-subtle text-erp-text-muted border border-erp-border">
                {designStyles.length}
              </span>
            )}
          </button>
          <button
            type="button"
            onClick={() => setActiveTab("mood-tones")}
            className={`flex items-center gap-2 py-3 px-1 border-b-2 font-medium text-sm transition-colors ${
              activeTab === "mood-tones"
                ? "border-erp-navy text-erp-navy font-semibold"
                : "border-transparent text-erp-text-muted hover:text-erp-text-main hover:border-erp-border"
            }`}
          >
            <IconPalette size={16} />
            <span>{t("tabs.moodTones")}</span>
            {moodTones && (
              <span className="ml-1 px-1.5 py-0.5 text-xs bg-erp-surface-subtle text-erp-text-muted border border-erp-border">
                {moodTones.length}
              </span>
            )}
          </button>
          <button
            type="button"
            onClick={() => setActiveTab("templates")}
            className={`flex items-center gap-2 py-3 px-1 border-b-2 font-medium text-sm transition-colors ${
              activeTab === "templates"
                ? "border-erp-navy text-erp-navy font-semibold"
                : "border-transparent text-erp-text-muted hover:text-erp-text-main hover:border-erp-border"
            }`}
          >
            <IconFileText size={16} />
            <span>{t("tabs.templates")}</span>
          </button>
        </nav>
      </div>

      {/* Content */}
      {activeTab === "templates" ? (
        <SurveyTemplateManager />
      ) : activeTab === "design-styles" ? (
        <DataTable<SurveyDesignStyleResponse>
          columns={styleColumns}
          data={designStyles ?? []}
          isLoading={isLoadingStyles}
          isError={isErrorStyles}
          error={isErrorStyles ? t("loadError") : null}
          onRetry={() => refetchStyles()}
          hidePagination={true}
        />
      ) : (
        <DataTable<SurveyMoodToneResponse>
          columns={toneColumns}
          data={moodTones ?? []}
          isLoading={isLoadingTones}
          isError={isErrorTones}
          error={isErrorTones ? t("loadError") : null}
          onRetry={() => refetchTones()}
          hidePagination={true}
        />
      )}

      {/* Slide-over Drawer for Create / Edit */}
      <Drawer
        isOpen={isDrawerOpen}
        onClose={handleCloseDrawer}
        title={
          activeTab === "design-styles"
            ? editorState.id
              ? t("actions.editStyle")
              : t("actions.addStyle")
            : editorState.id
            ? t("actions.editMoodTone")
            : t("actions.addMoodTone")
        }
        size="md"
        footer={
          <div className="flex items-center justify-end gap-2 w-full">
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={handleCloseDrawer}
              disabled={isSaving}
            >
              <span>{t("actions.cancel")}</span>
            </Button>
            <Button
              type="submit"
              form="survey-item-editor-form"
              variant="primary"
              size="sm"
              isLoading={isSaving}
              disabled={isSaving}
            >
              <span>{t("actions.save")}</span>
            </Button>
          </div>
        }
      >
        <form
          id="survey-item-editor-form"
          onSubmit={handleSave}
          className="flex flex-col gap-4"
        >
          {validationError && (
            <div className="p-3 bg-red-50 border border-red-200 text-xs text-red-600 rounded-none">
              {validationError}
            </div>
          )}

          {/* Code */}
          <div className="erp-form-group">
            <label className="erp-label">
              {t("fields.code")} <span className="text-red-500">*</span>
            </label>
            <Input
              value={editorState.code}
              onChange={(e) =>
                setEditorState((prev) => ({
                  ...prev,
                  code: e.target.value.toLowerCase().replace(/[^a-z0-9_-]/g, ""),
                }))
              }
              placeholder={t("fields.codePlaceholder")}
              disabled={Boolean(editorState.id) || isSaving}
              className="font-mono h-11 min-h-[44px]"
            />
          </div>

          {/* Name (MultiLangInput: TH / EN stored as jsonb) */}
          <MultiLangInput
            label={
              activeTab === "design-styles"
                ? t("fields.styleName")
                : t("fields.moodToneName")
            }
            value={{
              th: editorState.nameTh,
              en: editorState.nameEn,
            }}
            onChange={(val: MultiLangText) =>
              setEditorState((prev) => ({
                ...prev,
                nameTh: val.th ?? "",
                nameEn: val.en ?? "",
              }))
            }
            required
            disabled={isSaving}
          />

          {/* Description (MultiLangInput) */}
          <MultiLangInput
            label={t("fields.description")}
            value={{
              th: editorState.descriptionTh,
              en: editorState.descriptionEn,
            }}
            onChange={(val: MultiLangText) =>
              setEditorState((prev) => ({
                ...prev,
                descriptionTh: val.th ?? "",
                descriptionEn: val.en ?? "",
              }))
            }
            type="textarea"
            placeholder={t("fields.descriptionPlaceholder")}
            disabled={isSaving}
          />

          {/* Design Style Illustrations (Multi-image & Preview) */}
          {activeTab === "design-styles" && (
            <div className="erp-form-group">
              <div className="flex items-center justify-between mb-1">
                <label className="erp-label">{t("fields.illustration")}</label>
                {totalImageCount > 0 && (
                  <span className="text-xs font-mono font-medium text-erp-navy">
                    {t("fields.imageCount", { count: totalImageCount })}
                  </span>
                )}
              </div>
              <p className="text-xs text-erp-text-muted mb-3">
                {t("fields.illustrationHelp")}
              </p>

              {/* Multi-image Cards Grid */}
              <div className="flex flex-col gap-3 p-3 bg-erp-surface-subtle border border-erp-border">
                {totalImageCount === 0 ? (
                  <div className="flex items-center gap-4">
                    {/* Default Architectural Artwork */}
                    <div className="relative w-28 h-20 border border-erp-border bg-white overflow-hidden shrink-0 shadow-2xs">
                      <DesignStyleIllustration
                        code={editorState.code || "default"}
                        alt={editorState.nameTh || editorState.code}
                      />
                    </div>
                    <div className="flex flex-col gap-1">
                      <span className="text-xs font-medium text-erp-text-main">
                        {t("fields.removeCustomImage")}
                      </span>
                      <span className="text-[11px] text-erp-text-muted">
                        ระบบจะแสดงภาพประกอบเชิงสถาปัตยกรรมมาตรฐานสำหรับสไตล์นี้
                      </span>
                    </div>
                  </div>
                ) : (
                  <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
                    {/* Existing Uploaded Images */}
                    {existingImageFileIds.map((fileId, idx) => {
                      const isCover = idx === 0;
                      return (
                        <div
                          key={`existing-${fileId}`}
                          className="relative group border border-erp-border bg-white overflow-hidden shadow-2xs flex flex-col"
                        >
                          <div
                            className="relative w-full aspect-4/3 cursor-pointer overflow-hidden bg-erp-surface-subtle"
                            onClick={() => openDrawerLightboxAtIndex(idx)}
                            title={t("fields.preview")}
                          >
                            <AuthenticatedFileImage
                              fileId={fileId}
                              alt={editorState.nameTh || editorState.code}
                              className="w-full h-full object-cover"
                            />
                            {/* Hover Overlay */}
                            <div className="absolute inset-0 bg-black/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center text-white gap-2">
                              <IconEye size={18} />
                              <span className="text-xs">{t("fields.preview")}</span>
                            </div>
                            {/* Cover Badge */}
                            {isCover && (
                              <div className="absolute top-1 left-1 px-1.5 py-0.5 text-[10px] font-bold bg-erp-navy text-white shadow-xs">
                                {t("fields.coverBadge")}
                              </div>
                            )}
                          </div>

                          {/* Card Actions Bar */}
                          <div className="flex items-center justify-between p-1.5 border-t border-erp-border bg-erp-surface-subtle text-xs">
                            {!isCover ? (
                              <button
                                type="button"
                                onClick={() => handleSetCoverExisting(fileId)}
                                className="inline-flex items-center gap-1 text-[11px] text-erp-text-muted hover:text-erp-navy transition-colors"
                                title={t("fields.setCover")}
                              >
                                <IconCheckCircle size={12} />
                                <span>{t("fields.setCover")}</span>
                              </button>
                            ) : (
                              <span className="text-[11px] font-semibold text-erp-navy flex items-center gap-1">
                                <IconCheckCircle size={12} className="text-erp-navy" />
                                <span>{t("fields.coverBadge")}</span>
                              </span>
                            )}
                            <button
                              type="button"
                              onClick={() => handleRemoveExistingImage(fileId)}
                              className="text-erp-text-muted hover:text-erp-danger p-1 transition-colors"
                              title={t("fields.removeCustomImage")}
                            >
                              <IconTrash size={14} />
                            </button>
                          </div>
                        </div>
                      );
                    })}

                    {/* Pending Newly Selected Images */}
                    {pendingImages.map((p, idx) => {
                      const absoluteIdx = existingImageFileIds.length + idx;
                      const isCover = absoluteIdx === 0;
                      return (
                        <div
                          key={p.id}
                          className="relative group border border-erp-border bg-white overflow-hidden shadow-2xs flex flex-col"
                        >
                          <div
                            className="relative w-full aspect-4/3 cursor-pointer overflow-hidden bg-erp-surface-subtle"
                            onClick={() => openDrawerLightboxAtIndex(absoluteIdx)}
                            title={t("fields.preview")}
                          >
                            {/* eslint-disable-next-line @next/next/no-img-element */}
                            <img
                              src={p.previewUrl}
                              alt={p.file.name}
                              className="w-full h-full object-cover"
                            />
                            {/* Hover Overlay */}
                            <div className="absolute inset-0 bg-black/40 opacity-0 group-hover:opacity-100 transition-opacity flex items-center justify-center text-white gap-2">
                              <IconEye size={18} />
                              <span className="text-xs">{t("fields.preview")}</span>
                            </div>
                            {/* Cover Badge */}
                            {isCover && (
                              <div className="absolute top-1 left-1 px-1.5 py-0.5 text-[10px] font-bold bg-erp-navy text-white shadow-xs">
                                {t("fields.coverBadge")}
                              </div>
                            )}
                          </div>

                          {/* Card Actions Bar */}
                          <div className="flex items-center justify-between p-1.5 border-t border-erp-border bg-erp-surface-subtle text-xs">
                            {!isCover ? (
                              <button
                                type="button"
                                onClick={() => handleSetCoverPending(p.id)}
                                className="inline-flex items-center gap-1 text-[11px] text-erp-text-muted hover:text-erp-navy transition-colors"
                                title={t("fields.setCover")}
                              >
                                <IconCheckCircle size={12} />
                                <span>{t("fields.setCover")}</span>
                              </button>
                            ) : (
                              <span className="text-[11px] font-semibold text-erp-navy flex items-center gap-1">
                                <IconCheckCircle size={12} className="text-erp-navy" />
                                <span>{t("fields.coverBadge")}</span>
                              </span>
                            )}
                            <button
                              type="button"
                              onClick={() => handleRemovePendingImage(p.id)}
                              className="text-erp-text-muted hover:text-erp-danger p-1 transition-colors"
                              title={t("fields.removeCustomImage")}
                            >
                              <IconTrash size={14} />
                            </button>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                )}

                {/* Toolbar for Uploading & Removing */}
                <div className="flex flex-wrap items-center justify-between gap-2 pt-2 border-t border-erp-border">
                  <div className="flex items-center gap-2">
                    <input
                      type="file"
                      ref={fileInputRef}
                      multiple
                      onChange={(e) => handleFilesSelect(e.target.files)}
                      accept="image/png,image/jpeg,image/webp"
                      className="hidden"
                      id="survey-design-style-images-input"
                      disabled={isSaving}
                    />
                    <Button
                      type="button"
                      variant="outline"
                      size="sm"
                      onClick={() => fileInputRef.current?.click()}
                      disabled={isSaving}
                      icon={<IconUpload size={14} />}
                    >
                      <span>
                        {totalImageCount > 0
                          ? t("fields.uploadImage")
                          : t("fields.uploadImage")}
                      </span>
                    </Button>

                    {totalImageCount > 0 && (
                      <Button
                        type="button"
                        variant="ghost"
                        size="sm"
                        onClick={handleRemoveAllImages}
                        disabled={isSaving}
                        className="text-erp-text-muted hover:text-erp-danger"
                        icon={<IconTrash size={14} />}
                      >
                        <span>{t("fields.removeAllImages")}</span>
                      </Button>
                    )}
                  </div>
                  <span className="text-[11px] text-erp-text-muted">
                    PNG, JPG, WebP (max 5MB)
                  </span>
                </div>
              </div>
            </div>
          )}

          {/* Color Palette (Mood & Tone only) */}
          {activeTab === "mood-tones" && (
            <div className="erp-form-group">
              <label className="erp-label">{t("fields.colorPalette")}</label>
              <div className="flex items-center gap-2 mb-2">
                <input
                  type="color"
                  value={newColorInput}
                  onChange={(e) => setNewColorInput(e.target.value)}
                  className="w-11 h-11 p-0.5 border border-erp-border bg-white cursor-pointer rounded-none"
                  title="Pick color"
                />
                <Input
                  value={newColorInput}
                  onChange={(e) => setNewColorInput(e.target.value)}
                  placeholder={t("fields.colorHexPlaceholder")}
                  className="font-mono h-11 min-h-[44px] flex-1"
                />
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={handleAddColor}
                  className="h-11 min-h-[44px]"
                  icon={<IconPlus size={14} />}
                >
                  <span>{t("actions.addColor")}</span>
                </Button>
              </div>

              {/* Swatches preview list */}
              {editorState.colorPalette.length > 0 && (
                <div className="flex flex-wrap gap-2 p-2.5 bg-erp-surface-subtle border border-erp-border">
                  {editorState.colorPalette.map((color, idx) => (
                    <div
                      key={`palette-chip-${color}-${idx}`}
                      className="inline-flex items-center gap-1.5 px-2 py-1 bg-white border border-erp-border shadow-2xs text-xs font-mono"
                    >
                      <span
                        className="w-4 h-4 border border-erp-border shrink-0 rounded-none"
                        style={{ backgroundColor: color }}
                      />
                      <span>{color}</span>
                      <button
                        type="button"
                        onClick={() => handleRemoveColor(idx)}
                        className="text-erp-text-muted hover:text-red-500 ml-1 p-0.5"
                        title={t("actions.removeColor")}
                      >
                        <IconClose size={12} />
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {/* Sort Order */}
          <div className="erp-form-group">
            <label className="erp-label">{t("fields.sortOrder")}</label>
            <Input
              type="number"
              value={editorState.sortOrder}
              onChange={(e) =>
                setEditorState((prev) => ({
                  ...prev,
                  sortOrder: parseInt(e.target.value, 10) || 0,
                }))
              }
              placeholder={t("fields.sortOrderPlaceholder")}
              disabled={isSaving}
              className="h-11 min-h-[44px]"
            />
          </div>

          {/* Is Active */}
          <div className="pt-2">
            <Checkbox
              id="survey-master-data-active"
              checked={editorState.isActive}
              onChange={(e) =>
                setEditorState((prev) => ({ ...prev, isActive: e.target.checked }))
              }
              label={t("fields.isActive")}
              disabled={isSaving}
            />
          </div>
        </form>
      </Drawer>

      {/* Drawer Gallery Lightbox Modal */}
      {drawerLightboxItems.length > 0 && (
        <GalleryLightboxModal
          isOpen={isDrawerLightboxOpen}
          onClose={() => setIsDrawerLightboxOpen(false)}
          items={drawerLightboxItems}
          currentIndex={drawerLightboxIndex}
          onIndexChange={setDrawerLightboxIndex}
          title={editorState.nameTh || editorState.code}
        />
      )}
    </div>
  );
}
