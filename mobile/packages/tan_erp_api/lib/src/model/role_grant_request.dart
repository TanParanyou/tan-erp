//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'role_grant_request.g.dart';

/// RoleGrantRequest
///
/// Properties:
/// * [permissionKey] 
/// * [scope] 
/// * [branchId] 
@BuiltValue()
abstract class RoleGrantRequest implements Built<RoleGrantRequest, RoleGrantRequestBuilder> {
  @BuiltValueField(wireName: r'permissionKey')
  String? get permissionKey;

  @BuiltValueField(wireName: r'scope')
  String? get scope;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  RoleGrantRequest._();

  factory RoleGrantRequest([void updates(RoleGrantRequestBuilder b)]) = _$RoleGrantRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RoleGrantRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RoleGrantRequest> get serializer => _$RoleGrantRequestSerializer();
}

class _$RoleGrantRequestSerializer implements PrimitiveSerializer<RoleGrantRequest> {
  @override
  final Iterable<Type> types = const [RoleGrantRequest, _$RoleGrantRequest];

  @override
  final String wireName = r'RoleGrantRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RoleGrantRequest object, {
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
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RoleGrantRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RoleGrantRequestBuilder result,
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
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  RoleGrantRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RoleGrantRequestBuilder();
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


