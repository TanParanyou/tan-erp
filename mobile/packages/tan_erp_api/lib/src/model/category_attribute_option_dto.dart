//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_input.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'category_attribute_option_dto.g.dart';

/// CategoryAttributeOptionDto
///
/// Properties:
/// * [value] 
/// * [label] 
@BuiltValue()
abstract class CategoryAttributeOptionDto implements Built<CategoryAttributeOptionDto, CategoryAttributeOptionDtoBuilder> {
  @BuiltValueField(wireName: r'value')
  String? get value;

  @BuiltValueField(wireName: r'label')
  LocalizedTextInput? get label;

  CategoryAttributeOptionDto._();

  factory CategoryAttributeOptionDto([void updates(CategoryAttributeOptionDtoBuilder b)]) = _$CategoryAttributeOptionDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CategoryAttributeOptionDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CategoryAttributeOptionDto> get serializer => _$CategoryAttributeOptionDtoSerializer();
}

class _$CategoryAttributeOptionDtoSerializer implements PrimitiveSerializer<CategoryAttributeOptionDto> {
  @override
  final Iterable<Type> types = const [CategoryAttributeOptionDto, _$CategoryAttributeOptionDto];

  @override
  final String wireName = r'CategoryAttributeOptionDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CategoryAttributeOptionDto object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.value != null) {
      yield r'value';
      yield serializers.serialize(
        object.value,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.label != null) {
      yield r'label';
      yield serializers.serialize(
        object.label,
        specifiedType: const FullType(LocalizedTextInput),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CategoryAttributeOptionDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CategoryAttributeOptionDtoBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'value':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.value = valueDes;
          break;
        case r'label':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextInput),
          ) as LocalizedTextInput?;
          if (valueDes == null) continue;
          result.label.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CategoryAttributeOptionDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CategoryAttributeOptionDtoBuilder();
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


