//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_wbs_node_version_request.g.dart';

/// ProjectWbsNodeVersionRequest
///
/// Properties:
/// * [expectedVersion] 
@BuiltValue()
abstract class ProjectWbsNodeVersionRequest implements Built<ProjectWbsNodeVersionRequest, ProjectWbsNodeVersionRequestBuilder> {
  @BuiltValueField(wireName: r'expectedVersion')
  String? get expectedVersion;

  ProjectWbsNodeVersionRequest._();

  factory ProjectWbsNodeVersionRequest([void updates(ProjectWbsNodeVersionRequestBuilder b)]) = _$ProjectWbsNodeVersionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectWbsNodeVersionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectWbsNodeVersionRequest> get serializer => _$ProjectWbsNodeVersionRequestSerializer();
}

class _$ProjectWbsNodeVersionRequestSerializer implements PrimitiveSerializer<ProjectWbsNodeVersionRequest> {
  @override
  final Iterable<Type> types = const [ProjectWbsNodeVersionRequest, _$ProjectWbsNodeVersionRequest];

  @override
  final String wireName = r'ProjectWbsNodeVersionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectWbsNodeVersionRequest object, {
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
    ProjectWbsNodeVersionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectWbsNodeVersionRequestBuilder result,
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
  ProjectWbsNodeVersionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectWbsNodeVersionRequestBuilder();
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


