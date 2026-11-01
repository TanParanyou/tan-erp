//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/inventory_period_close_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'inventory_period_close_list_response.g.dart';

/// InventoryPeriodCloseListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class InventoryPeriodCloseListResponse implements Built<InventoryPeriodCloseListResponse, InventoryPeriodCloseListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<InventoryPeriodCloseResponse>? get items;

  InventoryPeriodCloseListResponse._();

  factory InventoryPeriodCloseListResponse([void updates(InventoryPeriodCloseListResponseBuilder b)]) = _$InventoryPeriodCloseListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(InventoryPeriodCloseListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<InventoryPeriodCloseListResponse> get serializer => _$InventoryPeriodCloseListResponseSerializer();
}

class _$InventoryPeriodCloseListResponseSerializer implements PrimitiveSerializer<InventoryPeriodCloseListResponse> {
  @override
  final Iterable<Type> types = const [InventoryPeriodCloseListResponse, _$InventoryPeriodCloseListResponse];

  @override
  final String wireName = r'InventoryPeriodCloseListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    InventoryPeriodCloseListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(InventoryPeriodCloseResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    InventoryPeriodCloseListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required InventoryPeriodCloseListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(InventoryPeriodCloseResponse)]),
          ) as BuiltList<InventoryPeriodCloseResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  InventoryPeriodCloseListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = InventoryPeriodCloseListResponseBuilder();
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


