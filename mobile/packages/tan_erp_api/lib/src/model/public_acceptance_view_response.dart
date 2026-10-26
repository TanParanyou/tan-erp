//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/quotation_document_response.dart';
import 'package:tan_erp_api/src/model/public_quotation_pdf_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'public_acceptance_view_response.g.dart';

/// PublicAcceptanceViewResponse
///
/// Properties:
/// * [status] 
/// * [expiresAtUtc] 
/// * [signerHint] 
/// * [consentVersion] 
/// * [acceptedAtUtc] 
/// * [document] 
/// * [pdf] 
/// * [otpRequired] 
/// * [otpChannels] 
@BuiltValue()
abstract class PublicAcceptanceViewResponse implements Built<PublicAcceptanceViewResponse, PublicAcceptanceViewResponseBuilder> {
  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'expiresAtUtc')
  DateTime? get expiresAtUtc;

  @BuiltValueField(wireName: r'signerHint')
  String? get signerHint;

  @BuiltValueField(wireName: r'consentVersion')
  String? get consentVersion;

  @BuiltValueField(wireName: r'acceptedAtUtc')
  DateTime? get acceptedAtUtc;

  @BuiltValueField(wireName: r'document')
  QuotationDocumentResponse? get document;

  @BuiltValueField(wireName: r'pdf')
  PublicQuotationPdfResponse? get pdf;

  @BuiltValueField(wireName: r'otpRequired')
  bool? get otpRequired;

  @BuiltValueField(wireName: r'otpChannels')
  BuiltList<String>? get otpChannels;

  PublicAcceptanceViewResponse._();

  factory PublicAcceptanceViewResponse([void updates(PublicAcceptanceViewResponseBuilder b)]) = _$PublicAcceptanceViewResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PublicAcceptanceViewResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PublicAcceptanceViewResponse> get serializer => _$PublicAcceptanceViewResponseSerializer();
}

class _$PublicAcceptanceViewResponseSerializer implements PrimitiveSerializer<PublicAcceptanceViewResponse> {
  @override
  final Iterable<Type> types = const [PublicAcceptanceViewResponse, _$PublicAcceptanceViewResponse];

  @override
  final String wireName = r'PublicAcceptanceViewResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PublicAcceptanceViewResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
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
    if (object.signerHint != null) {
      yield r'signerHint';
      yield serializers.serialize(
        object.signerHint,
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
    if (object.acceptedAtUtc != null) {
      yield r'acceptedAtUtc';
      yield serializers.serialize(
        object.acceptedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.document != null) {
      yield r'document';
      yield serializers.serialize(
        object.document,
        specifiedType: const FullType(QuotationDocumentResponse),
      );
    }
    if (object.pdf != null) {
      yield r'pdf';
      yield serializers.serialize(
        object.pdf,
        specifiedType: const FullType(PublicQuotationPdfResponse),
      );
    }
    if (object.otpRequired != null) {
      yield r'otpRequired';
      yield serializers.serialize(
        object.otpRequired,
        specifiedType: const FullType(bool),
      );
    }
    if (object.otpChannels != null) {
      yield r'otpChannels';
      yield serializers.serialize(
        object.otpChannels,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PublicAcceptanceViewResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PublicAcceptanceViewResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'expiresAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.expiresAtUtc = valueDes;
          break;
        case r'signerHint':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.signerHint = valueDes;
          break;
        case r'consentVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.consentVersion = valueDes;
          break;
        case r'acceptedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.acceptedAtUtc = valueDes;
          break;
        case r'document':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QuotationDocumentResponse),
          ) as QuotationDocumentResponse?;
          if (valueDes == null) continue;
          result.document.replace(valueDes);
          break;
        case r'pdf':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(PublicQuotationPdfResponse),
          ) as PublicQuotationPdfResponse?;
          if (valueDes == null) continue;
          result.pdf.replace(valueDes);
          break;
        case r'otpRequired':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.otpRequired = valueDes;
          break;
        case r'otpChannels':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.otpChannels.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PublicAcceptanceViewResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PublicAcceptanceViewResponseBuilder();
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


