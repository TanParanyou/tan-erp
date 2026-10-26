//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'inventory_warehouse_ref_response.g.dart';

/// InventoryWarehouseRefResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [name] 
@BuiltValue()
abstract class InventoryWarehouseRefResponse implements Built<InventoryWarehouseRefResponse, InventoryWarehouseRefResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  InventoryWarehouseRefResponse._();

  factory InventoryWarehouseRefResponse([void updates(InventoryWarehouseRefResponseBuilder b)]) = _$InventoryWarehouseRefResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(InventoryWarehouseRefResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<InventoryWarehouseRefResponse> get serializer => _$InventoryWarehouseRefResponseSerializer();
}

class _$InventoryWarehouseRefResponseSerializer implements PrimitiveSerializer<InventoryWarehouseRefResponse> {
  @override
  final Iterable<Type> types = const [InventoryWarehouseRefResponse, _$InventoryWarehouseRefResponse];

  @override
  final String wireName = r'InventoryWarehouseRefResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    InventoryWarehouseRefResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    InventoryWarehouseRefResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required InventoryWarehouseRefResponseBuilder result,
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
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  InventoryWarehouseRefResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = InventoryWarehouseRefResponseBuilder();
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


