//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/duplicate_customer_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/customer_contact_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'customer_response.g.dart';

/// CustomerResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [customerType] 
/// * [displayNameTh] 
/// * [displayNameEn] 
/// * [preferredLocale] 
/// * [status] 
/// * [primaryContact] 
/// * [duplicateCandidates] 
/// * [rowVersion] 
/// * [createdAtUtc] 
/// * [leadSource] 
/// * [leadSourceNote] 
/// * [imageFileId] 
/// * [legalName] 
/// * [taxIdentifier] 
/// * [branchCode] 
/// * [creditTermDays] 
/// * [creditLimit] 
/// * [currencyCode] 
/// * [billingCycle] 
/// * [billingDay] 
/// * [paymentConditionNote] 
/// * [inactiveReason] 
@BuiltValue()
abstract class CustomerResponse implements Built<CustomerResponse, CustomerResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'customerType')
  String? get customerType;

  @BuiltValueField(wireName: r'displayNameTh')
  String? get displayNameTh;

  @BuiltValueField(wireName: r'displayNameEn')
  String? get displayNameEn;

  @BuiltValueField(wireName: r'preferredLocale')
  String? get preferredLocale;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'primaryContact')
  CustomerContactResponse? get primaryContact;

  @BuiltValueField(wireName: r'duplicateCandidates')
  BuiltList<DuplicateCustomerResponse>? get duplicateCandidates;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'leadSource')
  String? get leadSource;

  @BuiltValueField(wireName: r'leadSourceNote')
  String? get leadSourceNote;

  @BuiltValueField(wireName: r'imageFileId')
  String? get imageFileId;

  @BuiltValueField(wireName: r'legalName')
  String? get legalName;

  @BuiltValueField(wireName: r'taxIdentifier')
  String? get taxIdentifier;

  @BuiltValueField(wireName: r'branchCode')
  String? get branchCode;

  @BuiltValueField(wireName: r'creditTermDays')
  int? get creditTermDays;

  @BuiltValueField(wireName: r'creditLimit')
  double? get creditLimit;

  @BuiltValueField(wireName: r'currencyCode')
  String? get currencyCode;

  @BuiltValueField(wireName: r'billingCycle')
  String? get billingCycle;

  @BuiltValueField(wireName: r'billingDay')
  int? get billingDay;

  @BuiltValueField(wireName: r'paymentConditionNote')
  String? get paymentConditionNote;

  @BuiltValueField(wireName: r'inactiveReason')
  String? get inactiveReason;

  CustomerResponse._();

  factory CustomerResponse([void updates(CustomerResponseBuilder b)]) = _$CustomerResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CustomerResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CustomerResponse> get serializer => _$CustomerResponseSerializer();
}

class _$CustomerResponseSerializer implements PrimitiveSerializer<CustomerResponse> {
  @override
  final Iterable<Type> types = const [CustomerResponse, _$CustomerResponse];

  @override
  final String wireName = r'CustomerResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CustomerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.customerType != null) {
      yield r'customerType';
      yield serializers.serialize(
        object.customerType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayNameTh != null) {
      yield r'displayNameTh';
      yield serializers.serialize(
        object.displayNameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayNameEn != null) {
      yield r'displayNameEn';
      yield serializers.serialize(
        object.displayNameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.preferredLocale != null) {
      yield r'preferredLocale';
      yield serializers.serialize(
        object.preferredLocale,
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
    if (object.primaryContact != null) {
      yield r'primaryContact';
      yield serializers.serialize(
        object.primaryContact,
        specifiedType: const FullType(CustomerContactResponse),
      );
    }
    if (object.duplicateCandidates != null) {
      yield r'duplicateCandidates';
      yield serializers.serialize(
        object.duplicateCandidates,
        specifiedType: const FullType.nullable(BuiltList, [FullType(DuplicateCustomerResponse)]),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.leadSource != null) {
      yield r'leadSource';
      yield serializers.serialize(
        object.leadSource,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.leadSourceNote != null) {
      yield r'leadSourceNote';
      yield serializers.serialize(
        object.leadSourceNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.imageFileId != null) {
      yield r'imageFileId';
      yield serializers.serialize(
        object.imageFileId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.legalName != null) {
      yield r'legalName';
      yield serializers.serialize(
        object.legalName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxIdentifier != null) {
      yield r'taxIdentifier';
      yield serializers.serialize(
        object.taxIdentifier,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.branchCode != null) {
      yield r'branchCode';
      yield serializers.serialize(
        object.branchCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.creditTermDays != null) {
      yield r'creditTermDays';
      yield serializers.serialize(
        object.creditTermDays,
        specifiedType: const FullType.nullable(int),
      );
    }
    if (object.creditLimit != null) {
      yield r'creditLimit';
      yield serializers.serialize(
        object.creditLimit,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.currencyCode != null) {
      yield r'currencyCode';
      yield serializers.serialize(
        object.currencyCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.billingCycle != null) {
      yield r'billingCycle';
      yield serializers.serialize(
        object.billingCycle,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.billingDay != null) {
      yield r'billingDay';
      yield serializers.serialize(
        object.billingDay,
        specifiedType: const FullType.nullable(int),
      );
    }
    if (object.paymentConditionNote != null) {
      yield r'paymentConditionNote';
      yield serializers.serialize(
        object.paymentConditionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.inactiveReason != null) {
      yield r'inactiveReason';
      yield serializers.serialize(
        object.inactiveReason,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CustomerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CustomerResponseBuilder result,
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
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'customerType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerType = valueDes;
          break;
        case r'displayNameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayNameTh = valueDes;
          break;
        case r'displayNameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayNameEn = valueDes;
          break;
        case r'preferredLocale':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.preferredLocale = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'primaryContact':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CustomerContactResponse),
          ) as CustomerContactResponse?;
          if (valueDes == null) continue;
          result.primaryContact.replace(valueDes);
          break;
        case r'duplicateCandidates':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(DuplicateCustomerResponse)]),
          ) as BuiltList<DuplicateCustomerResponse>?;
          if (valueDes == null) continue;
          result.duplicateCandidates.replace(valueDes);
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'leadSource':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.leadSource = valueDes;
          break;
        case r'leadSourceNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.leadSourceNote = valueDes;
          break;
        case r'imageFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.imageFileId = valueDes;
          break;
        case r'legalName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.legalName = valueDes;
          break;
        case r'taxIdentifier':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxIdentifier = valueDes;
          break;
        case r'branchCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchCode = valueDes;
          break;
        case r'creditTermDays':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.creditTermDays = valueDes;
          break;
        case r'creditLimit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.creditLimit = valueDes;
          break;
        case r'currencyCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currencyCode = valueDes;
          break;
        case r'billingCycle':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.billingCycle = valueDes;
          break;
        case r'billingDay':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.billingDay = valueDes;
          break;
        case r'paymentConditionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.paymentConditionNote = valueDes;
          break;
        case r'inactiveReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.inactiveReason = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CustomerResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CustomerResponseBuilder();
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


