//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_progress_response.g.dart';

/// ProjectProgressResponse
///
/// Properties:
/// * [totalMilestones] 
/// * [completedMilestones] 
/// * [totalWeight] 
/// * [completedWeight] 
/// * [percent] 
@BuiltValue()
abstract class ProjectProgressResponse implements Built<ProjectProgressResponse, ProjectProgressResponseBuilder> {
  @BuiltValueField(wireName: r'totalMilestones')
  int? get totalMilestones;

  @BuiltValueField(wireName: r'completedMilestones')
  int? get completedMilestones;

  @BuiltValueField(wireName: r'totalWeight')
  int? get totalWeight;

  @BuiltValueField(wireName: r'completedWeight')
  int? get completedWeight;

  @BuiltValueField(wireName: r'percent')
  double? get percent;

  ProjectProgressResponse._();

  factory ProjectProgressResponse([void updates(ProjectProgressResponseBuilder b)]) = _$ProjectProgressResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectProgressResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectProgressResponse> get serializer => _$ProjectProgressResponseSerializer();
}

class _$ProjectProgressResponseSerializer implements PrimitiveSerializer<ProjectProgressResponse> {
  @override
  final Iterable<Type> types = const [ProjectProgressResponse, _$ProjectProgressResponse];

  @override
  final String wireName = r'ProjectProgressResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectProgressResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.totalMilestones != null) {
      yield r'totalMilestones';
      yield serializers.serialize(
        object.totalMilestones,
        specifiedType: const FullType(int),
      );
    }
    if (object.completedMilestones != null) {
      yield r'completedMilestones';
      yield serializers.serialize(
        object.completedMilestones,
        specifiedType: const FullType(int),
      );
    }
    if (object.totalWeight != null) {
      yield r'totalWeight';
      yield serializers.serialize(
        object.totalWeight,
        specifiedType: const FullType(int),
      );
    }
    if (object.completedWeight != null) {
      yield r'completedWeight';
      yield serializers.serialize(
        object.completedWeight,
        specifiedType: const FullType(int),
      );
    }
    if (object.percent != null) {
      yield r'percent';
      yield serializers.serialize(
        object.percent,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectProgressResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectProgressResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'totalMilestones':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.totalMilestones = valueDes;
          break;
        case r'completedMilestones':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.completedMilestones = valueDes;
          break;
        case r'totalWeight':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.totalWeight = valueDes;
          break;
        case r'completedWeight':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.completedWeight = valueDes;
          break;
        case r'percent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.percent = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectProgressResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectProgressResponseBuilder();
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


