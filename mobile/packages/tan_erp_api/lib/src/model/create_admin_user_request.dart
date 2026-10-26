//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_admin_user_request.g.dart';

/// CreateAdminUserRequest
///
/// Properties:
/// * [displayName] 
/// * [email] 
/// * [branchId] 
/// * [roleIds] 
/// * [phoneNumber] 
/// * [employeeCode] 
/// * [departmentId] 
/// * [jobTitleId] 
/// * [avatarFileId] 
/// * [notes] 
/// * [startsAtUtc] 
/// * [expiresAtUtc] 
@BuiltValue()
abstract class CreateAdminUserRequest implements Built<CreateAdminUserRequest, CreateAdminUserRequestBuilder> {
  @BuiltValueField(wireName: r'displayName')
  String? get displayName;

  @BuiltValueField(wireName: r'email')
  String? get email;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'roleIds')
  BuiltList<String>? get roleIds;

  @BuiltValueField(wireName: r'phoneNumber')
  String? get phoneNumber;

  @BuiltValueField(wireName: r'employeeCode')
  String? get employeeCode;

  @BuiltValueField(wireName: r'departmentId')
  String? get departmentId;

  @BuiltValueField(wireName: r'jobTitleId')
  String? get jobTitleId;

  @BuiltValueField(wireName: r'avatarFileId')
  String? get avatarFileId;

  @BuiltValueField(wireName: r'notes')
  String? get notes;

  @BuiltValueField(wireName: r'startsAtUtc')
  DateTime? get startsAtUtc;

  @BuiltValueField(wireName: r'expiresAtUtc')
  DateTime? get expiresAtUtc;

  CreateAdminUserRequest._();

  factory CreateAdminUserRequest([void updates(CreateAdminUserRequestBuilder b)]) = _$CreateAdminUserRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateAdminUserRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateAdminUserRequest> get serializer => _$CreateAdminUserRequestSerializer();
}

class _$CreateAdminUserRequestSerializer implements PrimitiveSerializer<CreateAdminUserRequest> {
  @override
  final Iterable<Type> types = const [CreateAdminUserRequest, _$CreateAdminUserRequest];

  @override
  final String wireName = r'CreateAdminUserRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateAdminUserRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.displayName != null) {
      yield r'displayName';
      yield serializers.serialize(
        object.displayName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.email != null) {
      yield r'email';
      yield serializers.serialize(
        object.email,
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
    if (object.roleIds != null) {
      yield r'roleIds';
      yield serializers.serialize(
        object.roleIds,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.phoneNumber != null) {
      yield r'phoneNumber';
      yield serializers.serialize(
        object.phoneNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.employeeCode != null) {
      yield r'employeeCode';
      yield serializers.serialize(
        object.employeeCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.departmentId != null) {
      yield r'departmentId';
      yield serializers.serialize(
        object.departmentId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.jobTitleId != null) {
      yield r'jobTitleId';
      yield serializers.serialize(
        object.jobTitleId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.avatarFileId != null) {
      yield r'avatarFileId';
      yield serializers.serialize(
        object.avatarFileId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.notes != null) {
      yield r'notes';
      yield serializers.serialize(
        object.notes,
        specifiedType: const FullType.nullable(String),
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
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateAdminUserRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateAdminUserRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'displayName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayName = valueDes;
          break;
        case r'email':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.email = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'roleIds':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.roleIds.replace(valueDes);
          break;
        case r'phoneNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.phoneNumber = valueDes;
          break;
        case r'employeeCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.employeeCode = valueDes;
          break;
        case r'departmentId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.departmentId = valueDes;
          break;
        case r'jobTitleId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.jobTitleId = valueDes;
          break;
        case r'avatarFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.avatarFileId = valueDes;
          break;
        case r'notes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.notes = valueDes;
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateAdminUserRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateAdminUserRequestBuilder();
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


