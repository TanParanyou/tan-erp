//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_customer_address_request.g.dart';

/// CreateCustomerAddressRequest
///
/// Properties:
/// * [addressType] 
/// * [label] 
/// * [addressLine1] 
/// * [subdistrict] 
/// * [district] 
/// * [province] 
/// * [postalCode] 
/// * [countryCode] 
/// * [isPrimary] 
@BuiltValue()
abstract class CreateCustomerAddressRequest implements Built<CreateCustomerAddressRequest, CreateCustomerAddressRequestBuilder> {
  @BuiltValueField(wireName: r'addressType')
  String? get addressType;

  @BuiltValueField(wireName: r'label')
  String? get label;

  @BuiltValueField(wireName: r'addressLine1')
  String? get addressLine1;

  @BuiltValueField(wireName: r'subdistrict')
  String? get subdistrict;

  @BuiltValueField(wireName: r'district')
  String? get district;

  @BuiltValueField(wireName: r'province')
  String? get province;

  @BuiltValueField(wireName: r'postalCode')
  String? get postalCode;

  @BuiltValueField(wireName: r'countryCode')
  String? get countryCode;

  @BuiltValueField(wireName: r'isPrimary')
  bool? get isPrimary;

  CreateCustomerAddressRequest._();

  factory CreateCustomerAddressRequest([void updates(CreateCustomerAddressRequestBuilder b)]) = _$CreateCustomerAddressRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateCustomerAddressRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateCustomerAddressRequest> get serializer => _$CreateCustomerAddressRequestSerializer();
}

class _$CreateCustomerAddressRequestSerializer implements PrimitiveSerializer<CreateCustomerAddressRequest> {
  @override
  final Iterable<Type> types = const [CreateCustomerAddressRequest, _$CreateCustomerAddressRequest];

  @override
  final String wireName = r'CreateCustomerAddressRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateCustomerAddressRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.addressType != null) {
      yield r'addressType';
      yield serializers.serialize(
        object.addressType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.label != null) {
      yield r'label';
      yield serializers.serialize(
        object.label,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.addressLine1 != null) {
      yield r'addressLine1';
      yield serializers.serialize(
        object.addressLine1,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.subdistrict != null) {
      yield r'subdistrict';
      yield serializers.serialize(
        object.subdistrict,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.district != null) {
      yield r'district';
      yield serializers.serialize(
        object.district,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.province != null) {
      yield r'province';
      yield serializers.serialize(
        object.province,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.postalCode != null) {
      yield r'postalCode';
      yield serializers.serialize(
        object.postalCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.countryCode != null) {
      yield r'countryCode';
      yield serializers.serialize(
        object.countryCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isPrimary != null) {
      yield r'isPrimary';
      yield serializers.serialize(
        object.isPrimary,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateCustomerAddressRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateCustomerAddressRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'addressType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.addressType = valueDes;
          break;
        case r'label':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.label = valueDes;
          break;
        case r'addressLine1':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.addressLine1 = valueDes;
          break;
        case r'subdistrict':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.subdistrict = valueDes;
          break;
        case r'district':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.district = valueDes;
          break;
        case r'province':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.province = valueDes;
          break;
        case r'postalCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.postalCode = valueDes;
          break;
        case r'countryCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.countryCode = valueDes;
          break;
        case r'isPrimary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isPrimary = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateCustomerAddressRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateCustomerAddressRequestBuilder();
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


