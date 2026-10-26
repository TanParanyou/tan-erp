//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/localized_text_response.dart';
import 'package:tan_erp_api/src/model/category_attribute_template_dto.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'category_attribute_template_response.g.dart';

/// CategoryAttributeTemplateResponse
///
/// Properties:
/// * [categoryId] 
/// * [categoryCode] 
/// * [categoryName] 
/// * [templates] 
@BuiltValue()
abstract class CategoryAttributeTemplateResponse implements Built<CategoryAttributeTemplateResponse, CategoryAttributeTemplateResponseBuilder> {
  @BuiltValueField(wireName: r'categoryId')
  String? get categoryId;

  @BuiltValueField(wireName: r'categoryCode')
  String? get categoryCode;

  @BuiltValueField(wireName: r'categoryName')
  LocalizedTextResponse? get categoryName;

  @BuiltValueField(wireName: r'templates')
  BuiltList<CategoryAttributeTemplateDto>? get templates;

  CategoryAttributeTemplateResponse._();

  factory CategoryAttributeTemplateResponse([void updates(CategoryAttributeTemplateResponseBuilder b)]) = _$CategoryAttributeTemplateResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CategoryAttributeTemplateResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CategoryAttributeTemplateResponse> get serializer => _$CategoryAttributeTemplateResponseSerializer();
}

class _$CategoryAttributeTemplateResponseSerializer implements PrimitiveSerializer<CategoryAttributeTemplateResponse> {
  @override
  final Iterable<Type> types = const [CategoryAttributeTemplateResponse, _$CategoryAttributeTemplateResponse];

  @override
  final String wireName = r'CategoryAttributeTemplateResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CategoryAttributeTemplateResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.categoryId != null) {
      yield r'categoryId';
      yield serializers.serialize(
        object.categoryId,
        specifiedType: const FullType(String),
      );
    }
    if (object.categoryCode != null) {
      yield r'categoryCode';
      yield serializers.serialize(
        object.categoryCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.categoryName != null) {
      yield r'categoryName';
      yield serializers.serialize(
        object.categoryName,
        specifiedType: const FullType(LocalizedTextResponse),
      );
    }
    if (object.templates != null) {
      yield r'templates';
      yield serializers.serialize(
        object.templates,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CategoryAttributeTemplateDto)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CategoryAttributeTemplateResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CategoryAttributeTemplateResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'categoryId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.categoryId = valueDes;
          break;
        case r'categoryCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.categoryCode = valueDes;
          break;
        case r'categoryName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextResponse),
          ) as LocalizedTextResponse?;
          if (valueDes == null) continue;
          result.categoryName.replace(valueDes);
          break;
        case r'templates':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CategoryAttributeTemplateDto)]),
          ) as BuiltList<CategoryAttributeTemplateDto>?;
          if (valueDes == null) continue;
          result.templates.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CategoryAttributeTemplateResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CategoryAttributeTemplateResponseBuilder();
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


