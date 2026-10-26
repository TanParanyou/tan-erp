//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_change_order_action_request.g.dart';

/// ProjectChangeOrderActionRequest
///
/// Properties:
/// * [expectedVersion] 
/// * [note] 
@BuiltValue()
abstract class ProjectChangeOrderActionRequest implements Built<ProjectChangeOrderActionRequest, ProjectChangeOrderActionRequestBuilder> {
  @BuiltValueField(wireName: r'expectedVersion')
  String? get expectedVersion;

  @BuiltValueField(wireName: r'note')
  String? get note;

  ProjectChangeOrderActionRequest._();

  factory ProjectChangeOrderActionRequest([void updates(ProjectChangeOrderActionRequestBuilder b)]) = _$ProjectChangeOrderActionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectChangeOrderActionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectChangeOrderActionRequest> get serializer => _$ProjectChangeOrderActionRequestSerializer();
}

class _$ProjectChangeOrderActionRequestSerializer implements PrimitiveSerializer<ProjectChangeOrderActionRequest> {
  @override
  final Iterable<Type> types = const [ProjectChangeOrderActionRequest, _$ProjectChangeOrderActionRequest];

  @override
  final String wireName = r'ProjectChangeOrderActionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectChangeOrderActionRequest object, {
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
    ProjectChangeOrderActionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectChangeOrderActionRequestBuilder result,
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
  ProjectChangeOrderActionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectChangeOrderActionRequestBuilder();
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


