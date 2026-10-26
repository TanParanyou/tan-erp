//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/admin_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'audit_event_response.g.dart';

/// AuditEventResponse
///
/// Properties:
/// * [id] 
/// * [occurredAtUtc] 
/// * [action] 
/// * [resourceType] 
/// * [resourceId] 
/// * [actor] 
/// * [branchId] 
/// * [traceId] 
/// * [requestId] 
/// * [reason] 
/// * [changesJson] 
@BuiltValue()
abstract class AuditEventResponse implements Built<AuditEventResponse, AuditEventResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'occurredAtUtc')
  DateTime? get occurredAtUtc;

  @BuiltValueField(wireName: r'action')
  String? get action;

  @BuiltValueField(wireName: r'resourceType')
  String? get resourceType;

  @BuiltValueField(wireName: r'resourceId')
  String? get resourceId;

  @BuiltValueField(wireName: r'actor')
  AdminRefResponse? get actor;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'traceId')
  String? get traceId;

  @BuiltValueField(wireName: r'requestId')
  String? get requestId;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'changesJson')
  String? get changesJson;

  AuditEventResponse._();

  factory AuditEventResponse([void updates(AuditEventResponseBuilder b)]) = _$AuditEventResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AuditEventResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AuditEventResponse> get serializer => _$AuditEventResponseSerializer();
}

class _$AuditEventResponseSerializer implements PrimitiveSerializer<AuditEventResponse> {
  @override
  final Iterable<Type> types = const [AuditEventResponse, _$AuditEventResponse];

  @override
  final String wireName = r'AuditEventResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AuditEventResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.occurredAtUtc != null) {
      yield r'occurredAtUtc';
      yield serializers.serialize(
        object.occurredAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.action != null) {
      yield r'action';
      yield serializers.serialize(
        object.action,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.resourceType != null) {
      yield r'resourceType';
      yield serializers.serialize(
        object.resourceType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.resourceId != null) {
      yield r'resourceId';
      yield serializers.serialize(
        object.resourceId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.actor != null) {
      yield r'actor';
      yield serializers.serialize(
        object.actor,
        specifiedType: const FullType(AdminRefResponse),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.traceId != null) {
      yield r'traceId';
      yield serializers.serialize(
        object.traceId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.requestId != null) {
      yield r'requestId';
      yield serializers.serialize(
        object.requestId,
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
    if (object.changesJson != null) {
      yield r'changesJson';
      yield serializers.serialize(
        object.changesJson,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AuditEventResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AuditEventResponseBuilder result,
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
        case r'occurredAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.occurredAtUtc = valueDes;
          break;
        case r'action':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.action = valueDes;
          break;
        case r'resourceType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.resourceType = valueDes;
          break;
        case r'resourceId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.resourceId = valueDes;
          break;
        case r'actor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AdminRefResponse),
          ) as AdminRefResponse?;
          if (valueDes == null) continue;
          result.actor.replace(valueDes);
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'traceId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.traceId = valueDes;
          break;
        case r'requestId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.requestId = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'changesJson':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.changesJson = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AuditEventResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AuditEventResponseBuilder();
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


