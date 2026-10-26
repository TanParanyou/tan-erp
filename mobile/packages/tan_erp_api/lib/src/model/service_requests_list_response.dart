//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/service_pagination_response.dart';
import 'package:tan_erp_api/src/model/service_request_list_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'service_requests_list_response.g.dart';

/// ServiceRequestsListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class ServiceRequestsListResponse implements Built<ServiceRequestsListResponse, ServiceRequestsListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<ServiceRequestListItemResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ServicePaginationResponse? get pagination;

  ServiceRequestsListResponse._();

  factory ServiceRequestsListResponse([void updates(ServiceRequestsListResponseBuilder b)]) = _$ServiceRequestsListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ServiceRequestsListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ServiceRequestsListResponse> get serializer => _$ServiceRequestsListResponseSerializer();
}

class _$ServiceRequestsListResponseSerializer implements PrimitiveSerializer<ServiceRequestsListResponse> {
  @override
  final Iterable<Type> types = const [ServiceRequestsListResponse, _$ServiceRequestsListResponse];

  @override
  final String wireName = r'ServiceRequestsListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ServiceRequestsListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ServiceRequestListItemResponse)]),
      );
    }
    if (object.pagination != null) {
      yield r'pagination';
      yield serializers.serialize(
        object.pagination,
        specifiedType: const FullType(ServicePaginationResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ServiceRequestsListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ServiceRequestsListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ServiceRequestListItemResponse)]),
          ) as BuiltList<ServiceRequestListItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'pagination':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServicePaginationResponse),
          ) as ServicePaginationResponse?;
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
  ServiceRequestsListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ServiceRequestsListResponseBuilder();
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


