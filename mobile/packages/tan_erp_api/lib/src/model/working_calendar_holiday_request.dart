//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'working_calendar_holiday_request.g.dart';

/// WorkingCalendarHolidayRequest
///
/// Properties:
/// * [holidayDate] 
/// * [name] 
/// * [branchId] 
@BuiltValue()
abstract class WorkingCalendarHolidayRequest implements Built<WorkingCalendarHolidayRequest, WorkingCalendarHolidayRequestBuilder> {
  @BuiltValueField(wireName: r'holidayDate')
  Date? get holidayDate;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  WorkingCalendarHolidayRequest._();

  factory WorkingCalendarHolidayRequest([void updates(WorkingCalendarHolidayRequestBuilder b)]) = _$WorkingCalendarHolidayRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkingCalendarHolidayRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkingCalendarHolidayRequest> get serializer => _$WorkingCalendarHolidayRequestSerializer();
}

class _$WorkingCalendarHolidayRequestSerializer implements PrimitiveSerializer<WorkingCalendarHolidayRequest> {
  @override
  final Iterable<Type> types = const [WorkingCalendarHolidayRequest, _$WorkingCalendarHolidayRequest];

  @override
  final String wireName = r'WorkingCalendarHolidayRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkingCalendarHolidayRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.holidayDate != null) {
      yield r'holidayDate';
      yield serializers.serialize(
        object.holidayDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkingCalendarHolidayRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkingCalendarHolidayRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'holidayDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.holidayDate = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkingCalendarHolidayRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkingCalendarHolidayRequestBuilder();
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


