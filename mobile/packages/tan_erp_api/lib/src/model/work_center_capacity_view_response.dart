//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/work_center_capacity_row_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_center_capacity_view_response.g.dart';

/// WorkCenterCapacityViewResponse
///
/// Properties:
/// * [fromDate] 
/// * [toDate] 
/// * [workCenters] 
@BuiltValue()
abstract class WorkCenterCapacityViewResponse implements Built<WorkCenterCapacityViewResponse, WorkCenterCapacityViewResponseBuilder> {
  @BuiltValueField(wireName: r'fromDate')
  Date? get fromDate;

  @BuiltValueField(wireName: r'toDate')
  Date? get toDate;

  @BuiltValueField(wireName: r'workCenters')
  BuiltList<WorkCenterCapacityRowResponse>? get workCenters;

  WorkCenterCapacityViewResponse._();

  factory WorkCenterCapacityViewResponse([void updates(WorkCenterCapacityViewResponseBuilder b)]) = _$WorkCenterCapacityViewResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkCenterCapacityViewResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkCenterCapacityViewResponse> get serializer => _$WorkCenterCapacityViewResponseSerializer();
}

class _$WorkCenterCapacityViewResponseSerializer implements PrimitiveSerializer<WorkCenterCapacityViewResponse> {
  @override
  final Iterable<Type> types = const [WorkCenterCapacityViewResponse, _$WorkCenterCapacityViewResponse];

  @override
  final String wireName = r'WorkCenterCapacityViewResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkCenterCapacityViewResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.fromDate != null) {
      yield r'fromDate';
      yield serializers.serialize(
        object.fromDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.toDate != null) {
      yield r'toDate';
      yield serializers.serialize(
        object.toDate,
        specifiedType: const FullType(Date),
      );
    }
    if (object.workCenters != null) {
      yield r'workCenters';
      yield serializers.serialize(
        object.workCenters,
        specifiedType: const FullType.nullable(BuiltList, [FullType(WorkCenterCapacityRowResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkCenterCapacityViewResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkCenterCapacityViewResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'fromDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.fromDate = valueDes;
          break;
        case r'toDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.toDate = valueDes;
          break;
        case r'workCenters':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(WorkCenterCapacityRowResponse)]),
          ) as BuiltList<WorkCenterCapacityRowResponse>?;
          if (valueDes == null) continue;
          result.workCenters.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkCenterCapacityViewResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkCenterCapacityViewResponseBuilder();
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


