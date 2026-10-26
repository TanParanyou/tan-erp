//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/admin_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'admin_role_request_response.g.dart';

/// AdminRoleRequestResponse
///
/// Properties:
/// * [id] 
/// * [rowVersion] 
/// * [status] 
/// * [role] 
/// * [membership] 
/// * [requestedBy] 
/// * [requestedAtUtc] 
@BuiltValue()
abstract class AdminRoleRequestResponse implements Built<AdminRoleRequestResponse, AdminRoleRequestResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'role')
  AdminRefResponse? get role;

  @BuiltValueField(wireName: r'membership')
  AdminRefResponse? get membership;

  @BuiltValueField(wireName: r'requestedBy')
  AdminRefResponse? get requestedBy;

  @BuiltValueField(wireName: r'requestedAtUtc')
  DateTime? get requestedAtUtc;

  AdminRoleRequestResponse._();

  factory AdminRoleRequestResponse([void updates(AdminRoleRequestResponseBuilder b)]) = _$AdminRoleRequestResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdminRoleRequestResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdminRoleRequestResponse> get serializer => _$AdminRoleRequestResponseSerializer();
}

class _$AdminRoleRequestResponseSerializer implements PrimitiveSerializer<AdminRoleRequestResponse> {
  @override
  final Iterable<Type> types = const [AdminRoleRequestResponse, _$AdminRoleRequestResponse];

  @override
  final String wireName = r'AdminRoleRequestResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdminRoleRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.role != null) {
      yield r'role';
      yield serializers.serialize(
        object.role,
        specifiedType: const FullType(AdminRefResponse),
      );
    }
    if (object.membership != null) {
      yield r'membership';
      yield serializers.serialize(
        object.membership,
        specifiedType: const FullType(AdminRefResponse),
      );
    }
    if (object.requestedBy != null) {
      yield r'requestedBy';
      yield serializers.serialize(
        object.requestedBy,
        specifiedType: const FullType(AdminRefResponse),
      );
    }
    if (object.requestedAtUtc != null) {
      yield r'requestedAtUtc';
      yield serializers.serialize(
        object.requestedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AdminRoleRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdminRoleRequestResponseBuilder result,
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
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'role':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminRefResponse),
          ) as AdminRefResponse?;
          if (valueDes == null) continue;
          result.role.replace(valueDes);
          break;
        case r'membership':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminRefResponse),
          ) as AdminRefResponse?;
          if (valueDes == null) continue;
          result.membership.replace(valueDes);
          break;
        case r'requestedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminRefResponse),
          ) as AdminRefResponse?;
          if (valueDes == null) continue;
          result.requestedBy.replace(valueDes);
          break;
        case r'requestedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.requestedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AdminRoleRequestResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdminRoleRequestResponseBuilder();
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


