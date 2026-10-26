//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'acceptance_evidence_response.g.dart';

/// AcceptanceEvidenceResponse
///
/// Properties:
/// * [signerName] 
/// * [signerRole] 
/// * [consentVersion] 
/// * [hasSignatureImage] 
/// * [signatureHash] 
/// * [acceptedAtUtc] 
@BuiltValue()
abstract class AcceptanceEvidenceResponse implements Built<AcceptanceEvidenceResponse, AcceptanceEvidenceResponseBuilder> {
  @BuiltValueField(wireName: r'signerName')
  String? get signerName;

  @BuiltValueField(wireName: r'signerRole')
  String? get signerRole;

  @BuiltValueField(wireName: r'consentVersion')
  String? get consentVersion;

  @BuiltValueField(wireName: r'hasSignatureImage')
  bool? get hasSignatureImage;

  @BuiltValueField(wireName: r'signatureHash')
  String? get signatureHash;

  @BuiltValueField(wireName: r'acceptedAtUtc')
  DateTime? get acceptedAtUtc;

  AcceptanceEvidenceResponse._();

  factory AcceptanceEvidenceResponse([void updates(AcceptanceEvidenceResponseBuilder b)]) = _$AcceptanceEvidenceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AcceptanceEvidenceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AcceptanceEvidenceResponse> get serializer => _$AcceptanceEvidenceResponseSerializer();
}

class _$AcceptanceEvidenceResponseSerializer implements PrimitiveSerializer<AcceptanceEvidenceResponse> {
  @override
  final Iterable<Type> types = const [AcceptanceEvidenceResponse, _$AcceptanceEvidenceResponse];

  @override
  final String wireName = r'AcceptanceEvidenceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AcceptanceEvidenceResponse object, {
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
    if (object.consentVersion != null) {
      yield r'consentVersion';
      yield serializers.serialize(
        object.consentVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.hasSignatureImage != null) {
      yield r'hasSignatureImage';
      yield serializers.serialize(
        object.hasSignatureImage,
        specifiedType: const FullType(bool),
      );
    }
    if (object.signatureHash != null) {
      yield r'signatureHash';
      yield serializers.serialize(
        object.signatureHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.acceptedAtUtc != null) {
      yield r'acceptedAtUtc';
      yield serializers.serialize(
        object.acceptedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AcceptanceEvidenceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AcceptanceEvidenceResponseBuilder result,
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
        case r'consentVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.consentVersion = valueDes;
          break;
        case r'hasSignatureImage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.hasSignatureImage = valueDes;
          break;
        case r'signatureHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.signatureHash = valueDes;
          break;
        case r'acceptedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.acceptedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AcceptanceEvidenceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AcceptanceEvidenceResponseBuilder();
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


