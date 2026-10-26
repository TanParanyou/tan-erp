//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/customer_contact_detail_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'customer_contact_list_response.g.dart';

/// CustomerContactListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class CustomerContactListResponse implements Built<CustomerContactListResponse, CustomerContactListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<CustomerContactDetailResponse>? get items;

  CustomerContactListResponse._();

  factory CustomerContactListResponse([void updates(CustomerContactListResponseBuilder b)]) = _$CustomerContactListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CustomerContactListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CustomerContactListResponse> get serializer => _$CustomerContactListResponseSerializer();
}

class _$CustomerContactListResponseSerializer implements PrimitiveSerializer<CustomerContactListResponse> {
  @override
  final Iterable<Type> types = const [CustomerContactListResponse, _$CustomerContactListResponse];

  @override
  final String wireName = r'CustomerContactListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CustomerContactListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CustomerContactDetailResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CustomerContactListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CustomerContactListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CustomerContactDetailResponse)]),
          ) as BuiltList<CustomerContactDetailResponse>?;
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
  CustomerContactListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CustomerContactListResponseBuilder();
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


