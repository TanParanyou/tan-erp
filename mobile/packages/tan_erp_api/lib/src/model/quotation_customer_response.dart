//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/quotation_address_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_customer_response.g.dart';

/// QuotationCustomerResponse
///
/// Properties:
/// * [customerType] 
/// * [displayName] 
/// * [displayNameTh] 
/// * [displayNameEn] 
/// * [legalName] 
/// * [taxIdentifier] 
/// * [branchCode] 
/// * [address] 
@BuiltValue()
abstract class QuotationCustomerResponse implements Built<QuotationCustomerResponse, QuotationCustomerResponseBuilder> {
  @BuiltValueField(wireName: r'customerType')
  String? get customerType;

  @BuiltValueField(wireName: r'displayName')
  String? get displayName;

  @BuiltValueField(wireName: r'displayNameTh')
  String? get displayNameTh;

  @BuiltValueField(wireName: r'displayNameEn')
  String? get displayNameEn;

  @BuiltValueField(wireName: r'legalName')
  String? get legalName;

  @BuiltValueField(wireName: r'taxIdentifier')
  String? get taxIdentifier;

  @BuiltValueField(wireName: r'branchCode')
  String? get branchCode;

  @BuiltValueField(wireName: r'address')
  QuotationAddressResponse? get address;

  QuotationCustomerResponse._();

  factory QuotationCustomerResponse([void updates(QuotationCustomerResponseBuilder b)]) = _$QuotationCustomerResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationCustomerResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationCustomerResponse> get serializer => _$QuotationCustomerResponseSerializer();
}

class _$QuotationCustomerResponseSerializer implements PrimitiveSerializer<QuotationCustomerResponse> {
  @override
  final Iterable<Type> types = const [QuotationCustomerResponse, _$QuotationCustomerResponse];

  @override
  final String wireName = r'QuotationCustomerResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationCustomerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.customerType != null) {
      yield r'customerType';
      yield serializers.serialize(
        object.customerType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayName != null) {
      yield r'displayName';
      yield serializers.serialize(
        object.displayName,
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
    if (object.address != null) {
      yield r'address';
      yield serializers.serialize(
        object.address,
        specifiedType: const FullType(QuotationAddressResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationCustomerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationCustomerResponseBuilder result,
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
        case r'displayName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayName = valueDes;
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
        case r'address':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QuotationAddressResponse),
          ) as QuotationAddressResponse?;
          if (valueDes == null) continue;
          result.address.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuotationCustomerResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationCustomerResponseBuilder();
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


