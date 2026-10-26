//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_order_line_response.g.dart';

/// PurchaseOrderLineResponse
///
/// Properties:
/// * [id] 
/// * [lineNo] 
/// * [itemId] 
/// * [itemCode] 
/// * [itemNameTh] 
/// * [unitId] 
/// * [unitCode] 
/// * [quantity] 
/// * [unitPrice] 
/// * [lineTotal] 
/// * [receivedQuantity] 
/// * [remainingQuantity] 
@BuiltValue()
abstract class PurchaseOrderLineResponse implements Built<PurchaseOrderLineResponse, PurchaseOrderLineResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'lineNo')
  int? get lineNo;

  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'itemCode')
  String? get itemCode;

  @BuiltValueField(wireName: r'itemNameTh')
  String? get itemNameTh;

  @BuiltValueField(wireName: r'unitId')
  String? get unitId;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'unitPrice')
  double? get unitPrice;

  @BuiltValueField(wireName: r'lineTotal')
  double? get lineTotal;

  @BuiltValueField(wireName: r'receivedQuantity')
  double? get receivedQuantity;

  @BuiltValueField(wireName: r'remainingQuantity')
  double? get remainingQuantity;

  PurchaseOrderLineResponse._();

  factory PurchaseOrderLineResponse([void updates(PurchaseOrderLineResponseBuilder b)]) = _$PurchaseOrderLineResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseOrderLineResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseOrderLineResponse> get serializer => _$PurchaseOrderLineResponseSerializer();
}

class _$PurchaseOrderLineResponseSerializer implements PrimitiveSerializer<PurchaseOrderLineResponse> {
  @override
  final Iterable<Type> types = const [PurchaseOrderLineResponse, _$PurchaseOrderLineResponse];

  @override
  final String wireName = r'PurchaseOrderLineResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseOrderLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.lineNo != null) {
      yield r'lineNo';
      yield serializers.serialize(
        object.lineNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
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
    if (object.unitId != null) {
      yield r'unitId';
      yield serializers.serialize(
        object.unitId,
        specifiedType: const FullType(String),
      );
    }
    if (object.unitCode != null) {
      yield r'unitCode';
      yield serializers.serialize(
        object.unitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.unitPrice != null) {
      yield r'unitPrice';
      yield serializers.serialize(
        object.unitPrice,
        specifiedType: const FullType(double),
      );
    }
    if (object.lineTotal != null) {
      yield r'lineTotal';
      yield serializers.serialize(
        object.lineTotal,
        specifiedType: const FullType(double),
      );
    }
    if (object.receivedQuantity != null) {
      yield r'receivedQuantity';
      yield serializers.serialize(
        object.receivedQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.remainingQuantity != null) {
      yield r'remainingQuantity';
      yield serializers.serialize(
        object.remainingQuantity,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PurchaseOrderLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseOrderLineResponseBuilder result,
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
        case r'lineNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.lineNo = valueDes;
          break;
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
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
        case r'unitId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitId = valueDes;
          break;
        case r'unitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitCode = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        case r'unitPrice':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitPrice = valueDes;
          break;
        case r'lineTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.lineTotal = valueDes;
          break;
        case r'receivedQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.receivedQuantity = valueDes;
          break;
        case r'remainingQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.remainingQuantity = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PurchaseOrderLineResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseOrderLineResponseBuilder();
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


