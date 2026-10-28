import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:image_picker/image_picker.dart';
import 'package:intl/intl.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/geometry/plane_measurement.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/offline/sync_controller.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_attachment_strip.dart';
import 'package:tan_erp_field/core/ui/erp_bottom_action_bar.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_confirm_dialog.dart';
import 'package:tan_erp_field/core/ui/erp_empty_state.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_measure_display.dart';
import 'package:tan_erp_field/core/ui/erp_number_pad_sheet.dart';
import 'package:tan_erp_field/core/ui/erp_page_padding.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/core/ui/erp_select.dart';
import 'package:tan_erp_field/core/ui/erp_status_badge.dart';
import 'package:tan_erp_field/core/ui/erp_text_field.dart';
import 'package:tan_erp_field/features/device_lab/data/ar_measure_repository.dart';
import 'package:tan_erp_field/features/device_lab/presentation/ar_measure_controller.dart';
import 'package:tan_erp_field/features/device_lab/presentation/photo_area_controller.dart';
import 'package:tan_erp_field/features/device_lab/presentation/photo_area_page.dart';
import 'package:tan_erp_field/features/quick_estimate/presentation/quick_estimate_page.dart';
import 'package:tan_erp_field/features/survey/domain/field_survey_models.dart';
import 'package:tan_erp_field/features/survey/presentation/survey_controller.dart';
import 'package:tan_erp_field/native/laser_meter/laser_meter_service.dart';
import 'package:uuid/uuid.dart';

/// Field survey workspace for recording areas and dimensions on site.
class SurveyWorkspacePage extends ConsumerStatefulWidget {
  const SurveyWorkspacePage({required this.initialRecord, super.key});

  final FieldSurveyRecord initialRecord;

  @override
  ConsumerState<SurveyWorkspacePage> createState() =>
      _SurveyWorkspacePageState();
}

class _SurveyWorkspacePageState extends ConsumerState<SurveyWorkspacePage> {
  static const _uuid = Uuid();

  late FieldSurveyRecord _record;
  late final FormControl<String> _scopeControl;
  int _selectedAreaIndex = 0;
  bool _saving = false;
  String? _bannerMessage;
  ErpAlertSeverity _bannerSeverity = ErpAlertSeverity.success;

  bool get _isReadOnly =>
      _record.status == 'ready' || _record.status == 'voided';

  @override
  void initState() {
    super.initState();
    _record = widget.initialRecord;
    _scopeControl = FormControl<String>(
      value: _record.scopeSummary,
      disabled: _isReadOnly,
    );
  }

  @override
  void dispose() {
    _scopeControl.dispose();
    super.dispose();
  }

  FieldSurveyRecord _currentRecord() => _record.copyWith(
    scopeSummary: (_scopeControl.value ?? '').trim(),
  );

  Future<void> _saveDraft() async {
    if (_saving || _isReadOnly) return;
    final l10n = context.l10n;
    setState(() {
      _saving = true;
      _bannerMessage = null;
    });
    try {
      final updated = await ref
          .read(surveyListProvider.notifier)
          .saveDraft(_currentRecord());
      final isOnline = ref.read(syncControllerProvider).isOnline;
      if (!mounted) return;
      setState(() {
        _record = updated;
        _bannerMessage = isOnline
            ? l10n.surveySavedOnline
            : l10n.surveySavedOffline;
        _bannerSeverity = isOnline
            ? ErpAlertSeverity.success
            : ErpAlertSeverity.info;
      });
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  Future<void> _confirmMarkReady() async {
    if (_saving || _isReadOnly) return;
    final l10n = context.l10n;
    await showErpConfirmDialog(
      context,
      title: l10n.surveyMarkReady,
      message: l10n.surveyMarkReadyConfirm,
      confirmLabel: l10n.commonConfirm,
      cancelLabel: l10n.commonCancel,
      onConfirm: () async {
        final draftSaved = await ref
            .read(surveyListProvider.notifier)
            .saveDraft(_currentRecord());
        final ready = await ref
            .read(surveyListProvider.notifier)
            .markReady(draftSaved);
        if (!mounted) return;
        setState(() {
          _record = ready;
          _scopeControl.markAsDisabled();
          _bannerMessage = l10n.surveySavedOnline;
          _bannerSeverity = ErpAlertSeverity.success;
        });
      },
    );
  }

  Future<void> _showAddAreaSheet() async {
    if (_isReadOnly) return;
    final l10n = context.l10n;
    final t = context.tokens;
    final nextIndex = _record.areas.length + 1;
    final defaultCode = 'A-${nextIndex.toString().padLeft(2, '0')}';
    final codeCtrl = FormControl<String>(value: defaultCode);
    final nameCtrl = FormControl<String>();
    final notesCtrl = FormControl<String>();

    final created = await showModalBottomSheet<FieldSurveyArea>(
      context: context,
      isScrollControlled: true,
      backgroundColor: t.surface,
      shape: RoundedRectangleBorder(borderRadius: t.corner),
      builder: (sheetCtx) => Padding(
        padding: EdgeInsets.only(
          left: t.space4,
          right: t.space4,
          top: t.space4,
          bottom: MediaQuery.viewInsetsOf(sheetCtx).bottom + t.space4,
        ),
        child: SafeArea(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(
                l10n.surveyAddArea,
                style: Theme.of(sheetCtx).textTheme.titleMedium,
              ),
              SizedBox(height: t.space3),
              ErpTextField(
                control: codeCtrl,
                label: l10n.surveyAreaCodeLabel,
                placeholder: l10n.surveyAreaCodePlaceholder,
              ),
              SizedBox(height: t.space3),
              ErpTextField(
                control: nameCtrl,
                label: l10n.surveyAreaNameLabel,
                placeholder: l10n.surveyAreaNamePlaceholder,
              ),
              SizedBox(height: t.space3),
              ErpTextField(
                control: notesCtrl,
                label: l10n.surveyAreaNotesLabel,
                placeholder: l10n.surveyAreaNotesPlaceholder,
              ),
              SizedBox(height: t.space4),
              Row(
                children: [
                  Expanded(
                    child: ErpButton(
                      label: l10n.commonCancel,
                      variant: ErpButtonVariant.outline,
                      onPressed: () => Navigator.of(sheetCtx).pop(),
                    ),
                  ),
                  SizedBox(width: t.space3),
                  Expanded(
                    child: ErpButton(
                      label: l10n.commonConfirm,
                      onPressed: () {
                        final rawName = (nameCtrl.value ?? '').trim();
                        final rawCode = (codeCtrl.value ?? '').trim();
                        final rawNotes = (notesCtrl.value ?? '').trim();
                        final code = rawCode.isEmpty ? defaultCode : rawCode;
                        final name = rawName.isEmpty ? code : rawName;
                        Navigator.of(sheetCtx).pop(
                          FieldSurveyArea(
                            id: _uuid.v4(),
                            code: code,
                            name: name,
                            description: rawNotes.isEmpty ? null : rawNotes,
                          ),
                        );
                      },
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    );

    codeCtrl.dispose();
    nameCtrl.dispose();
    notesCtrl.dispose();

    if (created != null && mounted) {
      setState(() {
        _record = _record.copyWith(areas: [..._record.areas, created]);
        _selectedAreaIndex = _record.areas.length - 1;
      });
    }
  }

  Future<void> _deleteArea(FieldSurveyArea area) async {
    if (_isReadOnly) return;
    final l10n = context.l10n;
    await showErpConfirmDialog(
      context,
      title: l10n.surveyDeleteArea,
      message: l10n.surveyDeleteAreaConfirm,
      confirmLabel: l10n.commonConfirm,
      cancelLabel: l10n.commonCancel,
      onConfirm: () async {
        if (!mounted) return;
        setState(() {
          final nextAreas =
              _record.areas.where((a) => a.id != area.id).toList();
          _record = _record.copyWith(areas: nextAreas);
          if (_selectedAreaIndex >= nextAreas.length) {
            _selectedAreaIndex = nextAreas.isEmpty ? 0 : nextAreas.length - 1;
          }
        });
      },
    );
  }

  void _addAttachment(int areaIndex, FieldAttachmentItem item) {
    if (_isReadOnly) return;
    setState(() {
      final areas = [..._record.areas];
      final area = areas[areaIndex];
      areas[areaIndex] = area.copyWith(
        attachments: [...area.attachments, item],
      );
      _record = _record.copyWith(areas: areas);
    });
  }

  void _removeAttachment(int areaIndex, String attachmentId) {
    if (_isReadOnly) return;
    setState(() {
      final areas = [..._record.areas];
      final area = areas[areaIndex];
      areas[areaIndex] = area.copyWith(
        attachments:
            area.attachments.where((a) => a.id != attachmentId).toList(),
      );
      _record = _record.copyWith(areas: areas);
    });
  }

  Future<void> _openMeasurePicker({
    required FieldSurveyArea area,
    FieldSurveyMeasurement? existing,
  }) async {
    if (_isReadOnly) return;
    final l10n = context.l10n;
    final t = context.tokens;

    var selectedType = existing?.measurementType ?? 'area';
    var selectedUnit = existing?.unitCode ?? 'sqm';
    final notesCtrl = FormControl<String>(value: existing?.notes ?? '');

    final method = await showModalBottomSheet<String>(
      context: context,
      isScrollControlled: true,
      backgroundColor: t.surface,
      shape: RoundedRectangleBorder(borderRadius: t.corner),
      builder: (sheetCtx) => StatefulBuilder(
        builder: (ctx, setSheetState) => Padding(
          padding: EdgeInsets.only(
            left: t.space4,
            right: t.space4,
            top: t.space4,
            bottom: MediaQuery.viewInsetsOf(ctx).bottom + t.space4,
          ),
          child: SafeArea(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Text(
                  '${area.code} · ${area.name}',
                  style: Theme.of(ctx).textTheme.titleMedium,
                ),
                SizedBox(height: t.space3),
                Row(
                  children: [
                    Expanded(
                      child: ErpSelect<String>(
                        label: l10n.surveyMeasurementsSection,
                        placeholder: l10n.commonSelect,
                        value: selectedType,
                        options: [
                          ErpOption(
                            value: 'area',
                            label: l10n.surveyMeasureTypeArea,
                          ),
                          ErpOption(
                            value: 'length',
                            label: l10n.surveyMeasureTypeLength,
                          ),
                          ErpOption(
                            value: 'width',
                            label: l10n.surveyMeasureTypeWidth,
                          ),
                          ErpOption(
                            value: 'height',
                            label: l10n.surveyMeasureTypeHeight,
                          ),
                          ErpOption(
                            value: 'count',
                            label: l10n.surveyMeasureTypeCount,
                          ),
                        ],
                        onChanged: (v) => setSheetState(() {
                          selectedType = v;
                          selectedUnit = switch (v) {
                            'area' => 'sqm',
                            'count' => 'pcs',
                            _ => 'm',
                          };
                        }),
                      ),
                    ),
                    SizedBox(width: t.space3),
                    Expanded(
                      child: ErpSelect<String>(
                        label: l10n.scanBaseUnit,
                        placeholder: l10n.commonSelect,
                        value: selectedUnit,
                        options: [
                          ErpOption(value: 'sqm', label: l10n.surveyUnitSqm),
                          ErpOption(value: 'm', label: l10n.surveyUnitM),
                          ErpOption(value: 'mm', label: l10n.surveyUnitMm),
                          ErpOption(value: 'pcs', label: l10n.surveyUnitPcs),
                        ],
                        onChanged: (v) => setSheetState(() => selectedUnit = v),
                      ),
                    ),
                  ],
                ),
                SizedBox(height: t.space3),
                ErpTextField(
                  control: notesCtrl,
                  label: l10n.surveyMeasureNotesLabel,
                  placeholder: l10n.surveyMeasureNotesPlaceholder,
                ),
                SizedBox(height: t.space4),
                ErpButton(
                  label: l10n.surveyMeasureByPad,
                  onPressed: () => Navigator.of(ctx).pop('pad'),
                ),
                SizedBox(height: t.space2),
                ErpButton(
                  label: l10n.surveyMeasureByLaser,
                  variant: ErpButtonVariant.outline,
                  onPressed: () => Navigator.of(ctx).pop('laser'),
                ),
                SizedBox(height: t.space2),
                ErpButton(
                  label: l10n.surveyMeasureByPhoto,
                  variant: ErpButtonVariant.outline,
                  onPressed: () => Navigator.of(ctx).pop('photo'),
                ),
                SizedBox(height: t.space2),
                ErpButton(
                  label: l10n.surveyMeasureByAr,
                  variant: ErpButtonVariant.outline,
                  onPressed: () => Navigator.of(ctx).pop('ar'),
                ),
              ],
            ),
          ),
        ),
      ),
    );

    final notesText = (notesCtrl.value ?? '').trim();
    notesCtrl.dispose();
    if (method == null || !mounted) return;

    final notes = notesText.isEmpty ? null : notesText;
    switch (method) {
      case 'pad':
        await _measureWithPad(
          area: area,
          existing: existing,
          measurementType: selectedType,
          unitCode: selectedUnit,
          notes: notes,
        );
      case 'laser':
        await _measureWithLaser(
          area: area,
          existing: existing,
          measurementType: selectedType,
          unitCode: selectedUnit,
          notes: notes,
        );
      case 'photo':
        await _measureWithPhoto(
          area: area,
          existing: existing,
          notes: notes,
        );
      case 'ar':
        await _measureWithAr(
          area: area,
          existing: existing,
          notes: notes,
        );
    }
  }

  Future<void> _measureWithPad({
    required FieldSurveyArea area,
    required String measurementType,
    required String unitCode,
    FieldSurveyMeasurement? existing,
    String? notes,
  }) async {
    final l10n = context.l10n;
    final val = await showErpNumberPadSheet(
      context,
      title: _typeLabel(l10n, measurementType),
      unitLabel: _unitLabel(l10n, unitCode),
      initialValue: existing?.value,
    );
    if (val == null || !mounted) return;
    _upsertMeasurement(
      area: area,
      existingId: existing?.id,
      item: FieldSurveyMeasurement(
        id: existing?.id ?? _uuid.v4(),
        measurementType: measurementType,
        value: val,
        unitCode: unitCode,
        captureMethod: 'measured',
        notes: notes,
      ),
    );
  }

  Future<void> _measureWithLaser({
    required FieldSurveyArea area,
    required String measurementType,
    required String unitCode,
    FieldSurveyMeasurement? existing,
    String? notes,
  }) async {
    final meters = await showLaserMeterSheet(context);
    if (meters == null || !mounted) return;
    final converted = switch (unitCode) {
      'mm' => meters * 1000.0,
      _ => meters,
    };
    final resolvedUnit = switch (unitCode) {
      'mm' => 'mm',
      'sqm' || 'pcs' => 'm',
      _ => 'm',
    };
    final resolvedType = switch (measurementType) {
      'area' || 'count' => 'length',
      _ => measurementType,
    };
    final rounded = double.parse(converted.toStringAsFixed(2));
    _upsertMeasurement(
      area: area,
      existingId: existing?.id,
      item: FieldSurveyMeasurement(
        id: existing?.id ?? _uuid.v4(),
        measurementType: resolvedType,
        value: rounded,
        unitCode: resolvedUnit,
        captureMethod: 'laser_measured',
        notes: notes,
      ),
    );
  }

  Future<void> _measureWithPhoto({
    required FieldSurveyArea area,
    FieldSurveyMeasurement? existing,
    String? notes,
  }) async {
    final l10n = context.l10n;
    final navigator = Navigator.of(context);
    try {
      final file = await ImagePicker().pickImage(source: ImageSource.gallery);
      if (file == null) return;
      final image = await decodeImageFromList(await file.readAsBytes());
      if (!mounted) {
        image.dispose();
        return;
      }
      ref.read(photoAreaControllerProvider.notifier).clear();
      final result = await navigator.push<PlaneMeasurementResult>(
        MaterialPageRoute(
          builder: (_) => PhotoAreaPage(
            image: image,
            referenceSize: const ReferenceSizeMm(widthMm: 210, heightMm: 297),
          ),
        ),
      );
      image.dispose();
      if (result == null || !mounted) return;
      _upsertMeasurement(
        area: area,
        existingId: existing?.id,
        item: FieldSurveyMeasurement(
          id: existing?.id ?? _uuid.v4(),
          measurementType: 'area',
          value: double.parse(result.areaSqm.toStringAsFixed(2)),
          unitCode: 'sqm',
          captureMethod: 'photo_estimated',
          notes: notes,
        ),
      );
    } on Exception {
      if (!mounted) return;
      setState(() {
        _bannerMessage = l10n.photoAreaErrorPick;
        _bannerSeverity = ErpAlertSeverity.danger;
      });
    }
  }

  Future<void> _measureWithAr({
    required FieldSurveyArea area,
    FieldSurveyMeasurement? existing,
    String? notes,
  }) async {
    final req = _buildArRequest(context);
    await ref.read(arMeasureControllerProvider.notifier).measure(req);
    if (!mounted) return;
    final state = ref.read(arMeasureControllerProvider);
    final result = state.result;
    if (result == null) return;
    _upsertMeasurement(
      area: area,
      existingId: existing?.id,
      item: FieldSurveyMeasurement(
        id: existing?.id ?? _uuid.v4(),
        measurementType: 'area',
        value: double.parse(result.areaSqm.toStringAsFixed(2)),
        unitCode: 'sqm',
        captureMethod: 'measured',
        notes: notes,
      ),
    );
  }

  void _upsertMeasurement({
    required FieldSurveyArea area,
    required FieldSurveyMeasurement item,
    String? existingId,
  }) {
    setState(() {
      final updatedAreas = <FieldSurveyArea>[];
      for (final a in _record.areas) {
        if (a.id != area.id) {
          updatedAreas.add(a);
          continue;
        }
        final list = <FieldSurveyMeasurement>[];
        var replaced = false;
        for (final m in a.measurements) {
          if (existingId != null && m.id == existingId) {
            list.add(item);
            replaced = true;
          } else {
            list.add(m);
          }
        }
        if (!replaced) list.add(item);
        updatedAreas.add(a.copyWith(measurements: list));
      }
      _record = _record.copyWith(areas: updatedAreas);
    });
  }

  void _deleteMeasurement({
    required FieldSurveyArea area,
    required String measurementId,
  }) {
    if (_isReadOnly) return;
    setState(() {
      final updatedAreas = <FieldSurveyArea>[];
      for (final a in _record.areas) {
        if (a.id != area.id) {
          updatedAreas.add(a);
          continue;
        }
        updatedAreas.add(
          a.copyWith(
            measurements:
                a.measurements.where((m) => m.id != measurementId).toList(),
          ),
        );
      }
      _record = _record.copyWith(areas: updatedAreas);
    });
  }

  static ArMeasureRequest _buildArRequest(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    return ArMeasureRequest(
      strings: ArMeasureStrings(
        localeTag: Localizations.localeOf(context).toLanguageTag(),
        searchingPlane: l10n.arSearchingPlane,
        tapToPlace: l10n.arTapToPlace,
        addMorePoints: l10n.arAddMorePoints,
        readyToClose: l10n.arReadyToClose,
        closedHint: l10n.arClosedHint,
        trackingLost: l10n.arTrackingLost,
        notOnPlane: l10n.arNotOnPlane,
        selfIntersecting: l10n.arSelfIntersecting,
        areaLabel: l10n.arResultArea,
        unitMeter: l10n.arUnitMeter,
        unitSquareMeter: l10n.arUnitSquareMeter,
        undo: l10n.arUndo,
        closeShape: l10n.arCloseShape,
        done: l10n.arDone,
        cancel: l10n.commonCancel,
      ),
      colors: ArMeasureColors(
        surface: t.surface.toARGB32(),
        textMain: t.textMain.toARGB32(),
        textMuted: t.textMuted.toARGB32(),
        action: t.action.toARGB32(),
        onAction: t.onAction.toARGB32(),
        accentMeasure: t.accentMeasure.toARGB32(),
        danger: t.danger.toARGB32(),
        borderStrong: t.borderStrong.toARGB32(),
        disabledBackground: t.surfaceSubtle.toARGB32(),
        disabledForeground: t.textSubtle.toARGB32(),
      ),
    );
  }

  static String _typeLabel(AppLocalizations l10n, String type) =>
      switch (type) {
        'area' => l10n.surveyMeasureTypeArea,
        'length' => l10n.surveyMeasureTypeLength,
        'width' => l10n.surveyMeasureTypeWidth,
        'height' => l10n.surveyMeasureTypeHeight,
        'count' => l10n.surveyMeasureTypeCount,
        _ => l10n.surveyMeasureTypeArea,
      };

  static String _unitLabel(AppLocalizations l10n, String unit) =>
      switch (unit) {
        'sqm' => l10n.surveyUnitSqm,
        'm' => l10n.surveyUnitM,
        'mm' => l10n.surveyUnitMm,
        'pcs' => l10n.surveyUnitPcs,
        _ => unit,
      };

  static String _captureLabel(AppLocalizations l10n, String method) =>
      switch (method) {
        'photo_estimated' => l10n.surveyCapturePhoto,
        'customer_provided' => l10n.surveyCaptureCustomer,
        'derived' => l10n.surveyCaptureAr,
        'laser_measured' => l10n.surveyCaptureLaser,
        _ => l10n.surveyCaptureMeasured,
      };

  static (String, ErpBadgeTone) _statusBadge(
    AppLocalizations l10n,
    String status,
  ) => switch (status) {
    'ready' => (l10n.surveyStatusReady, ErpBadgeTone.success),
    'scheduled' => (l10n.surveyStatusScheduled, ErpBadgeTone.info),
    'voided' => (l10n.surveyStatusVoided, ErpBadgeTone.danger),
    _ => (l10n.surveyStatusDraft, ErpBadgeTone.warning),
  };

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final (statusText, statusTone) = _statusBadge(l10n, _record.status);
    final fmt = NumberFormat(
      '#,##0.##',
      Localizations.localeOf(context).toLanguageTag(),
    );

    final isTablet = MediaQuery.sizeOf(context).width >= 768;
    final hasAreas = _record.areas.isNotEmpty;
    final safeAreaIndex = hasAreas
        ? _selectedAreaIndex.clamp(0, _record.areas.length - 1)
        : 0;
    final activeArea = hasAreas ? _record.areas[safeAreaIndex] : null;

    final projectCard = ErpCard(
      children: [
        Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Text(
                '${_record.opportunityCode} · ${_record.opportunityTitle}',
                style: text.titleMedium,
              ),
            ),
            SizedBox(width: t.space2),
            ErpStatusBadge(label: statusText, tone: statusTone),
          ],
        ),
        SizedBox(height: t.space2),
        Text(
          '${_record.customerName} · '
          '${_record.siteName.isEmpty ? l10n.todayNoSite : _record.siteName}',
          style: text.bodyMedium?.copyWith(color: t.textMuted),
        ),
        SizedBox(height: t.space3),
        ErpTextField(
          control: _scopeControl,
          label: l10n.surveyScopeLabel,
          placeholder: l10n.surveyScopePlaceholder,
        ),
        SizedBox(height: t.space3),
        ErpButton(
          label: l10n.quickEstimateOpenButton,
          variant: ErpButtonVariant.outline,
          onPressed: () => Navigator.of(context).push(
            MaterialPageRoute<void>(
              builder: (_) => QuickEstimatePage(surveyRecord: _record),
            ),
          ),
        ),
      ],
    );

    return ErpScreen(
      title: l10n.surveyDetailTitle(_record.surveyNumber),
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Expanded(
            child: isTablet
                ? _buildTabletWorkspace(
                    context: context,
                    projectCard: projectCard,
                    activeArea: activeArea,
                    safeAreaIndex: safeAreaIndex,
                    numberFormat: fmt,
                  )
                : _buildMobileWorkspace(
                    context: context,
                    projectCard: projectCard,
                    activeArea: activeArea,
                    safeAreaIndex: safeAreaIndex,
                    numberFormat: fmt,
                  ),
          ),
          if (!_isReadOnly)
            ErpBottomActionBar(
              label: l10n.surveySaveDraft,
              isLoading: _saving,
              onPressed: _saveDraft,
              secondaryLabel: l10n.surveyMarkReady,
              onSecondaryPressed: _confirmMarkReady,
            ),
        ],
      ),
    );
  }

  Widget _buildMobileWorkspace({
    required BuildContext context,
    required Widget projectCard,
    required FieldSurveyArea? activeArea,
    required int safeAreaIndex,
    required NumberFormat numberFormat,
  }) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;

    return ListView(
      padding: ErpPagePadding.of(context),
      children: [
        if (_bannerMessage != null) ...[
          ErpAlert(
            message: _bannerMessage!,
            severity: _bannerSeverity,
          ),
          SizedBox(height: t.space3),
        ],
        projectCard,
        SizedBox(height: t.space4),
        Row(
          children: [
            Expanded(
              child: Text(
                l10n.surveyAreasSection,
                style: text.titleMedium,
              ),
            ),
            if (!_isReadOnly)
              ErpButton(
                label: l10n.surveyAddArea,
                variant: ErpButtonVariant.outline,
                expand: false,
                onPressed: _showAddAreaSheet,
              ),
          ],
        ),
        SizedBox(height: t.space3),
        if (_record.areas.isEmpty)
          ErpEmptyState(
            icon: ErpIcons.ruler,
            title: l10n.surveyNoAreas,
            actionLabel: _isReadOnly ? null : l10n.surveyAddArea,
            onAction: _isReadOnly ? null : _showAddAreaSheet,
          )
        else ...[
          _RoomThumbStrip(
            areas: _record.areas,
            selectedIndex: safeAreaIndex,
            onSelect: (idx) => setState(() => _selectedAreaIndex = idx),
            onAddArea: _isReadOnly ? null : _showAddAreaSheet,
          ),
          SizedBox(height: t.space3),
          if (activeArea != null)
            _ActiveAreaFocusCard(
              area: activeArea,
              isReadOnly: _isReadOnly,
              numberFormat: numberFormat,
              onAddOrEditMeasurement: (m) => _openMeasurePicker(
                area: activeArea,
                existing: m,
              ),
              onDeleteMeasurement: (mId) => _deleteMeasurement(
                area: activeArea,
                measurementId: mId,
              ),
              onMeasureWithPad: () => _measureWithPad(
                area: activeArea,
                measurementType: 'area',
                unitCode: 'sqm',
              ),
              onMeasureWithLaser: () => _measureWithLaser(
                area: activeArea,
                measurementType: 'length',
                unitCode: 'm',
              ),
              onMeasureWithPhoto: () => _measureWithPhoto(area: activeArea),
              onMeasureWithAr: () => _measureWithAr(area: activeArea),
              onDeleteArea: () => _deleteArea(activeArea),
              onAttachmentAdded: (item) => _addAttachment(safeAreaIndex, item),
              onAttachmentRemoved: (id) =>
                  _removeAttachment(safeAreaIndex, id),
            ),
        ],
      ],
    );
  }

  Widget _buildTabletWorkspace({
    required BuildContext context,
    required Widget projectCard,
    required FieldSurveyArea? activeArea,
    required int safeAreaIndex,
    required NumberFormat numberFormat,
  }) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;

    return Padding(
      padding: ErpPagePadding.of(context),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          if (_bannerMessage != null) ...[
            ErpAlert(
              message: _bannerMessage!,
              severity: _bannerSeverity,
            ),
            SizedBox(height: t.space3),
          ],
          Expanded(
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Master Left Pane (Width 360dp)
                SizedBox(
                  width: 360,
                  child: ListView(
                    children: [
                      projectCard,
                      SizedBox(height: t.space4),
                      Row(
                        children: [
                          Expanded(
                            child: Text(
                              l10n.surveyAreasSection,
                              style: text.titleMedium,
                            ),
                          ),
                          if (!_isReadOnly)
                            ErpButton(
                              label: l10n.surveyAddArea,
                              variant: ErpButtonVariant.outline,
                              expand: false,
                              onPressed: _showAddAreaSheet,
                            ),
                        ],
                      ),
                      SizedBox(height: t.space3),
                      if (_record.areas.isEmpty)
                        ErpEmptyState(
                          icon: ErpIcons.ruler,
                          title: l10n.surveyNoAreas,
                          actionLabel: _isReadOnly ? null : l10n.surveyAddArea,
                          onAction: _isReadOnly ? null : _showAddAreaSheet,
                        )
                      else
                        for (var i = 0; i < _record.areas.length; i++) ...[
                          _TabletAreaListItem(
                            area: _record.areas[i],
                            isSelected: i == safeAreaIndex,
                            onTap: () => setState(() => _selectedAreaIndex = i),
                          ),
                          SizedBox(height: t.space2),
                        ],
                    ],
                  ),
                ),
                SizedBox(width: t.space4),
                // Detail Right Inspector Pane
                Expanded(
                  child: activeArea == null
                      ? Center(
                          child: Text(
                            l10n.surveySelectArea,
                            style: text.bodyLarge?.copyWith(
                              color: t.textMuted,
                            ),
                          ),
                        )
                      : ListView(
                          children: [
                            _ActiveAreaFocusCard(
                              area: activeArea,
                              isReadOnly: _isReadOnly,
                              numberFormat: numberFormat,
                              onAddOrEditMeasurement: (m) => _openMeasurePicker(
                                area: activeArea,
                                existing: m,
                              ),
                              onDeleteMeasurement: (mId) => _deleteMeasurement(
                                area: activeArea,
                                measurementId: mId,
                              ),
                              onMeasureWithPad: () => _measureWithPad(
                                area: activeArea,
                                measurementType: 'area',
                                unitCode: 'sqm',
                              ),
                              onMeasureWithLaser: () => _measureWithLaser(
                                area: activeArea,
                                measurementType: 'length',
                                unitCode: 'm',
                              ),
                              onMeasureWithPhoto: () =>
                                  _measureWithPhoto(area: activeArea),
                              onMeasureWithAr: () =>
                                  _measureWithAr(area: activeArea),
                              onDeleteArea: () => _deleteArea(activeArea),
                              onAttachmentAdded: (item) =>
                                  _addAttachment(safeAreaIndex, item),
                              onAttachmentRemoved: (id) =>
                                  _removeAttachment(safeAreaIndex, id),
                            ),
                          ],
                        ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

/// Horizontal room thumb strip for one-handed thumb navigation.
class _RoomThumbStrip extends StatelessWidget {
  const _RoomThumbStrip({
    required this.areas,
    required this.selectedIndex,
    required this.onSelect,
    required this.onAddArea,
  });

  final List<FieldSurveyArea> areas;
  final int selectedIndex;
  final ValueChanged<int> onSelect;
  final VoidCallback? onAddArea;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final text = Theme.of(context).textTheme;

    return SizedBox(
      height: 72,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        itemCount: areas.length + (onAddArea != null ? 1 : 0),
        separatorBuilder: (_, _) => SizedBox(width: t.space2),
        itemBuilder: (ctx, index) {
          if (index == areas.length) {
            return InkWell(
              onTap: onAddArea,
              borderRadius: t.corner,
              child: Container(
                width: 72,
                decoration: BoxDecoration(
                  border: Border.all(color: t.border),
                  color: t.surfaceSubtle,
                ),
                child: Center(
                  child: ErpIcon(ErpIcons.plus, color: t.action),
                ),
              ),
            );
          }

          final area = areas[index];
          final isSelected = index == selectedIndex;
          final measureCount = area.measurements.length;

          return InkWell(
            onTap: () => onSelect(index),
            borderRadius: t.corner,
            child: Container(
              width: 140,
              padding: EdgeInsets.symmetric(
                horizontal: t.space3,
                vertical: t.space2,
              ),
              decoration: BoxDecoration(
                color: isSelected ? t.surface : t.surfaceSubtle,
                border: Border.all(
                  color: isSelected ? t.action : t.border,
                  width: isSelected ? 2 : 1,
                ),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Row(
                    children: [
                      Text(
                        area.code,
                        style: text.labelSmall?.copyWith(
                          fontWeight: FontWeight.bold,
                          color: isSelected ? t.action : t.textMuted,
                        ),
                      ),
                      const Spacer(),
                      Text(
                        '$measureCount',
                        style: text.labelSmall?.copyWith(
                          color: t.textMuted,
                        ),
                      ),
                    ],
                  ),
                  SizedBox(height: t.space1),
                  Text(
                    area.name,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: text.bodySmall?.copyWith(
                      fontWeight:
                          isSelected ? FontWeight.bold : FontWeight.normal,
                      color: t.textMain,
                    ),
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}

/// Left pane item for Tablet Split View.
class _TabletAreaListItem extends StatelessWidget {
  const _TabletAreaListItem({
    required this.area,
    required this.isSelected,
    required this.onTap,
  });

  final FieldSurveyArea area;
  final bool isSelected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final text = Theme.of(context).textTheme;

    return InkWell(
      onTap: onTap,
      borderRadius: t.corner,
      child: Container(
        padding: EdgeInsets.symmetric(
          horizontal: t.space3,
          vertical: t.space3,
        ),
        decoration: BoxDecoration(
          color: isSelected ? t.surface : t.surfaceSubtle,
          border: Border.all(
            color: isSelected ? t.action : t.border,
            width: isSelected ? 2 : 1,
          ),
        ),
        child: Row(
          children: [
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    area.code,
                    style: text.labelSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                      color: isSelected ? t.action : t.textMuted,
                    ),
                  ),
                  SizedBox(height: t.space1),
                  Text(
                    area.name,
                    style: text.bodyMedium?.copyWith(
                      fontWeight:
                          isSelected ? FontWeight.bold : FontWeight.normal,
                    ),
                  ),
                ],
              ),
            ),
            Text(
              '${area.measurements.length}',
              style: text.labelMedium?.copyWith(color: t.textMuted),
            ),
          ],
        ),
      ),
    );
  }
}

/// Detail card for the active/selected room containing Hero 44sp measure,
/// Quick Instrument Bar, Breakdown list, and Attachments.
class _ActiveAreaFocusCard extends StatelessWidget {
  const _ActiveAreaFocusCard({
    required this.area,
    required this.isReadOnly,
    required this.numberFormat,
    required this.onAddOrEditMeasurement,
    required this.onDeleteMeasurement,
    required this.onMeasureWithPad,
    required this.onMeasureWithLaser,
    required this.onMeasureWithPhoto,
    required this.onMeasureWithAr,
    required this.onDeleteArea,
    required this.onAttachmentAdded,
    required this.onAttachmentRemoved,
  });

  final FieldSurveyArea area;
  final bool isReadOnly;
  final NumberFormat numberFormat;
  final ValueChanged<FieldSurveyMeasurement?> onAddOrEditMeasurement;
  final ValueChanged<String> onDeleteMeasurement;
  final VoidCallback onMeasureWithPad;
  final VoidCallback onMeasureWithLaser;
  final VoidCallback onMeasureWithPhoto;
  final VoidCallback onMeasureWithAr;
  final VoidCallback onDeleteArea;
  final ValueChanged<FieldAttachmentItem> onAttachmentAdded;
  final ValueChanged<String> onAttachmentRemoved;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;

    final primaryMeasurement = area.measurements.isNotEmpty
        ? area.measurements.first
        : null;

    return ErpCard(
      children: [
        Row(
          children: [
            Expanded(
              child: Text(
                '${area.code} · ${area.name}',
                style: text.titleMedium,
              ),
            ),
            if (!isReadOnly)
              IconButton(
                tooltip: l10n.surveyDeleteArea,
                onPressed: onDeleteArea,
                icon: ErpIcon(ErpIcons.trash, color: t.danger, size: 20),
              ),
          ],
        ),
        if (area.description != null && area.description!.isNotEmpty) ...[
          SizedBox(height: t.space1),
          Text(
            area.description!,
            style: text.bodySmall?.copyWith(color: t.textMuted),
          ),
        ],
        SizedBox(height: t.space3),

        // Hero 44sp Measure Display with Oak dimension underline
        ErpMeasureDisplay(
          label: primaryMeasurement != null
              ? _SurveyWorkspacePageState._typeLabel(
                  l10n,
                  primaryMeasurement.measurementType,
                )
              : l10n.surveyMeasurementsSection,
          valueText: primaryMeasurement != null
              ? '${numberFormat.format(primaryMeasurement.value)} '
                  '${_SurveyWorkspacePageState._unitLabel(
                  l10n,
                  primaryMeasurement.unitCode,
                )}'
              : '-  ',
          subLabel: primaryMeasurement != null
              ? _SurveyWorkspacePageState._captureLabel(
                  l10n,
                  primaryMeasurement.captureMethod,
                )
              : l10n.surveyAddMeasurement,
          onTap: isReadOnly
              ? null
              : () => onAddOrEditMeasurement(primaryMeasurement),
        ),
        SizedBox(height: t.space3),

        // Quick Instrument Bar (4-key instrument row)
        if (!isReadOnly) ...[
          _QuickInstrumentBar(
            onPad: onMeasureWithPad,
            onLaser: onMeasureWithLaser,
            onPhoto: onMeasureWithPhoto,
            onAr: onMeasureWithAr,
          ),
          SizedBox(height: t.space4),
        ],

        // Breakdown Measurements List
        Row(
          children: [
            Expanded(
              child: Text(
                l10n.surveyBreakdownMeasurements,
                style: text.titleSmall?.copyWith(fontWeight: FontWeight.bold),
              ),
            ),
            if (!isReadOnly)
              InkWell(
                onTap: () => onAddOrEditMeasurement(null),
                child: Padding(
                  padding: EdgeInsets.symmetric(
                    horizontal: t.space2,
                    vertical: t.space1,
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      ErpIcon(ErpIcons.plus, color: t.action, size: 16),
                      SizedBox(width: t.space1),
                      Text(
                        l10n.surveyAddMeasurement,
                        style: text.labelSmall?.copyWith(
                          color: t.action,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
          ],
        ),
        SizedBox(height: t.space2),
        if (area.measurements.isEmpty)
          Padding(
            padding: EdgeInsets.symmetric(vertical: t.space2),
            child: Text(
              l10n.surveyAddMeasurement,
              style: text.bodySmall?.copyWith(color: t.textMuted),
            ),
          )
        else
          for (final m in area.measurements) ...[
            Container(
              margin: EdgeInsets.only(bottom: t.space2),
              padding: EdgeInsets.symmetric(
                horizontal: t.space3,
                vertical: t.space2,
              ),
              decoration: BoxDecoration(
                color: t.surfaceSubtle,
                border: Border.all(color: t.border),
              ),
              child: Row(
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          _SurveyWorkspacePageState._typeLabel(
                            l10n,
                            m.measurementType,
                          ),
                          style: text.labelSmall?.copyWith(color: t.textMuted),
                        ),
                        SizedBox(height: t.space1),
                        Text(
                          '${numberFormat.format(m.value)} '
                          '${_SurveyWorkspacePageState._unitLabel(
                            l10n,
                            m.unitCode,
                          )}',
                          style: text.titleSmall?.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        if (m.notes != null && m.notes!.isNotEmpty) ...[
                          SizedBox(height: t.space1),
                          Text(
                            m.notes!,
                            style: text.bodySmall?.copyWith(color: t.textMuted),
                          ),
                        ],
                      ],
                    ),
                  ),
                  ErpStatusBadge(
                    label: _SurveyWorkspacePageState._captureLabel(
                      l10n,
                      m.captureMethod,
                    ),
                    tone: switch (m.captureMethod) {
                      'photo_estimated' => ErpBadgeTone.info,
                      'laser_measured' => ErpBadgeTone.success,
                      _ => ErpBadgeTone.neutral,
                    },
                  ),
                  if (!isReadOnly) ...[
                    IconButton(
                      icon: ErpIcon(ErpIcons.trash, color: t.danger, size: 18),
                      tooltip: l10n.surveyDeleteArea,
                      onPressed: () => onDeleteMeasurement(m.id),
                    ),
                  ],
                ],
              ),
            ),
          ],
        SizedBox(height: t.space3),

        // Attachments
        ErpAttachmentStrip(
          items: area.attachments,
          isReadOnly: isReadOnly,
          onAdded: onAttachmentAdded,
          onRemoved: (item) => onAttachmentRemoved(item.id),
        ),
      ],
    );
  }
}

/// 4-key quick instrument bar for immediate measurement input.
class _QuickInstrumentBar extends StatelessWidget {
  const _QuickInstrumentBar({
    required this.onPad,
    required this.onLaser,
    required this.onPhoto,
    required this.onAr,
  });

  final VoidCallback onPad;
  final VoidCallback onLaser;
  final VoidCallback onPhoto;
  final VoidCallback onAr;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;

    return Row(
      children: [
        Expanded(
          child: _InstrumentKey(
            icon: ErpIcons.calculator,
            label: l10n.surveyMeasureByPad,
            onTap: onPad,
          ),
        ),
        SizedBox(width: t.space2),
        Expanded(
          child: _InstrumentKey(
            icon: ErpIcons.ruler,
            label: l10n.surveyMeasureByLaser,
            onTap: onLaser,
          ),
        ),
        SizedBox(width: t.space2),
        Expanded(
          child: _InstrumentKey(
            icon: ErpIcons.camera,
            label: l10n.surveyMeasureByPhoto,
            onTap: onPhoto,
          ),
        ),
        SizedBox(width: t.space2),
        Expanded(
          child: _InstrumentKey(
            icon: ErpIcons.box,
            label: l10n.surveyMeasureByAr,
            onTap: onAr,
          ),
        ),
      ],
    );
  }
}

class _InstrumentKey extends StatelessWidget {
  const _InstrumentKey({
    required this.icon,
    required this.label,
    required this.onTap,
  });

  final ErpIcons icon;
  final String label;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final text = Theme.of(context).textTheme;

    return InkWell(
      onTap: onTap,
      borderRadius: t.corner,
      child: Container(
        height: 60,
        padding: EdgeInsets.symmetric(
          horizontal: t.space1,
          vertical: t.space2,
        ),
        decoration: BoxDecoration(
          color: t.surfaceSubtle,
          border: Border.all(color: t.border),
        ),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            ErpIcon(icon, color: t.action, size: 20),
            SizedBox(height: t.space1),
            Text(
              label,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              textAlign: TextAlign.center,
              style: text.labelSmall?.copyWith(
                fontWeight: FontWeight.bold,
                color: t.textMain,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
