//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/actor_summary_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'opportunity_stage_history_item_response.g.dart';

/// OpportunityStageHistoryItemResponse
///
/// Properties:
/// * [id] 
/// * [opportunityId] 
/// * [fromStage] 
/// * [toStage] 
/// * [reasonCode] 
/// * [note] 
/// * [actorUserId] 
/// * [occurredAtUtc] 
/// * [policyVersion] 
/// * [actor] 
@BuiltValue()
abstract class OpportunityStageHistoryItemResponse implements Built<OpportunityStageHistoryItemResponse, OpportunityStageHistoryItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'opportunityId')
  String? get opportunityId;

  @BuiltValueField(wireName: r'fromStage')
  String? get fromStage;

  @BuiltValueField(wireName: r'toStage')
  String? get toStage;

  @BuiltValueField(wireName: r'reasonCode')
  String? get reasonCode;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'actorUserId')
  String? get actorUserId;

  @BuiltValueField(wireName: r'occurredAtUtc')
  DateTime? get occurredAtUtc;

  @BuiltValueField(wireName: r'policyVersion')
  String? get policyVersion;

  @BuiltValueField(wireName: r'actor')
  ActorSummaryResponse? get actor;

  OpportunityStageHistoryItemResponse._();

  factory OpportunityStageHistoryItemResponse([void updates(OpportunityStageHistoryItemResponseBuilder b)]) = _$OpportunityStageHistoryItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OpportunityStageHistoryItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OpportunityStageHistoryItemResponse> get serializer => _$OpportunityStageHistoryItemResponseSerializer();
}

class _$OpportunityStageHistoryItemResponseSerializer implements PrimitiveSerializer<OpportunityStageHistoryItemResponse> {
  @override
  final Iterable<Type> types = const [OpportunityStageHistoryItemResponse, _$OpportunityStageHistoryItemResponse];

  @override
  final String wireName = r'OpportunityStageHistoryItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OpportunityStageHistoryItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.opportunityId != null) {
      yield r'opportunityId';
      yield serializers.serialize(
        object.opportunityId,
        specifiedType: const FullType(String),
      );
    }
    if (object.fromStage != null) {
      yield r'fromStage';
      yield serializers.serialize(
        object.fromStage,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.toStage != null) {
      yield r'toStage';
      yield serializers.serialize(
        object.toStage,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reasonCode != null) {
      yield r'reasonCode';
      yield serializers.serialize(
        object.reasonCode,
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
    if (object.actorUserId != null) {
      yield r'actorUserId';
      yield serializers.serialize(
        object.actorUserId,
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
    if (object.policyVersion != null) {
      yield r'policyVersion';
      yield serializers.serialize(
        object.policyVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.actor != null) {
      yield r'actor';
      yield serializers.serialize(
        object.actor,
        specifiedType: const FullType(ActorSummaryResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OpportunityStageHistoryItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OpportunityStageHistoryItemResponseBuilder result,
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
        case r'opportunityId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityId = valueDes;
          break;
        case r'fromStage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fromStage = valueDes;
          break;
        case r'toStage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.toStage = valueDes;
          break;
        case r'reasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reasonCode = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'actorUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.actorUserId = valueDes;
          break;
        case r'occurredAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.occurredAtUtc = valueDes;
          break;
        case r'policyVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.policyVersion = valueDes;
          break;
        case r'actor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ActorSummaryResponse),
          ) as ActorSummaryResponse?;
          if (valueDes == null) continue;
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
  OpportunityStageHistoryItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OpportunityStageHistoryItemResponseBuilder();
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


