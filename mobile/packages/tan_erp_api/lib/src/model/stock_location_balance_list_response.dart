//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_pagination_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/stock_location_balance_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'stock_location_balance_list_response.g.dart';

/// StockLocationBalanceListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class StockLocationBalanceListResponse implements Built<StockLocationBalanceListResponse, StockLocationBalanceListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<StockLocationBalanceResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  InventoryPaginationResponse? get pagination;

  StockLocationBalanceListResponse._();

  factory StockLocationBalanceListResponse([void updates(StockLocationBalanceListResponseBuilder b)]) = _$StockLocationBalanceListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(StockLocationBalanceListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<StockLocationBalanceListResponse> get serializer => _$StockLocationBalanceListResponseSerializer();
}

class _$StockLocationBalanceListResponseSerializer implements PrimitiveSerializer<StockLocationBalanceListResponse> {
  @override
  final Iterable<Type> types = const [StockLocationBalanceListResponse, _$StockLocationBalanceListResponse];

  @override
  final String wireName = r'StockLocationBalanceListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    StockLocationBalanceListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(StockLocationBalanceResponse)]),
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
    StockLocationBalanceListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required StockLocationBalanceListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(StockLocationBalanceResponse)]),
          ) as BuiltList<StockLocationBalanceResponse>?;
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
  StockLocationBalanceListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = StockLocationBalanceListResponseBuilder();
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


