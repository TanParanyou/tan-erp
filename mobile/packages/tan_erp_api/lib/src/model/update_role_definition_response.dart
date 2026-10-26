//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/role_change_request_response.dart';
import 'package:tan_erp_api/src/model/role_definition_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_role_definition_response.g.dart';

/// UpdateRoleDefinitionResponse
///
/// Properties:
/// * [role] 
/// * [pendingRequest] 
@BuiltValue()
abstract class UpdateRoleDefinitionResponse implements Built<UpdateRoleDefinitionResponse, UpdateRoleDefinitionResponseBuilder> {
  @BuiltValueField(wireName: r'role')
  RoleDefinitionResponse? get role;

  @BuiltValueField(wireName: r'pendingRequest')
  RoleChangeRequestResponse? get pendingRequest;

  UpdateRoleDefinitionResponse._();

  factory UpdateRoleDefinitionResponse([void updates(UpdateRoleDefinitionResponseBuilder b)]) = _$UpdateRoleDefinitionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateRoleDefinitionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateRoleDefinitionResponse> get serializer => _$UpdateRoleDefinitionResponseSerializer();
}

class _$UpdateRoleDefinitionResponseSerializer implements PrimitiveSerializer<UpdateRoleDefinitionResponse> {
  @override
  final Iterable<Type> types = const [UpdateRoleDefinitionResponse, _$UpdateRoleDefinitionResponse];

  @override
  final String wireName = r'UpdateRoleDefinitionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateRoleDefinitionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.role != null) {
      yield r'role';
      yield serializers.serialize(
        object.role,
        specifiedType: const FullType(RoleDefinitionResponse),
      );
    }
    if (object.pendingRequest != null) {
      yield r'pendingRequest';
      yield serializers.serialize(
        object.pendingRequest,
        specifiedType: const FullType(RoleChangeRequestResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateRoleDefinitionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateRoleDefinitionResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'role':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(RoleDefinitionResponse),
          ) as RoleDefinitionResponse?;
          if (valueDes == null) continue;
          result.role.replace(valueDes);
          break;
        case r'pendingRequest':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(RoleChangeRequestResponse),
          ) as RoleChangeRequestResponse?;
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
  UpdateRoleDefinitionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateRoleDefinitionResponseBuilder();
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


