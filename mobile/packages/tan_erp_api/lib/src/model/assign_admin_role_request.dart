//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'assign_admin_role_request.g.dart';

/// AssignAdminRoleRequest
///
/// Properties:
/// * [roleId] 
@BuiltValue()
abstract class AssignAdminRoleRequest implements Built<AssignAdminRoleRequest, AssignAdminRoleRequestBuilder> {
  @BuiltValueField(wireName: r'roleId')
  String? get roleId;

  AssignAdminRoleRequest._();

  factory AssignAdminRoleRequest([void updates(AssignAdminRoleRequestBuilder b)]) = _$AssignAdminRoleRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AssignAdminRoleRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AssignAdminRoleRequest> get serializer => _$AssignAdminRoleRequestSerializer();
}

class _$AssignAdminRoleRequestSerializer implements PrimitiveSerializer<AssignAdminRoleRequest> {
  @override
  final Iterable<Type> types = const [AssignAdminRoleRequest, _$AssignAdminRoleRequest];

  @override
  final String wireName = r'AssignAdminRoleRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AssignAdminRoleRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.roleId != null) {
      yield r'roleId';
      yield serializers.serialize(
        object.roleId,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AssignAdminRoleRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AssignAdminRoleRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'roleId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.roleId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AssignAdminRoleRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AssignAdminRoleRequestBuilder();
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


