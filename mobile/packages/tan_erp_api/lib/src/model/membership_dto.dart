//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/branch_dto.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/organization_dto.dart';
import 'package:tan_erp_api/src/model/permission_dto.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'membership_dto.g.dart';

/// MembershipDto
///
/// Properties:
/// * [id] 
/// * [organization] 
/// * [branch] 
/// * [permissions] 
@BuiltValue()
abstract class MembershipDto implements Built<MembershipDto, MembershipDtoBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'organization')
  OrganizationDto? get organization;

  @BuiltValueField(wireName: r'branch')
  BranchDto? get branch;

  @BuiltValueField(wireName: r'permissions')
  BuiltList<PermissionDto>? get permissions;

  MembershipDto._();

  factory MembershipDto([void updates(MembershipDtoBuilder b)]) = _$MembershipDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MembershipDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MembershipDto> get serializer => _$MembershipDtoSerializer();
}

class _$MembershipDtoSerializer implements PrimitiveSerializer<MembershipDto> {
  @override
  final Iterable<Type> types = const [MembershipDto, _$MembershipDto];

  @override
  final String wireName = r'MembershipDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MembershipDto object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.organization != null) {
      yield r'organization';
      yield serializers.serialize(
        object.organization,
        specifiedType: const FullType(OrganizationDto),
      );
    }
    if (object.branch != null) {
      yield r'branch';
      yield serializers.serialize(
        object.branch,
        specifiedType: const FullType(BranchDto),
      );
    }
    if (object.permissions != null) {
      yield r'permissions';
      yield serializers.serialize(
        object.permissions,
        specifiedType: const FullType.nullable(BuiltList, [FullType(PermissionDto)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MembershipDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MembershipDtoBuilder result,
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
        case r'organization':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(OrganizationDto),
          ) as OrganizationDto?;
          if (valueDes == null) continue;
          result.organization.replace(valueDes);
          break;
        case r'branch':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BranchDto),
          ) as BranchDto?;
          if (valueDes == null) continue;
          result.branch.replace(valueDes);
          break;
        case r'permissions':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(PermissionDto)]),
          ) as BuiltList<PermissionDto>?;
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
  MembershipDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MembershipDtoBuilder();
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


