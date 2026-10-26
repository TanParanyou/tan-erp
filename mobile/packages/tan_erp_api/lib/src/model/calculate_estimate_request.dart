//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'calculate_estimate_request.g.dart';

/// CalculateEstimateRequest
///
/// Properties:
/// * [expectedRevisionVersion] 
/// * [discountAmount] 
/// * [discountType] 
/// * [discountValue] 
/// * [discountReasonCode] 
@BuiltValue()
abstract class CalculateEstimateRequest implements Built<CalculateEstimateRequest, CalculateEstimateRequestBuilder> {
  @BuiltValueField(wireName: r'expectedRevisionVersion')
  String? get expectedRevisionVersion;

  @BuiltValueField(wireName: r'discountAmount')
  double? get discountAmount;

  @BuiltValueField(wireName: r'discountType')
  String? get discountType;

  @BuiltValueField(wireName: r'discountValue')
  double? get discountValue;

  @BuiltValueField(wireName: r'discountReasonCode')
  String? get discountReasonCode;

  CalculateEstimateRequest._();

  factory CalculateEstimateRequest([void updates(CalculateEstimateRequestBuilder b)]) = _$CalculateEstimateRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CalculateEstimateRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CalculateEstimateRequest> get serializer => _$CalculateEstimateRequestSerializer();
}

class _$CalculateEstimateRequestSerializer implements PrimitiveSerializer<CalculateEstimateRequest> {
  @override
  final Iterable<Type> types = const [CalculateEstimateRequest, _$CalculateEstimateRequest];

  @override
  final String wireName = r'CalculateEstimateRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CalculateEstimateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.expectedRevisionVersion != null) {
      yield r'expectedRevisionVersion';
      yield serializers.serialize(
        object.expectedRevisionVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.discountAmount != null) {
      yield r'discountAmount';
      yield serializers.serialize(
        object.discountAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.discountType != null) {
      yield r'discountType';
      yield serializers.serialize(
        object.discountType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.discountValue != null) {
      yield r'discountValue';
      yield serializers.serialize(
        object.discountValue,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.discountReasonCode != null) {
      yield r'discountReasonCode';
      yield serializers.serialize(
        object.discountReasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CalculateEstimateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CalculateEstimateRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'expectedRevisionVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedRevisionVersion = valueDes;
          break;
        case r'discountAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.discountAmount = valueDes;
          break;
        case r'discountType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.discountType = valueDes;
          break;
        case r'discountValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.discountValue = valueDes;
          break;
        case r'discountReasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.discountReasonCode = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CalculateEstimateRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CalculateEstimateRequestBuilder();
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


