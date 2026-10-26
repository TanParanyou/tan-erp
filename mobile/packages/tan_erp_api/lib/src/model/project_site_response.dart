//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_site_response.g.dart';

/// ProjectSiteResponse
///
/// Properties:
/// * [id] 
/// * [label] 
@BuiltValue()
abstract class ProjectSiteResponse implements Built<ProjectSiteResponse, ProjectSiteResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'label')
  String? get label;

  ProjectSiteResponse._();

  factory ProjectSiteResponse([void updates(ProjectSiteResponseBuilder b)]) = _$ProjectSiteResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectSiteResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectSiteResponse> get serializer => _$ProjectSiteResponseSerializer();
}

class _$ProjectSiteResponseSerializer implements PrimitiveSerializer<ProjectSiteResponse> {
  @override
  final Iterable<Type> types = const [ProjectSiteResponse, _$ProjectSiteResponse];

  @override
  final String wireName = r'ProjectSiteResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectSiteResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.label != null) {
      yield r'label';
      yield serializers.serialize(
        object.label,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectSiteResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectSiteResponseBuilder result,
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
        case r'label':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.label = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectSiteResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectSiteResponseBuilder();
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


