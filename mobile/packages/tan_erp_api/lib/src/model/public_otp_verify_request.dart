//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'public_otp_verify_request.g.dart';

/// PublicOtpVerifyRequest
///
/// Properties:
/// * [code] 
@BuiltValue()
abstract class PublicOtpVerifyRequest implements Built<PublicOtpVerifyRequest, PublicOtpVerifyRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  PublicOtpVerifyRequest._();

  factory PublicOtpVerifyRequest([void updates(PublicOtpVerifyRequestBuilder b)]) = _$PublicOtpVerifyRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PublicOtpVerifyRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PublicOtpVerifyRequest> get serializer => _$PublicOtpVerifyRequestSerializer();
}

class _$PublicOtpVerifyRequestSerializer implements PrimitiveSerializer<PublicOtpVerifyRequest> {
  @override
  final Iterable<Type> types = const [PublicOtpVerifyRequest, _$PublicOtpVerifyRequest];

  @override
  final String wireName = r'PublicOtpVerifyRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PublicOtpVerifyRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PublicOtpVerifyRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PublicOtpVerifyRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PublicOtpVerifyRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PublicOtpVerifyRequestBuilder();
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


