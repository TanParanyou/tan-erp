//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/production_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_material_response.g.dart';

/// WorkOrderMaterialResponse
///
/// Properties:
/// * [id] 
/// * [item] 
/// * [requiredQuantity] 
/// * [issuedQuantity] 
/// * [returnedQuantity] 
/// * [netIssuedQuantity] 
/// * [remainingQuantity] 
/// * [issuedValue] 
/// * [returnedValue] 
@BuiltValue()
abstract class WorkOrderMaterialResponse implements Built<WorkOrderMaterialResponse, WorkOrderMaterialResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'item')
  ProductionItemResponse? get item;

  @BuiltValueField(wireName: r'requiredQuantity')
  double? get requiredQuantity;

  @BuiltValueField(wireName: r'issuedQuantity')
  double? get issuedQuantity;

  @BuiltValueField(wireName: r'returnedQuantity')
  double? get returnedQuantity;

  @BuiltValueField(wireName: r'netIssuedQuantity')
  double? get netIssuedQuantity;

  @BuiltValueField(wireName: r'remainingQuantity')
  double? get remainingQuantity;

  @BuiltValueField(wireName: r'issuedValue')
  double? get issuedValue;

  @BuiltValueField(wireName: r'returnedValue')
  double? get returnedValue;

  WorkOrderMaterialResponse._();

  factory WorkOrderMaterialResponse([void updates(WorkOrderMaterialResponseBuilder b)]) = _$WorkOrderMaterialResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderMaterialResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderMaterialResponse> get serializer => _$WorkOrderMaterialResponseSerializer();
}

class _$WorkOrderMaterialResponseSerializer implements PrimitiveSerializer<WorkOrderMaterialResponse> {
  @override
  final Iterable<Type> types = const [WorkOrderMaterialResponse, _$WorkOrderMaterialResponse];

  @override
  final String wireName = r'WorkOrderMaterialResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderMaterialResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.item != null) {
      yield r'item';
      yield serializers.serialize(
        object.item,
        specifiedType: const FullType(ProductionItemResponse),
      );
    }
    if (object.requiredQuantity != null) {
      yield r'requiredQuantity';
      yield serializers.serialize(
        object.requiredQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.issuedQuantity != null) {
      yield r'issuedQuantity';
      yield serializers.serialize(
        object.issuedQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.returnedQuantity != null) {
      yield r'returnedQuantity';
      yield serializers.serialize(
        object.returnedQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.netIssuedQuantity != null) {
      yield r'netIssuedQuantity';
      yield serializers.serialize(
        object.netIssuedQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.remainingQuantity != null) {
      yield r'remainingQuantity';
      yield serializers.serialize(
        object.remainingQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.issuedValue != null) {
      yield r'issuedValue';
      yield serializers.serialize(
        object.issuedValue,
        specifiedType: const FullType(double),
      );
    }
    if (object.returnedValue != null) {
      yield r'returnedValue';
      yield serializers.serialize(
        object.returnedValue,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkOrderMaterialResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderMaterialResponseBuilder result,
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
        case r'item':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProductionItemResponse),
          ) as ProductionItemResponse?;
          if (valueDes == null) continue;
          result.item.replace(valueDes);
          break;
        case r'requiredQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.requiredQuantity = valueDes;
          break;
        case r'issuedQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.issuedQuantity = valueDes;
          break;
        case r'returnedQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.returnedQuantity = valueDes;
          break;
        case r'netIssuedQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.netIssuedQuantity = valueDes;
          break;
        case r'remainingQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.remainingQuantity = valueDes;
          break;
        case r'issuedValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.issuedValue = valueDes;
          break;
        case r'returnedValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.returnedValue = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkOrderMaterialResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderMaterialResponseBuilder();
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


