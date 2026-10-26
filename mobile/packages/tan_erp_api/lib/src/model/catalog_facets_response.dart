//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/catalog_facet_value_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/catalog_category_facet_response.dart';
import 'package:tan_erp_api/src/model/catalog_brand_facet_response.dart';
import 'package:tan_erp_api/src/model/catalog_attribute_facet_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'catalog_facets_response.g.dart';

/// CatalogFacetsResponse
///
/// Properties:
/// * [itemTypes] 
/// * [categories] 
/// * [brands] 
/// * [attributes] 
@BuiltValue()
abstract class CatalogFacetsResponse implements Built<CatalogFacetsResponse, CatalogFacetsResponseBuilder> {
  @BuiltValueField(wireName: r'itemTypes')
  BuiltList<CatalogFacetValueResponse>? get itemTypes;

  @BuiltValueField(wireName: r'categories')
  BuiltList<CatalogCategoryFacetResponse>? get categories;

  @BuiltValueField(wireName: r'brands')
  BuiltList<CatalogBrandFacetResponse>? get brands;

  @BuiltValueField(wireName: r'attributes')
  BuiltList<CatalogAttributeFacetResponse>? get attributes;

  CatalogFacetsResponse._();

  factory CatalogFacetsResponse([void updates(CatalogFacetsResponseBuilder b)]) = _$CatalogFacetsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CatalogFacetsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CatalogFacetsResponse> get serializer => _$CatalogFacetsResponseSerializer();
}

class _$CatalogFacetsResponseSerializer implements PrimitiveSerializer<CatalogFacetsResponse> {
  @override
  final Iterable<Type> types = const [CatalogFacetsResponse, _$CatalogFacetsResponse];

  @override
  final String wireName = r'CatalogFacetsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CatalogFacetsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.itemTypes != null) {
      yield r'itemTypes';
      yield serializers.serialize(
        object.itemTypes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CatalogFacetValueResponse)]),
      );
    }
    if (object.categories != null) {
      yield r'categories';
      yield serializers.serialize(
        object.categories,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CatalogCategoryFacetResponse)]),
      );
    }
    if (object.brands != null) {
      yield r'brands';
      yield serializers.serialize(
        object.brands,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CatalogBrandFacetResponse)]),
      );
    }
    if (object.attributes != null) {
      yield r'attributes';
      yield serializers.serialize(
        object.attributes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CatalogAttributeFacetResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CatalogFacetsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CatalogFacetsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'itemTypes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CatalogFacetValueResponse)]),
          ) as BuiltList<CatalogFacetValueResponse>?;
          if (valueDes == null) continue;
          result.itemTypes.replace(valueDes);
          break;
        case r'categories':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CatalogCategoryFacetResponse)]),
          ) as BuiltList<CatalogCategoryFacetResponse>?;
          if (valueDes == null) continue;
          result.categories.replace(valueDes);
          break;
        case r'brands':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CatalogBrandFacetResponse)]),
          ) as BuiltList<CatalogBrandFacetResponse>?;
          if (valueDes == null) continue;
          result.brands.replace(valueDes);
          break;
        case r'attributes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CatalogAttributeFacetResponse)]),
          ) as BuiltList<CatalogAttributeFacetResponse>?;
          if (valueDes == null) continue;
          result.attributes.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CatalogFacetsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CatalogFacetsResponseBuilder();
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


