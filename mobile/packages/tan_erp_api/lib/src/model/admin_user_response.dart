//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/admin_department_response.dart';
import 'package:tan_erp_api/src/model/admin_membership_response.dart';
import 'package:tan_erp_api/src/model/admin_job_title_response.dart';
import 'package:tan_erp_api/src/model/admin_avatar_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'admin_user_response.g.dart';

/// AdminUserResponse
///
/// Properties:
/// * [id] 
/// * [displayName] 
/// * [email] 
/// * [status] 
/// * [rowVersion] 
/// * [memberships] 
/// * [phoneNumber] 
/// * [employeeCode] 
/// * [department] 
/// * [jobTitle] 
/// * [avatar] 
/// * [notes] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class AdminUserResponse implements Built<AdminUserResponse, AdminUserResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'displayName')
  String? get displayName;

  @BuiltValueField(wireName: r'email')
  String? get email;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'memberships')
  BuiltList<AdminMembershipResponse>? get memberships;

  @BuiltValueField(wireName: r'phoneNumber')
  String? get phoneNumber;

  @BuiltValueField(wireName: r'employeeCode')
  String? get employeeCode;

  @BuiltValueField(wireName: r'department')
  AdminDepartmentResponse? get department;

  @BuiltValueField(wireName: r'jobTitle')
  AdminJobTitleResponse? get jobTitle;

  @BuiltValueField(wireName: r'avatar')
  AdminAvatarResponse? get avatar;

  @BuiltValueField(wireName: r'notes')
  String? get notes;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  AdminUserResponse._();

  factory AdminUserResponse([void updates(AdminUserResponseBuilder b)]) = _$AdminUserResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AdminUserResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AdminUserResponse> get serializer => _$AdminUserResponseSerializer();
}

class _$AdminUserResponseSerializer implements PrimitiveSerializer<AdminUserResponse> {
  @override
  final Iterable<Type> types = const [AdminUserResponse, _$AdminUserResponse];

  @override
  final String wireName = r'AdminUserResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AdminUserResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
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
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.memberships != null) {
      yield r'memberships';
      yield serializers.serialize(
        object.memberships,
        specifiedType: const FullType.nullable(BuiltList, [FullType(AdminMembershipResponse)]),
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
    if (object.department != null) {
      yield r'department';
      yield serializers.serialize(
        object.department,
        specifiedType: const FullType(AdminDepartmentResponse),
      );
    }
    if (object.jobTitle != null) {
      yield r'jobTitle';
      yield serializers.serialize(
        object.jobTitle,
        specifiedType: const FullType(AdminJobTitleResponse),
      );
    }
    if (object.avatar != null) {
      yield r'avatar';
      yield serializers.serialize(
        object.avatar,
        specifiedType: const FullType(AdminAvatarResponse),
      );
    }
    if (object.notes != null) {
      yield r'notes';
      yield serializers.serialize(
        object.notes,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AdminUserResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AdminUserResponseBuilder result,
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
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'memberships':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(AdminMembershipResponse)]),
          ) as BuiltList<AdminMembershipResponse>?;
          if (valueDes == null) continue;
          result.memberships.replace(valueDes);
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
        case r'department':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminDepartmentResponse),
          ) as AdminDepartmentResponse?;
          if (valueDes == null) continue;
          result.department.replace(valueDes);
          break;
        case r'jobTitle':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminJobTitleResponse),
          ) as AdminJobTitleResponse?;
          if (valueDes == null) continue;
          result.jobTitle.replace(valueDes);
          break;
        case r'avatar':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminAvatarResponse),
          ) as AdminAvatarResponse?;
          if (valueDes == null) continue;
          result.avatar.replace(valueDes);
          break;
        case r'notes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.notes = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AdminUserResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AdminUserResponseBuilder();
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


