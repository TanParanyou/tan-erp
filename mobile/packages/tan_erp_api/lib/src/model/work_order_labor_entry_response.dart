//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/production_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_labor_entry_response.g.dart';

/// WorkOrderLaborEntryResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [worker] 
/// * [workDate] 
/// * [hours] 
/// * [hourlyRate] 
/// * [costAmount] 
/// * [taskDescription] 
/// * [decidedAtUtc] 
@BuiltValue()
abstract class WorkOrderLaborEntryResponse implements Built<WorkOrderLaborEntryResponse, WorkOrderLaborEntryResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'worker')
  ProductionPersonResponse? get worker;

  @BuiltValueField(wireName: r'workDate')
  Date? get workDate;

  @BuiltValueField(wireName: r'hours')
  double? get hours;

  @BuiltValueField(wireName: r'hourlyRate')
  double? get hourlyRate;

  @BuiltValueField(wireName: r'costAmount')
  double? get costAmount;

  @BuiltValueField(wireName: r'taskDescription')
  String? get taskDescription;

  @BuiltValueField(wireName: r'decidedAtUtc')
  DateTime? get decidedAtUtc;

  WorkOrderLaborEntryResponse._();

  factory WorkOrderLaborEntryResponse([void updates(WorkOrderLaborEntryResponseBuilder b)]) = _$WorkOrderLaborEntryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderLaborEntryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderLaborEntryResponse> get serializer => _$WorkOrderLaborEntryResponseSerializer();
}

class _$WorkOrderLaborEntryResponseSerializer implements PrimitiveSerializer<WorkOrderLaborEntryResponse> {
  @override
  final Iterable<Type> types = const [WorkOrderLaborEntryResponse, _$WorkOrderLaborEntryResponse];

  @override
  final String wireName = r'WorkOrderLaborEntryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderLaborEntryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.worker != null) {
      yield r'worker';
      yield serializers.serialize(
        object.worker,
        specifiedType: const FullType(ProductionPersonResponse),
      );
    }
    if (object.workDate != null) {
      yield r'workDate';
      yield serializers.serialize(
        object.workDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.hours != null) {
      yield r'hours';
      yield serializers.serialize(
        object.hours,
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
    if (object.costAmount != null) {
      yield r'costAmount';
      yield serializers.serialize(
        object.costAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.taskDescription != null) {
      yield r'taskDescription';
      yield serializers.serialize(
        object.taskDescription,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.decidedAtUtc != null) {
      yield r'decidedAtUtc';
      yield serializers.serialize(
        object.decidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkOrderLaborEntryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderLaborEntryResponseBuilder result,
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
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'worker':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProductionPersonResponse),
          ) as ProductionPersonResponse?;
          if (valueDes == null) continue;
          result.worker.replace(valueDes);
          break;
        case r'workDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.workDate = valueDes;
          break;
        case r'hours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.hours = valueDes;
          break;
        case r'hourlyRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.hourlyRate = valueDes;
          break;
        case r'costAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.costAmount = valueDes;
          break;
        case r'taskDescription':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taskDescription = valueDes;
          break;
        case r'decidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.decidedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkOrderLaborEntryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderLaborEntryResponseBuilder();
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


