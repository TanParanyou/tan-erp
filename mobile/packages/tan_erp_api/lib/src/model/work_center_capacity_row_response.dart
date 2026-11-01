//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/work_center_daily_capacity_point_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_center_capacity_row_response.g.dart';

/// WorkCenterCapacityRowResponse
///
/// Properties:
/// * [workCenterId] 
/// * [branchId] 
/// * [code] 
/// * [name] 
/// * [dailyCapacityHours] 
/// * [hourlyRate] 
/// * [totalCapacityHours] 
/// * [totalPlannedHours] 
/// * [utilizationPercent] 
/// * [isOverloaded] 
/// * [days] 
@BuiltValue()
abstract class WorkCenterCapacityRowResponse implements Built<WorkCenterCapacityRowResponse, WorkCenterCapacityRowResponseBuilder> {
  @BuiltValueField(wireName: r'workCenterId')
  String? get workCenterId;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'dailyCapacityHours')
  double? get dailyCapacityHours;

  @BuiltValueField(wireName: r'hourlyRate')
  double? get hourlyRate;

  @BuiltValueField(wireName: r'totalCapacityHours')
  double? get totalCapacityHours;

  @BuiltValueField(wireName: r'totalPlannedHours')
  double? get totalPlannedHours;

  @BuiltValueField(wireName: r'utilizationPercent')
  double? get utilizationPercent;

  @BuiltValueField(wireName: r'isOverloaded')
  bool? get isOverloaded;

  @BuiltValueField(wireName: r'days')
  BuiltList<WorkCenterDailyCapacityPointResponse>? get days;

  WorkCenterCapacityRowResponse._();

  factory WorkCenterCapacityRowResponse([void updates(WorkCenterCapacityRowResponseBuilder b)]) = _$WorkCenterCapacityRowResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkCenterCapacityRowResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkCenterCapacityRowResponse> get serializer => _$WorkCenterCapacityRowResponseSerializer();
}

class _$WorkCenterCapacityRowResponseSerializer implements PrimitiveSerializer<WorkCenterCapacityRowResponse> {
  @override
  final Iterable<Type> types = const [WorkCenterCapacityRowResponse, _$WorkCenterCapacityRowResponse];

  @override
  final String wireName = r'WorkCenterCapacityRowResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkCenterCapacityRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.workCenterId != null) {
      yield r'workCenterId';
      yield serializers.serialize(
        object.workCenterId,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
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
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.dailyCapacityHours != null) {
      yield r'dailyCapacityHours';
      yield serializers.serialize(
        object.dailyCapacityHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.hourlyRate != null) {
      yield r'hourlyRate';
      yield serializers.serialize(
        object.hourlyRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalCapacityHours != null) {
      yield r'totalCapacityHours';
      yield serializers.serialize(
        object.totalCapacityHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalPlannedHours != null) {
      yield r'totalPlannedHours';
      yield serializers.serialize(
        object.totalPlannedHours,
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
    if (object.days != null) {
      yield r'days';
      yield serializers.serialize(
        object.days,
        specifiedType: const FullType.nullable(BuiltList, [FullType(WorkCenterDailyCapacityPointResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkCenterCapacityRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkCenterCapacityRowResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'workCenterId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workCenterId = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'dailyCapacityHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.dailyCapacityHours = valueDes;
          break;
        case r'hourlyRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.hourlyRate = valueDes;
          break;
        case r'totalCapacityHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalCapacityHours = valueDes;
          break;
        case r'totalPlannedHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalPlannedHours = valueDes;
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
        case r'days':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(WorkCenterDailyCapacityPointResponse)]),
          ) as BuiltList<WorkCenterDailyCapacityPointResponse>?;
          if (valueDes == null) continue;
          result.days.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkCenterCapacityRowResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkCenterCapacityRowResponseBuilder();
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


