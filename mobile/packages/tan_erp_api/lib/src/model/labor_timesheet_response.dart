//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_person_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'labor_timesheet_response.g.dart';

/// LaborTimesheetResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [worker] 
/// * [workDate] 
/// * [hours] 
/// * [hourlyRate] 
/// * [costAmount] 
/// * [wbsNodeId] 
/// * [wbsCode] 
/// * [workOrderId] 
/// * [workOrderNumber] 
/// * [taskDescription] 
/// * [status] 
/// * [isOvertimeWarning] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [submittedAtUtc] 
/// * [decidedBy] 
/// * [decidedAtUtc] 
/// * [decisionNote] 
/// * [rowVersion] 
@BuiltValue()
abstract class LaborTimesheetResponse implements Built<LaborTimesheetResponse, LaborTimesheetResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'worker')
  ProjectPersonResponse? get worker;

  @BuiltValueField(wireName: r'workDate')
  Date? get workDate;

  @BuiltValueField(wireName: r'hours')
  double? get hours;

  @BuiltValueField(wireName: r'hourlyRate')
  double? get hourlyRate;

  @BuiltValueField(wireName: r'costAmount')
  double? get costAmount;

  @BuiltValueField(wireName: r'wbsNodeId')
  String? get wbsNodeId;

  @BuiltValueField(wireName: r'wbsCode')
  String? get wbsCode;

  @BuiltValueField(wireName: r'workOrderId')
  String? get workOrderId;

  @BuiltValueField(wireName: r'workOrderNumber')
  String? get workOrderNumber;

  @BuiltValueField(wireName: r'taskDescription')
  String? get taskDescription;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'isOvertimeWarning')
  bool? get isOvertimeWarning;

  @BuiltValueField(wireName: r'createdBy')
  ProjectPersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'submittedAtUtc')
  DateTime? get submittedAtUtc;

  @BuiltValueField(wireName: r'decidedBy')
  ProjectPersonResponse? get decidedBy;

  @BuiltValueField(wireName: r'decidedAtUtc')
  DateTime? get decidedAtUtc;

  @BuiltValueField(wireName: r'decisionNote')
  String? get decisionNote;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  LaborTimesheetResponse._();

  factory LaborTimesheetResponse([void updates(LaborTimesheetResponseBuilder b)]) = _$LaborTimesheetResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(LaborTimesheetResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<LaborTimesheetResponse> get serializer => _$LaborTimesheetResponseSerializer();
}

class _$LaborTimesheetResponseSerializer implements PrimitiveSerializer<LaborTimesheetResponse> {
  @override
  final Iterable<Type> types = const [LaborTimesheetResponse, _$LaborTimesheetResponse];

  @override
  final String wireName = r'LaborTimesheetResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    LaborTimesheetResponse object, {
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
        specifiedType: const FullType(ProjectPersonResponse),
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
    if (object.wbsNodeId != null) {
      yield r'wbsNodeId';
      yield serializers.serialize(
        object.wbsNodeId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.wbsCode != null) {
      yield r'wbsCode';
      yield serializers.serialize(
        object.wbsCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.workOrderId != null) {
      yield r'workOrderId';
      yield serializers.serialize(
        object.workOrderId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.workOrderNumber != null) {
      yield r'workOrderNumber';
      yield serializers.serialize(
        object.workOrderNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taskDescription != null) {
      yield r'taskDescription';
      yield serializers.serialize(
        object.taskDescription,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isOvertimeWarning != null) {
      yield r'isOvertimeWarning';
      yield serializers.serialize(
        object.isOvertimeWarning,
        specifiedType: const FullType(bool),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(ProjectPersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.submittedAtUtc != null) {
      yield r'submittedAtUtc';
      yield serializers.serialize(
        object.submittedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.decidedBy != null) {
      yield r'decidedBy';
      yield serializers.serialize(
        object.decidedBy,
        specifiedType: const FullType(ProjectPersonResponse),
      );
    }
    if (object.decidedAtUtc != null) {
      yield r'decidedAtUtc';
      yield serializers.serialize(
        object.decidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.decisionNote != null) {
      yield r'decisionNote';
      yield serializers.serialize(
        object.decisionNote,
        specifiedType: const FullType.nullable(String),
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
    LaborTimesheetResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required LaborTimesheetResponseBuilder result,
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
            specifiedType: const FullType.nullable(ProjectPersonResponse),
          ) as ProjectPersonResponse?;
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
        case r'wbsNodeId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.wbsNodeId = valueDes;
          break;
        case r'wbsCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.wbsCode = valueDes;
          break;
        case r'workOrderId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workOrderId = valueDes;
          break;
        case r'workOrderNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workOrderNumber = valueDes;
          break;
        case r'taskDescription':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taskDescription = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'isOvertimeWarning':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isOvertimeWarning = valueDes;
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectPersonResponse),
          ) as ProjectPersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'submittedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.submittedAtUtc = valueDes;
          break;
        case r'decidedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectPersonResponse),
          ) as ProjectPersonResponse?;
          if (valueDes == null) continue;
          result.decidedBy.replace(valueDes);
          break;
        case r'decidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.decidedAtUtc = valueDes;
          break;
        case r'decisionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.decisionNote = valueDes;
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
  LaborTimesheetResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = LaborTimesheetResponseBuilder();
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


