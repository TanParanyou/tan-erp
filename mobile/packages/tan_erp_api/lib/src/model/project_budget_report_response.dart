//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_budget_report_row_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/project_budget_summary_metrics_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_budget_report_response.g.dart';

/// ProjectBudgetReportResponse
///
/// Properties:
/// * [summary] 
/// * [rows] 
@BuiltValue()
abstract class ProjectBudgetReportResponse implements Built<ProjectBudgetReportResponse, ProjectBudgetReportResponseBuilder> {
  @BuiltValueField(wireName: r'summary')
  ProjectBudgetSummaryMetricsResponse? get summary;

  @BuiltValueField(wireName: r'rows')
  BuiltList<ProjectBudgetReportRowResponse>? get rows;

  ProjectBudgetReportResponse._();

  factory ProjectBudgetReportResponse([void updates(ProjectBudgetReportResponseBuilder b)]) = _$ProjectBudgetReportResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectBudgetReportResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectBudgetReportResponse> get serializer => _$ProjectBudgetReportResponseSerializer();
}

class _$ProjectBudgetReportResponseSerializer implements PrimitiveSerializer<ProjectBudgetReportResponse> {
  @override
  final Iterable<Type> types = const [ProjectBudgetReportResponse, _$ProjectBudgetReportResponse];

  @override
  final String wireName = r'ProjectBudgetReportResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectBudgetReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.summary != null) {
      yield r'summary';
      yield serializers.serialize(
        object.summary,
        specifiedType: const FullType(ProjectBudgetSummaryMetricsResponse),
      );
    }
    if (object.rows != null) {
      yield r'rows';
      yield serializers.serialize(
        object.rows,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectBudgetReportRowResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectBudgetReportResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectBudgetReportResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'summary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectBudgetSummaryMetricsResponse),
          ) as ProjectBudgetSummaryMetricsResponse?;
          if (valueDes == null) continue;
          result.summary.replace(valueDes);
          break;
        case r'rows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ProjectBudgetReportRowResponse)]),
          ) as BuiltList<ProjectBudgetReportRowResponse>?;
          if (valueDes == null) continue;
          result.rows.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectBudgetReportResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectBudgetReportResponseBuilder();
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


