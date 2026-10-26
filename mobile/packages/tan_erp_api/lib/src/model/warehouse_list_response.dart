//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_pagination_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/warehouse_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'warehouse_list_response.g.dart';

/// WarehouseListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class WarehouseListResponse implements Built<WarehouseListResponse, WarehouseListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<WarehouseResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  InventoryPaginationResponse? get pagination;

  WarehouseListResponse._();

  factory WarehouseListResponse([void updates(WarehouseListResponseBuilder b)]) = _$WarehouseListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WarehouseListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WarehouseListResponse> get serializer => _$WarehouseListResponseSerializer();
}

class _$WarehouseListResponseSerializer implements PrimitiveSerializer<WarehouseListResponse> {
  @override
  final Iterable<Type> types = const [WarehouseListResponse, _$WarehouseListResponse];

  @override
  final String wireName = r'WarehouseListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WarehouseListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(WarehouseResponse)]),
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
    WarehouseListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WarehouseListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(WarehouseResponse)]),
          ) as BuiltList<WarehouseResponse>?;
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
  WarehouseListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WarehouseListResponseBuilder();
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


