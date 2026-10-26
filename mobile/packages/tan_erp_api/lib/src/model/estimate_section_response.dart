//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/estimate_work_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_section_response.g.dart';

/// EstimateSectionResponse
///
/// Properties:
/// * [id] 
/// * [estimateRevisionId] 
/// * [code] 
/// * [nameTh] 
/// * [nameEn] 
/// * [sortOrder] 
/// * [subtotalCost] 
/// * [subtotalSellingPrice] 
/// * [workItems] 
@BuiltValue()
abstract class EstimateSectionResponse implements Built<EstimateSectionResponse, EstimateSectionResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'estimateRevisionId')
  String? get estimateRevisionId;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'nameTh')
  String? get nameTh;

  @BuiltValueField(wireName: r'nameEn')
  String? get nameEn;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  @BuiltValueField(wireName: r'subtotalCost')
  double? get subtotalCost;

  @BuiltValueField(wireName: r'subtotalSellingPrice')
  double? get subtotalSellingPrice;

  @BuiltValueField(wireName: r'workItems')
  BuiltList<EstimateWorkItemResponse>? get workItems;

  EstimateSectionResponse._();

  factory EstimateSectionResponse([void updates(EstimateSectionResponseBuilder b)]) = _$EstimateSectionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateSectionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateSectionResponse> get serializer => _$EstimateSectionResponseSerializer();
}

class _$EstimateSectionResponseSerializer implements PrimitiveSerializer<EstimateSectionResponse> {
  @override
  final Iterable<Type> types = const [EstimateSectionResponse, _$EstimateSectionResponse];

  @override
  final String wireName = r'EstimateSectionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateSectionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.estimateRevisionId != null) {
      yield r'estimateRevisionId';
      yield serializers.serialize(
        object.estimateRevisionId,
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
    if (object.nameTh != null) {
      yield r'nameTh';
      yield serializers.serialize(
        object.nameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.nameEn != null) {
      yield r'nameEn';
      yield serializers.serialize(
        object.nameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
      );
    }
    if (object.subtotalCost != null) {
      yield r'subtotalCost';
      yield serializers.serialize(
        object.subtotalCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.subtotalSellingPrice != null) {
      yield r'subtotalSellingPrice';
      yield serializers.serialize(
        object.subtotalSellingPrice,
        specifiedType: const FullType(double),
      );
    }
    if (object.workItems != null) {
      yield r'workItems';
      yield serializers.serialize(
        object.workItems,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateWorkItemResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateSectionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateSectionResponseBuilder result,
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
        case r'estimateRevisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateRevisionId = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'nameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nameTh = valueDes;
          break;
        case r'nameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nameEn = valueDes;
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        case r'subtotalCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.subtotalCost = valueDes;
          break;
        case r'subtotalSellingPrice':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.subtotalSellingPrice = valueDes;
          break;
        case r'workItems':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateWorkItemResponse)]),
          ) as BuiltList<EstimateWorkItemResponse>?;
          if (valueDes == null) continue;
          result.workItems.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateSectionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateSectionResponseBuilder();
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


