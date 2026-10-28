import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_empty_state.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_mono_loading.dart';
import 'package:tan_erp_field/core/ui/erp_page_padding.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/core/ui/erp_status_badge.dart';
import 'package:tan_erp_field/core/ui/erp_text_field.dart';
import 'package:tan_erp_field/features/survey/domain/field_survey_models.dart';
import 'package:tan_erp_field/features/survey/presentation/survey_controller.dart';
import 'package:tan_erp_field/features/survey/presentation/survey_workspace_page.dart';

enum _TodayFilter { all, surveying, qualified }

/// Today's field tasks and scheduled site surveys for the active branch.
class TodayPage extends ConsumerStatefulWidget {
  const TodayPage({super.key});

  @override
  ConsumerState<TodayPage> createState() => _TodayPageState();
}

class _TodayPageState extends ConsumerState<TodayPage> {
  final _searchControl = FormControl<String>(value: '');
  String _query = '';
  _TodayFilter _filter = _TodayFilter.all;

  @override
  void initState() {
    super.initState();
    _searchControl.valueChanges.listen((val) {
      if (!mounted) return;
      setState(() => _query = (val ?? '').trim().toLowerCase());
    });
  }

  @override
  void dispose() {
    _searchControl.dispose();
    super.dispose();
  }

  List<FieldSurveyRecord> _applyFilter(List<FieldSurveyRecord> list) {
    return list.where((item) {
      final matchesFilter = switch (_filter) {
        _TodayFilter.all => true,
        _TodayFilter.surveying => item.status == 'draft',
        _TodayFilter.qualified => item.status == 'scheduled',
      };
      if (!matchesFilter) return false;
      if (_query.isEmpty) return true;
      final haystack = [
        item.opportunityCode,
        item.opportunityTitle,
        item.surveyNumber,
        item.customerName,
        item.siteName,
      ].join(' ').toLowerCase();
      return haystack.contains(_query);
    }).toList();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final asyncList = ref.watch(surveyListProvider);

    return ErpScreen(
      title: l10n.todayTitle,
      actions: [
        IconButton(
          tooltip: l10n.todayRefresh,
          onPressed: () => ref.read(surveyListProvider.notifier).refresh(),
          icon: ErpIcon(ErpIcons.refresh, color: t.textMain),
        ),
      ],
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Padding(
            padding: EdgeInsets.fromLTRB(
              t.space4,
              t.space3,
              t.space4,
              t.space2,
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                ErpTextField(
                  control: _searchControl,
                  label: l10n.todayTitle,
                  placeholder: l10n.todaySearchPlaceholder,
                  textInputAction: TextInputAction.search,
                ),
                SizedBox(height: t.space3),
                Row(
                  children: [
                    Expanded(
                      child: ErpButton(
                        label: l10n.todayFilterAll,
                        variant: _filter == _TodayFilter.all
                            ? ErpButtonVariant.primary
                            : ErpButtonVariant.outline,
                        onPressed: () =>
                            setState(() => _filter = _TodayFilter.all),
                      ),
                    ),
                    SizedBox(width: t.space2),
                    Expanded(
                      child: ErpButton(
                        label: l10n.todayFilterSurveying,
                        variant: _filter == _TodayFilter.surveying
                            ? ErpButtonVariant.primary
                            : ErpButtonVariant.outline,
                        onPressed: () =>
                            setState(() => _filter = _TodayFilter.surveying),
                      ),
                    ),
                    SizedBox(width: t.space2),
                    Expanded(
                      child: ErpButton(
                        label: l10n.todayFilterQualified,
                        variant: _filter == _TodayFilter.qualified
                            ? ErpButtonVariant.primary
                            : ErpButtonVariant.outline,
                        onPressed: () =>
                            setState(() => _filter = _TodayFilter.qualified),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
          Expanded(
            child: asyncList.when(
              loading: () => const ErpMonoLoading(),
              error: (_, _) => Padding(
                padding: ErpPagePadding.of(context),
                child: Column(
                  children: [
                    ErpAlert(
                      message: l10n.errorUnknown,
                      severity: ErpAlertSeverity.danger,
                    ),
                  ],
                ),
              ),
              data: (records) {
                final filtered = _applyFilter(records);
                if (filtered.isEmpty) {
                  return ErpEmptyState(
                    icon: ErpIcons.calendar,
                    title: l10n.todayEmptyTitle,
                    message: l10n.todayEmptyBody,
                    actionLabel: l10n.todayRefresh,
                    onAction: () =>
                        ref.read(surveyListProvider.notifier).refresh(),
                  );
                }
                return RefreshIndicator(
                  onRefresh: () =>
                      ref.read(surveyListProvider.notifier).refresh(),
                  child: ListView.separated(
                    padding: ErpPagePadding.of(context),
                    itemCount: filtered.length,
                    separatorBuilder: (_, _) => SizedBox(height: t.space3),
                    itemBuilder: (ctx, index) {
                      final item = filtered[index];
                      return _TodayTaskCard(
                        record: item,
                        onOpen: () => Navigator.of(ctx).push<void>(
                          MaterialPageRoute(
                            builder: (_) =>
                                SurveyWorkspacePage(initialRecord: item),
                          ),
                        ),
                      );
                    },
                  ),
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}

class _TodayTaskCard extends StatelessWidget {
  const _TodayTaskCard({required this.record, required this.onOpen});

  final FieldSurveyRecord record;
  final VoidCallback onOpen;

  static (String, ErpBadgeTone) _stageBadge(
    AppLocalizations l10n,
    String status,
  ) => switch (status) {
    'ready' => (l10n.todayStageEstimating, ErpBadgeTone.success),
    'scheduled' => (l10n.todayStageQualified, ErpBadgeTone.info),
    _ => (l10n.todayStageSurveying, ErpBadgeTone.warning),
  };

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final (stageLabel, stageTone) = _stageBadge(l10n, record.status);
    final siteText = record.siteName.isEmpty
        ? l10n.todayNoSite
        : record.siteName;
    final dateFmt = DateFormat(
      'dd/MM HH:mm',
      Localizations.localeOf(context).toLanguageTag(),
    );
    final schedText = record.scheduledAtUtc != null
        ? l10n.todayScheduledAt(
            dateFmt.format(record.scheduledAtUtc!.toLocal()),
          )
        : record.surveyNumber;
    final actionText = record.status == 'scheduled'
        ? l10n.todayStartSurvey
        : l10n.todayOpenSurvey;

    return ErpCard(
      children: [
        // Line 1: Opportunity code & title + stage ErpStatusBadge
        Row(
          children: [
            Expanded(
              child: Text(
                '${record.opportunityCode} · ${record.opportunityTitle}',
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: text.titleMedium,
              ),
            ),
            SizedBox(width: t.space2),
            ErpStatusBadge(label: stageLabel, tone: stageTone),
          ],
        ),
        SizedBox(height: t.space1),
        // Line 2: Customer name + primary site name
        Text(
          '${record.customerName} · $siteText',
          maxLines: 1,
          overflow: TextOverflow.ellipsis,
          style: text.bodyMedium?.copyWith(color: t.textMuted),
        ),
        SizedBox(height: t.space3),
        // Line 3: Scheduled time or survey number + action button
        Row(
          children: [
            Expanded(
              child: Text(
                schedText,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: text.bodySmall?.copyWith(color: t.textMuted),
              ),
            ),
            SizedBox(width: t.space2),
            ErpButton(
              label: actionText,
              variant: ErpButtonVariant.outline,
              expand: false,
              onPressed: onOpen,
            ),
          ],
        ),
      ],
    );
  }
}
