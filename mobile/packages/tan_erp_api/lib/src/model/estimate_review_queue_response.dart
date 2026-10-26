//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/estimate_review_queue_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_review_queue_response.g.dart';

/// EstimateReviewQueueResponse
///
/// Properties:
/// * [items] 
/// * [totalCount] 
/// * [pageNumber] 
/// * [pageSize] 
@BuiltValue()
abstract class EstimateReviewQueueResponse implements Built<EstimateReviewQueueResponse, EstimateReviewQueueResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<EstimateReviewQueueItemResponse>? get items;

  @BuiltValueField(wireName: r'totalCount')
  int? get totalCount;

  @BuiltValueField(wireName: r'pageNumber')
  int? get pageNumber;

  @BuiltValueField(wireName: r'pageSize')
  int? get pageSize;

  EstimateReviewQueueResponse._();

  factory EstimateReviewQueueResponse([void updates(EstimateReviewQueueResponseBuilder b)]) = _$EstimateReviewQueueResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateReviewQueueResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateReviewQueueResponse> get serializer => _$EstimateReviewQueueResponseSerializer();
}

class _$EstimateReviewQueueResponseSerializer implements PrimitiveSerializer<EstimateReviewQueueResponse> {
  @override
  final Iterable<Type> types = const [EstimateReviewQueueResponse, _$EstimateReviewQueueResponse];

  @override
  final String wireName = r'EstimateReviewQueueResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateReviewQueueResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReviewQueueItemResponse)]),
      );
    }
    if (object.totalCount != null) {
      yield r'totalCount';
      yield serializers.serialize(
        object.totalCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.pageNumber != null) {
      yield r'pageNumber';
      yield serializers.serialize(
        object.pageNumber,
        specifiedType: const FullType(int),
      );
    }
    if (object.pageSize != null) {
      yield r'pageSize';
      yield serializers.serialize(
        object.pageSize,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateReviewQueueResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateReviewQueueResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReviewQueueItemResponse)]),
          ) as BuiltList<EstimateReviewQueueItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'totalCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.totalCount = valueDes;
          break;
        case r'pageNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.pageNumber = valueDes;
          break;
        case r'pageSize':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.pageSize = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateReviewQueueResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateReviewQueueResponseBuilder();
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


