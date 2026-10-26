//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'disable_cost_record_request.g.dart';

/// DisableCostRecordRequest
///
/// Properties:
/// * [reason] 
@BuiltValue()
abstract class DisableCostRecordRequest implements Built<DisableCostRecordRequest, DisableCostRecordRequestBuilder> {
  @BuiltValueField(wireName: r'reason')
  String get reason;

  DisableCostRecordRequest._();

  factory DisableCostRecordRequest([void updates(DisableCostRecordRequestBuilder b)]) = _$DisableCostRecordRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(DisableCostRecordRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<DisableCostRecordRequest> get serializer => _$DisableCostRecordRequestSerializer();
}

class _$DisableCostRecordRequestSerializer implements PrimitiveSerializer<DisableCostRecordRequest> {
  @override
  final Iterable<Type> types = const [DisableCostRecordRequest, _$DisableCostRecordRequest];

  @override
  final String wireName = r'DisableCostRecordRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    DisableCostRecordRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'reason';
    yield serializers.serialize(
      object.reason,
      specifiedType: const FullType(String),
    );
  }

  @override
  Object serialize(
    Serializers serializers,
    DisableCostRecordRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required DisableCostRecordRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
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
  DisableCostRecordRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = DisableCostRecordRequestBuilder();
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


