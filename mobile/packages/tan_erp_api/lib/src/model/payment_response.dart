//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/finance_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'payment_response.g.dart';

/// PaymentResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [amount] 
/// * [method] 
/// * [reference] 
/// * [receivedDate] 
/// * [status] 
/// * [reversalReason] 
/// * [reversedAtUtc] 
/// * [recordedBy] 
/// * [recordedAtUtc] 
/// * [receiptNumber] 
/// * [whtRate] 
/// * [whtAmount] 
/// * [netCashReceived] 
/// * [whtType] 
/// * [whtCertificateNumber] 
@BuiltValue()
abstract class PaymentResponse implements Built<PaymentResponse, PaymentResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

  @BuiltValueField(wireName: r'method')
  String? get method;

  @BuiltValueField(wireName: r'reference')
  String? get reference;

  @BuiltValueField(wireName: r'receivedDate')
  Date? get receivedDate;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'reversalReason')
  String? get reversalReason;

  @BuiltValueField(wireName: r'reversedAtUtc')
  DateTime? get reversedAtUtc;

  @BuiltValueField(wireName: r'recordedBy')
  FinancePersonResponse? get recordedBy;

  @BuiltValueField(wireName: r'recordedAtUtc')
  DateTime? get recordedAtUtc;

  @BuiltValueField(wireName: r'receiptNumber')
  String? get receiptNumber;

  @BuiltValueField(wireName: r'whtRate')
  double? get whtRate;

  @BuiltValueField(wireName: r'whtAmount')
  double? get whtAmount;

  @BuiltValueField(wireName: r'netCashReceived')
  double? get netCashReceived;

  @BuiltValueField(wireName: r'whtType')
  String? get whtType;

  @BuiltValueField(wireName: r'whtCertificateNumber')
  String? get whtCertificateNumber;

  PaymentResponse._();

  factory PaymentResponse([void updates(PaymentResponseBuilder b)]) = _$PaymentResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PaymentResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PaymentResponse> get serializer => _$PaymentResponseSerializer();
}

class _$PaymentResponseSerializer implements PrimitiveSerializer<PaymentResponse> {
  @override
  final Iterable<Type> types = const [PaymentResponse, _$PaymentResponse];

  @override
  final String wireName = r'PaymentResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PaymentResponse object, {
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
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
        specifiedType: const FullType(double),
      );
    }
    if (object.method != null) {
      yield r'method';
      yield serializers.serialize(
        object.method,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reference != null) {
      yield r'reference';
      yield serializers.serialize(
        object.reference,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.receivedDate != null) {
      yield r'receivedDate';
      yield serializers.serialize(
        object.receivedDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reversalReason != null) {
      yield r'reversalReason';
      yield serializers.serialize(
        object.reversalReason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reversedAtUtc != null) {
      yield r'reversedAtUtc';
      yield serializers.serialize(
        object.reversedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.recordedBy != null) {
      yield r'recordedBy';
      yield serializers.serialize(
        object.recordedBy,
        specifiedType: const FullType(FinancePersonResponse),
      );
    }
    if (object.recordedAtUtc != null) {
      yield r'recordedAtUtc';
      yield serializers.serialize(
        object.recordedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.receiptNumber != null) {
      yield r'receiptNumber';
      yield serializers.serialize(
        object.receiptNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.whtRate != null) {
      yield r'whtRate';
      yield serializers.serialize(
        object.whtRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.whtAmount != null) {
      yield r'whtAmount';
      yield serializers.serialize(
        object.whtAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.netCashReceived != null) {
      yield r'netCashReceived';
      yield serializers.serialize(
        object.netCashReceived,
        specifiedType: const FullType(double),
      );
    }
    if (object.whtType != null) {
      yield r'whtType';
      yield serializers.serialize(
        object.whtType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.whtCertificateNumber != null) {
      yield r'whtCertificateNumber';
      yield serializers.serialize(
        object.whtCertificateNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PaymentResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PaymentResponseBuilder result,
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
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        case r'method':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.method = valueDes;
          break;
        case r'reference':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reference = valueDes;
          break;
        case r'receivedDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.receivedDate = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'reversalReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reversalReason = valueDes;
          break;
        case r'reversedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.reversedAtUtc = valueDes;
          break;
        case r'recordedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(FinancePersonResponse),
          ) as FinancePersonResponse?;
          if (valueDes == null) continue;
          result.recordedBy.replace(valueDes);
          break;
        case r'recordedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.recordedAtUtc = valueDes;
          break;
        case r'receiptNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.receiptNumber = valueDes;
          break;
        case r'whtRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.whtRate = valueDes;
          break;
        case r'whtAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.whtAmount = valueDes;
          break;
        case r'netCashReceived':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.netCashReceived = valueDes;
          break;
        case r'whtType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.whtType = valueDes;
          break;
        case r'whtCertificateNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.whtCertificateNumber = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PaymentResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PaymentResponseBuilder();
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


