//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'goods_receipt_line_response.g.dart';

/// GoodsReceiptLineResponse
///
/// Properties:
/// * [id] 
/// * [purchaseOrderLineId] 
/// * [itemId] 
/// * [itemCode] 
/// * [itemNameTh] 
/// * [unitCode] 
/// * [quantity] 
/// * [unitPrice] 
/// * [returnedQuantity] 
/// * [remainingReturnableQuantity] 
@BuiltValue()
abstract class GoodsReceiptLineResponse implements Built<GoodsReceiptLineResponse, GoodsReceiptLineResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'purchaseOrderLineId')
  String? get purchaseOrderLineId;

  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'itemCode')
  String? get itemCode;

  @BuiltValueField(wireName: r'itemNameTh')
  String? get itemNameTh;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'unitPrice')
  double? get unitPrice;

  @BuiltValueField(wireName: r'returnedQuantity')
  double? get returnedQuantity;

  @BuiltValueField(wireName: r'remainingReturnableQuantity')
  double? get remainingReturnableQuantity;

  GoodsReceiptLineResponse._();

  factory GoodsReceiptLineResponse([void updates(GoodsReceiptLineResponseBuilder b)]) = _$GoodsReceiptLineResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(GoodsReceiptLineResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<GoodsReceiptLineResponse> get serializer => _$GoodsReceiptLineResponseSerializer();
}

class _$GoodsReceiptLineResponseSerializer implements PrimitiveSerializer<GoodsReceiptLineResponse> {
  @override
  final Iterable<Type> types = const [GoodsReceiptLineResponse, _$GoodsReceiptLineResponse];

  @override
  final String wireName = r'GoodsReceiptLineResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    GoodsReceiptLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.purchaseOrderLineId != null) {
      yield r'purchaseOrderLineId';
      yield serializers.serialize(
        object.purchaseOrderLineId,
        specifiedType: const FullType(String),
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
    if (object.returnedQuantity != null) {
      yield r'returnedQuantity';
      yield serializers.serialize(
        object.returnedQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.remainingReturnableQuantity != null) {
      yield r'remainingReturnableQuantity';
      yield serializers.serialize(
        object.remainingReturnableQuantity,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    GoodsReceiptLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required GoodsReceiptLineResponseBuilder result,
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
        case r'purchaseOrderLineId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.purchaseOrderLineId = valueDes;
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
        case r'returnedQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.returnedQuantity = valueDes;
          break;
        case r'remainingReturnableQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.remainingReturnableQuantity = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  GoodsReceiptLineResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = GoodsReceiptLineResponseBuilder();
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


