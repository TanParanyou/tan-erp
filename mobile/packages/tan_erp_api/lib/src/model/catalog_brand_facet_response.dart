//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'catalog_brand_facet_response.g.dart';

/// CatalogBrandFacetResponse
///
/// Properties:
/// * [id] 
/// * [name] 
/// * [imageFileId] 
/// * [count] 
@BuiltValue()
abstract class CatalogBrandFacetResponse implements Built<CatalogBrandFacetResponse, CatalogBrandFacetResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'name')
  LocalizedTextResponse? get name;

  @BuiltValueField(wireName: r'imageFileId')
  String? get imageFileId;

  @BuiltValueField(wireName: r'count')
  int? get count;

  CatalogBrandFacetResponse._();

  factory CatalogBrandFacetResponse([void updates(CatalogBrandFacetResponseBuilder b)]) = _$CatalogBrandFacetResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CatalogBrandFacetResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CatalogBrandFacetResponse> get serializer => _$CatalogBrandFacetResponseSerializer();
}

class _$CatalogBrandFacetResponseSerializer implements PrimitiveSerializer<CatalogBrandFacetResponse> {
  @override
  final Iterable<Type> types = const [CatalogBrandFacetResponse, _$CatalogBrandFacetResponse];

  @override
  final String wireName = r'CatalogBrandFacetResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CatalogBrandFacetResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType(LocalizedTextResponse),
      );
    }
    if (object.imageFileId != null) {
      yield r'imageFileId';
      yield serializers.serialize(
        object.imageFileId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.count != null) {
      yield r'count';
      yield serializers.serialize(
        object.count,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CatalogBrandFacetResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CatalogBrandFacetResponseBuilder result,
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
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextResponse),
          ) as LocalizedTextResponse?;
          if (valueDes == null) continue;
          result.name.replace(valueDes);
          break;
        case r'imageFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.imageFileId = valueDes;
          break;
        case r'count':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.count = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CatalogBrandFacetResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CatalogBrandFacetResponseBuilder();
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


