//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'pipeline_summary_metrics_response.g.dart';

/// PipelineSummaryMetricsResponse
///
/// Properties:
/// * [totalCount] 
/// * [totalExpectedBudget] 
/// * [openCount] 
/// * [openExpectedBudget] 
/// * [wonCount] 
/// * [wonBudget] 
/// * [lostCount] 
/// * [winRatePercent] 
@BuiltValue()
abstract class PipelineSummaryMetricsResponse implements Built<PipelineSummaryMetricsResponse, PipelineSummaryMetricsResponseBuilder> {
  @BuiltValueField(wireName: r'totalCount')
  int? get totalCount;

  @BuiltValueField(wireName: r'totalExpectedBudget')
  double? get totalExpectedBudget;

  @BuiltValueField(wireName: r'openCount')
  int? get openCount;

  @BuiltValueField(wireName: r'openExpectedBudget')
  double? get openExpectedBudget;

  @BuiltValueField(wireName: r'wonCount')
  int? get wonCount;

  @BuiltValueField(wireName: r'wonBudget')
  double? get wonBudget;

  @BuiltValueField(wireName: r'lostCount')
  int? get lostCount;

  @BuiltValueField(wireName: r'winRatePercent')
  double? get winRatePercent;

  PipelineSummaryMetricsResponse._();

  factory PipelineSummaryMetricsResponse([void updates(PipelineSummaryMetricsResponseBuilder b)]) = _$PipelineSummaryMetricsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PipelineSummaryMetricsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PipelineSummaryMetricsResponse> get serializer => _$PipelineSummaryMetricsResponseSerializer();
}

class _$PipelineSummaryMetricsResponseSerializer implements PrimitiveSerializer<PipelineSummaryMetricsResponse> {
  @override
  final Iterable<Type> types = const [PipelineSummaryMetricsResponse, _$PipelineSummaryMetricsResponse];

  @override
  final String wireName = r'PipelineSummaryMetricsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PipelineSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.totalCount != null) {
      yield r'totalCount';
      yield serializers.serialize(
        object.totalCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.totalExpectedBudget != null) {
      yield r'totalExpectedBudget';
      yield serializers.serialize(
        object.totalExpectedBudget,
        specifiedType: const FullType(double),
      );
    }
    if (object.openCount != null) {
      yield r'openCount';
      yield serializers.serialize(
        object.openCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.openExpectedBudget != null) {
      yield r'openExpectedBudget';
      yield serializers.serialize(
        object.openExpectedBudget,
        specifiedType: const FullType(double),
      );
    }
    if (object.wonCount != null) {
      yield r'wonCount';
      yield serializers.serialize(
        object.wonCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.wonBudget != null) {
      yield r'wonBudget';
      yield serializers.serialize(
        object.wonBudget,
        specifiedType: const FullType(double),
      );
    }
    if (object.lostCount != null) {
      yield r'lostCount';
      yield serializers.serialize(
        object.lostCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.winRatePercent != null) {
      yield r'winRatePercent';
      yield serializers.serialize(
        object.winRatePercent,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PipelineSummaryMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PipelineSummaryMetricsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'totalCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.totalCount = valueDes;
          break;
        case r'totalExpectedBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalExpectedBudget = valueDes;
          break;
        case r'openCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.openCount = valueDes;
          break;
        case r'openExpectedBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.openExpectedBudget = valueDes;
          break;
        case r'wonCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.wonCount = valueDes;
          break;
        case r'wonBudget':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.wonBudget = valueDes;
          break;
        case r'lostCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.lostCount = valueDes;
          break;
        case r'winRatePercent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.winRatePercent = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PipelineSummaryMetricsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PipelineSummaryMetricsResponseBuilder();
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


