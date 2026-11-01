//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_order_invoice_summary_response.g.dart';

/// PurchaseOrderInvoiceSummaryResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [supplierInvoiceNumber] 
/// * [status] 
/// * [matchStatus] 
/// * [invoiceDate] 
/// * [subtotalAmount] 
/// * [taxAmount] 
/// * [totalAmount] 
/// * [totalVarianceAmount] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class PurchaseOrderInvoiceSummaryResponse implements Built<PurchaseOrderInvoiceSummaryResponse, PurchaseOrderInvoiceSummaryResponseBuilder> {
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

  @BuiltValueField(wireName: r'invoiceDate')
  Date? get invoiceDate;

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

  PurchaseOrderInvoiceSummaryResponse._();

  factory PurchaseOrderInvoiceSummaryResponse([void updates(PurchaseOrderInvoiceSummaryResponseBuilder b)]) = _$PurchaseOrderInvoiceSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseOrderInvoiceSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseOrderInvoiceSummaryResponse> get serializer => _$PurchaseOrderInvoiceSummaryResponseSerializer();
}

class _$PurchaseOrderInvoiceSummaryResponseSerializer implements PrimitiveSerializer<PurchaseOrderInvoiceSummaryResponse> {
  @override
  final Iterable<Type> types = const [PurchaseOrderInvoiceSummaryResponse, _$PurchaseOrderInvoiceSummaryResponse];

  @override
  final String wireName = r'PurchaseOrderInvoiceSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseOrderInvoiceSummaryResponse object, {
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
    if (object.invoiceDate != null) {
      yield r'invoiceDate';
      yield serializers.serialize(
        object.invoiceDate,
        specifiedType: const FullType(Date),
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
    PurchaseOrderInvoiceSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseOrderInvoiceSummaryResponseBuilder result,
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
        case r'invoiceDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.invoiceDate = valueDes;
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
  PurchaseOrderInvoiceSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseOrderInvoiceSummaryResponseBuilder();
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


