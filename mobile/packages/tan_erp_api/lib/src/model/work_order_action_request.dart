//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_action_request.g.dart';

/// WorkOrderActionRequest
///
/// Properties:
/// * [reason] 
@BuiltValue()
abstract class WorkOrderActionRequest implements Built<WorkOrderActionRequest, WorkOrderActionRequestBuilder> {
  @BuiltValueField(wireName: r'reason')
  String? get reason;

  WorkOrderActionRequest._();

  factory WorkOrderActionRequest([void updates(WorkOrderActionRequestBuilder b)]) = _$WorkOrderActionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderActionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderActionRequest> get serializer => _$WorkOrderActionRequestSerializer();
}

class _$WorkOrderActionRequestSerializer implements PrimitiveSerializer<WorkOrderActionRequest> {
  @override
  final Iterable<Type> types = const [WorkOrderActionRequest, _$WorkOrderActionRequest];

  @override
  final String wireName = r'WorkOrderActionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderActionRequest object, {
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
    WorkOrderActionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderActionRequestBuilder result,
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
  WorkOrderActionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderActionRequestBuilder();
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


