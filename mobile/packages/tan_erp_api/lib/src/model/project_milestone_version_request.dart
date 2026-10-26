//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_milestone_version_request.g.dart';

/// ProjectMilestoneVersionRequest
///
/// Properties:
/// * [expectedVersion] 
@BuiltValue()
abstract class ProjectMilestoneVersionRequest implements Built<ProjectMilestoneVersionRequest, ProjectMilestoneVersionRequestBuilder> {
  @BuiltValueField(wireName: r'expectedVersion')
  String? get expectedVersion;

  ProjectMilestoneVersionRequest._();

  factory ProjectMilestoneVersionRequest([void updates(ProjectMilestoneVersionRequestBuilder b)]) = _$ProjectMilestoneVersionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectMilestoneVersionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectMilestoneVersionRequest> get serializer => _$ProjectMilestoneVersionRequestSerializer();
}

class _$ProjectMilestoneVersionRequestSerializer implements PrimitiveSerializer<ProjectMilestoneVersionRequest> {
  @override
  final Iterable<Type> types = const [ProjectMilestoneVersionRequest, _$ProjectMilestoneVersionRequest];

  @override
  final String wireName = r'ProjectMilestoneVersionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectMilestoneVersionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.expectedVersion != null) {
      yield r'expectedVersion';
      yield serializers.serialize(
        object.expectedVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectMilestoneVersionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectMilestoneVersionRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'expectedVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectMilestoneVersionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectMilestoneVersionRequestBuilder();
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


