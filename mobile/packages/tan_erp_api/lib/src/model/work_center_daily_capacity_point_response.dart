//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_center_daily_capacity_point_response.g.dart';

/// WorkCenterDailyCapacityPointResponse
///
/// Properties:
/// * [date] 
/// * [capacityHours] 
/// * [plannedHours] 
/// * [remainingHours] 
/// * [utilizationPercent] 
/// * [isOverloaded] 
@BuiltValue()
abstract class WorkCenterDailyCapacityPointResponse implements Built<WorkCenterDailyCapacityPointResponse, WorkCenterDailyCapacityPointResponseBuilder> {
  @BuiltValueField(wireName: r'date')
  Date? get date;

  @BuiltValueField(wireName: r'capacityHours')
  double? get capacityHours;

  @BuiltValueField(wireName: r'plannedHours')
  double? get plannedHours;

  @BuiltValueField(wireName: r'remainingHours')
  double? get remainingHours;

  @BuiltValueField(wireName: r'utilizationPercent')
  double? get utilizationPercent;

  @BuiltValueField(wireName: r'isOverloaded')
  bool? get isOverloaded;

  WorkCenterDailyCapacityPointResponse._();

  factory WorkCenterDailyCapacityPointResponse([void updates(WorkCenterDailyCapacityPointResponseBuilder b)]) = _$WorkCenterDailyCapacityPointResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkCenterDailyCapacityPointResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkCenterDailyCapacityPointResponse> get serializer => _$WorkCenterDailyCapacityPointResponseSerializer();
}

class _$WorkCenterDailyCapacityPointResponseSerializer implements PrimitiveSerializer<WorkCenterDailyCapacityPointResponse> {
  @override
  final Iterable<Type> types = const [WorkCenterDailyCapacityPointResponse, _$WorkCenterDailyCapacityPointResponse];

  @override
  final String wireName = r'WorkCenterDailyCapacityPointResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkCenterDailyCapacityPointResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.date != null) {
      yield r'date';
      yield serializers.serialize(
        object.date,
        specifiedType: const FullType(Date),
      );
    }
    if (object.capacityHours != null) {
      yield r'capacityHours';
      yield serializers.serialize(
        object.capacityHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.plannedHours != null) {
      yield r'plannedHours';
      yield serializers.serialize(
        object.plannedHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.remainingHours != null) {
      yield r'remainingHours';
      yield serializers.serialize(
        object.remainingHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.utilizationPercent != null) {
      yield r'utilizationPercent';
      yield serializers.serialize(
        object.utilizationPercent,
        specifiedType: const FullType(double),
      );
    }
    if (object.isOverloaded != null) {
      yield r'isOverloaded';
      yield serializers.serialize(
        object.isOverloaded,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkCenterDailyCapacityPointResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkCenterDailyCapacityPointResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'date':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.date = valueDes;
          break;
        case r'capacityHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.capacityHours = valueDes;
          break;
        case r'plannedHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.plannedHours = valueDes;
          break;
        case r'remainingHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.remainingHours = valueDes;
          break;
        case r'utilizationPercent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.utilizationPercent = valueDes;
          break;
        case r'isOverloaded':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isOverloaded = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkCenterDailyCapacityPointResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkCenterDailyCapacityPointResponseBuilder();
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


