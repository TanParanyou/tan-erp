//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'public_otp_request_response.g.dart';

/// PublicOtpRequestResponse
///
/// Properties:
/// * [resendAfterSeconds] 
/// * [expiresInSeconds] 
@BuiltValue()
abstract class PublicOtpRequestResponse implements Built<PublicOtpRequestResponse, PublicOtpRequestResponseBuilder> {
  @BuiltValueField(wireName: r'resendAfterSeconds')
  int? get resendAfterSeconds;

  @BuiltValueField(wireName: r'expiresInSeconds')
  int? get expiresInSeconds;

  PublicOtpRequestResponse._();

  factory PublicOtpRequestResponse([void updates(PublicOtpRequestResponseBuilder b)]) = _$PublicOtpRequestResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PublicOtpRequestResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PublicOtpRequestResponse> get serializer => _$PublicOtpRequestResponseSerializer();
}

class _$PublicOtpRequestResponseSerializer implements PrimitiveSerializer<PublicOtpRequestResponse> {
  @override
  final Iterable<Type> types = const [PublicOtpRequestResponse, _$PublicOtpRequestResponse];

  @override
  final String wireName = r'PublicOtpRequestResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PublicOtpRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.resendAfterSeconds != null) {
      yield r'resendAfterSeconds';
      yield serializers.serialize(
        object.resendAfterSeconds,
        specifiedType: const FullType(int),
      );
    }
    if (object.expiresInSeconds != null) {
      yield r'expiresInSeconds';
      yield serializers.serialize(
        object.expiresInSeconds,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PublicOtpRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PublicOtpRequestResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'resendAfterSeconds':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.resendAfterSeconds = valueDes;
          break;
        case r'expiresInSeconds':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.expiresInSeconds = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PublicOtpRequestResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PublicOtpRequestResponseBuilder();
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


