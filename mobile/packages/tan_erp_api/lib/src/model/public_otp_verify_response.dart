//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'public_otp_verify_response.g.dart';

/// PublicOtpVerifyResponse
///
/// Properties:
/// * [verificationToken] 
/// * [expiresAtUtc] 
@BuiltValue()
abstract class PublicOtpVerifyResponse implements Built<PublicOtpVerifyResponse, PublicOtpVerifyResponseBuilder> {
  @BuiltValueField(wireName: r'verificationToken')
  String? get verificationToken;

  @BuiltValueField(wireName: r'expiresAtUtc')
  DateTime? get expiresAtUtc;

  PublicOtpVerifyResponse._();

  factory PublicOtpVerifyResponse([void updates(PublicOtpVerifyResponseBuilder b)]) = _$PublicOtpVerifyResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PublicOtpVerifyResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PublicOtpVerifyResponse> get serializer => _$PublicOtpVerifyResponseSerializer();
}

class _$PublicOtpVerifyResponseSerializer implements PrimitiveSerializer<PublicOtpVerifyResponse> {
  @override
  final Iterable<Type> types = const [PublicOtpVerifyResponse, _$PublicOtpVerifyResponse];

  @override
  final String wireName = r'PublicOtpVerifyResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PublicOtpVerifyResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.verificationToken != null) {
      yield r'verificationToken';
      yield serializers.serialize(
        object.verificationToken,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.expiresAtUtc != null) {
      yield r'expiresAtUtc';
      yield serializers.serialize(
        object.expiresAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PublicOtpVerifyResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PublicOtpVerifyResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'verificationToken':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.verificationToken = valueDes;
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
  PublicOtpVerifyResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PublicOtpVerifyResponseBuilder();
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


