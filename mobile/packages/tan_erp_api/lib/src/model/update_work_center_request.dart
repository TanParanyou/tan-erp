//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_work_center_request.g.dart';

/// UpdateWorkCenterRequest
///
/// Properties:
/// * [name] 
/// * [dailyCapacityHours] 
/// * [hourlyRate] 
@BuiltValue()
abstract class UpdateWorkCenterRequest implements Built<UpdateWorkCenterRequest, UpdateWorkCenterRequestBuilder> {
  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'dailyCapacityHours')
  double? get dailyCapacityHours;

  @BuiltValueField(wireName: r'hourlyRate')
  double? get hourlyRate;

  UpdateWorkCenterRequest._();

  factory UpdateWorkCenterRequest([void updates(UpdateWorkCenterRequestBuilder b)]) = _$UpdateWorkCenterRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateWorkCenterRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateWorkCenterRequest> get serializer => _$UpdateWorkCenterRequestSerializer();
}

class _$UpdateWorkCenterRequestSerializer implements PrimitiveSerializer<UpdateWorkCenterRequest> {
  @override
  final Iterable<Type> types = const [UpdateWorkCenterRequest, _$UpdateWorkCenterRequest];

  @override
  final String wireName = r'UpdateWorkCenterRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateWorkCenterRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateWorkCenterRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateWorkCenterRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateWorkCenterRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateWorkCenterRequestBuilder();
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


