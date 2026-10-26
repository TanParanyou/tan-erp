//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_review_readiness_reason_response.g.dart';

/// EstimateReviewReadinessReasonResponse
///
/// Properties:
/// * [code] 
/// * [targetType] 
/// * [targetId] 
/// * [targetField] 
@BuiltValue()
abstract class EstimateReviewReadinessReasonResponse implements Built<EstimateReviewReadinessReasonResponse, EstimateReviewReadinessReasonResponseBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'targetType')
  String? get targetType;

  @BuiltValueField(wireName: r'targetId')
  String? get targetId;

  @BuiltValueField(wireName: r'targetField')
  String? get targetField;

  EstimateReviewReadinessReasonResponse._();

  factory EstimateReviewReadinessReasonResponse([void updates(EstimateReviewReadinessReasonResponseBuilder b)]) = _$EstimateReviewReadinessReasonResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateReviewReadinessReasonResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateReviewReadinessReasonResponse> get serializer => _$EstimateReviewReadinessReasonResponseSerializer();
}

class _$EstimateReviewReadinessReasonResponseSerializer implements PrimitiveSerializer<EstimateReviewReadinessReasonResponse> {
  @override
  final Iterable<Type> types = const [EstimateReviewReadinessReasonResponse, _$EstimateReviewReadinessReasonResponse];

  @override
  final String wireName = r'EstimateReviewReadinessReasonResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateReviewReadinessReasonResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.targetType != null) {
      yield r'targetType';
      yield serializers.serialize(
        object.targetType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.targetId != null) {
      yield r'targetId';
      yield serializers.serialize(
        object.targetId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.targetField != null) {
      yield r'targetField';
      yield serializers.serialize(
        object.targetField,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateReviewReadinessReasonResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateReviewReadinessReasonResponseBuilder result,
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
        case r'targetType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.targetType = valueDes;
          break;
        case r'targetId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.targetId = valueDes;
          break;
        case r'targetField':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.targetField = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateReviewReadinessReasonResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateReviewReadinessReasonResponseBuilder();
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


