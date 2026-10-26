//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_customer_address_request.g.dart';

/// UpdateCustomerAddressRequest
///
/// Properties:
/// * [label] 
/// * [addressLine1] 
/// * [subdistrict] 
/// * [district] 
/// * [province] 
/// * [postalCode] 
/// * [countryCode] 
@BuiltValue()
abstract class UpdateCustomerAddressRequest implements Built<UpdateCustomerAddressRequest, UpdateCustomerAddressRequestBuilder> {
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

  UpdateCustomerAddressRequest._();

  factory UpdateCustomerAddressRequest([void updates(UpdateCustomerAddressRequestBuilder b)]) = _$UpdateCustomerAddressRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateCustomerAddressRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateCustomerAddressRequest> get serializer => _$UpdateCustomerAddressRequestSerializer();
}

class _$UpdateCustomerAddressRequestSerializer implements PrimitiveSerializer<UpdateCustomerAddressRequest> {
  @override
  final Iterable<Type> types = const [UpdateCustomerAddressRequest, _$UpdateCustomerAddressRequest];

  @override
  final String wireName = r'UpdateCustomerAddressRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateCustomerAddressRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateCustomerAddressRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateCustomerAddressRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateCustomerAddressRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateCustomerAddressRequestBuilder();
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


