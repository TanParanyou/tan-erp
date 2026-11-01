//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_project_timesheet_request.g.dart';

/// CreateProjectTimesheetRequest
///
/// Properties:
/// * [workerUserId] 
/// * [workDate] 
/// * [hours] 
/// * [wbsNodeId] 
/// * [workOrderId] 
/// * [taskDescription] 
@BuiltValue()
abstract class CreateProjectTimesheetRequest implements Built<CreateProjectTimesheetRequest, CreateProjectTimesheetRequestBuilder> {
  @BuiltValueField(wireName: r'workerUserId')
  String? get workerUserId;

  @BuiltValueField(wireName: r'workDate')
  Date? get workDate;

  @BuiltValueField(wireName: r'hours')
  double? get hours;

  @BuiltValueField(wireName: r'wbsNodeId')
  String? get wbsNodeId;

  @BuiltValueField(wireName: r'workOrderId')
  String? get workOrderId;

  @BuiltValueField(wireName: r'taskDescription')
  String? get taskDescription;

  CreateProjectTimesheetRequest._();

  factory CreateProjectTimesheetRequest([void updates(CreateProjectTimesheetRequestBuilder b)]) = _$CreateProjectTimesheetRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateProjectTimesheetRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateProjectTimesheetRequest> get serializer => _$CreateProjectTimesheetRequestSerializer();
}

class _$CreateProjectTimesheetRequestSerializer implements PrimitiveSerializer<CreateProjectTimesheetRequest> {
  @override
  final Iterable<Type> types = const [CreateProjectTimesheetRequest, _$CreateProjectTimesheetRequest];

  @override
  final String wireName = r'CreateProjectTimesheetRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateProjectTimesheetRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.workerUserId != null) {
      yield r'workerUserId';
      yield serializers.serialize(
        object.workerUserId,
        specifiedType: const FullType(String),
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
    if (object.wbsNodeId != null) {
      yield r'wbsNodeId';
      yield serializers.serialize(
        object.wbsNodeId,
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
    if (object.taskDescription != null) {
      yield r'taskDescription';
      yield serializers.serialize(
        object.taskDescription,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreateProjectTimesheetRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateProjectTimesheetRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'workerUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workerUserId = valueDes;
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
        case r'wbsNodeId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.wbsNodeId = valueDes;
          break;
        case r'workOrderId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workOrderId = valueDes;
          break;
        case r'taskDescription':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taskDescription = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreateProjectTimesheetRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateProjectTimesheetRequestBuilder();
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


