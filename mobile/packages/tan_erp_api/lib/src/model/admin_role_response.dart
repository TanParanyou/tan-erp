//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'admin_role_response.g.dart';

/// AdminRoleResponse
///
/// Properties:
/// * [id] 
/// * [name] 
/// * [assignable] 
/// * [requiresApproval] 
/// * [permissionKeys] 
@BuiltValue()
abstract class AdminRoleResponse implements Built<AdminRoleResponse, AdminRoleResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'assignable')
  bool? get assignable;

  @BuiltValueField(wireName: r'requiresApproval')
  bool? get requiresApproval;

  @BuiltValueField(wireName: r'permissionKeys')
  BuiltList<String>? get permissionKeys;

  AdminRoleResponse._();

  factory AdminRoleResponse([void updates(AdminRoleResponseBuilder b)]) = _$AdminRoleResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdminRoleResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdminRoleResponse> get serializer => _$AdminRoleResponseSerializer();
}

class _$AdminRoleResponseSerializer implements PrimitiveSerializer<AdminRoleResponse> {
  @override
  final Iterable<Type> types = const [AdminRoleResponse, _$AdminRoleResponse];

  @override
  final String wireName = r'AdminRoleResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdminRoleResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.assignable != null) {
      yield r'assignable';
      yield serializers.serialize(
        object.assignable,
        specifiedType: const FullType(bool),
      );
    }
    if (object.requiresApproval != null) {
      yield r'requiresApproval';
      yield serializers.serialize(
        object.requiresApproval,
        specifiedType: const FullType(bool),
      );
    }
    if (object.permissionKeys != null) {
      yield r'permissionKeys';
      yield serializers.serialize(
        object.permissionKeys,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AdminRoleResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdminRoleResponseBuilder result,
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
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'assignable':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.assignable = valueDes;
          break;
        case r'requiresApproval':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.requiresApproval = valueDes;
          break;
        case r'permissionKeys':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.permissionKeys.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AdminRoleResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdminRoleResponseBuilder();
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


