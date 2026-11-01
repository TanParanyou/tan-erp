//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_timesheet_action_request.g.dart';

/// ProjectTimesheetActionRequest
///
/// Properties:
/// * [expectedVersion] 
/// * [note] 
@BuiltValue()
abstract class ProjectTimesheetActionRequest implements Built<ProjectTimesheetActionRequest, ProjectTimesheetActionRequestBuilder> {
  @BuiltValueField(wireName: r'expectedVersion')
  String? get expectedVersion;

  @BuiltValueField(wireName: r'note')
  String? get note;

  ProjectTimesheetActionRequest._();

  factory ProjectTimesheetActionRequest([void updates(ProjectTimesheetActionRequestBuilder b)]) = _$ProjectTimesheetActionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectTimesheetActionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectTimesheetActionRequest> get serializer => _$ProjectTimesheetActionRequestSerializer();
}

class _$ProjectTimesheetActionRequestSerializer implements PrimitiveSerializer<ProjectTimesheetActionRequest> {
  @override
  final Iterable<Type> types = const [ProjectTimesheetActionRequest, _$ProjectTimesheetActionRequest];

  @override
  final String wireName = r'ProjectTimesheetActionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectTimesheetActionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.expectedVersion != null) {
      yield r'expectedVersion';
      yield serializers.serialize(
        object.expectedVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectTimesheetActionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectTimesheetActionRequestBuilder result,
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
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectTimesheetActionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectTimesheetActionRequestBuilder();
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


