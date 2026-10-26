//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/category_attribute_template_dto.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'set_category_attribute_templates_request.g.dart';

/// SetCategoryAttributeTemplatesRequest
///
/// Properties:
/// * [templates] 
@BuiltValue()
abstract class SetCategoryAttributeTemplatesRequest implements Built<SetCategoryAttributeTemplatesRequest, SetCategoryAttributeTemplatesRequestBuilder> {
  @BuiltValueField(wireName: r'templates')
  BuiltList<CategoryAttributeTemplateDto>? get templates;

  SetCategoryAttributeTemplatesRequest._();

  factory SetCategoryAttributeTemplatesRequest([void updates(SetCategoryAttributeTemplatesRequestBuilder b)]) = _$SetCategoryAttributeTemplatesRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SetCategoryAttributeTemplatesRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SetCategoryAttributeTemplatesRequest> get serializer => _$SetCategoryAttributeTemplatesRequestSerializer();
}

class _$SetCategoryAttributeTemplatesRequestSerializer implements PrimitiveSerializer<SetCategoryAttributeTemplatesRequest> {
  @override
  final Iterable<Type> types = const [SetCategoryAttributeTemplatesRequest, _$SetCategoryAttributeTemplatesRequest];

  @override
  final String wireName = r'SetCategoryAttributeTemplatesRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SetCategoryAttributeTemplatesRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
    SetCategoryAttributeTemplatesRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SetCategoryAttributeTemplatesRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
  SetCategoryAttributeTemplatesRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SetCategoryAttributeTemplatesRequestBuilder();
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


