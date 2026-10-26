//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_admin_membership_request.g.dart';

/// UpdateAdminMembershipRequest
///
/// Properties:
/// * [branchId] 
/// * [startsAtUtc] 
/// * [expiresAtUtc] 
@BuiltValue()
abstract class UpdateAdminMembershipRequest implements Built<UpdateAdminMembershipRequest, UpdateAdminMembershipRequestBuilder> {
  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'startsAtUtc')
  DateTime? get startsAtUtc;

  @BuiltValueField(wireName: r'expiresAtUtc')
  DateTime? get expiresAtUtc;

  UpdateAdminMembershipRequest._();

  factory UpdateAdminMembershipRequest([void updates(UpdateAdminMembershipRequestBuilder b)]) = _$UpdateAdminMembershipRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateAdminMembershipRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateAdminMembershipRequest> get serializer => _$UpdateAdminMembershipRequestSerializer();
}

class _$UpdateAdminMembershipRequestSerializer implements PrimitiveSerializer<UpdateAdminMembershipRequest> {
  @override
  final Iterable<Type> types = const [UpdateAdminMembershipRequest, _$UpdateAdminMembershipRequest];

  @override
  final String wireName = r'UpdateAdminMembershipRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateAdminMembershipRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
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
    UpdateAdminMembershipRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateAdminMembershipRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
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
  UpdateAdminMembershipRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateAdminMembershipRequestBuilder();
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


