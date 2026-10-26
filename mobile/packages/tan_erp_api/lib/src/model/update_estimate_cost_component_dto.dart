//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_estimate_cost_component_dto.g.dart';

/// UpdateEstimateCostComponentDto
///
/// Properties:
/// * [id] 
/// * [type] 
/// * [description] 
/// * [quantity] 
/// * [unitCode] 
/// * [unitCost] 
/// * [currency] 
/// * [sortOrder] 
/// * [itemId] 
/// * [costRecordId] 
/// * [costRecordVersion] 
/// * [provisionalReasonCode] 
/// * [provisionalNote] 
/// * [conversionId] 
@BuiltValue()
abstract class UpdateEstimateCostComponentDto implements Built<UpdateEstimateCostComponentDto, UpdateEstimateCostComponentDtoBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'type')
  String? get type;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  @BuiltValueField(wireName: r'unitCost')
  double? get unitCost;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'costRecordId')
  String? get costRecordId;

  @BuiltValueField(wireName: r'costRecordVersion')
  int? get costRecordVersion;

  @BuiltValueField(wireName: r'provisionalReasonCode')
  String? get provisionalReasonCode;

  @BuiltValueField(wireName: r'provisionalNote')
  String? get provisionalNote;

  @BuiltValueField(wireName: r'conversionId')
  String? get conversionId;

  UpdateEstimateCostComponentDto._();

  factory UpdateEstimateCostComponentDto([void updates(UpdateEstimateCostComponentDtoBuilder b)]) = _$UpdateEstimateCostComponentDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateEstimateCostComponentDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateEstimateCostComponentDto> get serializer => _$UpdateEstimateCostComponentDtoSerializer();
}

class _$UpdateEstimateCostComponentDtoSerializer implements PrimitiveSerializer<UpdateEstimateCostComponentDto> {
  @override
  final Iterable<Type> types = const [UpdateEstimateCostComponentDto, _$UpdateEstimateCostComponentDto];

  @override
  final String wireName = r'UpdateEstimateCostComponentDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateEstimateCostComponentDto object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.type != null) {
      yield r'type';
      yield serializers.serialize(
        object.type,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
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
    if (object.unitCost != null) {
      yield r'unitCost';
      yield serializers.serialize(
        object.unitCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
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
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costRecordId != null) {
      yield r'costRecordId';
      yield serializers.serialize(
        object.costRecordId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costRecordVersion != null) {
      yield r'costRecordVersion';
      yield serializers.serialize(
        object.costRecordVersion,
        specifiedType: const FullType.nullable(int),
      );
    }
    if (object.provisionalReasonCode != null) {
      yield r'provisionalReasonCode';
      yield serializers.serialize(
        object.provisionalReasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.provisionalNote != null) {
      yield r'provisionalNote';
      yield serializers.serialize(
        object.provisionalNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.conversionId != null) {
      yield r'conversionId';
      yield serializers.serialize(
        object.conversionId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateEstimateCostComponentDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateEstimateCostComponentDtoBuilder result,
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
        case r'type':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.type = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
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
        case r'unitCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitCost = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'costRecordId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costRecordId = valueDes;
          break;
        case r'costRecordVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.costRecordVersion = valueDes;
          break;
        case r'provisionalReasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.provisionalReasonCode = valueDes;
          break;
        case r'provisionalNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.provisionalNote = valueDes;
          break;
        case r'conversionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.conversionId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateEstimateCostComponentDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateEstimateCostComponentDtoBuilder();
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


