//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_warehouse_location_request.g.dart';

/// UpdateWarehouseLocationRequest
///
/// Properties:
/// * [name] 
/// * [active] 
@BuiltValue()
abstract class UpdateWarehouseLocationRequest implements Built<UpdateWarehouseLocationRequest, UpdateWarehouseLocationRequestBuilder> {
  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'active')
  bool? get active;

  UpdateWarehouseLocationRequest._();

  factory UpdateWarehouseLocationRequest([void updates(UpdateWarehouseLocationRequestBuilder b)]) = _$UpdateWarehouseLocationRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateWarehouseLocationRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateWarehouseLocationRequest> get serializer => _$UpdateWarehouseLocationRequestSerializer();
}

class _$UpdateWarehouseLocationRequestSerializer implements PrimitiveSerializer<UpdateWarehouseLocationRequest> {
  @override
  final Iterable<Type> types = const [UpdateWarehouseLocationRequest, _$UpdateWarehouseLocationRequest];

  @override
  final String wireName = r'UpdateWarehouseLocationRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateWarehouseLocationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.active != null) {
      yield r'active';
      yield serializers.serialize(
        object.active,
        specifiedType: const FullType.nullable(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateWarehouseLocationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateWarehouseLocationRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'active':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.active = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateWarehouseLocationRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateWarehouseLocationRequestBuilder();
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


