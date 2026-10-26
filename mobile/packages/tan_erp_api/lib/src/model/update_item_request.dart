//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_input.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/item_capabilities_input.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_item_request.g.dart';

/// UpdateItemRequest
///
/// Properties:
/// * [code] 
/// * [itemType] 
/// * [categoryId] 
/// * [brandId] 
/// * [name] 
/// * [description] 
/// * [baseUnitId] 
/// * [availabilityMode] 
/// * [capabilities] 
/// * [selectedBranchIds] 
/// * [attributes] 
/// * [attributesSchemaVersion] 
/// * [taxCategoryCode] 
@BuiltValue()
abstract class UpdateItemRequest implements Built<UpdateItemRequest, UpdateItemRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String get code;

  @BuiltValueField(wireName: r'itemType')
  String get itemType;

  @BuiltValueField(wireName: r'categoryId')
  String get categoryId;

  @BuiltValueField(wireName: r'brandId')
  String? get brandId;

  @BuiltValueField(wireName: r'name')
  LocalizedTextInput get name;

  @BuiltValueField(wireName: r'description')
  LocalizedTextInput? get description;

  @BuiltValueField(wireName: r'baseUnitId')
  String get baseUnitId;

  @BuiltValueField(wireName: r'availabilityMode')
  String get availabilityMode;

  @BuiltValueField(wireName: r'capabilities')
  ItemCapabilitiesInput? get capabilities;

  @BuiltValueField(wireName: r'selectedBranchIds')
  BuiltList<String>? get selectedBranchIds;

  @BuiltValueField(wireName: r'attributes')
  BuiltMap<String, String>? get attributes;

  @BuiltValueField(wireName: r'attributesSchemaVersion')
  int? get attributesSchemaVersion;

  @BuiltValueField(wireName: r'taxCategoryCode')
  String? get taxCategoryCode;

  UpdateItemRequest._();

  factory UpdateItemRequest([void updates(UpdateItemRequestBuilder b)]) = _$UpdateItemRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateItemRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateItemRequest> get serializer => _$UpdateItemRequestSerializer();
}

class _$UpdateItemRequestSerializer implements PrimitiveSerializer<UpdateItemRequest> {
  @override
  final Iterable<Type> types = const [UpdateItemRequest, _$UpdateItemRequest];

  @override
  final String wireName = r'UpdateItemRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateItemRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'code';
    yield serializers.serialize(
      object.code,
      specifiedType: const FullType(String),
    );
    yield r'itemType';
    yield serializers.serialize(
      object.itemType,
      specifiedType: const FullType(String),
    );
    yield r'categoryId';
    yield serializers.serialize(
      object.categoryId,
      specifiedType: const FullType(String),
    );
    if (object.brandId != null) {
      yield r'brandId';
      yield serializers.serialize(
        object.brandId,
        specifiedType: const FullType.nullable(String),
      );
    }
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
    yield r'baseUnitId';
    yield serializers.serialize(
      object.baseUnitId,
      specifiedType: const FullType(String),
    );
    yield r'availabilityMode';
    yield serializers.serialize(
      object.availabilityMode,
      specifiedType: const FullType(String),
    );
    if (object.capabilities != null) {
      yield r'capabilities';
      yield serializers.serialize(
        object.capabilities,
        specifiedType: const FullType(ItemCapabilitiesInput),
      );
    }
    if (object.selectedBranchIds != null) {
      yield r'selectedBranchIds';
      yield serializers.serialize(
        object.selectedBranchIds,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.attributes != null) {
      yield r'attributes';
      yield serializers.serialize(
        object.attributes,
        specifiedType: const FullType.nullable(BuiltMap, [FullType(String), FullType(String)]),
      );
    }
    if (object.attributesSchemaVersion != null) {
      yield r'attributesSchemaVersion';
      yield serializers.serialize(
        object.attributesSchemaVersion,
        specifiedType: const FullType.nullable(int),
      );
    }
    if (object.taxCategoryCode != null) {
      yield r'taxCategoryCode';
      yield serializers.serialize(
        object.taxCategoryCode,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateItemRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateItemRequestBuilder result,
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
        case r'itemType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.itemType = valueDes;
          break;
        case r'categoryId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.categoryId = valueDes;
          break;
        case r'brandId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.brandId = valueDes;
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
        case r'baseUnitId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.baseUnitId = valueDes;
          break;
        case r'availabilityMode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.availabilityMode = valueDes;
          break;
        case r'capabilities':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ItemCapabilitiesInput),
          ) as ItemCapabilitiesInput?;
          if (valueDes == null) continue;
          result.capabilities.replace(valueDes);
          break;
        case r'selectedBranchIds':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.selectedBranchIds.replace(valueDes);
          break;
        case r'attributes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltMap, [FullType(String), FullType(String)]),
          ) as BuiltMap<String, String>?;
          if (valueDes == null) continue;
          result.attributes.replace(valueDes);
          break;
        case r'attributesSchemaVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.attributesSchemaVersion = valueDes;
          break;
        case r'taxCategoryCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxCategoryCode = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateItemRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateItemRequestBuilder();
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


