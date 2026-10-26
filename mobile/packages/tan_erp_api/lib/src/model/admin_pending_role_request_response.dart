//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/admin_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'admin_pending_role_request_response.g.dart';

/// AdminPendingRoleRequestResponse
///
/// Properties:
/// * [id] 
/// * [rowVersion] 
/// * [role] 
/// * [requestedBy] 
/// * [requestedAtUtc] 
@BuiltValue()
abstract class AdminPendingRoleRequestResponse implements Built<AdminPendingRoleRequestResponse, AdminPendingRoleRequestResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'role')
  AdminRefResponse? get role;

  @BuiltValueField(wireName: r'requestedBy')
  AdminRefResponse? get requestedBy;

  @BuiltValueField(wireName: r'requestedAtUtc')
  DateTime? get requestedAtUtc;

  AdminPendingRoleRequestResponse._();

  factory AdminPendingRoleRequestResponse([void updates(AdminPendingRoleRequestResponseBuilder b)]) = _$AdminPendingRoleRequestResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdminPendingRoleRequestResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdminPendingRoleRequestResponse> get serializer => _$AdminPendingRoleRequestResponseSerializer();
}

class _$AdminPendingRoleRequestResponseSerializer implements PrimitiveSerializer<AdminPendingRoleRequestResponse> {
  @override
  final Iterable<Type> types = const [AdminPendingRoleRequestResponse, _$AdminPendingRoleRequestResponse];

  @override
  final String wireName = r'AdminPendingRoleRequestResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdminPendingRoleRequestResponse object, {
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
    if (object.role != null) {
      yield r'role';
      yield serializers.serialize(
        object.role,
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
    AdminPendingRoleRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdminPendingRoleRequestResponseBuilder result,
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
        case r'role':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminRefResponse),
          ) as AdminRefResponse?;
          if (valueDes == null) continue;
          result.role.replace(valueDes);
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
  AdminPendingRoleRequestResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdminPendingRoleRequestResponseBuilder();
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


