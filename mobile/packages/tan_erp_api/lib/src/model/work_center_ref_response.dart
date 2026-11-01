//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_center_ref_response.g.dart';

/// WorkCenterRefResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [name] 
/// * [dailyCapacityHours] 
/// * [hourlyRate] 
@BuiltValue()
abstract class WorkCenterRefResponse implements Built<WorkCenterRefResponse, WorkCenterRefResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'dailyCapacityHours')
  double? get dailyCapacityHours;

  @BuiltValueField(wireName: r'hourlyRate')
  double? get hourlyRate;

  WorkCenterRefResponse._();

  factory WorkCenterRefResponse([void updates(WorkCenterRefResponseBuilder b)]) = _$WorkCenterRefResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkCenterRefResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkCenterRefResponse> get serializer => _$WorkCenterRefResponseSerializer();
}

class _$WorkCenterRefResponseSerializer implements PrimitiveSerializer<WorkCenterRefResponse> {
  @override
  final Iterable<Type> types = const [WorkCenterRefResponse, _$WorkCenterRefResponse];

  @override
  final String wireName = r'WorkCenterRefResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkCenterRefResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
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
    WorkCenterRefResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkCenterRefResponseBuilder result,
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
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
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
  WorkCenterRefResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkCenterRefResponseBuilder();
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


