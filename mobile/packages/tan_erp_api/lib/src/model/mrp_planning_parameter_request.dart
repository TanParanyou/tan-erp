//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mrp_planning_parameter_request.g.dart';

/// MrpPlanningParameterRequest
///
/// Properties:
/// * [itemId] 
/// * [warehouseId] 
/// * [safetyStock] 
/// * [minOrderQuantity] 
/// * [lotMultiple] 
/// * [leadTimeDays] 
@BuiltValue()
abstract class MrpPlanningParameterRequest implements Built<MrpPlanningParameterRequest, MrpPlanningParameterRequestBuilder> {
  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'warehouseId')
  String? get warehouseId;

  @BuiltValueField(wireName: r'safetyStock')
  double? get safetyStock;

  @BuiltValueField(wireName: r'minOrderQuantity')
  double? get minOrderQuantity;

  @BuiltValueField(wireName: r'lotMultiple')
  double? get lotMultiple;

  @BuiltValueField(wireName: r'leadTimeDays')
  int? get leadTimeDays;

  MrpPlanningParameterRequest._();

  factory MrpPlanningParameterRequest([void updates(MrpPlanningParameterRequestBuilder b)]) = _$MrpPlanningParameterRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MrpPlanningParameterRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MrpPlanningParameterRequest> get serializer => _$MrpPlanningParameterRequestSerializer();
}

class _$MrpPlanningParameterRequestSerializer implements PrimitiveSerializer<MrpPlanningParameterRequest> {
  @override
  final Iterable<Type> types = const [MrpPlanningParameterRequest, _$MrpPlanningParameterRequest];

  @override
  final String wireName = r'MrpPlanningParameterRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MrpPlanningParameterRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
      );
    }
    if (object.warehouseId != null) {
      yield r'warehouseId';
      yield serializers.serialize(
        object.warehouseId,
        specifiedType: const FullType.nullable(String),
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
  }

  @override
  Object serialize(
    Serializers serializers,
    MrpPlanningParameterRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MrpPlanningParameterRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'warehouseId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.warehouseId = valueDes;
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MrpPlanningParameterRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MrpPlanningParameterRequestBuilder();
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


