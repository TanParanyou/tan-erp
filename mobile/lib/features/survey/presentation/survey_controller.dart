import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/membership/membership_provider.dart';
import 'package:tan_erp_field/features/survey/data/field_survey_repository.dart';
import 'package:tan_erp_field/features/survey/domain/field_survey_models.dart';
import 'package:uuid/uuid.dart';

/// Manages the cached and synced list of field surveys for the active branch.
class SurveyListController extends AsyncNotifier<List<FieldSurveyRecord>> {
  static const _uuid = Uuid();

  @override
  Future<List<FieldSurveyRecord>> build() {
    ref.watch(activeMembershipIdProvider);
    return ref.read(fieldSurveyRepositoryProvider).loadSurveys();
  }

  Future<void> refresh() async {
    state = const AsyncLoading();
    state = await AsyncValue.guard(
      () => ref.read(fieldSurveyRepositoryProvider).loadSurveys(),
    );
  }

  Future<FieldSurveyRecord> openOrCreateForOpportunity({
    required String opportunityId,
    required String opportunityCode,
    required String opportunityTitle,
    required String customerName,
    required String siteName,
    String scopeSummary = '',
  }) async {
    final current = state.value ?? const <FieldSurveyRecord>[];
    final existing = current
        .where((r) => r.opportunityId == opportunityId)
        .firstOrNull;
    if (existing != null) return existing;

    final suffix = opportunityCode.split('-').last;
    final created = FieldSurveyRecord(
      surveyId: 'srv-$opportunityId',
      opportunityId: opportunityId,
      opportunityCode: opportunityCode,
      opportunityTitle: opportunityTitle,
      customerName: customerName,
      siteName: siteName,
      surveyNumber: 'SRV-$suffix',
      status: 'draft',
      revisionId: _uuid.v4(),
      revisionNumber: 1,
      rowVersion: '1',
      scopeSummary: scopeSummary,
      scheduledAtUtc: DateTime.now().toUtc(),
    );
    final updated = await ref
        .read(fieldSurveyRepositoryProvider)
        .saveDraft(created);
    state = AsyncData(updated);
    return created;
  }

  Future<FieldSurveyRecord> saveDraft(FieldSurveyRecord record) async {
    final nextRecord = record.status == 'scheduled'
        ? record.copyWith(status: 'draft')
        : record;
    final updated = await ref
        .read(fieldSurveyRepositoryProvider)
        .saveDraft(nextRecord);
    state = AsyncData(updated);
    return nextRecord;
  }

  Future<FieldSurveyRecord> markReady(FieldSurveyRecord record) async {
    final updated = await ref
        .read(fieldSurveyRepositoryProvider)
        .markReady(record);
    state = AsyncData(updated);
    return record.copyWith(status: 'ready');
  }
}

final surveyListProvider =
    AsyncNotifierProvider<SurveyListController, List<FieldSurveyRecord>>(
      SurveyListController.new,
    );
