//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'verify_two_factor_login_request.g.dart';

/// VerifyTwoFactorLoginRequest
///
/// Properties:
/// * [challengeToken] 
/// * [code] 
/// * [clientPlatform] 
@BuiltValue()
abstract class VerifyTwoFactorLoginRequest implements Built<VerifyTwoFactorLoginRequest, VerifyTwoFactorLoginRequestBuilder> {
  @BuiltValueField(wireName: r'challengeToken')
  String? get challengeToken;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'clientPlatform')
  String? get clientPlatform;

  VerifyTwoFactorLoginRequest._();

  factory VerifyTwoFactorLoginRequest([void updates(VerifyTwoFactorLoginRequestBuilder b)]) = _$VerifyTwoFactorLoginRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(VerifyTwoFactorLoginRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<VerifyTwoFactorLoginRequest> get serializer => _$VerifyTwoFactorLoginRequestSerializer();
}

class _$VerifyTwoFactorLoginRequestSerializer implements PrimitiveSerializer<VerifyTwoFactorLoginRequest> {
  @override
  final Iterable<Type> types = const [VerifyTwoFactorLoginRequest, _$VerifyTwoFactorLoginRequest];

  @override
  final String wireName = r'VerifyTwoFactorLoginRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    VerifyTwoFactorLoginRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.challengeToken != null) {
      yield r'challengeToken';
      yield serializers.serialize(
        object.challengeToken,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.clientPlatform != null) {
      yield r'clientPlatform';
      yield serializers.serialize(
        object.clientPlatform,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    VerifyTwoFactorLoginRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required VerifyTwoFactorLoginRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'challengeToken':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.challengeToken = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'clientPlatform':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.clientPlatform = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  VerifyTwoFactorLoginRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = VerifyTwoFactorLoginRequestBuilder();
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


