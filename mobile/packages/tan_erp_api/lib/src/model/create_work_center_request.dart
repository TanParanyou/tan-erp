//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'create_work_center_request.g.dart';

/// CreateWorkCenterRequest
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [dailyCapacityHours] 
/// * [hourlyRate] 
/// * [branchId] 
@BuiltValue()
abstract class CreateWorkCenterRequest implements Built<CreateWorkCenterRequest, CreateWorkCenterRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'dailyCapacityHours')
  double? get dailyCapacityHours;

  @BuiltValueField(wireName: r'hourlyRate')
  double? get hourlyRate;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  CreateWorkCenterRequest._();

  factory CreateWorkCenterRequest([void updates(CreateWorkCenterRequestBuilder b)]) = _$CreateWorkCenterRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreateWorkCenterRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreateWorkCenterRequest> get serializer => _$CreateWorkCenterRequestSerializer();
}

class _$CreateWorkCenterRequestSerializer implements PrimitiveSerializer<CreateWorkCenterRequest> {
  @override
  final Iterable<Type> types = const [CreateWorkCenterRequest, _$CreateWorkCenterRequest];

  @override
  final String wireName = r'CreateWorkCenterRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreateWorkCenterRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.hourlyRate != null) {
      yield r'hourlyRate';
      yield serializers.serialize(
        object.hourlyRate,
        specifiedType: const FullType.nullable(double),
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
    CreateWorkCenterRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreateWorkCenterRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
  CreateWorkCenterRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreateWorkCenterRequestBuilder();
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


