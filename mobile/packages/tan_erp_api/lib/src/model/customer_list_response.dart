//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/pagination_metadata_response.dart';
import 'package:tan_erp_api/src/model/customer_list_item_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'customer_list_response.g.dart';

/// CustomerListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class CustomerListResponse implements Built<CustomerListResponse, CustomerListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<CustomerListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  PaginationMetadataResponse? get pagination;

  CustomerListResponse._();

  factory CustomerListResponse([void updates(CustomerListResponseBuilder b)]) = _$CustomerListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CustomerListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CustomerListResponse> get serializer => _$CustomerListResponseSerializer();
}

class _$CustomerListResponseSerializer implements PrimitiveSerializer<CustomerListResponse> {
  @override
  final Iterable<Type> types = const [CustomerListResponse, _$CustomerListResponse];

  @override
  final String wireName = r'CustomerListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CustomerListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CustomerListItemResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(PaginationMetadataResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CustomerListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CustomerListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CustomerListItemResponse)]),
          ) as BuiltList<CustomerListItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(PaginationMetadataResponse),
          ) as PaginationMetadataResponse?;
          if (valueDes == null) continue;
          result.pagination.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CustomerListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CustomerListResponseBuilder();
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


