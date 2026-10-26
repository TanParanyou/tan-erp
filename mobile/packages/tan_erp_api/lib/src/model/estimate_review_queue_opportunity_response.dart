//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_review_queue_opportunity_response.g.dart';

/// EstimateReviewQueueOpportunityResponse
///
/// Properties:
/// * [id] 
/// * [title] 
@BuiltValue()
abstract class EstimateReviewQueueOpportunityResponse implements Built<EstimateReviewQueueOpportunityResponse, EstimateReviewQueueOpportunityResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'title')
  String? get title;

  EstimateReviewQueueOpportunityResponse._();

  factory EstimateReviewQueueOpportunityResponse([void updates(EstimateReviewQueueOpportunityResponseBuilder b)]) = _$EstimateReviewQueueOpportunityResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateReviewQueueOpportunityResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateReviewQueueOpportunityResponse> get serializer => _$EstimateReviewQueueOpportunityResponseSerializer();
}

class _$EstimateReviewQueueOpportunityResponseSerializer implements PrimitiveSerializer<EstimateReviewQueueOpportunityResponse> {
  @override
  final Iterable<Type> types = const [EstimateReviewQueueOpportunityResponse, _$EstimateReviewQueueOpportunityResponse];

  @override
  final String wireName = r'EstimateReviewQueueOpportunityResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateReviewQueueOpportunityResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.title != null) {
      yield r'title';
      yield serializers.serialize(
        object.title,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateReviewQueueOpportunityResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateReviewQueueOpportunityResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'title':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.title = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateReviewQueueOpportunityResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateReviewQueueOpportunityResponseBuilder();
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


