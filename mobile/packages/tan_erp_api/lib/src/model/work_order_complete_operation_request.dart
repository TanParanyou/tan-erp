//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_complete_operation_request.g.dart';

/// WorkOrderCompleteOperationRequest
///
/// Properties:
/// * [actualHours] 
/// * [note] 
@BuiltValue()
abstract class WorkOrderCompleteOperationRequest implements Built<WorkOrderCompleteOperationRequest, WorkOrderCompleteOperationRequestBuilder> {
  @BuiltValueField(wireName: r'actualHours')
  double? get actualHours;

  @BuiltValueField(wireName: r'note')
  String? get note;

  WorkOrderCompleteOperationRequest._();

  factory WorkOrderCompleteOperationRequest([void updates(WorkOrderCompleteOperationRequestBuilder b)]) = _$WorkOrderCompleteOperationRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderCompleteOperationRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderCompleteOperationRequest> get serializer => _$WorkOrderCompleteOperationRequestSerializer();
}

class _$WorkOrderCompleteOperationRequestSerializer implements PrimitiveSerializer<WorkOrderCompleteOperationRequest> {
  @override
  final Iterable<Type> types = const [WorkOrderCompleteOperationRequest, _$WorkOrderCompleteOperationRequest];

  @override
  final String wireName = r'WorkOrderCompleteOperationRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderCompleteOperationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.actualHours != null) {
      yield r'actualHours';
      yield serializers.serialize(
        object.actualHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkOrderCompleteOperationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderCompleteOperationRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'actualHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.actualHours = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkOrderCompleteOperationRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderCompleteOperationRequestBuilder();
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


