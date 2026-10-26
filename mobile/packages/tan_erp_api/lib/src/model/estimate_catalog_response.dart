//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/catalog_page_info_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/estimate_catalog_item_response.dart';
import 'package:tan_erp_api/src/model/catalog_facets_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_catalog_response.g.dart';

/// EstimateCatalogResponse
///
/// Properties:
/// * [items] 
/// * [facets] 
/// * [pageInfo] 
@BuiltValue()
abstract class EstimateCatalogResponse implements Built<EstimateCatalogResponse, EstimateCatalogResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<EstimateCatalogItemResponse>? get items;

  @BuiltValueField(wireName: r'facets')
  CatalogFacetsResponse? get facets;

  @BuiltValueField(wireName: r'pageInfo')
  CatalogPageInfoResponse? get pageInfo;

  EstimateCatalogResponse._();

  factory EstimateCatalogResponse([void updates(EstimateCatalogResponseBuilder b)]) = _$EstimateCatalogResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateCatalogResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateCatalogResponse> get serializer => _$EstimateCatalogResponseSerializer();
}

class _$EstimateCatalogResponseSerializer implements PrimitiveSerializer<EstimateCatalogResponse> {
  @override
  final Iterable<Type> types = const [EstimateCatalogResponse, _$EstimateCatalogResponse];

  @override
  final String wireName = r'EstimateCatalogResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateCatalogResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateCatalogItemResponse)]),
      );
    }
    if (object.facets != null) {
      yield r'facets';
      yield serializers.serialize(
        object.facets,
        specifiedType: const FullType(CatalogFacetsResponse),
      );
    }
    if (object.pageInfo != null) {
      yield r'pageInfo';
      yield serializers.serialize(
        object.pageInfo,
        specifiedType: const FullType(CatalogPageInfoResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateCatalogResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateCatalogResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateCatalogItemResponse)]),
          ) as BuiltList<EstimateCatalogItemResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        case r'facets':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CatalogFacetsResponse),
          ) as CatalogFacetsResponse?;
          if (valueDes == null) continue;
          result.facets.replace(valueDes);
          break;
        case r'pageInfo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CatalogPageInfoResponse),
          ) as CatalogPageInfoResponse?;
          if (valueDes == null) continue;
          result.pageInfo.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateCatalogResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateCatalogResponseBuilder();
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


