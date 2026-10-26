//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/update_estimate_cost_component_dto.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_estimate_work_item_dto.g.dart';

/// UpdateEstimateWorkItemDto
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [descriptionTh] 
/// * [descriptionEn] 
/// * [quantity] 
/// * [unitCode] 
/// * [sellingRuleType] 
/// * [sellingRuleValue] 
/// * [sortOrder] 
/// * [costComponents] 
/// * [sellingRuleReasonCode] 
/// * [itemId] 
/// * [overrideReasonCode] 
/// * [overrideReason] 
@BuiltValue()
abstract class UpdateEstimateWorkItemDto implements Built<UpdateEstimateWorkItemDto, UpdateEstimateWorkItemDtoBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'descriptionTh')
  String? get descriptionTh;

  @BuiltValueField(wireName: r'descriptionEn')
  String? get descriptionEn;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  @BuiltValueField(wireName: r'sellingRuleType')
  String? get sellingRuleType;

  @BuiltValueField(wireName: r'sellingRuleValue')
  double? get sellingRuleValue;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  @BuiltValueField(wireName: r'costComponents')
  BuiltList<UpdateEstimateCostComponentDto>? get costComponents;

  @BuiltValueField(wireName: r'sellingRuleReasonCode')
  String? get sellingRuleReasonCode;

  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'overrideReasonCode')
  String? get overrideReasonCode;

  @BuiltValueField(wireName: r'overrideReason')
  String? get overrideReason;

  UpdateEstimateWorkItemDto._();

  factory UpdateEstimateWorkItemDto([void updates(UpdateEstimateWorkItemDtoBuilder b)]) = _$UpdateEstimateWorkItemDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateEstimateWorkItemDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateEstimateWorkItemDto> get serializer => _$UpdateEstimateWorkItemDtoSerializer();
}

class _$UpdateEstimateWorkItemDtoSerializer implements PrimitiveSerializer<UpdateEstimateWorkItemDto> {
  @override
  final Iterable<Type> types = const [UpdateEstimateWorkItemDto, _$UpdateEstimateWorkItemDto];

  @override
  final String wireName = r'UpdateEstimateWorkItemDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateEstimateWorkItemDto object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.descriptionTh != null) {
      yield r'descriptionTh';
      yield serializers.serialize(
        object.descriptionTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.descriptionEn != null) {
      yield r'descriptionEn';
      yield serializers.serialize(
        object.descriptionEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.unitCode != null) {
      yield r'unitCode';
      yield serializers.serialize(
        object.unitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sellingRuleType != null) {
      yield r'sellingRuleType';
      yield serializers.serialize(
        object.sellingRuleType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sellingRuleValue != null) {
      yield r'sellingRuleValue';
      yield serializers.serialize(
        object.sellingRuleValue,
        specifiedType: const FullType(double),
      );
    }
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
      );
    }
    if (object.costComponents != null) {
      yield r'costComponents';
      yield serializers.serialize(
        object.costComponents,
        specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateEstimateCostComponentDto)]),
      );
    }
    if (object.sellingRuleReasonCode != null) {
      yield r'sellingRuleReasonCode';
      yield serializers.serialize(
        object.sellingRuleReasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.overrideReasonCode != null) {
      yield r'overrideReasonCode';
      yield serializers.serialize(
        object.overrideReasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.overrideReason != null) {
      yield r'overrideReason';
      yield serializers.serialize(
        object.overrideReason,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateEstimateWorkItemDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateEstimateWorkItemDtoBuilder result,
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
        case r'descriptionTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.descriptionTh = valueDes;
          break;
        case r'descriptionEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.descriptionEn = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        case r'unitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitCode = valueDes;
          break;
        case r'sellingRuleType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sellingRuleType = valueDes;
          break;
        case r'sellingRuleValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.sellingRuleValue = valueDes;
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        case r'costComponents':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateEstimateCostComponentDto)]),
          ) as BuiltList<UpdateEstimateCostComponentDto>?;
          if (valueDes == null) continue;
          result.costComponents.replace(valueDes);
          break;
        case r'sellingRuleReasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sellingRuleReasonCode = valueDes;
          break;
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'overrideReasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.overrideReasonCode = valueDes;
          break;
        case r'overrideReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.overrideReason = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateEstimateWorkItemDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateEstimateWorkItemDtoBuilder();
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


