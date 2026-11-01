//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'working_calendar_holiday_response.g.dart';

/// WorkingCalendarHolidayResponse
///
/// Properties:
/// * [id] 
/// * [branchId] 
/// * [holidayDate] 
/// * [name] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class WorkingCalendarHolidayResponse implements Built<WorkingCalendarHolidayResponse, WorkingCalendarHolidayResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'holidayDate')
  Date? get holidayDate;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  WorkingCalendarHolidayResponse._();

  factory WorkingCalendarHolidayResponse([void updates(WorkingCalendarHolidayResponseBuilder b)]) = _$WorkingCalendarHolidayResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkingCalendarHolidayResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkingCalendarHolidayResponse> get serializer => _$WorkingCalendarHolidayResponseSerializer();
}

class _$WorkingCalendarHolidayResponseSerializer implements PrimitiveSerializer<WorkingCalendarHolidayResponse> {
  @override
  final Iterable<Type> types = const [WorkingCalendarHolidayResponse, _$WorkingCalendarHolidayResponse];

  @override
  final String wireName = r'WorkingCalendarHolidayResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkingCalendarHolidayResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType.nullable(String),
      );
    }
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
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkingCalendarHolidayResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkingCalendarHolidayResponseBuilder result,
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
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
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
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkingCalendarHolidayResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkingCalendarHolidayResponseBuilder();
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


