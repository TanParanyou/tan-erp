//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_invoice_list_item_response.g.dart';

/// SupplierInvoiceListItemResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [supplierInvoiceNumber] 
/// * [status] 
/// * [matchStatus] 
/// * [purchaseOrderId] 
/// * [purchaseOrderNumber] 
/// * [supplierCode] 
/// * [supplierNameTh] 
/// * [projectCode] 
/// * [invoiceDate] 
/// * [dueDate] 
/// * [subtotalAmount] 
/// * [taxAmount] 
/// * [totalAmount] 
/// * [totalVarianceAmount] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class SupplierInvoiceListItemResponse implements Built<SupplierInvoiceListItemResponse, SupplierInvoiceListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'supplierInvoiceNumber')
  String? get supplierInvoiceNumber;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'matchStatus')
  String? get matchStatus;

  @BuiltValueField(wireName: r'purchaseOrderId')
  String? get purchaseOrderId;

  @BuiltValueField(wireName: r'purchaseOrderNumber')
  String? get purchaseOrderNumber;

  @BuiltValueField(wireName: r'supplierCode')
  String? get supplierCode;

  @BuiltValueField(wireName: r'supplierNameTh')
  String? get supplierNameTh;

  @BuiltValueField(wireName: r'projectCode')
  String? get projectCode;

  @BuiltValueField(wireName: r'invoiceDate')
  Date? get invoiceDate;

  @BuiltValueField(wireName: r'dueDate')
  Date? get dueDate;

  @BuiltValueField(wireName: r'subtotalAmount')
  double? get subtotalAmount;

  @BuiltValueField(wireName: r'taxAmount')
  double? get taxAmount;

  @BuiltValueField(wireName: r'totalAmount')
  double? get totalAmount;

  @BuiltValueField(wireName: r'totalVarianceAmount')
  double? get totalVarianceAmount;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  SupplierInvoiceListItemResponse._();

  factory SupplierInvoiceListItemResponse([void updates(SupplierInvoiceListItemResponseBuilder b)]) = _$SupplierInvoiceListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierInvoiceListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierInvoiceListItemResponse> get serializer => _$SupplierInvoiceListItemResponseSerializer();
}

class _$SupplierInvoiceListItemResponseSerializer implements PrimitiveSerializer<SupplierInvoiceListItemResponse> {
  @override
  final Iterable<Type> types = const [SupplierInvoiceListItemResponse, _$SupplierInvoiceListItemResponse];

  @override
  final String wireName = r'SupplierInvoiceListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierInvoiceListItemResponse object, {
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
    if (object.supplierInvoiceNumber != null) {
      yield r'supplierInvoiceNumber';
      yield serializers.serialize(
        object.supplierInvoiceNumber,
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
    if (object.matchStatus != null) {
      yield r'matchStatus';
      yield serializers.serialize(
        object.matchStatus,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.purchaseOrderId != null) {
      yield r'purchaseOrderId';
      yield serializers.serialize(
        object.purchaseOrderId,
        specifiedType: const FullType(String),
      );
    }
    if (object.purchaseOrderNumber != null) {
      yield r'purchaseOrderNumber';
      yield serializers.serialize(
        object.purchaseOrderNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.supplierCode != null) {
      yield r'supplierCode';
      yield serializers.serialize(
        object.supplierCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.supplierNameTh != null) {
      yield r'supplierNameTh';
      yield serializers.serialize(
        object.supplierNameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.projectCode != null) {
      yield r'projectCode';
      yield serializers.serialize(
        object.projectCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.invoiceDate != null) {
      yield r'invoiceDate';
      yield serializers.serialize(
        object.invoiceDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.dueDate != null) {
      yield r'dueDate';
      yield serializers.serialize(
        object.dueDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.subtotalAmount != null) {
      yield r'subtotalAmount';
      yield serializers.serialize(
        object.subtotalAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.taxAmount != null) {
      yield r'taxAmount';
      yield serializers.serialize(
        object.taxAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalAmount != null) {
      yield r'totalAmount';
      yield serializers.serialize(
        object.totalAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalVarianceAmount != null) {
      yield r'totalVarianceAmount';
      yield serializers.serialize(
        object.totalVarianceAmount,
        specifiedType: const FullType(double),
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
    SupplierInvoiceListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierInvoiceListItemResponseBuilder result,
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
        case r'supplierInvoiceNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supplierInvoiceNumber = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'matchStatus':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.matchStatus = valueDes;
          break;
        case r'purchaseOrderId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.purchaseOrderId = valueDes;
          break;
        case r'purchaseOrderNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.purchaseOrderNumber = valueDes;
          break;
        case r'supplierCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supplierCode = valueDes;
          break;
        case r'supplierNameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supplierNameTh = valueDes;
          break;
        case r'projectCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectCode = valueDes;
          break;
        case r'invoiceDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.invoiceDate = valueDes;
          break;
        case r'dueDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.dueDate = valueDes;
          break;
        case r'subtotalAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.subtotalAmount = valueDes;
          break;
        case r'taxAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.taxAmount = valueDes;
          break;
        case r'totalAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalAmount = valueDes;
          break;
        case r'totalVarianceAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalVarianceAmount = valueDes;
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
  SupplierInvoiceListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierInvoiceListItemResponseBuilder();
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


