//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'category_summary_response.g.dart';

/// CategorySummaryResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [name] 
/// * [parentCategoryId] 
/// * [imageFileId] 
@BuiltValue()
abstract class CategorySummaryResponse implements Built<CategorySummaryResponse, CategorySummaryResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  LocalizedTextResponse? get name;

  @BuiltValueField(wireName: r'parentCategoryId')
  String? get parentCategoryId;

  @BuiltValueField(wireName: r'imageFileId')
  String? get imageFileId;

  CategorySummaryResponse._();

  factory CategorySummaryResponse([void updates(CategorySummaryResponseBuilder b)]) = _$CategorySummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CategorySummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CategorySummaryResponse> get serializer => _$CategorySummaryResponseSerializer();
}

class _$CategorySummaryResponseSerializer implements PrimitiveSerializer<CategorySummaryResponse> {
  @override
  final Iterable<Type> types = const [CategorySummaryResponse, _$CategorySummaryResponse];

  @override
  final String wireName = r'CategorySummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CategorySummaryResponse object, {
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
    if (object.parentCategoryId != null) {
      yield r'parentCategoryId';
      yield serializers.serialize(
        object.parentCategoryId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.imageFileId != null) {
      yield r'imageFileId';
      yield serializers.serialize(
        object.imageFileId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CategorySummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CategorySummaryResponseBuilder result,
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
        case r'parentCategoryId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.parentCategoryId = valueDes;
          break;
        case r'imageFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.imageFileId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CategorySummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CategorySummaryResponseBuilder();
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


