//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'pipeline_stage_metrics_response.g.dart';

/// PipelineStageMetricsResponse
///
/// Properties:
/// * [stage] 
/// * [opportunityCount] 
/// * [expectedBudgetTotal] 
@BuiltValue()
abstract class PipelineStageMetricsResponse implements Built<PipelineStageMetricsResponse, PipelineStageMetricsResponseBuilder> {
  @BuiltValueField(wireName: r'stage')
  String? get stage;

  @BuiltValueField(wireName: r'opportunityCount')
  int? get opportunityCount;

  @BuiltValueField(wireName: r'expectedBudgetTotal')
  double? get expectedBudgetTotal;

  PipelineStageMetricsResponse._();

  factory PipelineStageMetricsResponse([void updates(PipelineStageMetricsResponseBuilder b)]) = _$PipelineStageMetricsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PipelineStageMetricsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PipelineStageMetricsResponse> get serializer => _$PipelineStageMetricsResponseSerializer();
}

class _$PipelineStageMetricsResponseSerializer implements PrimitiveSerializer<PipelineStageMetricsResponse> {
  @override
  final Iterable<Type> types = const [PipelineStageMetricsResponse, _$PipelineStageMetricsResponse];

  @override
  final String wireName = r'PipelineStageMetricsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PipelineStageMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.stage != null) {
      yield r'stage';
      yield serializers.serialize(
        object.stage,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.opportunityCount != null) {
      yield r'opportunityCount';
      yield serializers.serialize(
        object.opportunityCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.expectedBudgetTotal != null) {
      yield r'expectedBudgetTotal';
      yield serializers.serialize(
        object.expectedBudgetTotal,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PipelineStageMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PipelineStageMetricsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'stage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.stage = valueDes;
          break;
        case r'opportunityCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.opportunityCount = valueDes;
          break;
        case r'expectedBudgetTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.expectedBudgetTotal = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PipelineStageMetricsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PipelineStageMetricsResponseBuilder();
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


