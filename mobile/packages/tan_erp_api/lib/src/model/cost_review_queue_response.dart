//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/cost_review_queue_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'cost_review_queue_response.g.dart';

/// CostReviewQueueResponse
///
/// Properties:
/// * [items] 
/// * [totalCount] 
/// * [pageNumber] 
/// * [pageSize] 
@BuiltValue()
abstract class CostReviewQueueResponse implements Built<CostReviewQueueResponse, CostReviewQueueResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<CostReviewQueueItemResponse>? get items;

  @BuiltValueField(wireName: r'totalCount')
  int? get totalCount;

  @BuiltValueField(wireName: r'pageNumber')
  int? get pageNumber;

  @BuiltValueField(wireName: r'pageSize')
  int? get pageSize;

  CostReviewQueueResponse._();

  factory CostReviewQueueResponse([void updates(CostReviewQueueResponseBuilder b)]) = _$CostReviewQueueResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CostReviewQueueResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CostReviewQueueResponse> get serializer => _$CostReviewQueueResponseSerializer();
}

class _$CostReviewQueueResponseSerializer implements PrimitiveSerializer<CostReviewQueueResponse> {
  @override
  final Iterable<Type> types = const [CostReviewQueueResponse, _$CostReviewQueueResponse];

  @override
  final String wireName = r'CostReviewQueueResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CostReviewQueueResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CostReviewQueueItemResponse)]),
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
    CostReviewQueueResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CostReviewQueueResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CostReviewQueueItemResponse)]),
          ) as BuiltList<CostReviewQueueItemResponse>?;
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
  CostReviewQueueResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CostReviewQueueResponseBuilder();
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


