import 'package:tan_erp_field/features/survey/domain/field_survey_models.dart';

/// Same idea as the web's completeness check: warn, never block the save.
enum FieldSurveyWarningCode { noMeasurement, zeroValue, noPhoto }

class FieldSurveyWarning {
  const FieldSurveyWarning({required this.area, required this.code});

  final FieldSurveyArea area;
  final FieldSurveyWarningCode code;
}

/// Areas with no measurement, a zero/negative value, or no photo.
List<FieldSurveyWarning> evaluateFieldSurvey(FieldSurveyRecord record) => [
  for (final area in record.areas) ...[
    if (area.measurements.isEmpty)
      FieldSurveyWarning(area: area, code: FieldSurveyWarningCode.noMeasurement)
    else if (area.measurements.any((m) => m.value <= 0))
      FieldSurveyWarning(area: area, code: FieldSurveyWarningCode.zeroValue),
    if (area.attachments.isEmpty)
      FieldSurveyWarning(area: area, code: FieldSurveyWarningCode.noPhoto),
  ],
];
