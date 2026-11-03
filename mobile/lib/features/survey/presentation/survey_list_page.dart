import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/router/routes.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_empty_state.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_mono_loading.dart';
import 'package:tan_erp_field/core/ui/erp_page_padding.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/core/ui/erp_task_card.dart';
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
              showLabel: false,
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
    final (label, icon, color) = switch (record.status) {
      'ready' => (l10n.surveyStatusReady, ErpIcons.checkCircle, t.success),
      'scheduled' => (l10n.surveyStatusScheduled, ErpIcons.calendar, t.info),
      'voided' => (l10n.surveyStatusVoided, ErpIcons.alertCircle, t.danger),
      _ => (l10n.surveyStatusDraft, ErpIcons.ruler, t.warning),
    };
    return ErpTaskCard(
      title: record.opportunityTitle,
      subtitle: record.customerName,
      caption: record.siteName.isEmpty ? l10n.todayNoSite : record.siteName,
      meta: record.surveyNumber,
      statusIcon: icon,
      statusColor: color,
      statusLabel: label,
      onTap: onTap,
    );
  }
}
