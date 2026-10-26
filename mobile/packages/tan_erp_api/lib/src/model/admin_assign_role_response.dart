//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/admin_user_response.dart';
import 'package:tan_erp_api/src/model/admin_role_request_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'admin_assign_role_response.g.dart';

/// AdminAssignRoleResponse
///
/// Properties:
/// * [user] 
/// * [pendingRequest] 
@BuiltValue()
abstract class AdminAssignRoleResponse implements Built<AdminAssignRoleResponse, AdminAssignRoleResponseBuilder> {
  @BuiltValueField(wireName: r'user')
  AdminUserResponse? get user;

  @BuiltValueField(wireName: r'pendingRequest')
  AdminRoleRequestResponse? get pendingRequest;

  AdminAssignRoleResponse._();

  factory AdminAssignRoleResponse([void updates(AdminAssignRoleResponseBuilder b)]) = _$AdminAssignRoleResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdminAssignRoleResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdminAssignRoleResponse> get serializer => _$AdminAssignRoleResponseSerializer();
}

class _$AdminAssignRoleResponseSerializer implements PrimitiveSerializer<AdminAssignRoleResponse> {
  @override
  final Iterable<Type> types = const [AdminAssignRoleResponse, _$AdminAssignRoleResponse];

  @override
  final String wireName = r'AdminAssignRoleResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdminAssignRoleResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.user != null) {
      yield r'user';
      yield serializers.serialize(
        object.user,
        specifiedType: const FullType(AdminUserResponse),
      );
    }
    if (object.pendingRequest != null) {
      yield r'pendingRequest';
      yield serializers.serialize(
        object.pendingRequest,
        specifiedType: const FullType(AdminRoleRequestResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AdminAssignRoleResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdminAssignRoleResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'user':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminUserResponse),
          ) as AdminUserResponse?;
          if (valueDes == null) continue;
          result.user.replace(valueDes);
          break;
        case r'pendingRequest':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminRoleRequestResponse),
          ) as AdminRoleRequestResponse?;
          if (valueDes == null) continue;
          result.pendingRequest.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AdminAssignRoleResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdminAssignRoleResponseBuilder();
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


