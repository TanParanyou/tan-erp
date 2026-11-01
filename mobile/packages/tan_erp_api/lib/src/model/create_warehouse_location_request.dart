//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_warehouse_location_request.g.dart';

/// CreateWarehouseLocationRequest
///
/// Properties:
/// * [code] 
/// * [name] 
@BuiltValue()
abstract class CreateWarehouseLocationRequest implements Built<CreateWarehouseLocationRequest, CreateWarehouseLocationRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  CreateWarehouseLocationRequest._();

  factory CreateWarehouseLocationRequest([void updates(CreateWarehouseLocationRequestBuilder b)]) = _$CreateWarehouseLocationRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateWarehouseLocationRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateWarehouseLocationRequest> get serializer => _$CreateWarehouseLocationRequestSerializer();
}

class _$CreateWarehouseLocationRequestSerializer implements PrimitiveSerializer<CreateWarehouseLocationRequest> {
  @override
  final Iterable<Type> types = const [CreateWarehouseLocationRequest, _$CreateWarehouseLocationRequest];

  @override
  final String wireName = r'CreateWarehouseLocationRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateWarehouseLocationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
    CreateWarehouseLocationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateWarehouseLocationRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
  CreateWarehouseLocationRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateWarehouseLocationRequestBuilder();
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


