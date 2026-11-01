//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/working_calendar_holiday_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'working_calendar_holiday_list_response.g.dart';

/// WorkingCalendarHolidayListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class WorkingCalendarHolidayListResponse implements Built<WorkingCalendarHolidayListResponse, WorkingCalendarHolidayListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<WorkingCalendarHolidayResponse>? get items;

  WorkingCalendarHolidayListResponse._();

  factory WorkingCalendarHolidayListResponse([void updates(WorkingCalendarHolidayListResponseBuilder b)]) = _$WorkingCalendarHolidayListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkingCalendarHolidayListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkingCalendarHolidayListResponse> get serializer => _$WorkingCalendarHolidayListResponseSerializer();
}

class _$WorkingCalendarHolidayListResponseSerializer implements PrimitiveSerializer<WorkingCalendarHolidayListResponse> {
  @override
  final Iterable<Type> types = const [WorkingCalendarHolidayListResponse, _$WorkingCalendarHolidayListResponse];

  @override
  final String wireName = r'WorkingCalendarHolidayListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkingCalendarHolidayListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(WorkingCalendarHolidayResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkingCalendarHolidayListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkingCalendarHolidayListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(WorkingCalendarHolidayResponse)]),
          ) as BuiltList<WorkingCalendarHolidayResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkingCalendarHolidayListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkingCalendarHolidayListResponseBuilder();
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


