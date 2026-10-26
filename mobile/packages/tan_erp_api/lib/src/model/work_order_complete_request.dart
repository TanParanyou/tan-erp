//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_complete_request.g.dart';

/// WorkOrderCompleteRequest
///
/// Properties:
/// * [quantity] 
@BuiltValue()
abstract class WorkOrderCompleteRequest implements Built<WorkOrderCompleteRequest, WorkOrderCompleteRequestBuilder> {
  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  WorkOrderCompleteRequest._();

  factory WorkOrderCompleteRequest([void updates(WorkOrderCompleteRequestBuilder b)]) = _$WorkOrderCompleteRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderCompleteRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderCompleteRequest> get serializer => _$WorkOrderCompleteRequestSerializer();
}

class _$WorkOrderCompleteRequestSerializer implements PrimitiveSerializer<WorkOrderCompleteRequest> {
  @override
  final Iterable<Type> types = const [WorkOrderCompleteRequest, _$WorkOrderCompleteRequest];

  @override
  final String wireName = r'WorkOrderCompleteRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderCompleteRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkOrderCompleteRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderCompleteRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkOrderCompleteRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderCompleteRequestBuilder();
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


