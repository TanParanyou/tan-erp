//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_item_ref_response.dart';
import 'package:tan_erp_api/src/model/inventory_warehouse_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'stock_balance_response.g.dart';

/// StockBalanceResponse
///
/// Properties:
/// * [id] 
/// * [warehouse] 
/// * [item] 
/// * [onHand] 
/// * [reserved] 
/// * [available] 
/// * [averageCost] 
/// * [totalValue] 
/// * [updatedAtUtc] 
@BuiltValue()
abstract class StockBalanceResponse implements Built<StockBalanceResponse, StockBalanceResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'warehouse')
  InventoryWarehouseRefResponse? get warehouse;

  @BuiltValueField(wireName: r'item')
  InventoryItemRefResponse? get item;

  @BuiltValueField(wireName: r'onHand')
  double? get onHand;

  @BuiltValueField(wireName: r'reserved')
  double? get reserved;

  @BuiltValueField(wireName: r'available')
  double? get available;

  @BuiltValueField(wireName: r'averageCost')
  double? get averageCost;

  @BuiltValueField(wireName: r'totalValue')
  double? get totalValue;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime? get updatedAtUtc;

  StockBalanceResponse._();

  factory StockBalanceResponse([void updates(StockBalanceResponseBuilder b)]) = _$StockBalanceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(StockBalanceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<StockBalanceResponse> get serializer => _$StockBalanceResponseSerializer();
}

class _$StockBalanceResponseSerializer implements PrimitiveSerializer<StockBalanceResponse> {
  @override
  final Iterable<Type> types = const [StockBalanceResponse, _$StockBalanceResponse];

  @override
  final String wireName = r'StockBalanceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    StockBalanceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.warehouse != null) {
      yield r'warehouse';
      yield serializers.serialize(
        object.warehouse,
        specifiedType: const FullType(InventoryWarehouseRefResponse),
      );
    }
    if (object.item != null) {
      yield r'item';
      yield serializers.serialize(
        object.item,
        specifiedType: const FullType(InventoryItemRefResponse),
      );
    }
    if (object.onHand != null) {
      yield r'onHand';
      yield serializers.serialize(
        object.onHand,
        specifiedType: const FullType(double),
      );
    }
    if (object.reserved != null) {
      yield r'reserved';
      yield serializers.serialize(
        object.reserved,
        specifiedType: const FullType(double),
      );
    }
    if (object.available != null) {
      yield r'available';
      yield serializers.serialize(
        object.available,
        specifiedType: const FullType(double),
      );
    }
    if (object.averageCost != null) {
      yield r'averageCost';
      yield serializers.serialize(
        object.averageCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalValue != null) {
      yield r'totalValue';
      yield serializers.serialize(
        object.totalValue,
        specifiedType: const FullType(double),
      );
    }
    if (object.updatedAtUtc != null) {
      yield r'updatedAtUtc';
      yield serializers.serialize(
        object.updatedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    StockBalanceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required StockBalanceResponseBuilder result,
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
        case r'warehouse':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(InventoryWarehouseRefResponse),
          ) as InventoryWarehouseRefResponse?;
          if (valueDes == null) continue;
          result.warehouse.replace(valueDes);
          break;
        case r'item':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(InventoryItemRefResponse),
          ) as InventoryItemRefResponse?;
          if (valueDes == null) continue;
          result.item.replace(valueDes);
          break;
        case r'onHand':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.onHand = valueDes;
          break;
        case r'reserved':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.reserved = valueDes;
          break;
        case r'available':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.available = valueDes;
          break;
        case r'averageCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.averageCost = valueDes;
          break;
        case r'totalValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalValue = valueDes;
          break;
        case r'updatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.updatedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  StockBalanceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = StockBalanceResponseBuilder();
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


