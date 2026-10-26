//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'accept_quotation_response.g.dart';

/// AcceptQuotationResponse
///
/// Properties:
/// * [quotationId] 
/// * [opportunityId] 
/// * [opportunityStage] 
/// * [opportunityRowVersion] 
/// * [acceptedAtUtc] 
@BuiltValue()
abstract class AcceptQuotationResponse implements Built<AcceptQuotationResponse, AcceptQuotationResponseBuilder> {
  @BuiltValueField(wireName: r'quotationId')
  String? get quotationId;

  @BuiltValueField(wireName: r'opportunityId')
  String? get opportunityId;

  @BuiltValueField(wireName: r'opportunityStage')
  String? get opportunityStage;

  @BuiltValueField(wireName: r'opportunityRowVersion')
  String? get opportunityRowVersion;

  @BuiltValueField(wireName: r'acceptedAtUtc')
  DateTime? get acceptedAtUtc;

  AcceptQuotationResponse._();

  factory AcceptQuotationResponse([void updates(AcceptQuotationResponseBuilder b)]) = _$AcceptQuotationResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AcceptQuotationResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AcceptQuotationResponse> get serializer => _$AcceptQuotationResponseSerializer();
}

class _$AcceptQuotationResponseSerializer implements PrimitiveSerializer<AcceptQuotationResponse> {
  @override
  final Iterable<Type> types = const [AcceptQuotationResponse, _$AcceptQuotationResponse];

  @override
  final String wireName = r'AcceptQuotationResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AcceptQuotationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.quotationId != null) {
      yield r'quotationId';
      yield serializers.serialize(
        object.quotationId,
        specifiedType: const FullType(String),
      );
    }
    if (object.opportunityId != null) {
      yield r'opportunityId';
      yield serializers.serialize(
        object.opportunityId,
        specifiedType: const FullType(String),
      );
    }
    if (object.opportunityStage != null) {
      yield r'opportunityStage';
      yield serializers.serialize(
        object.opportunityStage,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.opportunityRowVersion != null) {
      yield r'opportunityRowVersion';
      yield serializers.serialize(
        object.opportunityRowVersion,
        specifiedType: const FullType(String),
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
    AcceptQuotationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AcceptQuotationResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'quotationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.quotationId = valueDes;
          break;
        case r'opportunityId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityId = valueDes;
          break;
        case r'opportunityStage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityStage = valueDes;
          break;
        case r'opportunityRowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityRowVersion = valueDes;
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
  AcceptQuotationResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AcceptQuotationResponseBuilder();
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


