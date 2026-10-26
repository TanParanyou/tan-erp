//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/user_dto.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/membership_dto.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'current_user_response.g.dart';

/// CurrentUserResponse
///
/// Properties:
/// * [user] 
/// * [memberships] 
@BuiltValue()
abstract class CurrentUserResponse implements Built<CurrentUserResponse, CurrentUserResponseBuilder> {
  @BuiltValueField(wireName: r'user')
  UserDto? get user;

  @BuiltValueField(wireName: r'memberships')
  BuiltList<MembershipDto>? get memberships;

  CurrentUserResponse._();

  factory CurrentUserResponse([void updates(CurrentUserResponseBuilder b)]) = _$CurrentUserResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CurrentUserResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CurrentUserResponse> get serializer => _$CurrentUserResponseSerializer();
}

class _$CurrentUserResponseSerializer implements PrimitiveSerializer<CurrentUserResponse> {
  @override
  final Iterable<Type> types = const [CurrentUserResponse, _$CurrentUserResponse];

  @override
  final String wireName = r'CurrentUserResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CurrentUserResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.user != null) {
      yield r'user';
      yield serializers.serialize(
        object.user,
        specifiedType: const FullType(UserDto),
      );
    }
    if (object.memberships != null) {
      yield r'memberships';
      yield serializers.serialize(
        object.memberships,
        specifiedType: const FullType.nullable(BuiltList, [FullType(MembershipDto)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CurrentUserResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CurrentUserResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'user':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(UserDto),
          ) as UserDto?;
          if (valueDes == null) continue;
          result.user.replace(valueDes);
          break;
        case r'memberships':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(MembershipDto)]),
          ) as BuiltList<MembershipDto>?;
          if (valueDes == null) continue;
          result.memberships.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CurrentUserResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CurrentUserResponseBuilder();
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


