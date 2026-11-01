//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/credit_override_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'customer_credit_exposure_response.g.dart';

/// CustomerCreditExposureResponse
///
/// Properties:
/// * [customerId] 
/// * [customerName] 
/// * [creditLimit] 
/// * [creditTermDays] 
/// * [outstandingReceivables] 
/// * [unbilledAcceptedAmount] 
/// * [totalExposure] 
/// * [effectiveCreditCeiling] 
/// * [availableCredit] 
/// * [isOverLimit] 
/// * [activeOverrides] 
@BuiltValue()
abstract class CustomerCreditExposureResponse implements Built<CustomerCreditExposureResponse, CustomerCreditExposureResponseBuilder> {
  @BuiltValueField(wireName: r'customerId')
  String? get customerId;

  @BuiltValueField(wireName: r'customerName')
  String? get customerName;

  @BuiltValueField(wireName: r'creditLimit')
  double? get creditLimit;

  @BuiltValueField(wireName: r'creditTermDays')
  int? get creditTermDays;

  @BuiltValueField(wireName: r'outstandingReceivables')
  double? get outstandingReceivables;

  @BuiltValueField(wireName: r'unbilledAcceptedAmount')
  double? get unbilledAcceptedAmount;

  @BuiltValueField(wireName: r'totalExposure')
  double? get totalExposure;

  @BuiltValueField(wireName: r'effectiveCreditCeiling')
  double? get effectiveCreditCeiling;

  @BuiltValueField(wireName: r'availableCredit')
  double? get availableCredit;

  @BuiltValueField(wireName: r'isOverLimit')
  bool? get isOverLimit;

  @BuiltValueField(wireName: r'activeOverrides')
  BuiltList<CreditOverrideResponse>? get activeOverrides;

  CustomerCreditExposureResponse._();

  factory CustomerCreditExposureResponse([void updates(CustomerCreditExposureResponseBuilder b)]) = _$CustomerCreditExposureResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CustomerCreditExposureResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CustomerCreditExposureResponse> get serializer => _$CustomerCreditExposureResponseSerializer();
}

class _$CustomerCreditExposureResponseSerializer implements PrimitiveSerializer<CustomerCreditExposureResponse> {
  @override
  final Iterable<Type> types = const [CustomerCreditExposureResponse, _$CustomerCreditExposureResponse];

  @override
  final String wireName = r'CustomerCreditExposureResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CustomerCreditExposureResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.customerId != null) {
      yield r'customerId';
      yield serializers.serialize(
        object.customerId,
        specifiedType: const FullType(String),
      );
    }
    if (object.customerName != null) {
      yield r'customerName';
      yield serializers.serialize(
        object.customerName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.creditLimit != null) {
      yield r'creditLimit';
      yield serializers.serialize(
        object.creditLimit,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.creditTermDays != null) {
      yield r'creditTermDays';
      yield serializers.serialize(
        object.creditTermDays,
        specifiedType: const FullType(int),
      );
    }
    if (object.outstandingReceivables != null) {
      yield r'outstandingReceivables';
      yield serializers.serialize(
        object.outstandingReceivables,
        specifiedType: const FullType(double),
      );
    }
    if (object.unbilledAcceptedAmount != null) {
      yield r'unbilledAcceptedAmount';
      yield serializers.serialize(
        object.unbilledAcceptedAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalExposure != null) {
      yield r'totalExposure';
      yield serializers.serialize(
        object.totalExposure,
        specifiedType: const FullType(double),
      );
    }
    if (object.effectiveCreditCeiling != null) {
      yield r'effectiveCreditCeiling';
      yield serializers.serialize(
        object.effectiveCreditCeiling,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.availableCredit != null) {
      yield r'availableCredit';
      yield serializers.serialize(
        object.availableCredit,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.isOverLimit != null) {
      yield r'isOverLimit';
      yield serializers.serialize(
        object.isOverLimit,
        specifiedType: const FullType(bool),
      );
    }
    if (object.activeOverrides != null) {
      yield r'activeOverrides';
      yield serializers.serialize(
        object.activeOverrides,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CreditOverrideResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CustomerCreditExposureResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CustomerCreditExposureResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'customerId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerId = valueDes;
          break;
        case r'customerName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerName = valueDes;
          break;
        case r'creditLimit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.creditLimit = valueDes;
          break;
        case r'creditTermDays':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.creditTermDays = valueDes;
          break;
        case r'outstandingReceivables':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.outstandingReceivables = valueDes;
          break;
        case r'unbilledAcceptedAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unbilledAcceptedAmount = valueDes;
          break;
        case r'totalExposure':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalExposure = valueDes;
          break;
        case r'effectiveCreditCeiling':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.effectiveCreditCeiling = valueDes;
          break;
        case r'availableCredit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.availableCredit = valueDes;
          break;
        case r'isOverLimit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isOverLimit = valueDes;
          break;
        case r'activeOverrides':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CreditOverrideResponse)]),
          ) as BuiltList<CreditOverrideResponse>?;
          if (valueDes == null) continue;
          result.activeOverrides.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CustomerCreditExposureResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CustomerCreditExposureResponseBuilder();
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


