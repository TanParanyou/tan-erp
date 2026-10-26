//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'capture_signature_request.g.dart';

/// CaptureSignatureRequest
///
/// Properties:
/// * [purpose] 
/// * [signerName] 
/// * [signerRole] 
/// * [imageFileId] 
/// * [consentAccepted] 
/// * [consentTextVersion] 
@BuiltValue()
abstract class CaptureSignatureRequest implements Built<CaptureSignatureRequest, CaptureSignatureRequestBuilder> {
  @BuiltValueField(wireName: r'purpose')
  String? get purpose;

  @BuiltValueField(wireName: r'signerName')
  String? get signerName;

  @BuiltValueField(wireName: r'signerRole')
  String? get signerRole;

  @BuiltValueField(wireName: r'imageFileId')
  String? get imageFileId;

  @BuiltValueField(wireName: r'consentAccepted')
  bool? get consentAccepted;

  @BuiltValueField(wireName: r'consentTextVersion')
  String? get consentTextVersion;

  CaptureSignatureRequest._();

  factory CaptureSignatureRequest([void updates(CaptureSignatureRequestBuilder b)]) = _$CaptureSignatureRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CaptureSignatureRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CaptureSignatureRequest> get serializer => _$CaptureSignatureRequestSerializer();
}

class _$CaptureSignatureRequestSerializer implements PrimitiveSerializer<CaptureSignatureRequest> {
  @override
  final Iterable<Type> types = const [CaptureSignatureRequest, _$CaptureSignatureRequest];

  @override
  final String wireName = r'CaptureSignatureRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CaptureSignatureRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.purpose != null) {
      yield r'purpose';
      yield serializers.serialize(
        object.purpose,
        specifiedType: const FullType.nullable(String),
      );
    }
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
    if (object.imageFileId != null) {
      yield r'imageFileId';
      yield serializers.serialize(
        object.imageFileId,
        specifiedType: const FullType(String),
      );
    }
    if (object.consentAccepted != null) {
      yield r'consentAccepted';
      yield serializers.serialize(
        object.consentAccepted,
        specifiedType: const FullType(bool),
      );
    }
    if (object.consentTextVersion != null) {
      yield r'consentTextVersion';
      yield serializers.serialize(
        object.consentTextVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CaptureSignatureRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CaptureSignatureRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'purpose':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.purpose = valueDes;
          break;
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
        case r'imageFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.imageFileId = valueDes;
          break;
        case r'consentAccepted':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.consentAccepted = valueDes;
          break;
        case r'consentTextVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.consentTextVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CaptureSignatureRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CaptureSignatureRequestBuilder();
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


