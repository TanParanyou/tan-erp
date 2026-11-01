//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/mrp_item_response.dart';
import 'package:tan_erp_api/src/model/mrp_warehouse_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_planning_parameter_response.g.dart';

/// MrpPlanningParameterResponse
///
/// Properties:
/// * [id] 
/// * [item] 
/// * [warehouse] 
/// * [safetyStock] 
/// * [minOrderQuantity] 
/// * [lotMultiple] 
/// * [leadTimeDays] 
/// * [updatedAtUtc] 
/// * [rowVersion] 
@BuiltValue()
abstract class MrpPlanningParameterResponse implements Built<MrpPlanningParameterResponse, MrpPlanningParameterResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'item')
  MrpItemResponse? get item;

  @BuiltValueField(wireName: r'warehouse')
  MrpWarehouseResponse? get warehouse;

  @BuiltValueField(wireName: r'safetyStock')
  double? get safetyStock;

  @BuiltValueField(wireName: r'minOrderQuantity')
  double? get minOrderQuantity;

  @BuiltValueField(wireName: r'lotMultiple')
  double? get lotMultiple;

  @BuiltValueField(wireName: r'leadTimeDays')
  int? get leadTimeDays;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime? get updatedAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  MrpPlanningParameterResponse._();

  factory MrpPlanningParameterResponse([void updates(MrpPlanningParameterResponseBuilder b)]) = _$MrpPlanningParameterResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpPlanningParameterResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpPlanningParameterResponse> get serializer => _$MrpPlanningParameterResponseSerializer();
}

class _$MrpPlanningParameterResponseSerializer implements PrimitiveSerializer<MrpPlanningParameterResponse> {
  @override
  final Iterable<Type> types = const [MrpPlanningParameterResponse, _$MrpPlanningParameterResponse];

  @override
  final String wireName = r'MrpPlanningParameterResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpPlanningParameterResponse object, {
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
        specifiedType: const FullType(MrpItemResponse),
      );
    }
    if (object.warehouse != null) {
      yield r'warehouse';
      yield serializers.serialize(
        object.warehouse,
        specifiedType: const FullType(MrpWarehouseResponse),
      );
    }
    if (object.safetyStock != null) {
      yield r'safetyStock';
      yield serializers.serialize(
        object.safetyStock,
        specifiedType: const FullType(double),
      );
    }
    if (object.minOrderQuantity != null) {
      yield r'minOrderQuantity';
      yield serializers.serialize(
        object.minOrderQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.lotMultiple != null) {
      yield r'lotMultiple';
      yield serializers.serialize(
        object.lotMultiple,
        specifiedType: const FullType(double),
      );
    }
    if (object.leadTimeDays != null) {
      yield r'leadTimeDays';
      yield serializers.serialize(
        object.leadTimeDays,
        specifiedType: const FullType.nullable(int),
      );
    }
    if (object.updatedAtUtc != null) {
      yield r'updatedAtUtc';
      yield serializers.serialize(
        object.updatedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpPlanningParameterResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpPlanningParameterResponseBuilder result,
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
            specifiedType: const FullType.nullable(MrpItemResponse),
          ) as MrpItemResponse?;
          if (valueDes == null) continue;
          result.item.replace(valueDes);
          break;
        case r'warehouse':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(MrpWarehouseResponse),
          ) as MrpWarehouseResponse?;
          if (valueDes == null) continue;
          result.warehouse.replace(valueDes);
          break;
        case r'safetyStock':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.safetyStock = valueDes;
          break;
        case r'minOrderQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.minOrderQuantity = valueDes;
          break;
        case r'lotMultiple':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.lotMultiple = valueDes;
          break;
        case r'leadTimeDays':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.leadTimeDays = valueDes;
          break;
        case r'updatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.updatedAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MrpPlanningParameterResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpPlanningParameterResponseBuilder();
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


