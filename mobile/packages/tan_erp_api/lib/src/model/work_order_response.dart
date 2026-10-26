//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/work_order_transaction_response.dart';
import 'package:tan_erp_api/src/model/work_order_material_response.dart';
import 'package:tan_erp_api/src/model/production_item_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/production_person_response.dart';
import 'package:tan_erp_api/src/model/work_order_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_response.g.dart';

/// WorkOrderResponse
///
/// Properties:
/// * [id] 
/// * [branchId] 
/// * [number] 
/// * [status] 
/// * [item] 
/// * [bomRevisionId] 
/// * [bomCode] 
/// * [bomRevisionNo] 
/// * [warehouse] 
/// * [project] 
/// * [plannedQuantity] 
/// * [completedQuantity] 
/// * [costAllocated] 
/// * [note] 
/// * [cancelReason] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [rowVersion] 
/// * [materials] 
/// * [transactions] 
@BuiltValue()
abstract class WorkOrderResponse implements Built<WorkOrderResponse, WorkOrderResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'item')
  ProductionItemResponse? get item;

  @BuiltValueField(wireName: r'bomRevisionId')
  String? get bomRevisionId;

  @BuiltValueField(wireName: r'bomCode')
  String? get bomCode;

  @BuiltValueField(wireName: r'bomRevisionNo')
  int? get bomRevisionNo;

  @BuiltValueField(wireName: r'warehouse')
  WorkOrderRefResponse? get warehouse;

  @BuiltValueField(wireName: r'project')
  WorkOrderRefResponse? get project;

  @BuiltValueField(wireName: r'plannedQuantity')
  double? get plannedQuantity;

  @BuiltValueField(wireName: r'completedQuantity')
  double? get completedQuantity;

  @BuiltValueField(wireName: r'costAllocated')
  double? get costAllocated;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'cancelReason')
  String? get cancelReason;

  @BuiltValueField(wireName: r'createdBy')
  ProductionPersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'materials')
  BuiltList<WorkOrderMaterialResponse>? get materials;

  @BuiltValueField(wireName: r'transactions')
  BuiltList<WorkOrderTransactionResponse>? get transactions;

  WorkOrderResponse._();

  factory WorkOrderResponse([void updates(WorkOrderResponseBuilder b)]) = _$WorkOrderResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderResponse> get serializer => _$WorkOrderResponseSerializer();
}

class _$WorkOrderResponseSerializer implements PrimitiveSerializer<WorkOrderResponse> {
  @override
  final Iterable<Type> types = const [WorkOrderResponse, _$WorkOrderResponse];

  @override
  final String wireName = r'WorkOrderResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.item != null) {
      yield r'item';
      yield serializers.serialize(
        object.item,
        specifiedType: const FullType(ProductionItemResponse),
      );
    }
    if (object.bomRevisionId != null) {
      yield r'bomRevisionId';
      yield serializers.serialize(
        object.bomRevisionId,
        specifiedType: const FullType(String),
      );
    }
    if (object.bomCode != null) {
      yield r'bomCode';
      yield serializers.serialize(
        object.bomCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.bomRevisionNo != null) {
      yield r'bomRevisionNo';
      yield serializers.serialize(
        object.bomRevisionNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.warehouse != null) {
      yield r'warehouse';
      yield serializers.serialize(
        object.warehouse,
        specifiedType: const FullType(WorkOrderRefResponse),
      );
    }
    if (object.project != null) {
      yield r'project';
      yield serializers.serialize(
        object.project,
        specifiedType: const FullType(WorkOrderRefResponse),
      );
    }
    if (object.plannedQuantity != null) {
      yield r'plannedQuantity';
      yield serializers.serialize(
        object.plannedQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.completedQuantity != null) {
      yield r'completedQuantity';
      yield serializers.serialize(
        object.completedQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.costAllocated != null) {
      yield r'costAllocated';
      yield serializers.serialize(
        object.costAllocated,
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
    if (object.cancelReason != null) {
      yield r'cancelReason';
      yield serializers.serialize(
        object.cancelReason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(ProductionPersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.materials != null) {
      yield r'materials';
      yield serializers.serialize(
        object.materials,
        specifiedType: const FullType.nullable(BuiltList, [FullType(WorkOrderMaterialResponse)]),
      );
    }
    if (object.transactions != null) {
      yield r'transactions';
      yield serializers.serialize(
        object.transactions,
        specifiedType: const FullType.nullable(BuiltList, [FullType(WorkOrderTransactionResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkOrderResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderResponseBuilder result,
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
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'item':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProductionItemResponse),
          ) as ProductionItemResponse?;
          if (valueDes == null) continue;
          result.item.replace(valueDes);
          break;
        case r'bomRevisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.bomRevisionId = valueDes;
          break;
        case r'bomCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.bomCode = valueDes;
          break;
        case r'bomRevisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.bomRevisionNo = valueDes;
          break;
        case r'warehouse':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(WorkOrderRefResponse),
          ) as WorkOrderRefResponse?;
          if (valueDes == null) continue;
          result.warehouse.replace(valueDes);
          break;
        case r'project':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(WorkOrderRefResponse),
          ) as WorkOrderRefResponse?;
          if (valueDes == null) continue;
          result.project.replace(valueDes);
          break;
        case r'plannedQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.plannedQuantity = valueDes;
          break;
        case r'completedQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.completedQuantity = valueDes;
          break;
        case r'costAllocated':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.costAllocated = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'cancelReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.cancelReason = valueDes;
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProductionPersonResponse),
          ) as ProductionPersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'materials':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(WorkOrderMaterialResponse)]),
          ) as BuiltList<WorkOrderMaterialResponse>?;
          if (valueDes == null) continue;
          result.materials.replace(valueDes);
          break;
        case r'transactions':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(WorkOrderTransactionResponse)]),
          ) as BuiltList<WorkOrderTransactionResponse>?;
          if (valueDes == null) continue;
          result.transactions.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkOrderResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderResponseBuilder();
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


