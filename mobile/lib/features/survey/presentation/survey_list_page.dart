import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/router/routes.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
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

/// Lists all site survey records with search and quick navigation to workspace.
class SurveyListPage extends ConsumerStatefulWidget {
  const SurveyListPage({super.key});

  @override
  ConsumerState<SurveyListPage> createState() => _SurveyListPageState();
}

class _SurveyListPageState extends ConsumerState<SurveyListPage> {
  final _searchControl = FormControl<String>(value: '');
  String _query = '';

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

  List<FieldSurveyRecord> _filter(List<FieldSurveyRecord> list) {
    if (_query.isEmpty) return list;
    return list.where((r) {
      final haystack = [
        r.surveyNumber,
        r.opportunityCode,
        r.opportunityTitle,
        r.customerName,
        r.siteName,
      ].join(' ').toLowerCase();
      return haystack.contains(_query);
    }).toList();
  }

  static (String, ErpBadgeTone) _badge(AppLocalizations l10n, String status) =>
      switch (status) {
        'ready' => (l10n.surveyStatusReady, ErpBadgeTone.success),
        'scheduled' => (l10n.surveyStatusScheduled, ErpBadgeTone.info),
        'voided' => (l10n.surveyStatusVoided, ErpBadgeTone.danger),
        _ => (l10n.surveyStatusDraft, ErpBadgeTone.warning),
      };

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final asyncList = ref.watch(surveyListProvider);

    return ErpScreen(
      title: l10n.surveyListTitle,
      actions: [
        IconButton(
          tooltip: l10n.todayRefresh,
          onPressed: () => ref.read(surveyListProvider.notifier).refresh(),
          icon: ErpIcon(ErpIcons.refresh, color: t.textMain),
        ),
      ],
      body: Column(
        children: [
          Padding(
            padding: EdgeInsets.fromLTRB(
              t.space4,
              t.space3,
              t.space4,
              t.space2,
            ),
            child: ErpTextField(
              control: _searchControl,
              label: l10n.surveyListTitle,
              placeholder: l10n.surveySearchPlaceholder,
              textInputAction: TextInputAction.search,
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
                final filtered = _filter(records);
                if (filtered.isEmpty) {
                  return ErpEmptyState(
                    icon: ErpIcons.mapPin,
                    title: l10n.surveyEmptyTitle,
                    message: l10n.surveyEmptyBody,
                    actionLabel: l10n.surveyGoToToday,
                    onAction: () => context.go(AppRoutes.today),
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
                      return _SurveyListCard(
                        record: item,
                        onTap: () => Navigator.of(ctx).push<void>(
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

class _SurveyListCard extends StatelessWidget {
  const _SurveyListCard({required this.record, required this.onTap});

  final FieldSurveyRecord record;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final text = Theme.of(context).textTheme;
    final (badgeText, badgeTone) = _SurveyListPageState._badge(
      l10n,
      record.status,
    );
    final siteText = record.siteName.isEmpty
        ? l10n.todayNoSite
        : record.siteName;

    return InkWell(
      onTap: onTap,
      child: ErpCard(
        children: [
          Row(
            children: [
              Expanded(
                child: Text(
                  '${record.surveyNumber} · ${record.opportunityTitle}',
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: text.titleMedium,
                ),
              ),
              SizedBox(width: t.space2),
              ErpStatusBadge(label: badgeText, tone: badgeTone),
            ],
          ),
          SizedBox(height: t.space1),
          Text(
            '${record.customerName} · $siteText',
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: text.bodyMedium?.copyWith(color: t.textMuted),
          ),
          SizedBox(height: t.space2),
          Row(
            children: [
              Expanded(
                child: Text(
                  '${l10n.surveyAreasSection}: ${record.areas.length}',
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: text.bodySmall?.copyWith(color: t.textMuted),
                ),
              ),
              ErpIcon(ErpIcons.chevronRight, color: t.textMuted, size: 20),
            ],
          ),
        ],
      ),
    );
  }
}
