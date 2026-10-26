//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/unit_summary_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/category_summary_response.dart';
import 'package:tan_erp_api/src/model/item_alias_response.dart';
import 'package:tan_erp_api/src/model/item_image_summary_response.dart';
import 'package:tan_erp_api/src/model/localized_text_response.dart';
import 'package:tan_erp_api/src/model/item_branch_availability_response.dart';
import 'package:tan_erp_api/src/model/item_capabilities_response.dart';
import 'package:tan_erp_api/src/model/brand_summary_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'item_response.g.dart';

/// ItemResponse
///
/// Properties:
/// * [id] 
/// * [organizationId] 
/// * [code] 
/// * [itemType] 
/// * [category] 
/// * [brand] 
/// * [baseUnit] 
/// * [name] 
/// * [description] 
/// * [taxCategoryCode] 
/// * [availabilityMode] 
/// * [capabilities] 
/// * [attributes] 
/// * [attributesSchemaVersion] 
/// * [status] 
/// * [activatedOnce] 
/// * [activatedAtUtc] 
/// * [activatedByUserId] 
/// * [inactiveAtUtc] 
/// * [inactiveByUserId] 
/// * [inactiveReasonCode] 
/// * [inactiveReason] 
/// * [aliases] 
/// * [branchAvailabilities] 
/// * [primaryImage] 
/// * [rowVersion] 
/// * [createdAtUtc] 
/// * [createdByUserId] 
/// * [updatedAtUtc] 
/// * [updatedByUserId] 
@BuiltValue()
abstract class ItemResponse implements Built<ItemResponse, ItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'organizationId')
  String? get organizationId;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'itemType')
  String? get itemType;

  @BuiltValueField(wireName: r'category')
  CategorySummaryResponse? get category;

  @BuiltValueField(wireName: r'brand')
  BrandSummaryResponse? get brand;

  @BuiltValueField(wireName: r'baseUnit')
  UnitSummaryResponse? get baseUnit;

  @BuiltValueField(wireName: r'name')
  LocalizedTextResponse? get name;

  @BuiltValueField(wireName: r'description')
  LocalizedTextResponse? get description;

  @BuiltValueField(wireName: r'taxCategoryCode')
  String? get taxCategoryCode;

  @BuiltValueField(wireName: r'availabilityMode')
  String? get availabilityMode;

  @BuiltValueField(wireName: r'capabilities')
  ItemCapabilitiesResponse? get capabilities;

  @BuiltValueField(wireName: r'attributes')
  BuiltMap<String, String>? get attributes;

  @BuiltValueField(wireName: r'attributesSchemaVersion')
  int? get attributesSchemaVersion;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'activatedOnce')
  bool? get activatedOnce;

  @BuiltValueField(wireName: r'activatedAtUtc')
  DateTime? get activatedAtUtc;

  @BuiltValueField(wireName: r'activatedByUserId')
  String? get activatedByUserId;

  @BuiltValueField(wireName: r'inactiveAtUtc')
  DateTime? get inactiveAtUtc;

  @BuiltValueField(wireName: r'inactiveByUserId')
  String? get inactiveByUserId;

  @BuiltValueField(wireName: r'inactiveReasonCode')
  String? get inactiveReasonCode;

  @BuiltValueField(wireName: r'inactiveReason')
  String? get inactiveReason;

  @BuiltValueField(wireName: r'aliases')
  BuiltList<ItemAliasResponse>? get aliases;

  @BuiltValueField(wireName: r'branchAvailabilities')
  BuiltList<ItemBranchAvailabilityResponse>? get branchAvailabilities;

  @BuiltValueField(wireName: r'primaryImage')
  ItemImageSummaryResponse? get primaryImage;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'createdByUserId')
  String? get createdByUserId;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime? get updatedAtUtc;

  @BuiltValueField(wireName: r'updatedByUserId')
  String? get updatedByUserId;

  ItemResponse._();

  factory ItemResponse([void updates(ItemResponseBuilder b)]) = _$ItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ItemResponse> get serializer => _$ItemResponseSerializer();
}

class _$ItemResponseSerializer implements PrimitiveSerializer<ItemResponse> {
  @override
  final Iterable<Type> types = const [ItemResponse, _$ItemResponse];

  @override
  final String wireName = r'ItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.organizationId != null) {
      yield r'organizationId';
      yield serializers.serialize(
        object.organizationId,
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
    if (object.itemType != null) {
      yield r'itemType';
      yield serializers.serialize(
        object.itemType,
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
    if (object.taxCategoryCode != null) {
      yield r'taxCategoryCode';
      yield serializers.serialize(
        object.taxCategoryCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.availabilityMode != null) {
      yield r'availabilityMode';
      yield serializers.serialize(
        object.availabilityMode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.capabilities != null) {
      yield r'capabilities';
      yield serializers.serialize(
        object.capabilities,
        specifiedType: const FullType(ItemCapabilitiesResponse),
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
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.activatedOnce != null) {
      yield r'activatedOnce';
      yield serializers.serialize(
        object.activatedOnce,
        specifiedType: const FullType(bool),
      );
    }
    if (object.activatedAtUtc != null) {
      yield r'activatedAtUtc';
      yield serializers.serialize(
        object.activatedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.activatedByUserId != null) {
      yield r'activatedByUserId';
      yield serializers.serialize(
        object.activatedByUserId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.inactiveAtUtc != null) {
      yield r'inactiveAtUtc';
      yield serializers.serialize(
        object.inactiveAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.inactiveByUserId != null) {
      yield r'inactiveByUserId';
      yield serializers.serialize(
        object.inactiveByUserId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.inactiveReasonCode != null) {
      yield r'inactiveReasonCode';
      yield serializers.serialize(
        object.inactiveReasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.inactiveReason != null) {
      yield r'inactiveReason';
      yield serializers.serialize(
        object.inactiveReason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.aliases != null) {
      yield r'aliases';
      yield serializers.serialize(
        object.aliases,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ItemAliasResponse)]),
      );
    }
    if (object.branchAvailabilities != null) {
      yield r'branchAvailabilities';
      yield serializers.serialize(
        object.branchAvailabilities,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ItemBranchAvailabilityResponse)]),
      );
    }
    if (object.primaryImage != null) {
      yield r'primaryImage';
      yield serializers.serialize(
        object.primaryImage,
        specifiedType: const FullType(ItemImageSummaryResponse),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.createdByUserId != null) {
      yield r'createdByUserId';
      yield serializers.serialize(
        object.createdByUserId,
        specifiedType: const FullType(String),
      );
    }
    if (object.updatedAtUtc != null) {
      yield r'updatedAtUtc';
      yield serializers.serialize(
        object.updatedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.updatedByUserId != null) {
      yield r'updatedByUserId';
      yield serializers.serialize(
        object.updatedByUserId,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ItemResponseBuilder result,
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
        case r'organizationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.organizationId = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'itemType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemType = valueDes;
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
        case r'taxCategoryCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxCategoryCode = valueDes;
          break;
        case r'availabilityMode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.availabilityMode = valueDes;
          break;
        case r'capabilities':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ItemCapabilitiesResponse),
          ) as ItemCapabilitiesResponse?;
          if (valueDes == null) continue;
          result.capabilities.replace(valueDes);
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
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'activatedOnce':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.activatedOnce = valueDes;
          break;
        case r'activatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.activatedAtUtc = valueDes;
          break;
        case r'activatedByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.activatedByUserId = valueDes;
          break;
        case r'inactiveAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.inactiveAtUtc = valueDes;
          break;
        case r'inactiveByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.inactiveByUserId = valueDes;
          break;
        case r'inactiveReasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.inactiveReasonCode = valueDes;
          break;
        case r'inactiveReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.inactiveReason = valueDes;
          break;
        case r'aliases':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ItemAliasResponse)]),
          ) as BuiltList<ItemAliasResponse>?;
          if (valueDes == null) continue;
          result.aliases.replace(valueDes);
          break;
        case r'branchAvailabilities':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ItemBranchAvailabilityResponse)]),
          ) as BuiltList<ItemBranchAvailabilityResponse>?;
          if (valueDes == null) continue;
          result.branchAvailabilities.replace(valueDes);
          break;
        case r'primaryImage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ItemImageSummaryResponse),
          ) as ItemImageSummaryResponse?;
          if (valueDes == null) continue;
          result.primaryImage.replace(valueDes);
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'createdByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.createdByUserId = valueDes;
          break;
        case r'updatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.updatedAtUtc = valueDes;
          break;
        case r'updatedByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.updatedByUserId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ItemResponseBuilder();
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


