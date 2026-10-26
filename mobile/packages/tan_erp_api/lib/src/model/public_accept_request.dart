//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'public_accept_request.g.dart';

/// PublicAcceptRequest
///
/// Properties:
/// * [signerName] 
/// * [signerRole] 
/// * [consentAccepted] 
/// * [consentVersion] 
/// * [signatureImage] 
/// * [documentSha256] 
/// * [otpVerificationToken] 
@BuiltValue()
abstract class PublicAcceptRequest implements Built<PublicAcceptRequest, PublicAcceptRequestBuilder> {
  @BuiltValueField(wireName: r'signerName')
  String? get signerName;

  @BuiltValueField(wireName: r'signerRole')
  String? get signerRole;

  @BuiltValueField(wireName: r'consentAccepted')
  bool? get consentAccepted;

  @BuiltValueField(wireName: r'consentVersion')
  String? get consentVersion;

  @BuiltValueField(wireName: r'signatureImage')
  String? get signatureImage;

  @BuiltValueField(wireName: r'documentSha256')
  String? get documentSha256;

  @BuiltValueField(wireName: r'otpVerificationToken')
  String? get otpVerificationToken;

  PublicAcceptRequest._();

  factory PublicAcceptRequest([void updates(PublicAcceptRequestBuilder b)]) = _$PublicAcceptRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PublicAcceptRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PublicAcceptRequest> get serializer => _$PublicAcceptRequestSerializer();
}

class _$PublicAcceptRequestSerializer implements PrimitiveSerializer<PublicAcceptRequest> {
  @override
  final Iterable<Type> types = const [PublicAcceptRequest, _$PublicAcceptRequest];

  @override
  final String wireName = r'PublicAcceptRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PublicAcceptRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.signerName != null) {
      yield r'signerName';
      yield serializers.serialize(
        object.signerName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.signerRole != null) {
      yield r'signerRole';
      yield serializers.serialize(
        object.signerRole,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.consentAccepted != null) {
      yield r'consentAccepted';
      yield serializers.serialize(
        object.consentAccepted,
        specifiedType: const FullType(bool),
      );
    }
    if (object.consentVersion != null) {
      yield r'consentVersion';
      yield serializers.serialize(
        object.consentVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.signatureImage != null) {
      yield r'signatureImage';
      yield serializers.serialize(
        object.signatureImage,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.documentSha256 != null) {
      yield r'documentSha256';
      yield serializers.serialize(
        object.documentSha256,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.otpVerificationToken != null) {
      yield r'otpVerificationToken';
      yield serializers.serialize(
        object.otpVerificationToken,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PublicAcceptRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PublicAcceptRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'signerName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.signerName = valueDes;
          break;
        case r'signerRole':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.signerRole = valueDes;
          break;
        case r'consentAccepted':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.consentAccepted = valueDes;
          break;
        case r'consentVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.consentVersion = valueDes;
          break;
        case r'signatureImage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.signatureImage = valueDes;
          break;
        case r'documentSha256':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.documentSha256 = valueDes;
          break;
        case r'otpVerificationToken':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.otpVerificationToken = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PublicAcceptRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PublicAcceptRequestBuilder();
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


