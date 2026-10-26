//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_customer_request.g.dart';

/// UpdateCustomerRequest
///
/// Properties:
/// * [customerType] 
/// * [displayNameTh] 
/// * [displayNameEn] 
/// * [preferredLocale] 
/// * [leadSource] 
/// * [leadSourceNote] 
/// * [legalName] 
/// * [taxIdentifier] 
/// * [branchCode] 
/// * [creditTermDays] 
/// * [creditLimit] 
/// * [currencyCode] 
/// * [billingCycle] 
/// * [billingDay] 
/// * [paymentConditionNote] 
/// * [hasTaxIdentifier] 
@BuiltValue()
abstract class UpdateCustomerRequest implements Built<UpdateCustomerRequest, UpdateCustomerRequestBuilder> {
  @BuiltValueField(wireName: r'customerType')
  String? get customerType;

  @BuiltValueField(wireName: r'displayNameTh')
  String? get displayNameTh;

  @BuiltValueField(wireName: r'displayNameEn')
  String? get displayNameEn;

  @BuiltValueField(wireName: r'preferredLocale')
  String? get preferredLocale;

  @BuiltValueField(wireName: r'leadSource')
  String? get leadSource;

  @BuiltValueField(wireName: r'leadSourceNote')
  String? get leadSourceNote;

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

  @BuiltValueField(wireName: r'hasTaxIdentifier')
  bool? get hasTaxIdentifier;

  UpdateCustomerRequest._();

  factory UpdateCustomerRequest([void updates(UpdateCustomerRequestBuilder b)]) = _$UpdateCustomerRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateCustomerRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateCustomerRequest> get serializer => _$UpdateCustomerRequestSerializer();
}

class _$UpdateCustomerRequestSerializer implements PrimitiveSerializer<UpdateCustomerRequest> {
  @override
  final Iterable<Type> types = const [UpdateCustomerRequest, _$UpdateCustomerRequest];

  @override
  final String wireName = r'UpdateCustomerRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateCustomerRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
    if (object.hasTaxIdentifier != null) {
      yield r'hasTaxIdentifier';
      yield serializers.serialize(
        object.hasTaxIdentifier,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateCustomerRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateCustomerRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
        case r'hasTaxIdentifier':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.hasTaxIdentifier = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateCustomerRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateCustomerRequestBuilder();
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


