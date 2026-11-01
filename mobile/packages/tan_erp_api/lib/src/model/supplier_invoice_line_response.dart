//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_invoice_line_response.g.dart';

/// SupplierInvoiceLineResponse
///
/// Properties:
/// * [id] 
/// * [lineNo] 
/// * [purchaseOrderLineId] 
/// * [itemId] 
/// * [itemCode] 
/// * [itemNameTh] 
/// * [unitId] 
/// * [unitCode] 
/// * [quantity] 
/// * [poUnitPrice] 
/// * [unitPrice] 
/// * [lineTotal] 
/// * [priceVarianceAmount] 
/// * [priceVariancePercent] 
/// * [exceedsTolerance] 
@BuiltValue()
abstract class SupplierInvoiceLineResponse implements Built<SupplierInvoiceLineResponse, SupplierInvoiceLineResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'lineNo')
  int? get lineNo;

  @BuiltValueField(wireName: r'purchaseOrderLineId')
  String? get purchaseOrderLineId;

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

  @BuiltValueField(wireName: r'poUnitPrice')
  double? get poUnitPrice;

  @BuiltValueField(wireName: r'unitPrice')
  double? get unitPrice;

  @BuiltValueField(wireName: r'lineTotal')
  double? get lineTotal;

  @BuiltValueField(wireName: r'priceVarianceAmount')
  double? get priceVarianceAmount;

  @BuiltValueField(wireName: r'priceVariancePercent')
  double? get priceVariancePercent;

  @BuiltValueField(wireName: r'exceedsTolerance')
  bool? get exceedsTolerance;

  SupplierInvoiceLineResponse._();

  factory SupplierInvoiceLineResponse([void updates(SupplierInvoiceLineResponseBuilder b)]) = _$SupplierInvoiceLineResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierInvoiceLineResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierInvoiceLineResponse> get serializer => _$SupplierInvoiceLineResponseSerializer();
}

class _$SupplierInvoiceLineResponseSerializer implements PrimitiveSerializer<SupplierInvoiceLineResponse> {
  @override
  final Iterable<Type> types = const [SupplierInvoiceLineResponse, _$SupplierInvoiceLineResponse];

  @override
  final String wireName = r'SupplierInvoiceLineResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierInvoiceLineResponse object, {
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
    if (object.poUnitPrice != null) {
      yield r'poUnitPrice';
      yield serializers.serialize(
        object.poUnitPrice,
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
    if (object.priceVarianceAmount != null) {
      yield r'priceVarianceAmount';
      yield serializers.serialize(
        object.priceVarianceAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.priceVariancePercent != null) {
      yield r'priceVariancePercent';
      yield serializers.serialize(
        object.priceVariancePercent,
        specifiedType: const FullType(double),
      );
    }
    if (object.exceedsTolerance != null) {
      yield r'exceedsTolerance';
      yield serializers.serialize(
        object.exceedsTolerance,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SupplierInvoiceLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierInvoiceLineResponseBuilder result,
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
        case r'poUnitPrice':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.poUnitPrice = valueDes;
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
        case r'priceVarianceAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.priceVarianceAmount = valueDes;
          break;
        case r'priceVariancePercent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.priceVariancePercent = valueDes;
          break;
        case r'exceedsTolerance':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.exceedsTolerance = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SupplierInvoiceLineResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierInvoiceLineResponseBuilder();
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


