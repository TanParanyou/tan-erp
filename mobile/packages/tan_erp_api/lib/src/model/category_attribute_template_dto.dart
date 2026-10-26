//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_input.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/category_attribute_option_dto.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'category_attribute_template_dto.g.dart';

/// CategoryAttributeTemplateDto
///
/// Properties:
/// * [key] 
/// * [name] 
/// * [dataType] 
/// * [unit] 
/// * [isRequired] 
/// * [defaultValue] 
/// * [options] 
@BuiltValue()
abstract class CategoryAttributeTemplateDto implements Built<CategoryAttributeTemplateDto, CategoryAttributeTemplateDtoBuilder> {
  @BuiltValueField(wireName: r'key')
  String? get key;

  @BuiltValueField(wireName: r'name')
  LocalizedTextInput? get name;

  @BuiltValueField(wireName: r'dataType')
  String? get dataType;

  @BuiltValueField(wireName: r'unit')
  String? get unit;

  @BuiltValueField(wireName: r'isRequired')
  bool? get isRequired;

  @BuiltValueField(wireName: r'defaultValue')
  String? get defaultValue;

  @BuiltValueField(wireName: r'options')
  BuiltList<CategoryAttributeOptionDto>? get options;

  CategoryAttributeTemplateDto._();

  factory CategoryAttributeTemplateDto([void updates(CategoryAttributeTemplateDtoBuilder b)]) = _$CategoryAttributeTemplateDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CategoryAttributeTemplateDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CategoryAttributeTemplateDto> get serializer => _$CategoryAttributeTemplateDtoSerializer();
}

class _$CategoryAttributeTemplateDtoSerializer implements PrimitiveSerializer<CategoryAttributeTemplateDto> {
  @override
  final Iterable<Type> types = const [CategoryAttributeTemplateDto, _$CategoryAttributeTemplateDto];

  @override
  final String wireName = r'CategoryAttributeTemplateDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CategoryAttributeTemplateDto object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.key != null) {
      yield r'key';
      yield serializers.serialize(
        object.key,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType(LocalizedTextInput),
      );
    }
    if (object.dataType != null) {
      yield r'dataType';
      yield serializers.serialize(
        object.dataType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unit != null) {
      yield r'unit';
      yield serializers.serialize(
        object.unit,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isRequired != null) {
      yield r'isRequired';
      yield serializers.serialize(
        object.isRequired,
        specifiedType: const FullType(bool),
      );
    }
    if (object.defaultValue != null) {
      yield r'defaultValue';
      yield serializers.serialize(
        object.defaultValue,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.options != null) {
      yield r'options';
      yield serializers.serialize(
        object.options,
        specifiedType: const FullType.nullable(BuiltList, [FullType(CategoryAttributeOptionDto)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CategoryAttributeTemplateDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CategoryAttributeTemplateDtoBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'key':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.key = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextInput),
          ) as LocalizedTextInput?;
          if (valueDes == null) continue;
          result.name.replace(valueDes);
          break;
        case r'dataType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.dataType = valueDes;
          break;
        case r'unit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unit = valueDes;
          break;
        case r'isRequired':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isRequired = valueDes;
          break;
        case r'defaultValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.defaultValue = valueDes;
          break;
        case r'options':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(CategoryAttributeOptionDto)]),
          ) as BuiltList<CategoryAttributeOptionDto>?;
          if (valueDes == null) continue;
          result.options.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CategoryAttributeTemplateDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CategoryAttributeTemplateDtoBuilder();
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


