//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/project_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'project_status_history_response.g.dart';

/// ProjectStatusHistoryResponse
///
/// Properties:
/// * [id] 
/// * [fromStatus] 
/// * [toStatus] 
/// * [reason] 
/// * [actor] 
/// * [occurredAtUtc] 
@BuiltValue()
abstract class ProjectStatusHistoryResponse implements Built<ProjectStatusHistoryResponse, ProjectStatusHistoryResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'fromStatus')
  String? get fromStatus;

  @BuiltValueField(wireName: r'toStatus')
  String? get toStatus;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'actor')
  ProjectPersonResponse? get actor;

  @BuiltValueField(wireName: r'occurredAtUtc')
  DateTime? get occurredAtUtc;

  ProjectStatusHistoryResponse._();

  factory ProjectStatusHistoryResponse([void updates(ProjectStatusHistoryResponseBuilder b)]) = _$ProjectStatusHistoryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ProjectStatusHistoryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ProjectStatusHistoryResponse> get serializer => _$ProjectStatusHistoryResponseSerializer();
}

class _$ProjectStatusHistoryResponseSerializer implements PrimitiveSerializer<ProjectStatusHistoryResponse> {
  @override
  final Iterable<Type> types = const [ProjectStatusHistoryResponse, _$ProjectStatusHistoryResponse];

  @override
  final String wireName = r'ProjectStatusHistoryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ProjectStatusHistoryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.fromStatus != null) {
      yield r'fromStatus';
      yield serializers.serialize(
        object.fromStatus,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.toStatus != null) {
      yield r'toStatus';
      yield serializers.serialize(
        object.toStatus,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.actor != null) {
      yield r'actor';
      yield serializers.serialize(
        object.actor,
        specifiedType: const FullType(ProjectPersonResponse),
      );
    }
    if (object.occurredAtUtc != null) {
      yield r'occurredAtUtc';
      yield serializers.serialize(
        object.occurredAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ProjectStatusHistoryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ProjectStatusHistoryResponseBuilder result,
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
        case r'fromStatus':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fromStatus = valueDes;
          break;
        case r'toStatus':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.toStatus = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'actor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProjectPersonResponse),
          ) as ProjectPersonResponse?;
          if (valueDes == null) continue;
          result.actor.replace(valueDes);
          break;
        case r'occurredAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.occurredAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ProjectStatusHistoryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ProjectStatusHistoryResponseBuilder();
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


