//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/estimate_history_event_response.dart';
import 'package:tan_erp_api/src/model/estimate_revision_summary_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_history_response.g.dart';

/// EstimateHistoryResponse
///
/// Properties:
/// * [revisions] 
/// * [events] 
@BuiltValue()
abstract class EstimateHistoryResponse implements Built<EstimateHistoryResponse, EstimateHistoryResponseBuilder> {
  @BuiltValueField(wireName: r'revisions')
  BuiltList<EstimateRevisionSummaryResponse> get revisions;

  @BuiltValueField(wireName: r'events')
  BuiltList<EstimateHistoryEventResponse> get events;

  EstimateHistoryResponse._();

  factory EstimateHistoryResponse([void updates(EstimateHistoryResponseBuilder b)]) = _$EstimateHistoryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateHistoryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateHistoryResponse> get serializer => _$EstimateHistoryResponseSerializer();
}

class _$EstimateHistoryResponseSerializer implements PrimitiveSerializer<EstimateHistoryResponse> {
  @override
  final Iterable<Type> types = const [EstimateHistoryResponse, _$EstimateHistoryResponse];

  @override
  final String wireName = r'EstimateHistoryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateHistoryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'revisions';
    yield serializers.serialize(
      object.revisions,
      specifiedType: const FullType(BuiltList, [FullType(EstimateRevisionSummaryResponse)]),
    );
    yield r'events';
    yield serializers.serialize(
      object.events,
      specifiedType: const FullType(BuiltList, [FullType(EstimateHistoryEventResponse)]),
    );
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateHistoryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateHistoryResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'revisions':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(BuiltList, [FullType(EstimateRevisionSummaryResponse)]),
          ) as BuiltList<EstimateRevisionSummaryResponse>;
          result.revisions.replace(valueDes);
          break;
        case r'events':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(BuiltList, [FullType(EstimateHistoryEventResponse)]),
          ) as BuiltList<EstimateHistoryEventResponse>;
          result.events.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateHistoryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateHistoryResponseBuilder();
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


