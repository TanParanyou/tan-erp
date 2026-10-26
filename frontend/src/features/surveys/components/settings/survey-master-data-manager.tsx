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
} from "@/components/common/Icons";
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

type MasterDataTab = "design-styles" | "mood-tones";

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
    updateToneMutation.isPending;

  // Open drawer for creating
  const handleAddNew = () => {
    setEditorState(initialEditorState);
    setValidationError(null);
    setIsDrawerOpen(true);
  };

  // Open drawer for editing design style
  const handleEditStyle = (item: SurveyDesignStyleResponse) => {
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
    });
    setValidationError(null);
    setIsDrawerOpen(true);
  };

  // Open drawer for editing mood tone
  const handleEditTone = (item: SurveyMoodToneResponse) => {
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
    });
    setValidationError(null);
    setIsDrawerOpen(true);
  };

  const handleCloseDrawer = () => {
    if (isSaving) return;
    setIsDrawerOpen(false);
    setEditorState(initialEditorState);
    setValidationError(null);
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
        if (editorState.id) {
          await updateStyleMutation.mutateAsync({
            id: editorState.id,
            payload: {
              name: namePayload,
              description: descriptionPayload,
              sortOrder: editorState.sortOrder,
              isActive: editorState.isActive,
            },
          });
          toast.success(t("messages.updateSuccess"));
        } else {
          await createStyleMutation.mutateAsync({
            code: editorState.code.trim(),
            name: namePayload,
            description: descriptionPayload,
            sortOrder: editorState.sortOrder,
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
        </nav>
      </div>

      {/* Table Preserved Layout */}
      {activeTab === "design-styles" ? (
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

      {/* Drawer for Create / Edit */}
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
              variant="outline"
              size="sm"
              onClick={handleCloseDrawer}
              disabled={isSaving}
            >
              {t("actions.cancel")}
            </Button>
            <Button
              variant="primary"
              size="sm"
              onClick={handleSave}
              isLoading={isSaving}
              disabled={isSaving}
            >
              {t("actions.save")}
            </Button>
          </div>
        }
      >
        <form onSubmit={handleSave} className="flex flex-col gap-4">
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
                setEditorState((prev) => ({ ...prev, code: e.target.value }))
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
                >
                  {t("actions.addColor")}
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
    </div>
  );
}