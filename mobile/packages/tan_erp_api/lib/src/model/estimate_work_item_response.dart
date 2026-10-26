//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/estimate_work_item_item_response.dart';
import 'package:tan_erp_api/src/model/estimate_cost_component_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_work_item_response.g.dart';

/// EstimateWorkItemResponse
///
/// Properties:
/// * [id] 
/// * [estimateSectionId] 
/// * [code] 
/// * [descriptionTh] 
/// * [descriptionEn] 
/// * [quantity] 
/// * [unitCode] 
/// * [sellingRuleType] 
/// * [sellingRuleValue] 
/// * [sellingRuleReasonCode] 
/// * [unitCost] 
/// * [totalCost] 
/// * [unitSellingPrice] 
/// * [totalSellingPrice] 
/// * [sortOrder] 
/// * [costComponents] 
/// * [item] 
/// * [overrideReasonCode] 
/// * [overrideReason] 
@BuiltValue()
abstract class EstimateWorkItemResponse implements Built<EstimateWorkItemResponse, EstimateWorkItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'estimateSectionId')
  String? get estimateSectionId;

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

  @BuiltValueField(wireName: r'sellingRuleReasonCode')
  String? get sellingRuleReasonCode;

  @BuiltValueField(wireName: r'unitCost')
  double? get unitCost;

  @BuiltValueField(wireName: r'totalCost')
  double? get totalCost;

  @BuiltValueField(wireName: r'unitSellingPrice')
  double? get unitSellingPrice;

  @BuiltValueField(wireName: r'totalSellingPrice')
  double? get totalSellingPrice;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  @BuiltValueField(wireName: r'costComponents')
  BuiltList<EstimateCostComponentResponse>? get costComponents;

  @BuiltValueField(wireName: r'item')
  EstimateWorkItemItemResponse? get item;

  @BuiltValueField(wireName: r'overrideReasonCode')
  String? get overrideReasonCode;

  @BuiltValueField(wireName: r'overrideReason')
  String? get overrideReason;

  EstimateWorkItemResponse._();

  factory EstimateWorkItemResponse([void updates(EstimateWorkItemResponseBuilder b)]) = _$EstimateWorkItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateWorkItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateWorkItemResponse> get serializer => _$EstimateWorkItemResponseSerializer();
}

class _$EstimateWorkItemResponseSerializer implements PrimitiveSerializer<EstimateWorkItemResponse> {
  @override
  final Iterable<Type> types = const [EstimateWorkItemResponse, _$EstimateWorkItemResponse];

  @override
  final String wireName = r'EstimateWorkItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateWorkItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.estimateSectionId != null) {
      yield r'estimateSectionId';
      yield serializers.serialize(
        object.estimateSectionId,
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
    if (object.sellingRuleReasonCode != null) {
      yield r'sellingRuleReasonCode';
      yield serializers.serialize(
        object.sellingRuleReasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitCost != null) {
      yield r'unitCost';
      yield serializers.serialize(
        object.unitCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalCost != null) {
      yield r'totalCost';
      yield serializers.serialize(
        object.totalCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.unitSellingPrice != null) {
      yield r'unitSellingPrice';
      yield serializers.serialize(
        object.unitSellingPrice,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalSellingPrice != null) {
      yield r'totalSellingPrice';
      yield serializers.serialize(
        object.totalSellingPrice,
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
        specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateCostComponentResponse)]),
      );
    }
    if (object.item != null) {
      yield r'item';
      yield serializers.serialize(
        object.item,
        specifiedType: const FullType(EstimateWorkItemItemResponse),
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
    EstimateWorkItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateWorkItemResponseBuilder result,
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
        case r'estimateSectionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateSectionId = valueDes;
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
        case r'sellingRuleReasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sellingRuleReasonCode = valueDes;
          break;
        case r'unitCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitCost = valueDes;
          break;
        case r'totalCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalCost = valueDes;
          break;
        case r'unitSellingPrice':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitSellingPrice = valueDes;
          break;
        case r'totalSellingPrice':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalSellingPrice = valueDes;
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
            specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateCostComponentResponse)]),
          ) as BuiltList<EstimateCostComponentResponse>?;
          if (valueDes == null) continue;
          result.costComponents.replace(valueDes);
          break;
        case r'item':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateWorkItemItemResponse),
          ) as EstimateWorkItemItemResponse?;
          if (valueDes == null) continue;
          result.item.replace(valueDes);
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
  EstimateWorkItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateWorkItemResponseBuilder();
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


