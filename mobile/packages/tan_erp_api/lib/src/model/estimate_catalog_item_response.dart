//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/catalog_resolved_cost_response.dart';
import 'package:tan_erp_api/src/model/unit_summary_response.dart';
import 'package:tan_erp_api/src/model/catalog_primary_image_response.dart';
import 'package:tan_erp_api/src/model/category_summary_response.dart';
import 'package:tan_erp_api/src/model/localized_text_response.dart';
import 'package:tan_erp_api/src/model/brand_summary_response.dart';
import 'package:built_value/json_object.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_catalog_item_response.g.dart';

/// EstimateCatalogItemResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [name] 
/// * [description] 
/// * [itemType] 
/// * [costComponentType] 
/// * [category] 
/// * [brand] 
/// * [baseUnit] 
/// * [attributes] 
/// * [primaryImage] 
/// * [resolvedCost] 
@BuiltValue()
abstract class EstimateCatalogItemResponse implements Built<EstimateCatalogItemResponse, EstimateCatalogItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  LocalizedTextResponse? get name;

  @BuiltValueField(wireName: r'description')
  LocalizedTextResponse? get description;

  @BuiltValueField(wireName: r'itemType')
  String? get itemType;

  @BuiltValueField(wireName: r'costComponentType')
  String? get costComponentType;

  @BuiltValueField(wireName: r'category')
  CategorySummaryResponse? get category;

  @BuiltValueField(wireName: r'brand')
  BrandSummaryResponse? get brand;

  @BuiltValueField(wireName: r'baseUnit')
  UnitSummaryResponse? get baseUnit;

  @BuiltValueField(wireName: r'attributes')
  JsonObject? get attributes;

  @BuiltValueField(wireName: r'primaryImage')
  CatalogPrimaryImageResponse? get primaryImage;

  @BuiltValueField(wireName: r'resolvedCost')
  CatalogResolvedCostResponse? get resolvedCost;

  EstimateCatalogItemResponse._();

  factory EstimateCatalogItemResponse([void updates(EstimateCatalogItemResponseBuilder b)]) = _$EstimateCatalogItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateCatalogItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateCatalogItemResponse> get serializer => _$EstimateCatalogItemResponseSerializer();
}

class _$EstimateCatalogItemResponseSerializer implements PrimitiveSerializer<EstimateCatalogItemResponse> {
  @override
  final Iterable<Type> types = const [EstimateCatalogItemResponse, _$EstimateCatalogItemResponse];

  @override
  final String wireName = r'EstimateCatalogItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateCatalogItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType(LocalizedTextResponse),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType(LocalizedTextResponse),
      );
    }
    if (object.itemType != null) {
      yield r'itemType';
      yield serializers.serialize(
        object.itemType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costComponentType != null) {
      yield r'costComponentType';
      yield serializers.serialize(
        object.costComponentType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.category != null) {
      yield r'category';
      yield serializers.serialize(
        object.category,
        specifiedType: const FullType(CategorySummaryResponse),
      );
    }
    if (object.brand != null) {
      yield r'brand';
      yield serializers.serialize(
        object.brand,
        specifiedType: const FullType(BrandSummaryResponse),
      );
    }
    if (object.baseUnit != null) {
      yield r'baseUnit';
      yield serializers.serialize(
        object.baseUnit,
        specifiedType: const FullType(UnitSummaryResponse),
      );
    }
    if (object.attributes != null) {
      yield r'attributes';
      yield serializers.serialize(
        object.attributes,
        specifiedType: const FullType.nullable(JsonObject),
      );
    }
    if (object.primaryImage != null) {
      yield r'primaryImage';
      yield serializers.serialize(
        object.primaryImage,
        specifiedType: const FullType(CatalogPrimaryImageResponse),
      );
    }
    if (object.resolvedCost != null) {
      yield r'resolvedCost';
      yield serializers.serialize(
        object.resolvedCost,
        specifiedType: const FullType(CatalogResolvedCostResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateCatalogItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateCatalogItemResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextResponse),
          ) as LocalizedTextResponse?;
          if (valueDes == null) continue;
          result.name.replace(valueDes);
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextResponse),
          ) as LocalizedTextResponse?;
          if (valueDes == null) continue;
          result.description.replace(valueDes);
          break;
        case r'itemType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemType = valueDes;
          break;
        case r'costComponentType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costComponentType = valueDes;
          break;
        case r'category':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CategorySummaryResponse),
          ) as CategorySummaryResponse?;
          if (valueDes == null) continue;
          result.category.replace(valueDes);
          break;
        case r'brand':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BrandSummaryResponse),
          ) as BrandSummaryResponse?;
          if (valueDes == null) continue;
          result.brand.replace(valueDes);
          break;
        case r'baseUnit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(UnitSummaryResponse),
          ) as UnitSummaryResponse?;
          if (valueDes == null) continue;
          result.baseUnit.replace(valueDes);
          break;
        case r'attributes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(JsonObject),
          ) as JsonObject?;
          if (valueDes == null) continue;
          result.attributes = valueDes;
          break;
        case r'primaryImage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CatalogPrimaryImageResponse),
          ) as CatalogPrimaryImageResponse?;
          if (valueDes == null) continue;
          result.primaryImage.replace(valueDes);
          break;
        case r'resolvedCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CatalogResolvedCostResponse),
          ) as CatalogResolvedCostResponse?;
          if (valueDes == null) continue;
          result.resolvedCost.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateCatalogItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateCatalogItemResponseBuilder();
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


