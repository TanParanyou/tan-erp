//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/service_pagination_response.dart';
import 'package:tan_erp_api/src/model/warranty_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'warranties_list_response.g.dart';

/// WarrantiesListResponse
///
/// Properties:
/// * [items] 
/// * [pagination] 
@BuiltValue()
abstract class WarrantiesListResponse implements Built<WarrantiesListResponse, WarrantiesListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<WarrantyResponse>? get items;

  @BuiltValueField(wireName: r'pagination')
  ServicePaginationResponse? get pagination;

  WarrantiesListResponse._();

  factory WarrantiesListResponse([void updates(WarrantiesListResponseBuilder b)]) = _$WarrantiesListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WarrantiesListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WarrantiesListResponse> get serializer => _$WarrantiesListResponseSerializer();
}

class _$WarrantiesListResponseSerializer implements PrimitiveSerializer<WarrantiesListResponse> {
  @override
  final Iterable<Type> types = const [WarrantiesListResponse, _$WarrantiesListResponse];

  @override
  final String wireName = r'WarrantiesListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WarrantiesListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(WarrantyResponse)]),
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
    WarrantiesListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WarrantiesListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(WarrantyResponse)]),
          ) as BuiltList<WarrantyResponse>?;
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
  WarrantiesListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WarrantiesListResponseBuilder();
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


