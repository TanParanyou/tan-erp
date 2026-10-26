//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_admin_user_profile_request.g.dart';

/// UpdateAdminUserProfileRequest
///
/// Properties:
/// * [displayName] 
/// * [phoneNumber] 
/// * [employeeCode] 
/// * [departmentId] 
/// * [jobTitleId] 
/// * [avatarFileId] 
/// * [notes] 
@BuiltValue()
abstract class UpdateAdminUserProfileRequest implements Built<UpdateAdminUserProfileRequest, UpdateAdminUserProfileRequestBuilder> {
  @BuiltValueField(wireName: r'displayName')
  String? get displayName;

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

  UpdateAdminUserProfileRequest._();

  factory UpdateAdminUserProfileRequest([void updates(UpdateAdminUserProfileRequestBuilder b)]) = _$UpdateAdminUserProfileRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateAdminUserProfileRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateAdminUserProfileRequest> get serializer => _$UpdateAdminUserProfileRequestSerializer();
}

class _$UpdateAdminUserProfileRequestSerializer implements PrimitiveSerializer<UpdateAdminUserProfileRequest> {
  @override
  final Iterable<Type> types = const [UpdateAdminUserProfileRequest, _$UpdateAdminUserProfileRequest];

  @override
  final String wireName = r'UpdateAdminUserProfileRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateAdminUserProfileRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.displayName != null) {
      yield r'displayName';
      yield serializers.serialize(
        object.displayName,
        specifiedType: const FullType.nullable(String),
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
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateAdminUserProfileRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateAdminUserProfileRequestBuilder result,
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateAdminUserProfileRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateAdminUserProfileRequestBuilder();
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


