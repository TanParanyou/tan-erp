import 'package:flutter/foundation.dart';

/// Single surveyed line/area contributing to a quick estimate.
@immutable
class FieldQuickEstimateLine {
  const FieldQuickEstimateLine({
    required this.areaCode,
    required this.areaName,
    required this.quantitySqm,
    required this.unitRate,
  });

  factory FieldQuickEstimateLine.fromJson(Map<String, Object?> json) {
    final qRaw = json['quantitySqm'];
    final rRaw = json['unitRate'];
    return FieldQuickEstimateLine(
      areaCode: json['areaCode'] as String? ?? '',
      areaName: json['areaName'] as String? ?? '',
      quantitySqm: qRaw is num ? qRaw.toDouble() : 0.0,
      unitRate: rRaw is num ? rRaw.toDouble() : 0.0,
    );
  }

  final String areaCode;
  final String areaName;
  final double quantitySqm;
  final double unitRate;

  double get lineTotal => quantitySqm * unitRate;

  FieldQuickEstimateLine copyWith({
    String? areaCode,
    String? areaName,
    double? quantitySqm,
    double? unitRate,
  }) => FieldQuickEstimateLine(
    areaCode: areaCode ?? this.areaCode,
    areaName: areaName ?? this.areaName,
    quantitySqm: quantitySqm ?? this.quantitySqm,
    unitRate: unitRate ?? this.unitRate,
  );

  Map<String, Object?> toJson() => <String, Object?>{
    'areaCode': areaCode,
    'areaName': areaName,
    'quantitySqm': quantitySqm,
    'unitRate': unitRate,
    'lineTotal': lineTotal,
  };
}

/// On-site quick estimate record with calculated price range.
@immutable
class FieldQuickEstimateRecord {
  const FieldQuickEstimateRecord({
    required this.id,
    required this.number,
    required this.opportunityId,
    required this.surveyRevisionId,
    required this.propertyType,
    required this.gradeCode,
    required this.calculationVersion,
    required this.netAmount,
    required this.taxAmount,
    required this.displayedLower,
    required this.displayedUpper,
    required this.validUntilText,
    this.lines = const [],
    this.isShared = false,
    this.isConverted = false,
  });

  factory FieldQuickEstimateRecord.fromJson(Map<String, Object?> json) {
    final rawLines = json['lines'];
    final parsedLines = <FieldQuickEstimateLine>[];
    if (rawLines is List<Object?>) {
      for (final item in rawLines) {
        if (item is Map<String, Object?>) {
          parsedLines.add(FieldQuickEstimateLine.fromJson(item));
        }
      }
    }
    final verRaw = json['calculationVersion'];
    final netRaw = json['netAmount'];
    final taxRaw = json['taxAmount'];
    final lowRaw = json['displayedLower'];
    final upRaw = json['displayedUpper'];

    return FieldQuickEstimateRecord(
      id: json['id'] as String? ?? '',
      number: json['number'] as String? ?? '',
      opportunityId: json['opportunityId'] as String? ?? '',
      surveyRevisionId: json['surveyRevisionId'] as String? ?? '',
      propertyType: json['propertyType'] as String? ?? 'condo',
      gradeCode: json['gradeCode'] as String? ?? 'standard',
      calculationVersion: verRaw is num ? verRaw.toInt() : 1,
      netAmount: netRaw is num ? netRaw.toDouble() : 0.0,
      taxAmount: taxRaw is num ? taxRaw.toDouble() : 0.0,
      displayedLower: lowRaw is num ? lowRaw.toDouble() : 0.0,
      displayedUpper: upRaw is num ? upRaw.toDouble() : 0.0,
      validUntilText: json['validUntilText'] as String? ?? '',
      lines: parsedLines,
      isShared: json['isShared'] as bool? ?? false,
      isConverted: json['isConverted'] as bool? ?? false,
    );
  }

  final String id;
  final String number;
  final String opportunityId;
  final String surveyRevisionId;

  /// `'condo' | 'house' | 'office'`.
  final String propertyType;

  /// `'standard' | 'premium' | 'luxury'`.
  final String gradeCode;
  final int calculationVersion;
  final double netAmount;
  final double taxAmount;
  final double displayedLower;
  final double displayedUpper;
  final String validUntilText;
  final List<FieldQuickEstimateLine> lines;
  final bool isShared;
  final bool isConverted;

  double get grossAmount => netAmount + taxAmount;

  FieldQuickEstimateRecord copyWith({
    String? id,
    String? number,
    String? opportunityId,
    String? surveyRevisionId,
    String? propertyType,
    String? gradeCode,
    int? calculationVersion,
    double? netAmount,
    double? taxAmount,
    double? displayedLower,
    double? displayedUpper,
    String? validUntilText,
    List<FieldQuickEstimateLine>? lines,
    bool? isShared,
    bool? isConverted,
  }) => FieldQuickEstimateRecord(
    id: id ?? this.id,
    number: number ?? this.number,
    opportunityId: opportunityId ?? this.opportunityId,
    surveyRevisionId: surveyRevisionId ?? this.surveyRevisionId,
    propertyType: propertyType ?? this.propertyType,
    gradeCode: gradeCode ?? this.gradeCode,
    calculationVersion: calculationVersion ?? this.calculationVersion,
    netAmount: netAmount ?? this.netAmount,
    taxAmount: taxAmount ?? this.taxAmount,
    displayedLower: displayedLower ?? this.displayedLower,
    displayedUpper: displayedUpper ?? this.displayedUpper,
    validUntilText: validUntilText ?? this.validUntilText,
    lines: lines ?? this.lines,
    isShared: isShared ?? this.isShared,
    isConverted: isConverted ?? this.isConverted,
  );

  Map<String, Object?> toJson() => <String, Object?>{
    'id': id,
    'number': number,
    'opportunityId': opportunityId,
    'surveyRevisionId': surveyRevisionId,
    'propertyType': propertyType,
    'gradeCode': gradeCode,
    'calculationVersion': calculationVersion,
    'netAmount': netAmount,
    'taxAmount': taxAmount,
    'displayedLower': displayedLower,
    'displayedUpper': displayedUpper,
    'validUntilText': validUntilText,
    'lines': [for (final line in lines) line.toJson()],
    'isShared': isShared,
    'isConverted': isConverted,
  };
}
