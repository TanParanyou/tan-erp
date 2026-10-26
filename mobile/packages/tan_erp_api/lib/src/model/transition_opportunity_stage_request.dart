//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'transition_opportunity_stage_request.g.dart';

/// TransitionOpportunityStageRequest
///
/// Properties:
/// * [targetStage] 
/// * [expectedVersion] 
/// * [reasonCode] 
/// * [note] 
@BuiltValue()
abstract class TransitionOpportunityStageRequest implements Built<TransitionOpportunityStageRequest, TransitionOpportunityStageRequestBuilder> {
  @BuiltValueField(wireName: r'targetStage')
  String? get targetStage;

  @BuiltValueField(wireName: r'expectedVersion')
  String? get expectedVersion;

  @BuiltValueField(wireName: r'reasonCode')
  String? get reasonCode;

  @BuiltValueField(wireName: r'note')
  String? get note;

  TransitionOpportunityStageRequest._();

  factory TransitionOpportunityStageRequest([void updates(TransitionOpportunityStageRequestBuilder b)]) = _$TransitionOpportunityStageRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(TransitionOpportunityStageRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<TransitionOpportunityStageRequest> get serializer => _$TransitionOpportunityStageRequestSerializer();
}

class _$TransitionOpportunityStageRequestSerializer implements PrimitiveSerializer<TransitionOpportunityStageRequest> {
  @override
  final Iterable<Type> types = const [TransitionOpportunityStageRequest, _$TransitionOpportunityStageRequest];

  @override
  final String wireName = r'TransitionOpportunityStageRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    TransitionOpportunityStageRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.targetStage != null) {
      yield r'targetStage';
      yield serializers.serialize(
        object.targetStage,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.expectedVersion != null) {
      yield r'expectedVersion';
      yield serializers.serialize(
        object.expectedVersion,
        specifiedType: const FullType(String),
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
  }

  @override
  Object serialize(
    Serializers serializers,
    TransitionOpportunityStageRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required TransitionOpportunityStageRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'targetStage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.targetStage = valueDes;
          break;
        case r'expectedVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedVersion = valueDes;
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  TransitionOpportunityStageRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = TransitionOpportunityStageRequestBuilder();
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


