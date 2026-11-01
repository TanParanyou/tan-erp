//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/supplier_invoice_line_request.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_invoice_request.g.dart';

/// SupplierInvoiceRequest
///
/// Properties:
/// * [purchaseOrderId] 
/// * [supplierInvoiceNumber] 
/// * [invoiceDate] 
/// * [dueDate] 
/// * [taxAmount] 
/// * [note] 
/// * [lines] 
@BuiltValue()
abstract class SupplierInvoiceRequest implements Built<SupplierInvoiceRequest, SupplierInvoiceRequestBuilder> {
  @BuiltValueField(wireName: r'purchaseOrderId')
  String? get purchaseOrderId;

  @BuiltValueField(wireName: r'supplierInvoiceNumber')
  String? get supplierInvoiceNumber;

  @BuiltValueField(wireName: r'invoiceDate')
  Date? get invoiceDate;

  @BuiltValueField(wireName: r'dueDate')
  Date? get dueDate;

  @BuiltValueField(wireName: r'taxAmount')
  double? get taxAmount;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'lines')
  BuiltList<SupplierInvoiceLineRequest>? get lines;

  SupplierInvoiceRequest._();

  factory SupplierInvoiceRequest([void updates(SupplierInvoiceRequestBuilder b)]) = _$SupplierInvoiceRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierInvoiceRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierInvoiceRequest> get serializer => _$SupplierInvoiceRequestSerializer();
}

class _$SupplierInvoiceRequestSerializer implements PrimitiveSerializer<SupplierInvoiceRequest> {
  @override
  final Iterable<Type> types = const [SupplierInvoiceRequest, _$SupplierInvoiceRequest];

  @override
  final String wireName = r'SupplierInvoiceRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierInvoiceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.purchaseOrderId != null) {
      yield r'purchaseOrderId';
      yield serializers.serialize(
        object.purchaseOrderId,
        specifiedType: const FullType(String),
      );
    }
    if (object.supplierInvoiceNumber != null) {
      yield r'supplierInvoiceNumber';
      yield serializers.serialize(
        object.supplierInvoiceNumber,
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
    if (object.taxAmount != null) {
      yield r'taxAmount';
      yield serializers.serialize(
        object.taxAmount,
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
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierInvoiceLineRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SupplierInvoiceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierInvoiceRequestBuilder result,
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
        case r'supplierInvoiceNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supplierInvoiceNumber = valueDes;
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
        case r'taxAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.taxAmount = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierInvoiceLineRequest)]),
          ) as BuiltList<SupplierInvoiceLineRequest>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SupplierInvoiceRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierInvoiceRequestBuilder();
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


