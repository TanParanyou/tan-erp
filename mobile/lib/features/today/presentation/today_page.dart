import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_empty_state.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_mono_loading.dart';
import 'package:tan_erp_field/core/ui/erp_page_padding.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/core/ui/erp_segmented_filter.dart';
import 'package:tan_erp_field/core/ui/erp_task_card.dart';
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
  FieldSurveyRecord? _selected;

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

  Widget _master({required bool split}) {
    final l10n = context.l10n;
    final t = context.tokens;
    final asyncList = ref.watch(surveyListProvider);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Padding(
          padding: EdgeInsets.fromLTRB(t.space4, t.space3, t.space4, t.space2),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              ErpTextField(
                control: _searchControl,
                label: l10n.todayTitle,
                placeholder: l10n.todaySearchPlaceholder,
                textInputAction: TextInputAction.search,
                showLabel: false,
              ),
              SizedBox(height: t.space3),
              ErpSegmentedFilter(
                labels: [
                  l10n.todayFilterAll,
                  l10n.todayFilterSurveying,
                  l10n.todayFilterQualified,
                ],
                selectedIndex: _filter.index,
                onSelected: (i) =>
                    setState(() => _filter = _TodayFilter.values[i]),
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
                      selected: split && _selected == item,
                      onOpen: () => split
                          ? setState(() => _selected = item)
                          : Navigator.of(ctx).push<void>(
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
    );
  }

  Widget _detail() {
    final record = _selected;
    if (record == null) {
      return ErpEmptyState(
        icon: ErpIcons.ruler,
        title: context.l10n.todaySelectPrompt,
      );
    }
    // Pane width, not screen width, decides the workspace's own layout.
    return LayoutBuilder(
      builder: (ctx, c) => MediaQuery(
        data: MediaQuery.of(ctx).copyWith(size: Size(c.maxWidth, c.maxHeight)),
        child: SurveyWorkspacePage(
          key: ValueKey(record.surveyNumber),
          initialRecord: record,
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    return ErpScreen(
      title: l10n.todayTitle,
      actions: [
        IconButton(
          tooltip: l10n.todayRefresh,
          onPressed: () => ref.read(surveyListProvider.notifier).refresh(),
          icon: ErpIcon(ErpIcons.refresh, color: t.textMain),
        ),
      ],
      body: LayoutBuilder(
        builder: (ctx, c) {
          final split =
              MediaQuery.sizeOf(ctx).shortestSide >= 600 && c.maxWidth >= 640;
          if (!split) return _master(split: false);
          return Row(
            children: [
              SizedBox(
                width: (c.maxWidth * 0.38).clamp(320.0, 420.0),
                child: _master(split: true),
              ),
              VerticalDivider(width: t.borderWidth, color: t.border),
              Expanded(child: _detail()),
            ],
          );
        },
      ),
    );
  }
}

class _TodayTaskCard extends StatelessWidget {
  const _TodayTaskCard({
    required this.record,
    required this.onOpen,
    this.selected = false,
  });

  final FieldSurveyRecord record;
  final VoidCallback onOpen;
  final bool selected;

  static (String, ErpIcons, Color) _stage(
    AppLocalizations l10n,
    ErpTokens t,
    String status,
  ) => switch (status) {
    'ready' => (l10n.todayStageEstimating, ErpIcons.checkCircle, t.success),
    'scheduled' => (l10n.todayStageQualified, ErpIcons.calendar, t.info),
    _ => (l10n.todayStageSurveying, ErpIcons.ruler, t.warning),
  };

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final (stageLabel, stageIcon, stageColor) = _stage(l10n, t, record.status);
    final dateFmt = DateFormat(
      'dd/MM HH:mm',
      Localizations.localeOf(context).toLanguageTag(),
    );
    return ErpTaskCard(
      title: record.opportunityTitle,
      subtitle: record.customerName,
      caption: record.siteName.isEmpty ? l10n.todayNoSite : record.siteName,
      meta: record.scheduledAtUtc != null
          ? l10n.todayScheduledAt(
              dateFmt.format(record.scheduledAtUtc!.toLocal()),
            )
          : record.surveyNumber,
      statusIcon: stageIcon,
      statusColor: stageColor,
      statusLabel: stageLabel,
      selected: selected,
      onTap: onOpen,
    );
  }
}
