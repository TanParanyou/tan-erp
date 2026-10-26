//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/customer_address_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'customer_address_list_response.g.dart';

/// CustomerAddressListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class CustomerAddressListResponse implements Built<CustomerAddressListResponse, CustomerAddressListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<CustomerAddressResponse>? get items;

  CustomerAddressListResponse._();

  factory CustomerAddressListResponse([void updates(CustomerAddressListResponseBuilder b)]) = _$CustomerAddressListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CustomerAddressListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CustomerAddressListResponse> get serializer => _$CustomerAddressListResponseSerializer();
}

class _$CustomerAddressListResponseSerializer implements PrimitiveSerializer<CustomerAddressListResponse> {
  @override
  final Iterable<Type> types = const [CustomerAddressListResponse, _$CustomerAddressListResponse];

  @override
  final String wireName = r'CustomerAddressListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CustomerAddressListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CustomerAddressResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CustomerAddressListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CustomerAddressListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CustomerAddressResponse)]),
          ) as BuiltList<CustomerAddressResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CustomerAddressListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CustomerAddressListResponseBuilder();
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


