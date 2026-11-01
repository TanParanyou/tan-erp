//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/warehouse_location_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'warehouse_location_list_response.g.dart';

/// WarehouseLocationListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class WarehouseLocationListResponse implements Built<WarehouseLocationListResponse, WarehouseLocationListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<WarehouseLocationResponse>? get items;

  WarehouseLocationListResponse._();

  factory WarehouseLocationListResponse([void updates(WarehouseLocationListResponseBuilder b)]) = _$WarehouseLocationListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WarehouseLocationListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WarehouseLocationListResponse> get serializer => _$WarehouseLocationListResponseSerializer();
}

class _$WarehouseLocationListResponseSerializer implements PrimitiveSerializer<WarehouseLocationListResponse> {
  @override
  final Iterable<Type> types = const [WarehouseLocationListResponse, _$WarehouseLocationListResponse];

  @override
  final String wireName = r'WarehouseLocationListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WarehouseLocationListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(WarehouseLocationResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WarehouseLocationListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WarehouseLocationListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(WarehouseLocationResponse)]),
          ) as BuiltList<WarehouseLocationResponse>?;
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
  WarehouseLocationListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WarehouseLocationListResponseBuilder();
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


