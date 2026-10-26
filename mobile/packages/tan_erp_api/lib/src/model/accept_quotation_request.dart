//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'accept_quotation_request.g.dart';

/// AcceptQuotationRequest
///
/// Properties:
/// * [expectedOpportunityVersion] 
/// * [decisionNote] 
@BuiltValue()
abstract class AcceptQuotationRequest implements Built<AcceptQuotationRequest, AcceptQuotationRequestBuilder> {
  @BuiltValueField(wireName: r'expectedOpportunityVersion')
  String? get expectedOpportunityVersion;

  @BuiltValueField(wireName: r'decisionNote')
  String? get decisionNote;

  AcceptQuotationRequest._();

  factory AcceptQuotationRequest([void updates(AcceptQuotationRequestBuilder b)]) = _$AcceptQuotationRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AcceptQuotationRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AcceptQuotationRequest> get serializer => _$AcceptQuotationRequestSerializer();
}

class _$AcceptQuotationRequestSerializer implements PrimitiveSerializer<AcceptQuotationRequest> {
  @override
  final Iterable<Type> types = const [AcceptQuotationRequest, _$AcceptQuotationRequest];

  @override
  final String wireName = r'AcceptQuotationRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AcceptQuotationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.expectedOpportunityVersion != null) {
      yield r'expectedOpportunityVersion';
      yield serializers.serialize(
        object.expectedOpportunityVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.decisionNote != null) {
      yield r'decisionNote';
      yield serializers.serialize(
        object.decisionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AcceptQuotationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AcceptQuotationRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'expectedOpportunityVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedOpportunityVersion = valueDes;
          break;
        case r'decisionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.decisionNote = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AcceptQuotationRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AcceptQuotationRequestBuilder();
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


