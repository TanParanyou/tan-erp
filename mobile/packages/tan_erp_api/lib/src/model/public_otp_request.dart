//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'public_otp_request.g.dart';

/// PublicOtpRequest
///
/// Properties:
/// * [channel] 
@BuiltValue()
abstract class PublicOtpRequest implements Built<PublicOtpRequest, PublicOtpRequestBuilder> {
  @BuiltValueField(wireName: r'channel')
  String? get channel;

  PublicOtpRequest._();

  factory PublicOtpRequest([void updates(PublicOtpRequestBuilder b)]) = _$PublicOtpRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PublicOtpRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PublicOtpRequest> get serializer => _$PublicOtpRequestSerializer();
}

class _$PublicOtpRequestSerializer implements PrimitiveSerializer<PublicOtpRequest> {
  @override
  final Iterable<Type> types = const [PublicOtpRequest, _$PublicOtpRequest];

  @override
  final String wireName = r'PublicOtpRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PublicOtpRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.channel != null) {
      yield r'channel';
      yield serializers.serialize(
        object.channel,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PublicOtpRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PublicOtpRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'channel':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.channel = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PublicOtpRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PublicOtpRequestBuilder();
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


