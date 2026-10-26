//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_pagination_response.dart';
import 'package:tan_erp_api/src/model/stock_balance_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'stock_balance_list_response.g.dart';

/// StockBalanceListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
/// * [totalValue] 
@BuiltValue()
abstract class StockBalanceListResponse implements Built<StockBalanceListResponse, StockBalanceListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<StockBalanceResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  InventoryPaginationResponse? get pagination;

  @BuiltValueField(wireName: r'totalValue')
  double? get totalValue;

  StockBalanceListResponse._();

  factory StockBalanceListResponse([void updates(StockBalanceListResponseBuilder b)]) = _$StockBalanceListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(StockBalanceListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<StockBalanceListResponse> get serializer => _$StockBalanceListResponseSerializer();
}

class _$StockBalanceListResponseSerializer implements PrimitiveSerializer<StockBalanceListResponse> {
  @override
  final Iterable<Type> types = const [StockBalanceListResponse, _$StockBalanceListResponse];

  @override
  final String wireName = r'StockBalanceListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    StockBalanceListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(StockBalanceResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(InventoryPaginationResponse),
      );
    }
    if (object.totalValue != null) {
      yield r'totalValue';
      yield serializers.serialize(
        object.totalValue,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    StockBalanceListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required StockBalanceListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(StockBalanceResponse)]),
          ) as BuiltList<StockBalanceResponse>?;
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
        case r'totalValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalValue = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  StockBalanceListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = StockBalanceListResponseBuilder();
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


