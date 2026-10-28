import 'package:flutter/foundation.dart';
import 'package:tan_erp_field/core/ui/erp_attachment_strip.dart';

/// Single measured dimension in a survey area.
@immutable
class FieldSurveyMeasurement {
  const FieldSurveyMeasurement({
    required this.id,
    required this.measurementType,
    required this.value,
    required this.unitCode,
    required this.captureMethod,
    this.notes,
  });

  factory FieldSurveyMeasurement.fromJson(Map<String, Object?> json) {
    final valRaw = json['value'];
    final val = valRaw is num ? valRaw.toDouble() : 0.0;
    return FieldSurveyMeasurement(
      id: json['id'] as String? ?? '',
      measurementType: json['measurementType'] as String? ?? 'area',
      value: val,
      unitCode: json['unitCode'] as String? ?? 'sqm',
      captureMethod: json['captureMethod'] as String? ?? 'measured',
      notes: json['notes'] as String?,
    );
  }

  final String id;

  /// `area | length | width | height | count`.
  final String measurementType;
  final double value;

  /// `sqm | m | mm | pcs`.
  final String unitCode;

  /// `measured | photo_estimated | derived | customer_provided`.
  final String captureMethod;
  final String? notes;

  FieldSurveyMeasurement copyWith({
    String? id,
    String? measurementType,
    double? value,
    String? unitCode,
    String? captureMethod,
    String? notes,
  }) => FieldSurveyMeasurement(
    id: id ?? this.id,
    measurementType: measurementType ?? this.measurementType,
    value: value ?? this.value,
    unitCode: unitCode ?? this.unitCode,
    captureMethod: captureMethod ?? this.captureMethod,
    notes: notes ?? this.notes,
  );

  Map<String, Object?> toJson() => <String, Object?>{
    'id': id,
    'measurementType': measurementType,
    'value': value,
    'unitCode': unitCode,
    'captureMethod': captureMethod,
    'notes': notes,
  };
}

/// Logical area or zone measured on site.
@immutable
class FieldSurveyArea {
  const FieldSurveyArea({
    required this.id,
    required this.code,
    required this.name,
    this.description,
    this.measurements = const [],
    this.attachments = const [],
  });

  factory FieldSurveyArea.fromJson(Map<String, Object?> json) {
    final rawList = json['measurements'];
    final items = <FieldSurveyMeasurement>[];
    if (rawList is List<Object?>) {
      for (final item in rawList) {
        if (item is Map<String, Object?>) {
          items.add(FieldSurveyMeasurement.fromJson(item));
        }
      }
    }
    final rawAtt = json['attachments'];
    final attItems = <FieldAttachmentItem>[];
    if (rawAtt is List<Object?>) {
      for (final item in rawAtt) {
        if (item is Map<String, Object?>) {
          attItems.add(FieldAttachmentItem.fromJson(item));
        }
      }
    }
    return FieldSurveyArea(
      id: json['id'] as String? ?? '',
      code: json['code'] as String? ?? '',
      name: json['name'] as String? ?? '',
      description: json['description'] as String?,
      measurements: items,
      attachments: attItems,
    );
  }

  final String id;
  final String code;
  final String name;
  final String? description;
  final List<FieldSurveyMeasurement> measurements;
  final List<FieldAttachmentItem> attachments;

  FieldSurveyArea copyWith({
    String? id,
    String? code,
    String? name,
    String? description,
    List<FieldSurveyMeasurement>? measurements,
    List<FieldAttachmentItem>? attachments,
  }) => FieldSurveyArea(
    id: id ?? this.id,
    code: code ?? this.code,
    name: name ?? this.name,
    description: description ?? this.description,
    measurements: measurements ?? this.measurements,
    attachments: attachments ?? this.attachments,
  );

  Map<String, Object?> toJson() => <String, Object?>{
    'id': id,
    'code': code,
    'name': name,
    'description': description,
    'measurements': [for (final m in measurements) m.toJson()],
    'attachments': [for (final a in attachments) a.toJson()],
  };
}

/// Field survey record combining opportunity summary and current revision.
@immutable
class FieldSurveyRecord {
  const FieldSurveyRecord({
    required this.surveyId,
    required this.opportunityId,
    required this.opportunityCode,
    required this.opportunityTitle,
    required this.customerName,
    required this.siteName,
    required this.surveyNumber,
    required this.status,
    required this.revisionId,
    required this.revisionNumber,
    required this.rowVersion,
    required this.scopeSummary,
    this.scheduledAtUtc,
    this.areas = const [],
  });

  factory FieldSurveyRecord.fromJson(Map<String, Object?> json) {
    final rawAreas = json['areas'];
    final parsedAreas = <FieldSurveyArea>[];
    if (rawAreas is List<Object?>) {
      for (final item in rawAreas) {
        if (item is Map<String, Object?>) {
          parsedAreas.add(FieldSurveyArea.fromJson(item));
        }
      }
    }
    final revRaw = json['revisionNumber'];
    final revNum = revRaw is num ? revRaw.toInt() : 1;
    final schedStr = json['scheduledAtUtc'] as String?;
    return FieldSurveyRecord(
      surveyId: json['surveyId'] as String? ?? '',
      opportunityId: json['opportunityId'] as String? ?? '',
      opportunityCode: json['opportunityCode'] as String? ?? '',
      opportunityTitle: json['opportunityTitle'] as String? ?? '',
      customerName: json['customerName'] as String? ?? '',
      siteName: json['siteName'] as String? ?? '',
      surveyNumber: json['surveyNumber'] as String? ?? '',
      status: json['status'] as String? ?? 'draft',
      revisionId: json['revisionId'] as String? ?? '',
      revisionNumber: revNum,
      rowVersion: json['rowVersion'] as String? ?? '1',
      scopeSummary: json['scopeSummary'] as String? ?? '',
      scheduledAtUtc: schedStr != null ? DateTime.tryParse(schedStr) : null,
      areas: parsedAreas,
    );
  }

  final String surveyId;
  final String opportunityId;
  final String opportunityCode;
  final String opportunityTitle;
  final String customerName;
  final String siteName;
  final String surveyNumber;

  /// `draft | ready | scheduled | voided`.
  final String status;
  final String revisionId;
  final int revisionNumber;
  final String rowVersion;
  final String scopeSummary;
  final DateTime? scheduledAtUtc;
  final List<FieldSurveyArea> areas;

  FieldSurveyRecord copyWith({
    String? surveyId,
    String? opportunityId,
    String? opportunityCode,
    String? opportunityTitle,
    String? customerName,
    String? siteName,
    String? surveyNumber,
    String? status,
    String? revisionId,
    int? revisionNumber,
    String? rowVersion,
    String? scopeSummary,
    DateTime? scheduledAtUtc,
    List<FieldSurveyArea>? areas,
  }) => FieldSurveyRecord(
    surveyId: surveyId ?? this.surveyId,
    opportunityId: opportunityId ?? this.opportunityId,
    opportunityCode: opportunityCode ?? this.opportunityCode,
    opportunityTitle: opportunityTitle ?? this.opportunityTitle,
    customerName: customerName ?? this.customerName,
    siteName: siteName ?? this.siteName,
    surveyNumber: surveyNumber ?? this.surveyNumber,
    status: status ?? this.status,
    revisionId: revisionId ?? this.revisionId,
    revisionNumber: revisionNumber ?? this.revisionNumber,
    rowVersion: rowVersion ?? this.rowVersion,
    scopeSummary: scopeSummary ?? this.scopeSummary,
    scheduledAtUtc: scheduledAtUtc ?? this.scheduledAtUtc,
    areas: areas ?? this.areas,
  );

  Map<String, Object?> toJson() => <String, Object?>{
    'surveyId': surveyId,
    'opportunityId': opportunityId,
    'opportunityCode': opportunityCode,
    'opportunityTitle': opportunityTitle,
    'customerName': customerName,
    'siteName': siteName,
    'surveyNumber': surveyNumber,
    'status': status,
    'revisionId': revisionId,
    'revisionNumber': revisionNumber,
    'rowVersion': rowVersion,
    'scopeSummary': scopeSummary,
    'scheduledAtUtc': scheduledAtUtc?.toUtc().toIso8601String(),
    'areas': [for (final a in areas) a.toJson()],
  };
}
