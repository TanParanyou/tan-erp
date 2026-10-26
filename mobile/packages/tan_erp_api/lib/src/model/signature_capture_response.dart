//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/attachment_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'signature_capture_response.g.dart';

/// SignatureCaptureResponse
///
/// Properties:
/// * [id] 
/// * [ownerType] 
/// * [ownerId] 
/// * [purpose] 
/// * [signerName] 
/// * [signerRole] 
/// * [signedAtUtc] 
/// * [imageFileId] 
/// * [servingUrl] 
/// * [consentTextVersion] 
/// * [contentHash] 
/// * [capturedBy] 
@BuiltValue()
abstract class SignatureCaptureResponse implements Built<SignatureCaptureResponse, SignatureCaptureResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'ownerType')
  String? get ownerType;

  @BuiltValueField(wireName: r'ownerId')
  String? get ownerId;

  @BuiltValueField(wireName: r'purpose')
  String? get purpose;

  @BuiltValueField(wireName: r'signerName')
  String? get signerName;

  @BuiltValueField(wireName: r'signerRole')
  String? get signerRole;

  @BuiltValueField(wireName: r'signedAtUtc')
  DateTime? get signedAtUtc;

  @BuiltValueField(wireName: r'imageFileId')
  String? get imageFileId;

  @BuiltValueField(wireName: r'servingUrl')
  String? get servingUrl;

  @BuiltValueField(wireName: r'consentTextVersion')
  String? get consentTextVersion;

  @BuiltValueField(wireName: r'contentHash')
  String? get contentHash;

  @BuiltValueField(wireName: r'capturedBy')
  AttachmentPersonResponse? get capturedBy;

  SignatureCaptureResponse._();

  factory SignatureCaptureResponse([void updates(SignatureCaptureResponseBuilder b)]) = _$SignatureCaptureResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SignatureCaptureResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SignatureCaptureResponse> get serializer => _$SignatureCaptureResponseSerializer();
}

class _$SignatureCaptureResponseSerializer implements PrimitiveSerializer<SignatureCaptureResponse> {
  @override
  final Iterable<Type> types = const [SignatureCaptureResponse, _$SignatureCaptureResponse];

  @override
  final String wireName = r'SignatureCaptureResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SignatureCaptureResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.ownerType != null) {
      yield r'ownerType';
      yield serializers.serialize(
        object.ownerType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.ownerId != null) {
      yield r'ownerId';
      yield serializers.serialize(
        object.ownerId,
        specifiedType: const FullType(String),
      );
    }
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
    if (object.signedAtUtc != null) {
      yield r'signedAtUtc';
      yield serializers.serialize(
        object.signedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.imageFileId != null) {
      yield r'imageFileId';
      yield serializers.serialize(
        object.imageFileId,
        specifiedType: const FullType(String),
      );
    }
    if (object.servingUrl != null) {
      yield r'servingUrl';
      yield serializers.serialize(
        object.servingUrl,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.consentTextVersion != null) {
      yield r'consentTextVersion';
      yield serializers.serialize(
        object.consentTextVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.contentHash != null) {
      yield r'contentHash';
      yield serializers.serialize(
        object.contentHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.capturedBy != null) {
      yield r'capturedBy';
      yield serializers.serialize(
        object.capturedBy,
        specifiedType: const FullType(AttachmentPersonResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SignatureCaptureResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SignatureCaptureResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'ownerType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.ownerType = valueDes;
          break;
        case r'ownerId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.ownerId = valueDes;
          break;
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
        case r'signedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.signedAtUtc = valueDes;
          break;
        case r'imageFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.imageFileId = valueDes;
          break;
        case r'servingUrl':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.servingUrl = valueDes;
          break;
        case r'consentTextVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.consentTextVersion = valueDes;
          break;
        case r'contentHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.contentHash = valueDes;
          break;
        case r'capturedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AttachmentPersonResponse),
          ) as AttachmentPersonResponse?;
          if (valueDes == null) continue;
          result.capturedBy.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SignatureCaptureResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SignatureCaptureResponseBuilder();
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


