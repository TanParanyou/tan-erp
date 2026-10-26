//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_input.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_item_category_request.g.dart';

/// UpdateItemCategoryRequest
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [description] 
/// * [parentCategoryId] 
/// * [allowedItemTypes] 
/// * [sortOrder] 
/// * [imageFileId] 
@BuiltValue()
abstract class UpdateItemCategoryRequest implements Built<UpdateItemCategoryRequest, UpdateItemCategoryRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String get code;

  @BuiltValueField(wireName: r'name')
  LocalizedTextInput get name;

  @BuiltValueField(wireName: r'description')
  LocalizedTextInput? get description;

  @BuiltValueField(wireName: r'parentCategoryId')
  String? get parentCategoryId;

  @BuiltValueField(wireName: r'allowedItemTypes')
  BuiltList<String>? get allowedItemTypes;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  @BuiltValueField(wireName: r'imageFileId')
  String? get imageFileId;

  UpdateItemCategoryRequest._();

  factory UpdateItemCategoryRequest([void updates(UpdateItemCategoryRequestBuilder b)]) = _$UpdateItemCategoryRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateItemCategoryRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateItemCategoryRequest> get serializer => _$UpdateItemCategoryRequestSerializer();
}

class _$UpdateItemCategoryRequestSerializer implements PrimitiveSerializer<UpdateItemCategoryRequest> {
  @override
  final Iterable<Type> types = const [UpdateItemCategoryRequest, _$UpdateItemCategoryRequest];

  @override
  final String wireName = r'UpdateItemCategoryRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateItemCategoryRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'code';
    yield serializers.serialize(
      object.code,
      specifiedType: const FullType(String),
    );
    yield r'name';
    yield serializers.serialize(
      object.name,
      specifiedType: const FullType(LocalizedTextInput),
    );
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType(LocalizedTextInput),
      );
    }
    if (object.parentCategoryId != null) {
      yield r'parentCategoryId';
      yield serializers.serialize(
        object.parentCategoryId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.allowedItemTypes != null) {
      yield r'allowedItemTypes';
      yield serializers.serialize(
        object.allowedItemTypes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
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
    UpdateItemCategoryRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateItemCategoryRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(LocalizedTextInput),
          ) as LocalizedTextInput;
          result.name.replace(valueDes);
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextInput),
          ) as LocalizedTextInput?;
          if (valueDes == null) continue;
          result.description.replace(valueDes);
          break;
        case r'parentCategoryId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.parentCategoryId = valueDes;
          break;
        case r'allowedItemTypes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.allowedItemTypes.replace(valueDes);
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
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
  UpdateItemCategoryRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateItemCategoryRequestBuilder();
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


