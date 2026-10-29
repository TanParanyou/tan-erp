import 'dart:convert';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_api/tan_erp_api.dart';
import 'package:tan_erp_field/core/membership/membership_provider.dart';
import 'package:tan_erp_field/core/network/api_client.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/offline/offline_store.dart';
import 'package:tan_erp_field/core/offline/sync_controller.dart';
import 'package:tan_erp_field/features/survey/domain/field_survey_models.dart';
import 'package:uuid/uuid.dart';

/// Offline-first repository for site survey records and outbox mutations.
class FieldSurveyRepository {
  FieldSurveyRepository(this._ref);

  static const _cacheKey = 'field_surveys_v1';
  static const _uuid = Uuid();

  final Ref _ref;

  String get _membershipId =>
      _ref.read(activeMembershipIdProvider) ?? 'default';

  Future<List<FieldSurveyRecord>> loadSurveys() async {
    final cached = await _readCache();
    final client = _ref.read(apiClientProvider);

    try {
      final assignedResp = await client.call(
        () => client.api.getSiteSurveysApi().apiV1SurveysAssignedGet(),
      );
      final items = assignedResp.items?.toList() ??
          const <AssignedSurveySyncItemResponse>[];
      final records = <FieldSurveyRecord>[];

      for (final item in items) {
        final oppId = item.opportunityId;
        if (oppId == null || oppId.isEmpty) continue;

        final existingLocal = cached
            .where((r) => r.opportunityId == oppId)
            .firstOrNull;

        if (item.survey != null) {
          records.add(_mapAssignedSurvey(item, item.survey!, existingLocal));
        } else {
          records.add(existingLocal ?? _buildDraftFromAssignedItem(item));
        }
      }

      if (records.isNotEmpty) {
        await _writeCache(records);
        return records;
      }

      if (cached.isNotEmpty) return cached;
      final samples = _seedSampleRecords();
      await _writeCache(samples);
      return samples;
    } on NetworkFailure {
      return await _fallbackOrSeed(cached);
    } on UnauthorizedFailure {
      return await _fallbackOrSeed(cached);
    } on ApiFailure {
      return await _fallbackOrSeed(cached);
    } on Object {
      return await _fallbackOrSeed(cached);
    }
  }

  Future<List<FieldSurveyRecord>> _fallbackOrSeed(
    List<FieldSurveyRecord> cached,
  ) async {
    if (cached.isNotEmpty) return cached;
    final samples = _seedSampleRecords();
    await _writeCache(samples);
    return samples;
  }

  Future<List<FieldSurveyRecord>> saveDraft(FieldSurveyRecord updated) async {
    final current = await _readCache();
    final next = <FieldSurveyRecord>[];
    var replaced = false;
    for (final item in current) {
      if (item.surveyId == updated.surveyId ||
          item.opportunityId == updated.opportunityId) {
        next.add(updated);
        replaced = true;
      } else {
        next.add(item);
      }
    }
    if (!replaced) next.insert(0, updated);

    await _writeCache(next);
    final notifier = _ref.read(syncControllerProvider.notifier);
    await notifier.enqueue(
      entityType: 'survey',
      entityId: updated.revisionId,
      operation: 'update_draft',
      title: updated.surveyNumber,
      expectedRowVersion: updated.rowVersion,
      payload: <String, Object?>{
        'opportunityId': updated.opportunityId,
        'surveyId': updated.surveyId,
        'revisionId': updated.revisionId,
        'scopeSummary': updated.scopeSummary,
        'areas': [for (final a in updated.areas) a.toJson()],
      },
    );
    for (final area in updated.areas) {
      for (final att in area.attachments) {
        if (!att.isSynced && att.localPath.isNotEmpty) {
          await notifier.enqueue(
            entityType: 'attachment',
            entityId: area.id,
            operation: 'upload_and_link',
            title: '${updated.surveyNumber} · ${area.code} · ${att.filename}',
            payload: <String, Object?>{
              'ownerType': 'survey-area',
              'ownerId': area.id,
              'filename': att.filename,
              'localPath': att.localPath,
              'purpose': 'evidence',
            },
          );
        }
      }
    }
    return next;
  }

  Future<List<FieldSurveyRecord>> markReady(FieldSurveyRecord record) async {
    final readyRecord = record.copyWith(status: 'ready');
    final current = await _readCache();
    final next = <FieldSurveyRecord>[
      for (final item in current)
        if (item.surveyId == record.surveyId ||
            item.opportunityId == record.opportunityId)
          readyRecord
        else
          item,
    ];
    if (!next.any((r) => r.surveyId == readyRecord.surveyId)) {
      next.insert(0, readyRecord);
    }

    await _writeCache(next);
    await _ref.read(syncControllerProvider.notifier).enqueue(
      entityType: 'survey',
      entityId: readyRecord.revisionId,
      operation: 'mark_ready',
      title: readyRecord.surveyNumber,
      expectedRowVersion: readyRecord.rowVersion,
      payload: <String, Object?>{
        'opportunityId': readyRecord.opportunityId,
        'surveyId': readyRecord.surveyId,
        'revisionId': readyRecord.revisionId,
      },
    );
    return next;
  }

  Future<List<FieldSurveyRecord>> _readCache() async {
    final store = _ref.read(offlineStoreProvider);
    final entry = await store.readCache(
      membershipId: _membershipId,
      cacheKey: _cacheKey,
    );
    if (entry == null) return const [];
    try {
      final decoded = jsonDecode(entry.payloadJson);
      if (decoded is! List<Object?>) return const [];
      return [
        for (final item in decoded)
          if (item is Map<String, Object?>) FieldSurveyRecord.fromJson(item),
      ];
    } on Object {
      return const [];
    }
  }

  Future<void> _writeCache(List<FieldSurveyRecord> records) async {
    final store = _ref.read(offlineStoreProvider);
    final payload = jsonEncode([for (final r in records) r.toJson()]);
    await store.writeCache(
      membershipId: _membershipId,
      cacheKey: _cacheKey,
      payloadJson: payload,
    );
  }

  FieldSurveyRecord _mapAssignedSurvey(
    AssignedSurveySyncItemResponse item,
    SiteSurveyResponse survey,
    FieldSurveyRecord? existingLocal,
  ) {
    final rev = survey.currentRevision;
    final remoteAreas =
        rev?.areas?.toList() ?? const <SiteSurveyAreaResponse>[];
    final areas = <FieldSurveyArea>[
      for (final a in remoteAreas)
        FieldSurveyArea(
          id: a.id ?? _uuid.v4(),
          code: a.code ?? 'A-01',
          name: a.name ?? '',
          description: a.description,
          measurements: [
            for (final m
                in a.measurements?.toList() ??
                    const <SiteSurveyMeasurementResponse>[])
              FieldSurveyMeasurement(
                id: m.id ?? _uuid.v4(),
                measurementType: (m.measurementType ?? 'area').toLowerCase(),
                value: m.value ?? 0,
                unitCode: (m.unitCode ?? 'sqm').toLowerCase(),
                captureMethod: (m.captureMethod ?? 'measured').toLowerCase(),
                notes: m.notes,
              ),
          ],
        ),
    ];

    final customerName = item.customerDisplayName ??
        existingLocal?.customerName ??
        '-';
    final siteName = survey.site?.label ??
        item.site?.label ??
        existingLocal?.siteName ??
        '';
    final rawStatus = (rev?.status ?? survey.status ?? 'draft').toLowerCase();

    return FieldSurveyRecord(
      surveyId: survey.id ?? existingLocal?.surveyId ?? _uuid.v4(),
      opportunityId: item.opportunityId ?? '',
      opportunityCode: item.opportunityCode ?? 'OPP-0001',
      opportunityTitle: item.opportunityTitle ?? '',
      customerName: customerName,
      siteName: siteName,
      surveyNumber:
          survey.surveyNumber ?? existingLocal?.surveyNumber ?? 'SRV-DRAFT',
      status: rawStatus,
      revisionId: rev?.id ?? existingLocal?.revisionId ?? _uuid.v4(),
      revisionNumber: rev?.revisionNumber ?? 1,
      rowVersion: rev?.rowVersion ??
          survey.rowVersion ??
          item.opportunityRowVersion ??
          '1',
      scopeSummary:
          rev?.scopeSummary ?? existingLocal?.scopeSummary ?? '',
      scheduledAtUtc: survey.scheduledStartUtc,
      areas: areas.isNotEmpty ? areas : (existingLocal?.areas ?? const []),
    );
  }

  FieldSurveyRecord _buildDraftFromAssignedItem(
    AssignedSurveySyncItemResponse item,
  ) {
    final code = item.opportunityCode ?? 'OPP-0001';
    final suffix = code.split('-').last;
    final customerName = item.customerDisplayName ?? '-';
    final siteName = item.site?.label ?? '';
    final stage = (item.opportunityStage ?? 'qualified').toLowerCase();
    final status = stage == 'qualified' ? 'scheduled' : 'draft';

    return FieldSurveyRecord(
      surveyId: 'srv-${item.opportunityId ?? _uuid.v4()}',
      opportunityId: item.opportunityId ?? _uuid.v4(),
      opportunityCode: code,
      opportunityTitle: item.opportunityTitle ?? '',
      customerName: customerName,
      siteName: siteName,
      surveyNumber: 'SRV-$suffix',
      status: status,
      revisionId: 'rev-${item.opportunityId ?? _uuid.v4()}',
      revisionNumber: 1,
      rowVersion: item.opportunityRowVersion ?? '1',
      scopeSummary: '',
      scheduledAtUtc: DateTime.now().toUtc(),
    );
  }

  List<FieldSurveyRecord> _seedSampleRecords() {
    final now = DateTime.now().toUtc();
    return [
      FieldSurveyRecord(
        surveyId: 'srv-sample-001',
        opportunityId: 'opp-sample-001',
        opportunityCode: 'OPP-2026-0101',
        opportunityTitle: 'งานปูพื้นไม้โอ๊คและกรุผนังห้องรับแขก',
        customerName: 'บริษัท แอทเทอเลียร์ เรสซิเดนซ์ จำกัด',
        siteName: 'โครงการ เดอะ เรสซิเดนซ์ สุขุมวิท 39',
        surveyNumber: 'SRV-2026-0101',
        status: 'draft',
        revisionId: 'rev-sample-001',
        revisionNumber: 1,
        rowVersion: '1',
        scopeSummary:
            'วัดพื้นที่ห้องโถงชั้น 1 และโถงทางเดินสำหรับติดตั้งพื้นไม้',
        scheduledAtUtc: now.add(const Duration(hours: 2)),
        areas: const [
          FieldSurveyArea(
            id: 'area-sample-001',
            code: 'A-01',
            name: 'ห้องโถงรับแขกชั้น 1',
            description: 'พื้นคอนกรีตขัดมัน ได้ระดับพร้อมปู',
            measurements: [
              FieldSurveyMeasurement(
                id: 'meas-sample-001',
                measurementType: 'area',
                value: 42.5,
                unitCode: 'sqm',
                captureMethod: 'measured',
                notes: 'หักเสากลางห้อง 1 ต้น',
              ),
              FieldSurveyMeasurement(
                id: 'meas-sample-002',
                measurementType: 'length',
                value: 8.5,
                unitCode: 'm',
                captureMethod: 'measured',
              ),
            ],
          ),
        ],
      ),
      FieldSurveyRecord(
        surveyId: 'srv-sample-002',
        opportunityId: 'opp-sample-002',
        opportunityCode: 'OPP-2026-0102',
        opportunityTitle: 'ปรับปรุงฝ้าเพดานและผนังอะคูสติกห้องประชุม',
        customerName: 'คุณกิตติพงศ์ วัฒนากุล',
        siteName: 'อาคารสาทร ซิตี้ ทาวเวอร์ ชั้น 18',
        surveyNumber: 'SRV-2026-0102',
        status: 'scheduled',
        revisionId: 'rev-sample-002',
        revisionNumber: 1,
        rowVersion: '1',
        scopeSummary: 'สำรวจความสูงฝ้าและขนาดผนังทั้ง 4 ด้านก่อนประเมินราคา',
        scheduledAtUtc: now.add(const Duration(hours: 5)),
      ),
    ];
  }
}

final fieldSurveyRepositoryProvider = Provider<FieldSurveyRepository>(
  FieldSurveyRepository.new,
);
