//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'schedule_request.g.dart';

/// ScheduleRequest
///
/// Properties:
/// * [date] 
@BuiltValue()
abstract class ScheduleRequest implements Built<ScheduleRequest, ScheduleRequestBuilder> {
  @BuiltValueField(wireName: r'date')
  Date? get date;

  ScheduleRequest._();

  factory ScheduleRequest([void updates(ScheduleRequestBuilder b)]) = _$ScheduleRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ScheduleRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ScheduleRequest> get serializer => _$ScheduleRequestSerializer();
}

class _$ScheduleRequestSerializer implements PrimitiveSerializer<ScheduleRequest> {
  @override
  final Iterable<Type> types = const [ScheduleRequest, _$ScheduleRequest];

  @override
  final String wireName = r'ScheduleRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ScheduleRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.date != null) {
      yield r'date';
      yield serializers.serialize(
        object.date,
        specifiedType: const FullType(Date),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ScheduleRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ScheduleRequestBuilder result,
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ScheduleRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ScheduleRequestBuilder();
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


