//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_review_approval_trigger_response.g.dart';

/// EstimateReviewApprovalTriggerResponse
///
/// Properties:
/// * [code] 
/// * [actualValue] 
/// * [thresholdValue] 
/// * [unit] 
@BuiltValue()
abstract class EstimateReviewApprovalTriggerResponse implements Built<EstimateReviewApprovalTriggerResponse, EstimateReviewApprovalTriggerResponseBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'actualValue')
  double? get actualValue;

  @BuiltValueField(wireName: r'thresholdValue')
  double? get thresholdValue;

  @BuiltValueField(wireName: r'unit')
  String? get unit;

  EstimateReviewApprovalTriggerResponse._();

  factory EstimateReviewApprovalTriggerResponse([void updates(EstimateReviewApprovalTriggerResponseBuilder b)]) = _$EstimateReviewApprovalTriggerResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateReviewApprovalTriggerResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateReviewApprovalTriggerResponse> get serializer => _$EstimateReviewApprovalTriggerResponseSerializer();
}

class _$EstimateReviewApprovalTriggerResponseSerializer implements PrimitiveSerializer<EstimateReviewApprovalTriggerResponse> {
  @override
  final Iterable<Type> types = const [EstimateReviewApprovalTriggerResponse, _$EstimateReviewApprovalTriggerResponse];

  @override
  final String wireName = r'EstimateReviewApprovalTriggerResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateReviewApprovalTriggerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.actualValue != null) {
      yield r'actualValue';
      yield serializers.serialize(
        object.actualValue,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.thresholdValue != null) {
      yield r'thresholdValue';
      yield serializers.serialize(
        object.thresholdValue,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.unit != null) {
      yield r'unit';
      yield serializers.serialize(
        object.unit,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateReviewApprovalTriggerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateReviewApprovalTriggerResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'actualValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.actualValue = valueDes;
          break;
        case r'thresholdValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.thresholdValue = valueDes;
          break;
        case r'unit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unit = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateReviewApprovalTriggerResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateReviewApprovalTriggerResponseBuilder();
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


