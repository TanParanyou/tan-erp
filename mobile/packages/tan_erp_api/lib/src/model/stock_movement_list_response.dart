//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_pagination_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/stock_movement_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'stock_movement_list_response.g.dart';

/// StockMovementListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class StockMovementListResponse implements Built<StockMovementListResponse, StockMovementListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<StockMovementResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  InventoryPaginationResponse? get pagination;

  StockMovementListResponse._();

  factory StockMovementListResponse([void updates(StockMovementListResponseBuilder b)]) = _$StockMovementListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(StockMovementListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<StockMovementListResponse> get serializer => _$StockMovementListResponseSerializer();
}

class _$StockMovementListResponseSerializer implements PrimitiveSerializer<StockMovementListResponse> {
  @override
  final Iterable<Type> types = const [StockMovementListResponse, _$StockMovementListResponse];

  @override
  final String wireName = r'StockMovementListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    StockMovementListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(StockMovementResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(InventoryPaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    StockMovementListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required StockMovementListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(StockMovementResponse)]),
          ) as BuiltList<StockMovementResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(InventoryPaginationResponse),
          ) as InventoryPaginationResponse?;
          if (valueDes == null) continue;
          result.pagination.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  StockMovementListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = StockMovementListResponseBuilder();
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


