//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/purchase_order_supplier_response.dart';
import 'package:tan_erp_api/src/model/purchase_order_project_response.dart';
import 'package:tan_erp_api/src/model/procurement_person_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/supplier_invoice_line_response.dart';
import 'package:tan_erp_api/src/model/supplier_invoice_order_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'supplier_invoice_response.g.dart';

/// SupplierInvoiceResponse
///
/// Properties:
/// * [id] 
/// * [branchId] 
/// * [number] 
/// * [supplierInvoiceNumber] 
/// * [status] 
/// * [matchStatus] 
/// * [currency] 
/// * [invoiceDate] 
/// * [dueDate] 
/// * [subtotalAmount] 
/// * [taxAmount] 
/// * [totalAmount] 
/// * [totalPoAmount] 
/// * [totalVarianceAmount] 
/// * [note] 
/// * [purchaseOrder] 
/// * [supplier] 
/// * [project] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [submittedAtUtc] 
/// * [postedAtUtc] 
/// * [decidedBy] 
/// * [decidedAtUtc] 
/// * [decisionNote] 
/// * [cancelReason] 
/// * [voidReason] 
/// * [voidedBy] 
/// * [voidedAtUtc] 
/// * [rowVersion] 
/// * [lines] 
@BuiltValue()
abstract class SupplierInvoiceResponse implements Built<SupplierInvoiceResponse, SupplierInvoiceResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'supplierInvoiceNumber')
  String? get supplierInvoiceNumber;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'matchStatus')
  String? get matchStatus;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

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

  @BuiltValueField(wireName: r'totalPoAmount')
  double? get totalPoAmount;

  @BuiltValueField(wireName: r'totalVarianceAmount')
  double? get totalVarianceAmount;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'purchaseOrder')
  SupplierInvoiceOrderRefResponse? get purchaseOrder;

  @BuiltValueField(wireName: r'supplier')
  PurchaseOrderSupplierResponse? get supplier;

  @BuiltValueField(wireName: r'project')
  PurchaseOrderProjectResponse? get project;

  @BuiltValueField(wireName: r'createdBy')
  ProcurementPersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'submittedAtUtc')
  DateTime? get submittedAtUtc;

  @BuiltValueField(wireName: r'postedAtUtc')
  DateTime? get postedAtUtc;

  @BuiltValueField(wireName: r'decidedBy')
  ProcurementPersonResponse? get decidedBy;

  @BuiltValueField(wireName: r'decidedAtUtc')
  DateTime? get decidedAtUtc;

  @BuiltValueField(wireName: r'decisionNote')
  String? get decisionNote;

  @BuiltValueField(wireName: r'cancelReason')
  String? get cancelReason;

  @BuiltValueField(wireName: r'voidReason')
  String? get voidReason;

  @BuiltValueField(wireName: r'voidedBy')
  ProcurementPersonResponse? get voidedBy;

  @BuiltValueField(wireName: r'voidedAtUtc')
  DateTime? get voidedAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'lines')
  BuiltList<SupplierInvoiceLineResponse>? get lines;

  SupplierInvoiceResponse._();

  factory SupplierInvoiceResponse([void updates(SupplierInvoiceResponseBuilder b)]) = _$SupplierInvoiceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SupplierInvoiceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SupplierInvoiceResponse> get serializer => _$SupplierInvoiceResponseSerializer();
}

class _$SupplierInvoiceResponseSerializer implements PrimitiveSerializer<SupplierInvoiceResponse> {
  @override
  final Iterable<Type> types = const [SupplierInvoiceResponse, _$SupplierInvoiceResponse];

  @override
  final String wireName = r'SupplierInvoiceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SupplierInvoiceResponse object, {
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
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
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
    if (object.totalPoAmount != null) {
      yield r'totalPoAmount';
      yield serializers.serialize(
        object.totalPoAmount,
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
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.purchaseOrder != null) {
      yield r'purchaseOrder';
      yield serializers.serialize(
        object.purchaseOrder,
        specifiedType: const FullType(SupplierInvoiceOrderRefResponse),
      );
    }
    if (object.supplier != null) {
      yield r'supplier';
      yield serializers.serialize(
        object.supplier,
        specifiedType: const FullType(PurchaseOrderSupplierResponse),
      );
    }
    if (object.project != null) {
      yield r'project';
      yield serializers.serialize(
        object.project,
        specifiedType: const FullType(PurchaseOrderProjectResponse),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(ProcurementPersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.submittedAtUtc != null) {
      yield r'submittedAtUtc';
      yield serializers.serialize(
        object.submittedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.postedAtUtc != null) {
      yield r'postedAtUtc';
      yield serializers.serialize(
        object.postedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.decidedBy != null) {
      yield r'decidedBy';
      yield serializers.serialize(
        object.decidedBy,
        specifiedType: const FullType(ProcurementPersonResponse),
      );
    }
    if (object.decidedAtUtc != null) {
      yield r'decidedAtUtc';
      yield serializers.serialize(
        object.decidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.decisionNote != null) {
      yield r'decisionNote';
      yield serializers.serialize(
        object.decisionNote,
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
    if (object.voidReason != null) {
      yield r'voidReason';
      yield serializers.serialize(
        object.voidReason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.voidedBy != null) {
      yield r'voidedBy';
      yield serializers.serialize(
        object.voidedBy,
        specifiedType: const FullType(ProcurementPersonResponse),
      );
    }
    if (object.voidedAtUtc != null) {
      yield r'voidedAtUtc';
      yield serializers.serialize(
        object.voidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierInvoiceLineResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SupplierInvoiceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SupplierInvoiceResponseBuilder result,
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
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
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
        case r'totalPoAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalPoAmount = valueDes;
          break;
        case r'totalVarianceAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalVarianceAmount = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'purchaseOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(SupplierInvoiceOrderRefResponse),
          ) as SupplierInvoiceOrderRefResponse?;
          if (valueDes == null) continue;
          result.purchaseOrder.replace(valueDes);
          break;
        case r'supplier':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(PurchaseOrderSupplierResponse),
          ) as PurchaseOrderSupplierResponse?;
          if (valueDes == null) continue;
          result.supplier.replace(valueDes);
          break;
        case r'project':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(PurchaseOrderProjectResponse),
          ) as PurchaseOrderProjectResponse?;
          if (valueDes == null) continue;
          result.project.replace(valueDes);
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProcurementPersonResponse),
          ) as ProcurementPersonResponse?;
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
        case r'submittedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.submittedAtUtc = valueDes;
          break;
        case r'postedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.postedAtUtc = valueDes;
          break;
        case r'decidedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProcurementPersonResponse),
          ) as ProcurementPersonResponse?;
          if (valueDes == null) continue;
          result.decidedBy.replace(valueDes);
          break;
        case r'decidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.decidedAtUtc = valueDes;
          break;
        case r'decisionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.decisionNote = valueDes;
          break;
        case r'cancelReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.cancelReason = valueDes;
          break;
        case r'voidReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.voidReason = valueDes;
          break;
        case r'voidedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProcurementPersonResponse),
          ) as ProcurementPersonResponse?;
          if (valueDes == null) continue;
          result.voidedBy.replace(valueDes);
          break;
        case r'voidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.voidedAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SupplierInvoiceLineResponse)]),
          ) as BuiltList<SupplierInvoiceLineResponse>?;
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
  SupplierInvoiceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SupplierInvoiceResponseBuilder();
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


