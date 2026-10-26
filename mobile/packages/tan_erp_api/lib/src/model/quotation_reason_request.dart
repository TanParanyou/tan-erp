//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_reason_request.g.dart';

/// QuotationReasonRequest
///
/// Properties:
/// * [reason] 
@BuiltValue()
abstract class QuotationReasonRequest implements Built<QuotationReasonRequest, QuotationReasonRequestBuilder> {
  @BuiltValueField(wireName: r'reason')
  String? get reason;

  QuotationReasonRequest._();

  factory QuotationReasonRequest([void updates(QuotationReasonRequestBuilder b)]) = _$QuotationReasonRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationReasonRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationReasonRequest> get serializer => _$QuotationReasonRequestSerializer();
}

class _$QuotationReasonRequestSerializer implements PrimitiveSerializer<QuotationReasonRequest> {
  @override
  final Iterable<Type> types = const [QuotationReasonRequest, _$QuotationReasonRequest];

  @override
  final String wireName = r'QuotationReasonRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationReasonRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationReasonRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationReasonRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuotationReasonRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationReasonRequestBuilder();
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


