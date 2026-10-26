//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/admin_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'role_change_request_response.g.dart';

/// RoleChangeRequestResponse
///
/// Properties:
/// * [id] 
/// * [rowVersion] 
/// * [status] 
/// * [role] 
/// * [requestedBy] 
/// * [requestedAtUtc] 
/// * [proposedName] 
/// * [proposedDescription] 
/// * [addedPermissionKeys] 
/// * [removedPermissionKeys] 
@BuiltValue()
abstract class RoleChangeRequestResponse implements Built<RoleChangeRequestResponse, RoleChangeRequestResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'role')
  AdminRefResponse? get role;

  @BuiltValueField(wireName: r'requestedBy')
  AdminRefResponse? get requestedBy;

  @BuiltValueField(wireName: r'requestedAtUtc')
  DateTime? get requestedAtUtc;

  @BuiltValueField(wireName: r'proposedName')
  String? get proposedName;

  @BuiltValueField(wireName: r'proposedDescription')
  String? get proposedDescription;

  @BuiltValueField(wireName: r'addedPermissionKeys')
  BuiltList<String>? get addedPermissionKeys;

  @BuiltValueField(wireName: r'removedPermissionKeys')
  BuiltList<String>? get removedPermissionKeys;

  RoleChangeRequestResponse._();

  factory RoleChangeRequestResponse([void updates(RoleChangeRequestResponseBuilder b)]) = _$RoleChangeRequestResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RoleChangeRequestResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RoleChangeRequestResponse> get serializer => _$RoleChangeRequestResponseSerializer();
}

class _$RoleChangeRequestResponseSerializer implements PrimitiveSerializer<RoleChangeRequestResponse> {
  @override
  final Iterable<Type> types = const [RoleChangeRequestResponse, _$RoleChangeRequestResponse];

  @override
  final String wireName = r'RoleChangeRequestResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RoleChangeRequestResponse object, {
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
    if (object.proposedName != null) {
      yield r'proposedName';
      yield serializers.serialize(
        object.proposedName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.proposedDescription != null) {
      yield r'proposedDescription';
      yield serializers.serialize(
        object.proposedDescription,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.addedPermissionKeys != null) {
      yield r'addedPermissionKeys';
      yield serializers.serialize(
        object.addedPermissionKeys,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.removedPermissionKeys != null) {
      yield r'removedPermissionKeys';
      yield serializers.serialize(
        object.removedPermissionKeys,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RoleChangeRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RoleChangeRequestResponseBuilder result,
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
        case r'proposedName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.proposedName = valueDes;
          break;
        case r'proposedDescription':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.proposedDescription = valueDes;
          break;
        case r'addedPermissionKeys':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.addedPermissionKeys.replace(valueDes);
          break;
        case r'removedPermissionKeys':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.removedPermissionKeys.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  RoleChangeRequestResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RoleChangeRequestResponseBuilder();
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


