//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'set_project_plan_request.g.dart';

/// SetProjectPlanRequest
///
/// Properties:
/// * [plannedStartDate] 
/// * [plannedEndDate] 
@BuiltValue()
abstract class SetProjectPlanRequest implements Built<SetProjectPlanRequest, SetProjectPlanRequestBuilder> {
  @BuiltValueField(wireName: r'plannedStartDate')
  Date? get plannedStartDate;

  @BuiltValueField(wireName: r'plannedEndDate')
  Date? get plannedEndDate;

  SetProjectPlanRequest._();

  factory SetProjectPlanRequest([void updates(SetProjectPlanRequestBuilder b)]) = _$SetProjectPlanRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SetProjectPlanRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SetProjectPlanRequest> get serializer => _$SetProjectPlanRequestSerializer();
}

class _$SetProjectPlanRequestSerializer implements PrimitiveSerializer<SetProjectPlanRequest> {
  @override
  final Iterable<Type> types = const [SetProjectPlanRequest, _$SetProjectPlanRequest];

  @override
  final String wireName = r'SetProjectPlanRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SetProjectPlanRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.plannedStartDate != null) {
      yield r'plannedStartDate';
      yield serializers.serialize(
        object.plannedStartDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.plannedEndDate != null) {
      yield r'plannedEndDate';
      yield serializers.serialize(
        object.plannedEndDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SetProjectPlanRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SetProjectPlanRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'plannedStartDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.plannedStartDate = valueDes;
          break;
        case r'plannedEndDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.plannedEndDate = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SetProjectPlanRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SetProjectPlanRequestBuilder();
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


