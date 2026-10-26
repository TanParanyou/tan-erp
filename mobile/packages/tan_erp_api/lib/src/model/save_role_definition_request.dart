//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/role_grant_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'save_role_definition_request.g.dart';

/// SaveRoleDefinitionRequest
///
/// Properties:
/// * [name] 
/// * [description] 
/// * [permissions] 
@BuiltValue()
abstract class SaveRoleDefinitionRequest implements Built<SaveRoleDefinitionRequest, SaveRoleDefinitionRequestBuilder> {
  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'permissions')
  BuiltList<RoleGrantRequest>? get permissions;

  SaveRoleDefinitionRequest._();

  factory SaveRoleDefinitionRequest([void updates(SaveRoleDefinitionRequestBuilder b)]) = _$SaveRoleDefinitionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SaveRoleDefinitionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SaveRoleDefinitionRequest> get serializer => _$SaveRoleDefinitionRequestSerializer();
}

class _$SaveRoleDefinitionRequestSerializer implements PrimitiveSerializer<SaveRoleDefinitionRequest> {
  @override
  final Iterable<Type> types = const [SaveRoleDefinitionRequest, _$SaveRoleDefinitionRequest];

  @override
  final String wireName = r'SaveRoleDefinitionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SaveRoleDefinitionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.permissions != null) {
      yield r'permissions';
      yield serializers.serialize(
        object.permissions,
        specifiedType: const FullType.nullable(BuiltList, [FullType(RoleGrantRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SaveRoleDefinitionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SaveRoleDefinitionRequestBuilder result,
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
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'permissions':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(RoleGrantRequest)]),
          ) as BuiltList<RoleGrantRequest>?;
          if (valueDes == null) continue;
          result.permissions.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SaveRoleDefinitionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SaveRoleDefinitionRequestBuilder();
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


