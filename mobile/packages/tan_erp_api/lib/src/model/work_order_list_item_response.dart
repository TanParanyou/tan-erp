//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_list_item_response.g.dart';

/// WorkOrderListItemResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [status] 
/// * [itemCode] 
/// * [itemNameTh] 
/// * [plannedQuantity] 
/// * [completedQuantity] 
/// * [projectCode] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class WorkOrderListItemResponse implements Built<WorkOrderListItemResponse, WorkOrderListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'itemCode')
  String? get itemCode;

  @BuiltValueField(wireName: r'itemNameTh')
  String? get itemNameTh;

  @BuiltValueField(wireName: r'plannedQuantity')
  double? get plannedQuantity;

  @BuiltValueField(wireName: r'completedQuantity')
  double? get completedQuantity;

  @BuiltValueField(wireName: r'projectCode')
  String? get projectCode;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  WorkOrderListItemResponse._();

  factory WorkOrderListItemResponse([void updates(WorkOrderListItemResponseBuilder b)]) = _$WorkOrderListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderListItemResponse> get serializer => _$WorkOrderListItemResponseSerializer();
}

class _$WorkOrderListItemResponseSerializer implements PrimitiveSerializer<WorkOrderListItemResponse> {
  @override
  final Iterable<Type> types = const [WorkOrderListItemResponse, _$WorkOrderListItemResponse];

  @override
  final String wireName = r'WorkOrderListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
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
    if (object.itemCode != null) {
      yield r'itemCode';
      yield serializers.serialize(
        object.itemCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.itemNameTh != null) {
      yield r'itemNameTh';
      yield serializers.serialize(
        object.itemNameTh,
        specifiedType: const FullType.nullable(String),
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
    if (object.projectCode != null) {
      yield r'projectCode';
      yield serializers.serialize(
        object.projectCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkOrderListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderListItemResponseBuilder result,
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
        case r'itemCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemCode = valueDes;
          break;
        case r'itemNameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemNameTh = valueDes;
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
        case r'projectCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectCode = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkOrderListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderListItemResponseBuilder();
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


