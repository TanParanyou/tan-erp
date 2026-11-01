//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/work_center_ref_response.dart';
import 'package:tan_erp_api/src/model/work_order_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_operation_response.g.dart';

/// WorkOrderOperationResponse
///
/// Properties:
/// * [id] 
/// * [sequence] 
/// * [operationCode] 
/// * [operationName] 
/// * [workCenter] 
/// * [plannedSetupHours] 
/// * [plannedRunHours] 
/// * [actualHours] 
/// * [isSubcontracted] 
/// * [subcontractPurchaseOrder] 
/// * [subcontractCost] 
/// * [status] 
/// * [note] 
/// * [completedAtUtc] 
@BuiltValue()
abstract class WorkOrderOperationResponse implements Built<WorkOrderOperationResponse, WorkOrderOperationResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'sequence')
  int? get sequence;

  @BuiltValueField(wireName: r'operationCode')
  String? get operationCode;

  @BuiltValueField(wireName: r'operationName')
  String? get operationName;

  @BuiltValueField(wireName: r'workCenter')
  WorkCenterRefResponse? get workCenter;

  @BuiltValueField(wireName: r'plannedSetupHours')
  double? get plannedSetupHours;

  @BuiltValueField(wireName: r'plannedRunHours')
  double? get plannedRunHours;

  @BuiltValueField(wireName: r'actualHours')
  double? get actualHours;

  @BuiltValueField(wireName: r'isSubcontracted')
  bool? get isSubcontracted;

  @BuiltValueField(wireName: r'subcontractPurchaseOrder')
  WorkOrderRefResponse? get subcontractPurchaseOrder;

  @BuiltValueField(wireName: r'subcontractCost')
  double? get subcontractCost;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'completedAtUtc')
  DateTime? get completedAtUtc;

  WorkOrderOperationResponse._();

  factory WorkOrderOperationResponse([void updates(WorkOrderOperationResponseBuilder b)]) = _$WorkOrderOperationResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderOperationResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderOperationResponse> get serializer => _$WorkOrderOperationResponseSerializer();
}

class _$WorkOrderOperationResponseSerializer implements PrimitiveSerializer<WorkOrderOperationResponse> {
  @override
  final Iterable<Type> types = const [WorkOrderOperationResponse, _$WorkOrderOperationResponse];

  @override
  final String wireName = r'WorkOrderOperationResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderOperationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.sequence != null) {
      yield r'sequence';
      yield serializers.serialize(
        object.sequence,
        specifiedType: const FullType(int),
      );
    }
    if (object.operationCode != null) {
      yield r'operationCode';
      yield serializers.serialize(
        object.operationCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.operationName != null) {
      yield r'operationName';
      yield serializers.serialize(
        object.operationName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.workCenter != null) {
      yield r'workCenter';
      yield serializers.serialize(
        object.workCenter,
        specifiedType: const FullType(WorkCenterRefResponse),
      );
    }
    if (object.plannedSetupHours != null) {
      yield r'plannedSetupHours';
      yield serializers.serialize(
        object.plannedSetupHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.plannedRunHours != null) {
      yield r'plannedRunHours';
      yield serializers.serialize(
        object.plannedRunHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.actualHours != null) {
      yield r'actualHours';
      yield serializers.serialize(
        object.actualHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.isSubcontracted != null) {
      yield r'isSubcontracted';
      yield serializers.serialize(
        object.isSubcontracted,
        specifiedType: const FullType(bool),
      );
    }
    if (object.subcontractPurchaseOrder != null) {
      yield r'subcontractPurchaseOrder';
      yield serializers.serialize(
        object.subcontractPurchaseOrder,
        specifiedType: const FullType(WorkOrderRefResponse),
      );
    }
    if (object.subcontractCost != null) {
      yield r'subcontractCost';
      yield serializers.serialize(
        object.subcontractCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.completedAtUtc != null) {
      yield r'completedAtUtc';
      yield serializers.serialize(
        object.completedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkOrderOperationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderOperationResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'sequence':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sequence = valueDes;
          break;
        case r'operationCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.operationCode = valueDes;
          break;
        case r'operationName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.operationName = valueDes;
          break;
        case r'workCenter':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(WorkCenterRefResponse),
          ) as WorkCenterRefResponse?;
          if (valueDes == null) continue;
          result.workCenter.replace(valueDes);
          break;
        case r'plannedSetupHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.plannedSetupHours = valueDes;
          break;
        case r'plannedRunHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.plannedRunHours = valueDes;
          break;
        case r'actualHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.actualHours = valueDes;
          break;
        case r'isSubcontracted':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isSubcontracted = valueDes;
          break;
        case r'subcontractPurchaseOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(WorkOrderRefResponse),
          ) as WorkOrderRefResponse?;
          if (valueDes == null) continue;
          result.subcontractPurchaseOrder.replace(valueDes);
          break;
        case r'subcontractCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.subcontractCost = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'completedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.completedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkOrderOperationResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderOperationResponseBuilder();
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


