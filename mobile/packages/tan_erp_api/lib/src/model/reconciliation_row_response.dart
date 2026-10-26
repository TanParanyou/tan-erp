//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_item_ref_response.dart';
import 'package:tan_erp_api/src/model/inventory_warehouse_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'reconciliation_row_response.g.dart';

/// ReconciliationRowResponse
///
/// Properties:
/// * [warehouse] 
/// * [item] 
/// * [balanceOnHand] 
/// * [ledgerQuantity] 
/// * [balanceValue] 
/// * [ledgerValue] 
/// * [isConsistent] 
@BuiltValue()
abstract class ReconciliationRowResponse implements Built<ReconciliationRowResponse, ReconciliationRowResponseBuilder> {
  @BuiltValueField(wireName: r'warehouse')
  InventoryWarehouseRefResponse? get warehouse;

  @BuiltValueField(wireName: r'item')
  InventoryItemRefResponse? get item;

  @BuiltValueField(wireName: r'balanceOnHand')
  double? get balanceOnHand;

  @BuiltValueField(wireName: r'ledgerQuantity')
  double? get ledgerQuantity;

  @BuiltValueField(wireName: r'balanceValue')
  double? get balanceValue;

  @BuiltValueField(wireName: r'ledgerValue')
  double? get ledgerValue;

  @BuiltValueField(wireName: r'isConsistent')
  bool? get isConsistent;

  ReconciliationRowResponse._();

  factory ReconciliationRowResponse([void updates(ReconciliationRowResponseBuilder b)]) = _$ReconciliationRowResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ReconciliationRowResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ReconciliationRowResponse> get serializer => _$ReconciliationRowResponseSerializer();
}

class _$ReconciliationRowResponseSerializer implements PrimitiveSerializer<ReconciliationRowResponse> {
  @override
  final Iterable<Type> types = const [ReconciliationRowResponse, _$ReconciliationRowResponse];

  @override
  final String wireName = r'ReconciliationRowResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ReconciliationRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
    if (object.balanceOnHand != null) {
      yield r'balanceOnHand';
      yield serializers.serialize(
        object.balanceOnHand,
        specifiedType: const FullType(double),
      );
    }
    if (object.ledgerQuantity != null) {
      yield r'ledgerQuantity';
      yield serializers.serialize(
        object.ledgerQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.balanceValue != null) {
      yield r'balanceValue';
      yield serializers.serialize(
        object.balanceValue,
        specifiedType: const FullType(double),
      );
    }
    if (object.ledgerValue != null) {
      yield r'ledgerValue';
      yield serializers.serialize(
        object.ledgerValue,
        specifiedType: const FullType(double),
      );
    }
    if (object.isConsistent != null) {
      yield r'isConsistent';
      yield serializers.serialize(
        object.isConsistent,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ReconciliationRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ReconciliationRowResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
        case r'balanceOnHand':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.balanceOnHand = valueDes;
          break;
        case r'ledgerQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.ledgerQuantity = valueDes;
          break;
        case r'balanceValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.balanceValue = valueDes;
          break;
        case r'ledgerValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.ledgerValue = valueDes;
          break;
        case r'isConsistent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isConsistent = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ReconciliationRowResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ReconciliationRowResponseBuilder();
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


