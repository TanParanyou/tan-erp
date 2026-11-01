//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'set_admin_user_password_request.g.dart';

/// SetAdminUserPasswordRequest
///
/// Properties:
/// * [newPassword] 
@BuiltValue()
abstract class SetAdminUserPasswordRequest implements Built<SetAdminUserPasswordRequest, SetAdminUserPasswordRequestBuilder> {
  @BuiltValueField(wireName: r'newPassword')
  String? get newPassword;

  SetAdminUserPasswordRequest._();

  factory SetAdminUserPasswordRequest([void updates(SetAdminUserPasswordRequestBuilder b)]) = _$SetAdminUserPasswordRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SetAdminUserPasswordRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SetAdminUserPasswordRequest> get serializer => _$SetAdminUserPasswordRequestSerializer();
}

class _$SetAdminUserPasswordRequestSerializer implements PrimitiveSerializer<SetAdminUserPasswordRequest> {
  @override
  final Iterable<Type> types = const [SetAdminUserPasswordRequest, _$SetAdminUserPasswordRequest];

  @override
  final String wireName = r'SetAdminUserPasswordRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SetAdminUserPasswordRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.newPassword != null) {
      yield r'newPassword';
      yield serializers.serialize(
        object.newPassword,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SetAdminUserPasswordRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SetAdminUserPasswordRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'newPassword':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.newPassword = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SetAdminUserPasswordRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SetAdminUserPasswordRequestBuilder();
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


