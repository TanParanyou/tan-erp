import 'dart:convert';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';
import 'package:tan_erp_field/core/membership/membership_provider.dart';
import 'package:tan_erp_field/core/offline/offline_store.dart';
import 'package:tan_erp_field/core/offline/sync_controller.dart';
import 'package:tan_erp_field/features/quick_estimate/domain/field_quick_estimate_models.dart';
import 'package:tan_erp_field/features/survey/domain/field_survey_models.dart';
import 'package:uuid/uuid.dart';

class FieldQuickEstimateRepository {
  const FieldQuickEstimateRepository(this._ref);

  final Ref _ref;
  static const _uuid = Uuid();

  String get _membershipId =>
      _ref.read(activeMembershipIdProvider) ?? 'default';

  OfflineStore get _store => _ref.read(offlineStoreProvider);

  String _cacheKey(String id) => 'quick_estimate_$id';

  double _calculateAreaSqm(FieldSurveyArea area) {
    for (final m in area.measurements) {
      if (m.measurementType == 'area' && m.value > 0) {
        return m.value;
      }
    }
    double? length;
    double? width;
    for (final m in area.measurements) {
      if (m.measurementType == 'length' && m.value > 0) {
        length = m.unitCode == 'mm' ? m.value / 1000.0 : m.value;
      } else if (m.measurementType == 'width' && m.value > 0) {
        width = m.unitCode == 'mm' ? m.value / 1000.0 : m.value;
      }
    }
    if (length != null && width != null && length > 0 && width > 0) {
      return double.parse((length * width).toStringAsFixed(2));
    }
    return 12;
  }

  static double _baseRate(String gradeCode) => switch (gradeCode) {
    'premium' => 6800.0,
    'luxury' => 9500.0,
    _ => 4500.0,
  };

  static double _multiplier(String propertyType) => switch (propertyType) {
    'house' => 1.05,
    'office' => 1.10,
    _ => 1.0,
  };

  FieldQuickEstimateRecord _buildRecord({
    required FieldSurveyRecord survey,
    required String propertyType,
    required String gradeCode,
    required int calculationVersion,
    String? id,
    String? number,
    bool isShared = false,
    bool isConverted = false,
  }) {
    final rate = _baseRate(gradeCode) * _multiplier(propertyType);
    final unitRate = double.parse(rate.toStringAsFixed(2));

    final lines = <FieldQuickEstimateLine>[];
    if (survey.areas.isEmpty) {
      lines.add(
        FieldQuickEstimateLine(
          areaCode: 'A-01',
          areaName: survey.siteName.isNotEmpty ? survey.siteName : 'Area',
          quantitySqm: 12,
          unitRate: unitRate,
        ),
      );
    } else {
      for (final a in survey.areas) {
        final sqm = _calculateAreaSqm(a);
        lines.add(
          FieldQuickEstimateLine(
            areaCode: a.code,
            areaName: a.name,
            quantitySqm: sqm,
            unitRate: unitRate,
          ),
        );
      }
    }

    final netAmount = lines.fold<double>(0, (sum, l) => sum + l.lineTotal);
    final taxAmount = double.parse((netAmount * 0.07).toStringAsFixed(2));
    final gross = netAmount + taxAmount;
    final displayedLower = (gross * 0.95 / 1000).floorToDouble() * 1000;
    final displayedUpper = (gross * 1.08 / 1000).ceilToDouble() * 1000;

    final validDate = DateTime.now().add(const Duration(days: 30));
    final validUntilText = DateFormat('yyyy-MM-dd').format(validDate);

    final resolvedId = id ?? _uuid.v4();
    final sNum = survey.surveyNumber.isNotEmpty
        ? survey.surveyNumber
        : survey.opportunityCode;
    final resolvedNumber = number ?? 'QE-$sNum';

    return FieldQuickEstimateRecord(
      id: resolvedId,
      number: resolvedNumber,
      opportunityId: survey.opportunityId,
      surveyRevisionId: survey.revisionId,
      propertyType: propertyType,
      gradeCode: gradeCode,
      calculationVersion: calculationVersion,
      netAmount: double.parse(netAmount.toStringAsFixed(2)),
      taxAmount: taxAmount,
      displayedLower: displayedLower,
      displayedUpper: displayedUpper,
      validUntilText: validUntilText,
      lines: lines,
      isShared: isShared,
      isConverted: isConverted,
    );
  }

  Future<FieldQuickEstimateRecord> loadOrCreateFromSurvey(
    FieldSurveyRecord survey,
  ) async {
    final key = _cacheKey(survey.surveyId);
    final cached = await _store.readCache(
      membershipId: _membershipId,
      cacheKey: key,
    );
    if (cached != null) {
      try {
        final map = jsonDecode(cached.payloadJson) as Map<String, Object?>;
        return FieldQuickEstimateRecord.fromJson(map);
      } on Object {
        // Fallback to fresh creation if cache corrupted.
      }
    }
    final record = _buildRecord(
      survey: survey,
      propertyType: 'condo',
      gradeCode: 'standard',
      calculationVersion: 1,
    );
    final jsonStr = jsonEncode(record.toJson());
    await _store.writeCache(
      membershipId: _membershipId,
      cacheKey: key,
      payloadJson: jsonStr,
    );
    await _store.writeCache(
      membershipId: _membershipId,
      cacheKey: _cacheKey(record.id),
      payloadJson: jsonStr,
    );
    return record;
  }

  Future<FieldQuickEstimateRecord> recalculate({
    required FieldSurveyRecord survey,
    required FieldQuickEstimateRecord current,
    required String propertyType,
    required String gradeCode,
  }) async {
    final record = _buildRecord(
      survey: survey,
      propertyType: propertyType,
      gradeCode: gradeCode,
      calculationVersion: current.calculationVersion + 1,
      id: current.id,
      number: current.number,
      isShared: current.isShared,
      isConverted: current.isConverted,
    );
    final jsonStr = jsonEncode(record.toJson());
    await _store.writeCache(
      membershipId: _membershipId,
      cacheKey: _cacheKey(survey.surveyId),
      payloadJson: jsonStr,
    );
    await _store.writeCache(
      membershipId: _membershipId,
      cacheKey: _cacheKey(record.id),
      payloadJson: jsonStr,
    );
    return record;
  }

  Future<FieldQuickEstimateRecord> recordCustomerShare(
    FieldQuickEstimateRecord record,
    String locale,
  ) async {
    final updated = record.copyWith(isShared: true);
    final jsonStr = jsonEncode(updated.toJson());
    await _store.writeCache(
      membershipId: _membershipId,
      cacheKey: _cacheKey(record.id),
      payloadJson: jsonStr,
    );
    await _ref.read(syncControllerProvider.notifier).enqueue(
      entityType: 'quick_estimate',
      entityId: record.id,
      operation: 'share',
      title: record.number,
      payload: <String, Object?>{
        'sourceVersion': record.calculationVersion,
        'locale': locale,
      },
    );
    return updated;
  }

  Future<FieldQuickEstimateRecord> convertToOfficialDraft(
    FieldQuickEstimateRecord record,
  ) async {
    final updated = record.copyWith(isConverted: true);
    final jsonStr = jsonEncode(updated.toJson());
    await _store.writeCache(
      membershipId: _membershipId,
      cacheKey: _cacheKey(record.id),
      payloadJson: jsonStr,
    );
    await _ref.read(syncControllerProvider.notifier).enqueue(
      entityType: 'quick_estimate',
      entityId: record.id,
      operation: 'convert',
      title: record.number,
      payload: <String, Object?>{
        'sourceVersion': record.calculationVersion,
        'siteSurveyRevisionId': record.surveyRevisionId,
      },
    );
    return updated;
  }
}

final fieldQuickEstimateRepositoryProvider =
    Provider<FieldQuickEstimateRepository>(FieldQuickEstimateRepository.new);
