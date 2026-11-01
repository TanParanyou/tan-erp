//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_item_ref_response.dart';
import 'package:tan_erp_api/src/model/inventory_warehouse_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'inventory_period_close_line_response.g.dart';

/// InventoryPeriodCloseLineResponse
///
/// Properties:
/// * [id] 
/// * [warehouse] 
/// * [item] 
/// * [onHandQuantity] 
/// * [averageCost] 
/// * [totalValue] 
@BuiltValue()
abstract class InventoryPeriodCloseLineResponse implements Built<InventoryPeriodCloseLineResponse, InventoryPeriodCloseLineResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'warehouse')
  InventoryWarehouseRefResponse? get warehouse;

  @BuiltValueField(wireName: r'item')
  InventoryItemRefResponse? get item;

  @BuiltValueField(wireName: r'onHandQuantity')
  double? get onHandQuantity;

  @BuiltValueField(wireName: r'averageCost')
  double? get averageCost;

  @BuiltValueField(wireName: r'totalValue')
  double? get totalValue;

  InventoryPeriodCloseLineResponse._();

  factory InventoryPeriodCloseLineResponse([void updates(InventoryPeriodCloseLineResponseBuilder b)]) = _$InventoryPeriodCloseLineResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(InventoryPeriodCloseLineResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<InventoryPeriodCloseLineResponse> get serializer => _$InventoryPeriodCloseLineResponseSerializer();
}

class _$InventoryPeriodCloseLineResponseSerializer implements PrimitiveSerializer<InventoryPeriodCloseLineResponse> {
  @override
  final Iterable<Type> types = const [InventoryPeriodCloseLineResponse, _$InventoryPeriodCloseLineResponse];

  @override
  final String wireName = r'InventoryPeriodCloseLineResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    InventoryPeriodCloseLineResponse object, {
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
    if (object.onHandQuantity != null) {
      yield r'onHandQuantity';
      yield serializers.serialize(
        object.onHandQuantity,
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
  }

  @override
  Object serialize(
    Serializers serializers,
    InventoryPeriodCloseLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required InventoryPeriodCloseLineResponseBuilder result,
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
        case r'onHandQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.onHandQuantity = valueDes;
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  InventoryPeriodCloseLineResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = InventoryPeriodCloseLineResponseBuilder();
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


