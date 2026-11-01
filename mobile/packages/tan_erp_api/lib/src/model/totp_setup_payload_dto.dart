//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'totp_setup_payload_dto.g.dart';

/// TotpSetupPayloadDto
///
/// Properties:
/// * [secretKey] 
/// * [otpAuthUri] 
@BuiltValue()
abstract class TotpSetupPayloadDto implements Built<TotpSetupPayloadDto, TotpSetupPayloadDtoBuilder> {
  @BuiltValueField(wireName: r'secretKey')
  String? get secretKey;

  @BuiltValueField(wireName: r'otpAuthUri')
  String? get otpAuthUri;

  TotpSetupPayloadDto._();

  factory TotpSetupPayloadDto([void updates(TotpSetupPayloadDtoBuilder b)]) = _$TotpSetupPayloadDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(TotpSetupPayloadDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<TotpSetupPayloadDto> get serializer => _$TotpSetupPayloadDtoSerializer();
}

class _$TotpSetupPayloadDtoSerializer implements PrimitiveSerializer<TotpSetupPayloadDto> {
  @override
  final Iterable<Type> types = const [TotpSetupPayloadDto, _$TotpSetupPayloadDto];

  @override
  final String wireName = r'TotpSetupPayloadDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    TotpSetupPayloadDto object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.secretKey != null) {
      yield r'secretKey';
      yield serializers.serialize(
        object.secretKey,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.otpAuthUri != null) {
      yield r'otpAuthUri';
      yield serializers.serialize(
        object.otpAuthUri,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    TotpSetupPayloadDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required TotpSetupPayloadDtoBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'secretKey':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.secretKey = valueDes;
          break;
        case r'otpAuthUri':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.otpAuthUri = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  TotpSetupPayloadDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = TotpSetupPayloadDtoBuilder();
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


