//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/admin_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'role_grant_response.g.dart';

/// RoleGrantResponse
///
/// Properties:
/// * [permissionKey] 
/// * [scope] 
/// * [branch] 
@BuiltValue()
abstract class RoleGrantResponse implements Built<RoleGrantResponse, RoleGrantResponseBuilder> {
  @BuiltValueField(wireName: r'permissionKey')
  String? get permissionKey;

  @BuiltValueField(wireName: r'scope')
  String? get scope;

  @BuiltValueField(wireName: r'branch')
  AdminRefResponse? get branch;

  RoleGrantResponse._();

  factory RoleGrantResponse([void updates(RoleGrantResponseBuilder b)]) = _$RoleGrantResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RoleGrantResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RoleGrantResponse> get serializer => _$RoleGrantResponseSerializer();
}

class _$RoleGrantResponseSerializer implements PrimitiveSerializer<RoleGrantResponse> {
  @override
  final Iterable<Type> types = const [RoleGrantResponse, _$RoleGrantResponse];

  @override
  final String wireName = r'RoleGrantResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RoleGrantResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.permissionKey != null) {
      yield r'permissionKey';
      yield serializers.serialize(
        object.permissionKey,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.scope != null) {
      yield r'scope';
      yield serializers.serialize(
        object.scope,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.branch != null) {
      yield r'branch';
      yield serializers.serialize(
        object.branch,
        specifiedType: const FullType(AdminRefResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RoleGrantResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RoleGrantResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'permissionKey':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.permissionKey = valueDes;
          break;
        case r'scope':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.scope = valueDes;
          break;
        case r'branch':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminRefResponse),
          ) as AdminRefResponse?;
          if (valueDes == null) continue;
          result.branch.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  RoleGrantResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RoleGrantResponseBuilder();
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


