//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'finance_reason_request.g.dart';

/// FinanceReasonRequest
///
/// Properties:
/// * [reason] 
@BuiltValue()
abstract class FinanceReasonRequest implements Built<FinanceReasonRequest, FinanceReasonRequestBuilder> {
  @BuiltValueField(wireName: r'reason')
  String? get reason;

  FinanceReasonRequest._();

  factory FinanceReasonRequest([void updates(FinanceReasonRequestBuilder b)]) = _$FinanceReasonRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(FinanceReasonRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<FinanceReasonRequest> get serializer => _$FinanceReasonRequestSerializer();
}

class _$FinanceReasonRequestSerializer implements PrimitiveSerializer<FinanceReasonRequest> {
  @override
  final Iterable<Type> types = const [FinanceReasonRequest, _$FinanceReasonRequest];

  @override
  final String wireName = r'FinanceReasonRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    FinanceReasonRequest object, {
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
    FinanceReasonRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required FinanceReasonRequestBuilder result,
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
  FinanceReasonRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = FinanceReasonRequestBuilder();
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


