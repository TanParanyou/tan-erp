//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'cancel_estimate_request.g.dart';

/// CancelEstimateRequest
///
/// Properties:
/// * [reason] 
@BuiltValue()
abstract class CancelEstimateRequest implements Built<CancelEstimateRequest, CancelEstimateRequestBuilder> {
  @BuiltValueField(wireName: r'reason')
  String? get reason;

  CancelEstimateRequest._();

  factory CancelEstimateRequest([void updates(CancelEstimateRequestBuilder b)]) = _$CancelEstimateRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CancelEstimateRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CancelEstimateRequest> get serializer => _$CancelEstimateRequestSerializer();
}

class _$CancelEstimateRequestSerializer implements PrimitiveSerializer<CancelEstimateRequest> {
  @override
  final Iterable<Type> types = const [CancelEstimateRequest, _$CancelEstimateRequest];

  @override
  final String wireName = r'CancelEstimateRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CancelEstimateRequest object, {
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
    CancelEstimateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CancelEstimateRequestBuilder result,
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
  CancelEstimateRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CancelEstimateRequestBuilder();
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


