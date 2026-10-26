//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_item_ref_response.dart';
import 'package:tan_erp_api/src/model/inventory_warehouse_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'stock_movement_response.g.dart';

/// StockMovementResponse
///
/// Properties:
/// * [id] 
/// * [stockDocumentId] 
/// * [documentType] 
/// * [documentNumber] 
/// * [warehouse] 
/// * [item] 
/// * [kind] 
/// * [quantityDelta] 
/// * [unitCost] 
/// * [valueDelta] 
/// * [onHandAfter] 
/// * [occurredAtUtc] 
/// * [postedAtUtc] 
@BuiltValue()
abstract class StockMovementResponse implements Built<StockMovementResponse, StockMovementResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'stockDocumentId')
  String? get stockDocumentId;

  @BuiltValueField(wireName: r'documentType')
  String? get documentType;

  @BuiltValueField(wireName: r'documentNumber')
  String? get documentNumber;

  @BuiltValueField(wireName: r'warehouse')
  InventoryWarehouseRefResponse? get warehouse;

  @BuiltValueField(wireName: r'item')
  InventoryItemRefResponse? get item;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'quantityDelta')
  double? get quantityDelta;

  @BuiltValueField(wireName: r'unitCost')
  double? get unitCost;

  @BuiltValueField(wireName: r'valueDelta')
  double? get valueDelta;

  @BuiltValueField(wireName: r'onHandAfter')
  double? get onHandAfter;

  @BuiltValueField(wireName: r'occurredAtUtc')
  DateTime? get occurredAtUtc;

  @BuiltValueField(wireName: r'postedAtUtc')
  DateTime? get postedAtUtc;

  StockMovementResponse._();

  factory StockMovementResponse([void updates(StockMovementResponseBuilder b)]) = _$StockMovementResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(StockMovementResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<StockMovementResponse> get serializer => _$StockMovementResponseSerializer();
}

class _$StockMovementResponseSerializer implements PrimitiveSerializer<StockMovementResponse> {
  @override
  final Iterable<Type> types = const [StockMovementResponse, _$StockMovementResponse];

  @override
  final String wireName = r'StockMovementResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    StockMovementResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.stockDocumentId != null) {
      yield r'stockDocumentId';
      yield serializers.serialize(
        object.stockDocumentId,
        specifiedType: const FullType(String),
      );
    }
    if (object.documentType != null) {
      yield r'documentType';
      yield serializers.serialize(
        object.documentType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.documentNumber != null) {
      yield r'documentNumber';
      yield serializers.serialize(
        object.documentNumber,
        specifiedType: const FullType.nullable(String),
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
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.quantityDelta != null) {
      yield r'quantityDelta';
      yield serializers.serialize(
        object.quantityDelta,
        specifiedType: const FullType(double),
      );
    }
    if (object.unitCost != null) {
      yield r'unitCost';
      yield serializers.serialize(
        object.unitCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.valueDelta != null) {
      yield r'valueDelta';
      yield serializers.serialize(
        object.valueDelta,
        specifiedType: const FullType(double),
      );
    }
    if (object.onHandAfter != null) {
      yield r'onHandAfter';
      yield serializers.serialize(
        object.onHandAfter,
        specifiedType: const FullType(double),
      );
    }
    if (object.occurredAtUtc != null) {
      yield r'occurredAtUtc';
      yield serializers.serialize(
        object.occurredAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.postedAtUtc != null) {
      yield r'postedAtUtc';
      yield serializers.serialize(
        object.postedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    StockMovementResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required StockMovementResponseBuilder result,
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
        case r'stockDocumentId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.stockDocumentId = valueDes;
          break;
        case r'documentType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.documentType = valueDes;
          break;
        case r'documentNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.documentNumber = valueDes;
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
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'quantityDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantityDelta = valueDes;
          break;
        case r'unitCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitCost = valueDes;
          break;
        case r'valueDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.valueDelta = valueDes;
          break;
        case r'onHandAfter':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.onHandAfter = valueDes;
          break;
        case r'occurredAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.occurredAtUtc = valueDes;
          break;
        case r'postedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.postedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  StockMovementResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = StockMovementResponseBuilder();
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


