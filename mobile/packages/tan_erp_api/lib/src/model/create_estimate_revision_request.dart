//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_estimate_revision_request.g.dart';

/// CreateEstimateRevisionRequest
///
/// Properties:
/// * [reason] 
@BuiltValue()
abstract class CreateEstimateRevisionRequest implements Built<CreateEstimateRevisionRequest, CreateEstimateRevisionRequestBuilder> {
  @BuiltValueField(wireName: r'reason')
  String? get reason;

  CreateEstimateRevisionRequest._();

  factory CreateEstimateRevisionRequest([void updates(CreateEstimateRevisionRequestBuilder b)]) = _$CreateEstimateRevisionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateEstimateRevisionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateEstimateRevisionRequest> get serializer => _$CreateEstimateRevisionRequestSerializer();
}

class _$CreateEstimateRevisionRequestSerializer implements PrimitiveSerializer<CreateEstimateRevisionRequest> {
  @override
  final Iterable<Type> types = const [CreateEstimateRevisionRequest, _$CreateEstimateRevisionRequest];

  @override
  final String wireName = r'CreateEstimateRevisionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateEstimateRevisionRequest object, {
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
    CreateEstimateRevisionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateEstimateRevisionRequestBuilder result,
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
  CreateEstimateRevisionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateEstimateRevisionRequestBuilder();
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


