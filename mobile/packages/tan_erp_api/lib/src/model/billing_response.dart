//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/finance_project_ref_response.dart';
import 'package:tan_erp_api/src/model/payment_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/billing_line_response.dart';
import 'package:tan_erp_api/src/model/finance_person_response.dart';
import 'package:tan_erp_api/src/model/billing_adjustment_note_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'billing_response.g.dart';

/// BillingResponse
///
/// Properties:
/// * [id] 
/// * [branchId] 
/// * [number] 
/// * [project] 
/// * [kind] 
/// * [description] 
/// * [amount] 
/// * [paidAmount] 
/// * [outstanding] 
/// * [currency] 
/// * [dueDate] 
/// * [status] 
/// * [referenceHash] 
/// * [voidReason] 
/// * [voidedAtUtc] 
/// * [createdBy] 
/// * [issuedAtUtc] 
/// * [rowVersion] 
/// * [payments] 
/// * [taxInvoiceNumber] 
/// * [vatRate] 
/// * [vatAmount] 
/// * [grossTotal] 
/// * [effectiveReceivable] 
/// * [retentionRate] 
/// * [retentionAmount] 
/// * [retentionStatus] 
/// * [releasedRetentionAtUtc] 
/// * [releasedRetentionBy] 
/// * [creditNoteBaseAmount] 
/// * [creditNoteVatAmount] 
/// * [debitNoteBaseAmount] 
/// * [debitNoteVatAmount] 
/// * [lines] 
/// * [notes] 
@BuiltValue()
abstract class BillingResponse implements Built<BillingResponse, BillingResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'project')
  FinanceProjectRefResponse? get project;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

  @BuiltValueField(wireName: r'paidAmount')
  double? get paidAmount;

  @BuiltValueField(wireName: r'outstanding')
  double? get outstanding;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'dueDate')
  Date? get dueDate;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'referenceHash')
  String? get referenceHash;

  @BuiltValueField(wireName: r'voidReason')
  String? get voidReason;

  @BuiltValueField(wireName: r'voidedAtUtc')
  DateTime? get voidedAtUtc;

  @BuiltValueField(wireName: r'createdBy')
  FinancePersonResponse? get createdBy;

  @BuiltValueField(wireName: r'issuedAtUtc')
  DateTime? get issuedAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'payments')
  BuiltList<PaymentResponse>? get payments;

  @BuiltValueField(wireName: r'taxInvoiceNumber')
  String? get taxInvoiceNumber;

  @BuiltValueField(wireName: r'vatRate')
  double? get vatRate;

  @BuiltValueField(wireName: r'vatAmount')
  double? get vatAmount;

  @BuiltValueField(wireName: r'grossTotal')
  double? get grossTotal;

  @BuiltValueField(wireName: r'effectiveReceivable')
  double? get effectiveReceivable;

  @BuiltValueField(wireName: r'retentionRate')
  double? get retentionRate;

  @BuiltValueField(wireName: r'retentionAmount')
  double? get retentionAmount;

  @BuiltValueField(wireName: r'retentionStatus')
  String? get retentionStatus;

  @BuiltValueField(wireName: r'releasedRetentionAtUtc')
  DateTime? get releasedRetentionAtUtc;

  @BuiltValueField(wireName: r'releasedRetentionBy')
  FinancePersonResponse? get releasedRetentionBy;

  @BuiltValueField(wireName: r'creditNoteBaseAmount')
  double? get creditNoteBaseAmount;

  @BuiltValueField(wireName: r'creditNoteVatAmount')
  double? get creditNoteVatAmount;

  @BuiltValueField(wireName: r'debitNoteBaseAmount')
  double? get debitNoteBaseAmount;

  @BuiltValueField(wireName: r'debitNoteVatAmount')
  double? get debitNoteVatAmount;

  @BuiltValueField(wireName: r'lines')
  BuiltList<BillingLineResponse>? get lines;

  @BuiltValueField(wireName: r'notes')
  BuiltList<BillingAdjustmentNoteResponse>? get notes;

  BillingResponse._();

  factory BillingResponse([void updates(BillingResponseBuilder b)]) = _$BillingResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BillingResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BillingResponse> get serializer => _$BillingResponseSerializer();
}

class _$BillingResponseSerializer implements PrimitiveSerializer<BillingResponse> {
  @override
  final Iterable<Type> types = const [BillingResponse, _$BillingResponse];

  @override
  final String wireName = r'BillingResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BillingResponse object, {
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
    if (object.project != null) {
      yield r'project';
      yield serializers.serialize(
        object.project,
        specifiedType: const FullType(FinanceProjectRefResponse),
      );
    }
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
        specifiedType: const FullType(double),
      );
    }
    if (object.paidAmount != null) {
      yield r'paidAmount';
      yield serializers.serialize(
        object.paidAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.outstanding != null) {
      yield r'outstanding';
      yield serializers.serialize(
        object.outstanding,
        specifiedType: const FullType(double),
      );
    }
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.dueDate != null) {
      yield r'dueDate';
      yield serializers.serialize(
        object.dueDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.referenceHash != null) {
      yield r'referenceHash';
      yield serializers.serialize(
        object.referenceHash,
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
    if (object.voidedAtUtc != null) {
      yield r'voidedAtUtc';
      yield serializers.serialize(
        object.voidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(FinancePersonResponse),
      );
    }
    if (object.issuedAtUtc != null) {
      yield r'issuedAtUtc';
      yield serializers.serialize(
        object.issuedAtUtc,
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
    if (object.payments != null) {
      yield r'payments';
      yield serializers.serialize(
        object.payments,
        specifiedType: const FullType.nullable(BuiltList, [FullType(PaymentResponse)]),
      );
    }
    if (object.taxInvoiceNumber != null) {
      yield r'taxInvoiceNumber';
      yield serializers.serialize(
        object.taxInvoiceNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.vatRate != null) {
      yield r'vatRate';
      yield serializers.serialize(
        object.vatRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.vatAmount != null) {
      yield r'vatAmount';
      yield serializers.serialize(
        object.vatAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.grossTotal != null) {
      yield r'grossTotal';
      yield serializers.serialize(
        object.grossTotal,
        specifiedType: const FullType(double),
      );
    }
    if (object.effectiveReceivable != null) {
      yield r'effectiveReceivable';
      yield serializers.serialize(
        object.effectiveReceivable,
        specifiedType: const FullType(double),
      );
    }
    if (object.retentionRate != null) {
      yield r'retentionRate';
      yield serializers.serialize(
        object.retentionRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.retentionAmount != null) {
      yield r'retentionAmount';
      yield serializers.serialize(
        object.retentionAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.retentionStatus != null) {
      yield r'retentionStatus';
      yield serializers.serialize(
        object.retentionStatus,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.releasedRetentionAtUtc != null) {
      yield r'releasedRetentionAtUtc';
      yield serializers.serialize(
        object.releasedRetentionAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.releasedRetentionBy != null) {
      yield r'releasedRetentionBy';
      yield serializers.serialize(
        object.releasedRetentionBy,
        specifiedType: const FullType(FinancePersonResponse),
      );
    }
    if (object.creditNoteBaseAmount != null) {
      yield r'creditNoteBaseAmount';
      yield serializers.serialize(
        object.creditNoteBaseAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.creditNoteVatAmount != null) {
      yield r'creditNoteVatAmount';
      yield serializers.serialize(
        object.creditNoteVatAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.debitNoteBaseAmount != null) {
      yield r'debitNoteBaseAmount';
      yield serializers.serialize(
        object.debitNoteBaseAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.debitNoteVatAmount != null) {
      yield r'debitNoteVatAmount';
      yield serializers.serialize(
        object.debitNoteVatAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(BillingLineResponse)]),
      );
    }
    if (object.notes != null) {
      yield r'notes';
      yield serializers.serialize(
        object.notes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(BillingAdjustmentNoteResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BillingResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BillingResponseBuilder result,
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
        case r'project':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(FinanceProjectRefResponse),
          ) as FinanceProjectRefResponse?;
          if (valueDes == null) continue;
          result.project.replace(valueDes);
          break;
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        case r'paidAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.paidAmount = valueDes;
          break;
        case r'outstanding':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.outstanding = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        case r'dueDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.dueDate = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'referenceHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.referenceHash = valueDes;
          break;
        case r'voidReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.voidReason = valueDes;
          break;
        case r'voidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.voidedAtUtc = valueDes;
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(FinancePersonResponse),
          ) as FinancePersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'issuedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.issuedAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'payments':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(PaymentResponse)]),
          ) as BuiltList<PaymentResponse>?;
          if (valueDes == null) continue;
          result.payments.replace(valueDes);
          break;
        case r'taxInvoiceNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxInvoiceNumber = valueDes;
          break;
        case r'vatRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.vatRate = valueDes;
          break;
        case r'vatAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.vatAmount = valueDes;
          break;
        case r'grossTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.grossTotal = valueDes;
          break;
        case r'effectiveReceivable':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.effectiveReceivable = valueDes;
          break;
        case r'retentionRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.retentionRate = valueDes;
          break;
        case r'retentionAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.retentionAmount = valueDes;
          break;
        case r'retentionStatus':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.retentionStatus = valueDes;
          break;
        case r'releasedRetentionAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.releasedRetentionAtUtc = valueDes;
          break;
        case r'releasedRetentionBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(FinancePersonResponse),
          ) as FinancePersonResponse?;
          if (valueDes == null) continue;
          result.releasedRetentionBy.replace(valueDes);
          break;
        case r'creditNoteBaseAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.creditNoteBaseAmount = valueDes;
          break;
        case r'creditNoteVatAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.creditNoteVatAmount = valueDes;
          break;
        case r'debitNoteBaseAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.debitNoteBaseAmount = valueDes;
          break;
        case r'debitNoteVatAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.debitNoteVatAmount = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(BillingLineResponse)]),
          ) as BuiltList<BillingLineResponse>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        case r'notes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(BillingAdjustmentNoteResponse)]),
          ) as BuiltList<BillingAdjustmentNoteResponse>?;
          if (valueDes == null) continue;
          result.notes.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BillingResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BillingResponseBuilder();
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


