//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_budget_summary_metrics_response.g.dart';

/// ProjectBudgetSummaryMetricsResponse
///
/// Properties:
/// * [projectCount] 
/// * [overBudgetCount] 
/// * [totalBaselineBudget] 
/// * [totalApprovedChangeOrderBudget] 
/// * [totalCurrentBudget] 
/// * [totalCommittedAmount] 
/// * [totalActualAmount] 
/// * [totalVarianceAmount] 
@BuiltValue()
abstract class ProjectBudgetSummaryMetricsResponse implements Built<ProjectBudgetSummaryMetricsResponse, ProjectBudgetSummaryMetricsResponseBuilder> {
  @BuiltValueField(wireName: r'projectCount')
  int? get projectCount;

  @BuiltValueField(wireName: r'overBudgetCount')
  int? get overBudgetCount;

  @BuiltValueField(wireName: r'totalBaselineBudget')
  double? get totalBaselineBudget;

  @BuiltValueField(wireName: r'totalApprovedChangeOrderBudget')
  double? get totalApprovedChangeOrderBudget;

  @BuiltValueField(wireName: r'totalCurrentBudget')
  double? get totalCurrentBudget;

  @BuiltValueField(wireName: r'totalCommittedAmount')
  double? get totalCommittedAmount;

  @BuiltValueField(wireName: r'totalActualAmount')
  double? get totalActualAmount;

  @BuiltValueField(wireName: r'totalVarianceAmount')
  double? get totalVarianceAmount;

  ProjectBudgetSummaryMetricsResponse._();

  factory ProjectBudgetSummaryMetricsResponse([void updates(ProjectBudgetSummaryMetricsResponseBuilder b)]) = _$ProjectBudgetSummaryMetricsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectBudgetSummaryMetricsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectBudgetSummaryMetricsResponse> get serializer => _$ProjectBudgetSummaryMetricsResponseSerializer();
}

class _$ProjectBudgetSummaryMetricsResponseSerializer implements PrimitiveSerializer<ProjectBudgetSummaryMetricsResponse> {
  @override
  final Iterable<Type> types = const [ProjectBudgetSummaryMetricsResponse, _$ProjectBudgetSummaryMetricsResponse];

  @override
  final String wireName = r'ProjectBudgetSummaryMetricsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectBudgetSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.projectCount != null) {
      yield r'projectCount';
      yield serializers.serialize(
        object.projectCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.overBudgetCount != null) {
      yield r'overBudgetCount';
      yield serializers.serialize(
        object.overBudgetCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.totalBaselineBudget != null) {
      yield r'totalBaselineBudget';
      yield serializers.serialize(
        object.totalBaselineBudget,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalApprovedChangeOrderBudget != null) {
      yield r'totalApprovedChangeOrderBudget';
      yield serializers.serialize(
        object.totalApprovedChangeOrderBudget,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalCurrentBudget != null) {
      yield r'totalCurrentBudget';
      yield serializers.serialize(
        object.totalCurrentBudget,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalCommittedAmount != null) {
      yield r'totalCommittedAmount';
      yield serializers.serialize(
        object.totalCommittedAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalActualAmount != null) {
      yield r'totalActualAmount';
      yield serializers.serialize(
        object.totalActualAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalVarianceAmount != null) {
      yield r'totalVarianceAmount';
      yield serializers.serialize(
        object.totalVarianceAmount,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectBudgetSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectBudgetSummaryMetricsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'projectCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.projectCount = valueDes;
          break;
        case r'overBudgetCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.overBudgetCount = valueDes;
          break;
        case r'totalBaselineBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalBaselineBudget = valueDes;
          break;
        case r'totalApprovedChangeOrderBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalApprovedChangeOrderBudget = valueDes;
          break;
        case r'totalCurrentBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalCurrentBudget = valueDes;
          break;
        case r'totalCommittedAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalCommittedAmount = valueDes;
          break;
        case r'totalActualAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalActualAmount = valueDes;
          break;
        case r'totalVarianceAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalVarianceAmount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectBudgetSummaryMetricsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectBudgetSummaryMetricsResponseBuilder();
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


