//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/estimate_actor_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_history_event_response.g.dart';

/// EstimateHistoryEventResponse
///
/// Properties:
/// * [occurredAtUtc] 
/// * [action] 
/// * [actor] 
@BuiltValue()
abstract class EstimateHistoryEventResponse implements Built<EstimateHistoryEventResponse, EstimateHistoryEventResponseBuilder> {
  @BuiltValueField(wireName: r'occurredAtUtc')
  DateTime get occurredAtUtc;

  @BuiltValueField(wireName: r'action')
  String get action;

  @BuiltValueField(wireName: r'actor')
  EstimateActorResponse get actor;

  EstimateHistoryEventResponse._();

  factory EstimateHistoryEventResponse([void updates(EstimateHistoryEventResponseBuilder b)]) = _$EstimateHistoryEventResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateHistoryEventResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateHistoryEventResponse> get serializer => _$EstimateHistoryEventResponseSerializer();
}

class _$EstimateHistoryEventResponseSerializer implements PrimitiveSerializer<EstimateHistoryEventResponse> {
  @override
  final Iterable<Type> types = const [EstimateHistoryEventResponse, _$EstimateHistoryEventResponse];

  @override
  final String wireName = r'EstimateHistoryEventResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateHistoryEventResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'occurredAtUtc';
    yield serializers.serialize(
      object.occurredAtUtc,
      specifiedType: const FullType(DateTime),
    );
    yield r'action';
    yield serializers.serialize(
      object.action,
      specifiedType: const FullType(String),
    );
    yield r'actor';
    yield serializers.serialize(
      object.actor,
      specifiedType: const FullType(EstimateActorResponse),
    );
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateHistoryEventResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateHistoryEventResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'occurredAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(DateTime),
          ) as DateTime;
          result.occurredAtUtc = valueDes;
          break;
        case r'action':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.action = valueDes;
          break;
        case r'actor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(EstimateActorResponse),
          ) as EstimateActorResponse;
          result.actor.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateHistoryEventResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateHistoryEventResponseBuilder();
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


