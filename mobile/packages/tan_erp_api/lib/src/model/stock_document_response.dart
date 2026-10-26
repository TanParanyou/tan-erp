//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/inventory_person_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/stock_movement_response.dart';
import 'package:tan_erp_api/src/model/inventory_warehouse_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'stock_document_response.g.dart';

/// StockDocumentResponse
///
/// Properties:
/// * [id] 
/// * [documentType] 
/// * [number] 
/// * [warehouse] 
/// * [toWarehouse] 
/// * [projectId] 
/// * [sourceType] 
/// * [sourceId] 
/// * [reason] 
/// * [occurredAtUtc] 
/// * [postedBy] 
/// * [postedAtUtc] 
/// * [movements] 
@BuiltValue()
abstract class StockDocumentResponse implements Built<StockDocumentResponse, StockDocumentResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'documentType')
  String? get documentType;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'warehouse')
  InventoryWarehouseRefResponse? get warehouse;

  @BuiltValueField(wireName: r'toWarehouse')
  InventoryWarehouseRefResponse? get toWarehouse;

  @BuiltValueField(wireName: r'projectId')
  String? get projectId;

  @BuiltValueField(wireName: r'sourceType')
  String? get sourceType;

  @BuiltValueField(wireName: r'sourceId')
  String? get sourceId;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'occurredAtUtc')
  DateTime? get occurredAtUtc;

  @BuiltValueField(wireName: r'postedBy')
  InventoryPersonResponse? get postedBy;

  @BuiltValueField(wireName: r'postedAtUtc')
  DateTime? get postedAtUtc;

  @BuiltValueField(wireName: r'movements')
  BuiltList<StockMovementResponse>? get movements;

  StockDocumentResponse._();

  factory StockDocumentResponse([void updates(StockDocumentResponseBuilder b)]) = _$StockDocumentResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(StockDocumentResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<StockDocumentResponse> get serializer => _$StockDocumentResponseSerializer();
}

class _$StockDocumentResponseSerializer implements PrimitiveSerializer<StockDocumentResponse> {
  @override
  final Iterable<Type> types = const [StockDocumentResponse, _$StockDocumentResponse];

  @override
  final String wireName = r'StockDocumentResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    StockDocumentResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.documentType != null) {
      yield r'documentType';
      yield serializers.serialize(
        object.documentType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.warehouse != null) {
      yield r'warehouse';
      yield serializers.serialize(
        object.warehouse,
        specifiedType: const FullType(InventoryWarehouseRefResponse),
      );
    }
    if (object.toWarehouse != null) {
      yield r'toWarehouse';
      yield serializers.serialize(
        object.toWarehouse,
        specifiedType: const FullType(InventoryWarehouseRefResponse),
      );
    }
    if (object.projectId != null) {
      yield r'projectId';
      yield serializers.serialize(
        object.projectId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sourceType != null) {
      yield r'sourceType';
      yield serializers.serialize(
        object.sourceType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sourceId != null) {
      yield r'sourceId';
      yield serializers.serialize(
        object.sourceId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.occurredAtUtc != null) {
      yield r'occurredAtUtc';
      yield serializers.serialize(
        object.occurredAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.postedBy != null) {
      yield r'postedBy';
      yield serializers.serialize(
        object.postedBy,
        specifiedType: const FullType(InventoryPersonResponse),
      );
    }
    if (object.postedAtUtc != null) {
      yield r'postedAtUtc';
      yield serializers.serialize(
        object.postedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.movements != null) {
      yield r'movements';
      yield serializers.serialize(
        object.movements,
        specifiedType: const FullType.nullable(BuiltList, [FullType(StockMovementResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    StockDocumentResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required StockDocumentResponseBuilder result,
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
        case r'documentType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.documentType = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'warehouse':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(InventoryWarehouseRefResponse),
          ) as InventoryWarehouseRefResponse?;
          if (valueDes == null) continue;
          result.warehouse.replace(valueDes);
          break;
        case r'toWarehouse':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(InventoryWarehouseRefResponse),
          ) as InventoryWarehouseRefResponse?;
          if (valueDes == null) continue;
          result.toWarehouse.replace(valueDes);
          break;
        case r'projectId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectId = valueDes;
          break;
        case r'sourceType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceType = valueDes;
          break;
        case r'sourceId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceId = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'occurredAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.occurredAtUtc = valueDes;
          break;
        case r'postedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(InventoryPersonResponse),
          ) as InventoryPersonResponse?;
          if (valueDes == null) continue;
          result.postedBy.replace(valueDes);
          break;
        case r'postedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.postedAtUtc = valueDes;
          break;
        case r'movements':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(StockMovementResponse)]),
          ) as BuiltList<StockMovementResponse>?;
          if (valueDes == null) continue;
          result.movements.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  StockDocumentResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = StockDocumentResponseBuilder();
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


