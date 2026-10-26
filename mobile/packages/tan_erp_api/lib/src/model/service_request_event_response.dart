//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/service_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'service_request_event_response.g.dart';

/// ServiceRequestEventResponse
///
/// Properties:
/// * [fromStatus] 
/// * [toStatus] 
/// * [note] 
/// * [actor] 
/// * [occurredAtUtc] 
@BuiltValue()
abstract class ServiceRequestEventResponse implements Built<ServiceRequestEventResponse, ServiceRequestEventResponseBuilder> {
  @BuiltValueField(wireName: r'fromStatus')
  String? get fromStatus;

  @BuiltValueField(wireName: r'toStatus')
  String? get toStatus;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'actor')
  ServicePersonResponse? get actor;

  @BuiltValueField(wireName: r'occurredAtUtc')
  DateTime? get occurredAtUtc;

  ServiceRequestEventResponse._();

  factory ServiceRequestEventResponse([void updates(ServiceRequestEventResponseBuilder b)]) = _$ServiceRequestEventResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ServiceRequestEventResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ServiceRequestEventResponse> get serializer => _$ServiceRequestEventResponseSerializer();
}

class _$ServiceRequestEventResponseSerializer implements PrimitiveSerializer<ServiceRequestEventResponse> {
  @override
  final Iterable<Type> types = const [ServiceRequestEventResponse, _$ServiceRequestEventResponse];

  @override
  final String wireName = r'ServiceRequestEventResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ServiceRequestEventResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.actor != null) {
      yield r'actor';
      yield serializers.serialize(
        object.actor,
        specifiedType: const FullType(ServicePersonResponse),
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
    ServiceRequestEventResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ServiceRequestEventResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'actor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ServicePersonResponse),
          ) as ServicePersonResponse?;
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
  ServiceRequestEventResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ServiceRequestEventResponseBuilder();
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


