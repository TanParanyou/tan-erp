//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_budget_summary_metrics_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_budget_summary_metrics_response_report_section_response.g.dart';

/// ProjectBudgetSummaryMetricsResponseReportSectionResponse
///
/// Properties:
/// * [permitted] 
/// * [summary] 
@BuiltValue()
abstract class ProjectBudgetSummaryMetricsResponseReportSectionResponse implements Built<ProjectBudgetSummaryMetricsResponseReportSectionResponse, ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder> {
  @BuiltValueField(wireName: r'permitted')
  bool? get permitted;

  @BuiltValueField(wireName: r'summary')
  ProjectBudgetSummaryMetricsResponse? get summary;

  ProjectBudgetSummaryMetricsResponseReportSectionResponse._();

  factory ProjectBudgetSummaryMetricsResponseReportSectionResponse([void updates(ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder b)]) = _$ProjectBudgetSummaryMetricsResponseReportSectionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectBudgetSummaryMetricsResponseReportSectionResponse> get serializer => _$ProjectBudgetSummaryMetricsResponseReportSectionResponseSerializer();
}

class _$ProjectBudgetSummaryMetricsResponseReportSectionResponseSerializer implements PrimitiveSerializer<ProjectBudgetSummaryMetricsResponseReportSectionResponse> {
  @override
  final Iterable<Type> types = const [ProjectBudgetSummaryMetricsResponseReportSectionResponse, _$ProjectBudgetSummaryMetricsResponseReportSectionResponse];

  @override
  final String wireName = r'ProjectBudgetSummaryMetricsResponseReportSectionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectBudgetSummaryMetricsResponseReportSectionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.permitted != null) {
      yield r'permitted';
      yield serializers.serialize(
        object.permitted,
        specifiedType: const FullType(bool),
      );
    }
    if (object.summary != null) {
      yield r'summary';
      yield serializers.serialize(
        object.summary,
        specifiedType: const FullType(ProjectBudgetSummaryMetricsResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectBudgetSummaryMetricsResponseReportSectionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'permitted':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.permitted = valueDes;
          break;
        case r'summary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectBudgetSummaryMetricsResponse),
          ) as ProjectBudgetSummaryMetricsResponse?;
          if (valueDes == null) continue;
          result.summary.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectBudgetSummaryMetricsResponseReportSectionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectBudgetSummaryMetricsResponseReportSectionResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


