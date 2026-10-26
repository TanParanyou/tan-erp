//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'opportunity_metrics_response.g.dart';

/// OpportunityMetricsResponse
///
/// Properties:
/// * [totalPipelineValue] 
/// * [activeDealsCount] 
/// * [closingThisMonthCount] 
/// * [wonValue] 
/// * [wonDealsCount] 
/// * [winRatePercentage] 
@BuiltValue()
abstract class OpportunityMetricsResponse implements Built<OpportunityMetricsResponse, OpportunityMetricsResponseBuilder> {
  @BuiltValueField(wireName: r'totalPipelineValue')
  double? get totalPipelineValue;

  @BuiltValueField(wireName: r'activeDealsCount')
  int? get activeDealsCount;

  @BuiltValueField(wireName: r'closingThisMonthCount')
  int? get closingThisMonthCount;

  @BuiltValueField(wireName: r'wonValue')
  double? get wonValue;

  @BuiltValueField(wireName: r'wonDealsCount')
  int? get wonDealsCount;

  @BuiltValueField(wireName: r'winRatePercentage')
  double? get winRatePercentage;

  OpportunityMetricsResponse._();

  factory OpportunityMetricsResponse([void updates(OpportunityMetricsResponseBuilder b)]) = _$OpportunityMetricsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OpportunityMetricsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OpportunityMetricsResponse> get serializer => _$OpportunityMetricsResponseSerializer();
}

class _$OpportunityMetricsResponseSerializer implements PrimitiveSerializer<OpportunityMetricsResponse> {
  @override
  final Iterable<Type> types = const [OpportunityMetricsResponse, _$OpportunityMetricsResponse];

  @override
  final String wireName = r'OpportunityMetricsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OpportunityMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.totalPipelineValue != null) {
      yield r'totalPipelineValue';
      yield serializers.serialize(
        object.totalPipelineValue,
        specifiedType: const FullType(double),
      );
    }
    if (object.activeDealsCount != null) {
      yield r'activeDealsCount';
      yield serializers.serialize(
        object.activeDealsCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.closingThisMonthCount != null) {
      yield r'closingThisMonthCount';
      yield serializers.serialize(
        object.closingThisMonthCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.wonValue != null) {
      yield r'wonValue';
      yield serializers.serialize(
        object.wonValue,
        specifiedType: const FullType(double),
      );
    }
    if (object.wonDealsCount != null) {
      yield r'wonDealsCount';
      yield serializers.serialize(
        object.wonDealsCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.winRatePercentage != null) {
      yield r'winRatePercentage';
      yield serializers.serialize(
        object.winRatePercentage,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OpportunityMetricsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OpportunityMetricsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'totalPipelineValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalPipelineValue = valueDes;
          break;
        case r'activeDealsCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.activeDealsCount = valueDes;
          break;
        case r'closingThisMonthCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.closingThisMonthCount = valueDes;
          break;
        case r'wonValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.wonValue = valueDes;
          break;
        case r'wonDealsCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.wonDealsCount = valueDes;
          break;
        case r'winRatePercentage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.winRatePercentage = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  OpportunityMetricsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OpportunityMetricsResponseBuilder();
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


