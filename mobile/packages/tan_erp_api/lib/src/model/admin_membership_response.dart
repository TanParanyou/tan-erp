//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/admin_pending_role_request_response.dart';
import 'package:tan_erp_api/src/model/admin_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'admin_membership_response.g.dart';

/// AdminMembershipResponse
///
/// Properties:
/// * [id] 
/// * [isActive] 
/// * [rowVersion] 
/// * [branch] 
/// * [startsAtUtc] 
/// * [expiresAtUtc] 
/// * [roles] 
/// * [pendingRoleRequests] 
@BuiltValue()
abstract class AdminMembershipResponse implements Built<AdminMembershipResponse, AdminMembershipResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'isActive')
  bool? get isActive;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'branch')
  AdminRefResponse? get branch;

  @BuiltValueField(wireName: r'startsAtUtc')
  DateTime? get startsAtUtc;

  @BuiltValueField(wireName: r'expiresAtUtc')
  DateTime? get expiresAtUtc;

  @BuiltValueField(wireName: r'roles')
  BuiltList<AdminRefResponse>? get roles;

  @BuiltValueField(wireName: r'pendingRoleRequests')
  BuiltList<AdminPendingRoleRequestResponse>? get pendingRoleRequests;

  AdminMembershipResponse._();

  factory AdminMembershipResponse([void updates(AdminMembershipResponseBuilder b)]) = _$AdminMembershipResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdminMembershipResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdminMembershipResponse> get serializer => _$AdminMembershipResponseSerializer();
}

class _$AdminMembershipResponseSerializer implements PrimitiveSerializer<AdminMembershipResponse> {
  @override
  final Iterable<Type> types = const [AdminMembershipResponse, _$AdminMembershipResponse];

  @override
  final String wireName = r'AdminMembershipResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdminMembershipResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.isActive != null) {
      yield r'isActive';
      yield serializers.serialize(
        object.isActive,
        specifiedType: const FullType(bool),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.branch != null) {
      yield r'branch';
      yield serializers.serialize(
        object.branch,
        specifiedType: const FullType(AdminRefResponse),
      );
    }
    if (object.startsAtUtc != null) {
      yield r'startsAtUtc';
      yield serializers.serialize(
        object.startsAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.expiresAtUtc != null) {
      yield r'expiresAtUtc';
      yield serializers.serialize(
        object.expiresAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.roles != null) {
      yield r'roles';
      yield serializers.serialize(
        object.roles,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AdminRefResponse)]),
      );
    }
    if (object.pendingRoleRequests != null) {
      yield r'pendingRoleRequests';
      yield serializers.serialize(
        object.pendingRoleRequests,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AdminPendingRoleRequestResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AdminMembershipResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdminMembershipResponseBuilder result,
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
        case r'isActive':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isActive = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'branch':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminRefResponse),
          ) as AdminRefResponse?;
          if (valueDes == null) continue;
          result.branch.replace(valueDes);
          break;
        case r'startsAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.startsAtUtc = valueDes;
          break;
        case r'expiresAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.expiresAtUtc = valueDes;
          break;
        case r'roles':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AdminRefResponse)]),
          ) as BuiltList<AdminRefResponse>?;
          if (valueDes == null) continue;
          result.roles.replace(valueDes);
          break;
        case r'pendingRoleRequests':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AdminPendingRoleRequestResponse)]),
          ) as BuiltList<AdminPendingRoleRequestResponse>?;
          if (valueDes == null) continue;
          result.pendingRoleRequests.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AdminMembershipResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdminMembershipResponseBuilder();
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


