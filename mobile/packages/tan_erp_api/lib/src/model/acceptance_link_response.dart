//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/quotation_person_response.dart';
import 'package:tan_erp_api/src/model/acceptance_evidence_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'acceptance_link_response.g.dart';

/// AcceptanceLinkResponse
///
/// Properties:
/// * [id] 
/// * [quotationId] 
/// * [quotationNumber] 
/// * [status] 
/// * [quotationStatus] 
/// * [isUsable] 
/// * [signerHint] 
/// * [expiresAtUtc] 
/// * [createdAtUtc] 
/// * [createdBy] 
/// * [revokedAtUtc] 
/// * [acceptedAtUtc] 
/// * [evidence] 
@BuiltValue()
abstract class AcceptanceLinkResponse implements Built<AcceptanceLinkResponse, AcceptanceLinkResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'quotationId')
  String? get quotationId;

  @BuiltValueField(wireName: r'quotationNumber')
  String? get quotationNumber;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'quotationStatus')
  String? get quotationStatus;

  @BuiltValueField(wireName: r'isUsable')
  bool? get isUsable;

  @BuiltValueField(wireName: r'signerHint')
  String? get signerHint;

  @BuiltValueField(wireName: r'expiresAtUtc')
  DateTime? get expiresAtUtc;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'createdBy')
  QuotationPersonResponse? get createdBy;

  @BuiltValueField(wireName: r'revokedAtUtc')
  DateTime? get revokedAtUtc;

  @BuiltValueField(wireName: r'acceptedAtUtc')
  DateTime? get acceptedAtUtc;

  @BuiltValueField(wireName: r'evidence')
  AcceptanceEvidenceResponse? get evidence;

  AcceptanceLinkResponse._();

  factory AcceptanceLinkResponse([void updates(AcceptanceLinkResponseBuilder b)]) = _$AcceptanceLinkResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AcceptanceLinkResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AcceptanceLinkResponse> get serializer => _$AcceptanceLinkResponseSerializer();
}

class _$AcceptanceLinkResponseSerializer implements PrimitiveSerializer<AcceptanceLinkResponse> {
  @override
  final Iterable<Type> types = const [AcceptanceLinkResponse, _$AcceptanceLinkResponse];

  @override
  final String wireName = r'AcceptanceLinkResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AcceptanceLinkResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.quotationId != null) {
      yield r'quotationId';
      yield serializers.serialize(
        object.quotationId,
        specifiedType: const FullType(String),
      );
    }
    if (object.quotationNumber != null) {
      yield r'quotationNumber';
      yield serializers.serialize(
        object.quotationNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.quotationStatus != null) {
      yield r'quotationStatus';
      yield serializers.serialize(
        object.quotationStatus,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isUsable != null) {
      yield r'isUsable';
      yield serializers.serialize(
        object.isUsable,
        specifiedType: const FullType(bool),
      );
    }
    if (object.signerHint != null) {
      yield r'signerHint';
      yield serializers.serialize(
        object.signerHint,
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
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(QuotationPersonResponse),
      );
    }
    if (object.revokedAtUtc != null) {
      yield r'revokedAtUtc';
      yield serializers.serialize(
        object.revokedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.acceptedAtUtc != null) {
      yield r'acceptedAtUtc';
      yield serializers.serialize(
        object.acceptedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.evidence != null) {
      yield r'evidence';
      yield serializers.serialize(
        object.evidence,
        specifiedType: const FullType(AcceptanceEvidenceResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AcceptanceLinkResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AcceptanceLinkResponseBuilder result,
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
        case r'quotationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationId = valueDes;
          break;
        case r'quotationNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationNumber = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'quotationStatus':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationStatus = valueDes;
          break;
        case r'isUsable':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isUsable = valueDes;
          break;
        case r'signerHint':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.signerHint = valueDes;
          break;
        case r'expiresAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.expiresAtUtc = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QuotationPersonResponse),
          ) as QuotationPersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'revokedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.revokedAtUtc = valueDes;
          break;
        case r'acceptedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.acceptedAtUtc = valueDes;
          break;
        case r'evidence':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AcceptanceEvidenceResponse),
          ) as AcceptanceEvidenceResponse?;
          if (valueDes == null) continue;
          result.evidence.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AcceptanceLinkResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AcceptanceLinkResponseBuilder();
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


