//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/acceptance_evidence_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'public_acceptance_response.g.dart';

/// PublicAcceptanceResponse
///
/// Properties:
/// * [status] 
/// * [acceptedAtUtc] 
/// * [quotationNumber] 
/// * [evidence] 
@BuiltValue()
abstract class PublicAcceptanceResponse implements Built<PublicAcceptanceResponse, PublicAcceptanceResponseBuilder> {
  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'acceptedAtUtc')
  DateTime? get acceptedAtUtc;

  @BuiltValueField(wireName: r'quotationNumber')
  String? get quotationNumber;

  @BuiltValueField(wireName: r'evidence')
  AcceptanceEvidenceResponse? get evidence;

  PublicAcceptanceResponse._();

  factory PublicAcceptanceResponse([void updates(PublicAcceptanceResponseBuilder b)]) = _$PublicAcceptanceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PublicAcceptanceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PublicAcceptanceResponse> get serializer => _$PublicAcceptanceResponseSerializer();
}

class _$PublicAcceptanceResponseSerializer implements PrimitiveSerializer<PublicAcceptanceResponse> {
  @override
  final Iterable<Type> types = const [PublicAcceptanceResponse, _$PublicAcceptanceResponse];

  @override
  final String wireName = r'PublicAcceptanceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PublicAcceptanceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
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
    if (object.quotationNumber != null) {
      yield r'quotationNumber';
      yield serializers.serialize(
        object.quotationNumber,
        specifiedType: const FullType.nullable(String),
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
    PublicAcceptanceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PublicAcceptanceResponseBuilder result,
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
        case r'acceptedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.acceptedAtUtc = valueDes;
          break;
        case r'quotationNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationNumber = valueDes;
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
  PublicAcceptanceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PublicAcceptanceResponseBuilder();
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


