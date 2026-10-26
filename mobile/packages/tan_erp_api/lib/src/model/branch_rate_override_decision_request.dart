//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'branch_rate_override_decision_request.g.dart';

/// BranchRateOverrideDecisionRequest
///
/// Properties:
/// * [decision] 
/// * [note] 
@BuiltValue()
abstract class BranchRateOverrideDecisionRequest implements Built<BranchRateOverrideDecisionRequest, BranchRateOverrideDecisionRequestBuilder> {
  @BuiltValueField(wireName: r'decision')
  String? get decision;

  @BuiltValueField(wireName: r'note')
  String? get note;

  BranchRateOverrideDecisionRequest._();

  factory BranchRateOverrideDecisionRequest([void updates(BranchRateOverrideDecisionRequestBuilder b)]) = _$BranchRateOverrideDecisionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BranchRateOverrideDecisionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BranchRateOverrideDecisionRequest> get serializer => _$BranchRateOverrideDecisionRequestSerializer();
}

class _$BranchRateOverrideDecisionRequestSerializer implements PrimitiveSerializer<BranchRateOverrideDecisionRequest> {
  @override
  final Iterable<Type> types = const [BranchRateOverrideDecisionRequest, _$BranchRateOverrideDecisionRequest];

  @override
  final String wireName = r'BranchRateOverrideDecisionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BranchRateOverrideDecisionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.decision != null) {
      yield r'decision';
      yield serializers.serialize(
        object.decision,
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
    BranchRateOverrideDecisionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BranchRateOverrideDecisionRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'decision':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.decision = valueDes;
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
  BranchRateOverrideDecisionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BranchRateOverrideDecisionRequestBuilder();
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


