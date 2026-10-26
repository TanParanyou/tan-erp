//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_review_price_override_response.g.dart';

/// EstimateReviewPriceOverrideResponse
///
/// Properties:
/// * [workItemCode] 
/// * [workItemDescription] 
/// * [fixedPriceUnitAmount] 
/// * [reasonCode] 
@BuiltValue()
abstract class EstimateReviewPriceOverrideResponse implements Built<EstimateReviewPriceOverrideResponse, EstimateReviewPriceOverrideResponseBuilder> {
  @BuiltValueField(wireName: r'workItemCode')
  String? get workItemCode;

  @BuiltValueField(wireName: r'workItemDescription')
  String? get workItemDescription;

  @BuiltValueField(wireName: r'fixedPriceUnitAmount')
  double? get fixedPriceUnitAmount;

  @BuiltValueField(wireName: r'reasonCode')
  String? get reasonCode;

  EstimateReviewPriceOverrideResponse._();

  factory EstimateReviewPriceOverrideResponse([void updates(EstimateReviewPriceOverrideResponseBuilder b)]) = _$EstimateReviewPriceOverrideResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateReviewPriceOverrideResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateReviewPriceOverrideResponse> get serializer => _$EstimateReviewPriceOverrideResponseSerializer();
}

class _$EstimateReviewPriceOverrideResponseSerializer implements PrimitiveSerializer<EstimateReviewPriceOverrideResponse> {
  @override
  final Iterable<Type> types = const [EstimateReviewPriceOverrideResponse, _$EstimateReviewPriceOverrideResponse];

  @override
  final String wireName = r'EstimateReviewPriceOverrideResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateReviewPriceOverrideResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.workItemCode != null) {
      yield r'workItemCode';
      yield serializers.serialize(
        object.workItemCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.workItemDescription != null) {
      yield r'workItemDescription';
      yield serializers.serialize(
        object.workItemDescription,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.fixedPriceUnitAmount != null) {
      yield r'fixedPriceUnitAmount';
      yield serializers.serialize(
        object.fixedPriceUnitAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.reasonCode != null) {
      yield r'reasonCode';
      yield serializers.serialize(
        object.reasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateReviewPriceOverrideResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateReviewPriceOverrideResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'workItemCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workItemCode = valueDes;
          break;
        case r'workItemDescription':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workItemDescription = valueDes;
          break;
        case r'fixedPriceUnitAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.fixedPriceUnitAmount = valueDes;
          break;
        case r'reasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reasonCode = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateReviewPriceOverrideResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateReviewPriceOverrideResponseBuilder();
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


