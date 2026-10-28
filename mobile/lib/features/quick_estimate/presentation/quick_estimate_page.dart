import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_bottom_action_bar.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_confirm_dialog.dart';
import 'package:tan_erp_field/core/ui/erp_measure_display.dart';
import 'package:tan_erp_field/core/ui/erp_mono_loading.dart';
import 'package:tan_erp_field/core/ui/erp_page_padding.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/core/ui/erp_select.dart';
import 'package:tan_erp_field/features/quick_estimate/data/field_quick_estimate_repository.dart';
import 'package:tan_erp_field/features/quick_estimate/domain/field_quick_estimate_models.dart';
import 'package:tan_erp_field/features/survey/domain/field_survey_models.dart';

/// On-site quick estimate calculation, range display, share, and conversion.
class QuickEstimatePage extends ConsumerStatefulWidget {
  const QuickEstimatePage({required this.surveyRecord, super.key});

  final FieldSurveyRecord surveyRecord;

  @override
  ConsumerState<QuickEstimatePage> createState() => _QuickEstimatePageState();
}

class _QuickEstimatePageState extends ConsumerState<QuickEstimatePage> {
  FieldQuickEstimateRecord? _record;
  bool _loading = true;
  bool _saving = false;
  String? _successMessage;
  String _selectedPropertyType = 'condo';
  String _selectedGradeCode = 'standard';

  @override
  void initState() {
    super.initState();
    unawaited(_load());
  }

  Future<void> _load() async {
    setState(() => _loading = true);
    try {
      final item = await ref
          .read(fieldQuickEstimateRepositoryProvider)
          .loadOrCreateFromSurvey(widget.surveyRecord);
      if (!mounted) return;
      setState(() {
        _record = item;
        _selectedPropertyType = item.propertyType;
        _selectedGradeCode = item.gradeCode;
        _loading = false;
      });
    } on Object {
      if (mounted) setState(() => _loading = false);
    }
  }

  Future<void> _recalculate() async {
    if (_record == null || _saving) return;
    setState(() => _saving = true);
    try {
      final updated = await ref
          .read(fieldQuickEstimateRepositoryProvider)
          .recalculate(
            survey: widget.surveyRecord,
            current: _record!,
            propertyType: _selectedPropertyType,
            gradeCode: _selectedGradeCode,
          );
      if (!mounted) return;
      setState(() {
        _record = updated;
        _successMessage = null;
      });
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  Future<void> _recordShare(FieldQuickEstimateRecord record) async {
    final l10n = context.l10n;
    final locale = Localizations.localeOf(context).toLanguageTag();
    setState(() => _saving = true);
    try {
      final updated = await ref
          .read(fieldQuickEstimateRepositoryProvider)
          .recordCustomerShare(record, locale);
      if (!mounted) return;
      setState(() {
        _record = updated;
        _successMessage = l10n.quickEstimateShareSuccess;
      });
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  Future<void> _openShareSheet(FieldQuickEstimateRecord record) async {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final fmt = NumberFormat(
      '#,##0',
      Localizations.localeOf(context).toLanguageTag(),
    );
    final rangeText =
        '฿${fmt.format(record.displayedLower)} – '
        '฿${fmt.format(record.displayedUpper)}';

    await showModalBottomSheet<void>(
      context: context,
      isScrollControlled: true,
      backgroundColor: t.surface,
      shape: RoundedRectangleBorder(borderRadius: t.corner),
      builder: (sheetCtx) => Padding(
        padding: EdgeInsets.all(t.space4),
        child: SafeArea(
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(
                l10n.quickEstimateShareSheetTitle,
                style: text.titleMedium,
              ),
              SizedBox(height: t.space3),
              ErpCard(
                children: [
                  ErpMeasureDisplay(
                    label: l10n.quickEstimateRangeLabel,
                    valueText: rangeText,
                  ),
                  SizedBox(height: t.space2),
                  Text(
                    l10n.quickEstimateShareDisclaimer,
                    style: text.bodySmall?.copyWith(color: t.textMuted),
                  ),
                ],
              ),
              SizedBox(height: t.space4),
              ErpButton(
                label: l10n.quickEstimateShareConfirmAction,
                onPressed: () async {
                  Navigator.of(sheetCtx).pop();
                  await _recordShare(record);
                },
              ),
            ],
          ),
        ),
      ),
    );
  }

  Future<void> _onConvert(FieldQuickEstimateRecord record) async {
    final l10n = context.l10n;
    await showErpConfirmDialog(
      context,
      title: l10n.quickEstimateConvertConfirmTitle,
      message: l10n.quickEstimateConvertConfirmBody,
      confirmLabel: l10n.commonConfirm,
      cancelLabel: l10n.commonCancel,
      onConfirm: () async {
        setState(() => _saving = true);
        try {
          final updated = await ref
              .read(fieldQuickEstimateRepositoryProvider)
              .convertToOfficialDraft(record);
          if (!mounted) return;
          setState(() {
            _record = updated;
            _successMessage = l10n.quickEstimateConvertSuccess;
          });
        } finally {
          if (mounted) setState(() => _saving = false);
        }
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final fmt = NumberFormat(
      '#,##0',
      Localizations.localeOf(context).toLanguageTag(),
    );
    final fmtDec = NumberFormat(
      '#,##0.00',
      Localizations.localeOf(context).toLanguageTag(),
    );

    if (_loading || _record == null) {
      return ErpScreen(
        title: l10n.quickEstimateOpenButton,
        body: Center(child: ErpMonoLoading(size: t.space8)),
      );
    }

    final record = _record!;
    final rangeText =
        '฿${fmt.format(record.displayedLower)} – '
        '฿${fmt.format(record.displayedUpper)}';

    return ErpScreen(
      title: l10n.quickEstimateTitle(record.number),
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Expanded(
            child: ListView(
              padding: ErpPagePadding.of(context),
              children: [
                if (_successMessage != null) ...[
                  ErpAlert(
                    message: _successMessage!,
                    severity: ErpAlertSeverity.success,
                  ),
                  SizedBox(height: t.space3),
                ],
                ErpCard(
                  children: [
                    ErpSelect<String>(
                      label: l10n.quickEstimatePropertyTypeLabel,
                      placeholder: l10n.commonSelect,
                      value: _selectedPropertyType,
                      options: [
                        ErpOption(
                          value: 'condo',
                          label: l10n.quickEstimatePropertyCondo,
                        ),
                        ErpOption(
                          value: 'house',
                          label: l10n.quickEstimatePropertyHouse,
                        ),
                        ErpOption(
                          value: 'office',
                          label: l10n.quickEstimatePropertyOffice,
                        ),
                      ],
                      onChanged: (v) =>
                          setState(() => _selectedPropertyType = v),
                    ),
                    SizedBox(height: t.space3),
                    ErpSelect<String>(
                      label: l10n.quickEstimateGradeLabel,
                      placeholder: l10n.commonSelect,
                      value: _selectedGradeCode,
                      options: [
                        ErpOption(
                          value: 'standard',
                          label: l10n.quickEstimateGradeStandard,
                        ),
                        ErpOption(
                          value: 'premium',
                          label: l10n.quickEstimateGradePremium,
                        ),
                        ErpOption(
                          value: 'luxury',
                          label: l10n.quickEstimateGradeLuxury,
                        ),
                      ],
                      onChanged: (v) => setState(() => _selectedGradeCode = v),
                    ),
                    SizedBox(height: t.space4),
                    ErpButton(
                      label: l10n.quickEstimateCalculateAction,
                      variant: ErpButtonVariant.outline,
                      isLoading: _saving,
                      onPressed: _recalculate,
                    ),
                  ],
                ),
                SizedBox(height: t.space4),
                ErpCard(
                  children: [
                    ErpMeasureDisplay(
                      label: l10n.quickEstimateRangeLabel,
                      valueText: rangeText,
                    ),
                    SizedBox(height: t.space3),
                    _summaryRow(
                      label: l10n.quickEstimateNetAmountLabel,
                      value: '฿${fmt.format(record.netAmount)}',
                      text: text,
                      t: t,
                    ),
                    _summaryRow(
                      label: l10n.quickEstimateTaxAmountLabel,
                      value: '฿${fmt.format(record.taxAmount)}',
                      text: text,
                      t: t,
                    ),
                    _summaryRow(
                      label: l10n.quickEstimateValidUntilLabel(
                        record.validUntilText,
                      ),
                      value: '',
                      text: text,
                      t: t,
                    ),
                  ],
                ),
                SizedBox(height: t.space4),
                ErpCard(
                  title: l10n.quickEstimateLinesSection,
                  children: [
                    for (final line in record.lines) ...[
                      Padding(
                        padding: EdgeInsets.only(bottom: t.space2),
                        child: Row(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Expanded(
                              child: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    '${line.areaCode} · ${line.areaName}',
                                    style: text.bodyMedium,
                                  ),
                                  SizedBox(height: t.space1),
                                  Text(
                                    '${fmtDec.format(line.quantitySqm)} '
                                    '${l10n.surveyUnitSqm} @ '
                                    '฿${fmt.format(line.unitRate)}',
                                    style: text.bodySmall?.copyWith(
                                      color: t.textMuted,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                            Text(
                              '฿${fmt.format(line.lineTotal)}',
                              style: text.bodyMedium,
                            ),
                          ],
                        ),
                      ),
                    ],
                  ],
                ),
              ],
            ),
          ),
          ErpBottomActionBar(
            label: l10n.quickEstimateConvertAction,
            isLoading: _saving,
            onPressed: record.isConverted ? null : () => _onConvert(record),
            secondaryLabel: l10n.quickEstimateShareAction,
            onSecondaryPressed: () => _openShareSheet(record),
          ),
        ],
      ),
    );
  }

  static Widget _summaryRow({
    required String label,
    required String value,
    required TextTheme text,
    required ErpTokens t,
  }) {
    return Padding(
      padding: EdgeInsets.only(top: t.space2),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Expanded(
            child: Text(
              label,
              style: text.bodyMedium?.copyWith(color: t.textMuted),
            ),
          ),
          if (value.isNotEmpty) Text(value, style: text.bodyMedium),
        ],
      ),
    );
  }
}
