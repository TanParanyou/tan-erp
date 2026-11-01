//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_subcontract_operation_request.g.dart';

/// WorkOrderSubcontractOperationRequest
///
/// Properties:
/// * [purchaseOrderId] 
/// * [costAmount] 
/// * [note] 
@BuiltValue()
abstract class WorkOrderSubcontractOperationRequest implements Built<WorkOrderSubcontractOperationRequest, WorkOrderSubcontractOperationRequestBuilder> {
  @BuiltValueField(wireName: r'purchaseOrderId')
  String? get purchaseOrderId;

  @BuiltValueField(wireName: r'costAmount')
  double? get costAmount;

  @BuiltValueField(wireName: r'note')
  String? get note;

  WorkOrderSubcontractOperationRequest._();

  factory WorkOrderSubcontractOperationRequest([void updates(WorkOrderSubcontractOperationRequestBuilder b)]) = _$WorkOrderSubcontractOperationRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderSubcontractOperationRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderSubcontractOperationRequest> get serializer => _$WorkOrderSubcontractOperationRequestSerializer();
}

class _$WorkOrderSubcontractOperationRequestSerializer implements PrimitiveSerializer<WorkOrderSubcontractOperationRequest> {
  @override
  final Iterable<Type> types = const [WorkOrderSubcontractOperationRequest, _$WorkOrderSubcontractOperationRequest];

  @override
  final String wireName = r'WorkOrderSubcontractOperationRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderSubcontractOperationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.purchaseOrderId != null) {
      yield r'purchaseOrderId';
      yield serializers.serialize(
        object.purchaseOrderId,
        specifiedType: const FullType(String),
      );
    }
    if (object.costAmount != null) {
      yield r'costAmount';
      yield serializers.serialize(
        object.costAmount,
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
    WorkOrderSubcontractOperationRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderSubcontractOperationRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'purchaseOrderId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.purchaseOrderId = valueDes;
          break;
        case r'costAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.costAmount = valueDes;
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
  WorkOrderSubcontractOperationRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderSubcontractOperationRequestBuilder();
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


