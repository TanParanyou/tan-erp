//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'branch_rate_override_revoke_request.g.dart';

/// BranchRateOverrideRevokeRequest
///
/// Properties:
/// * [reason] 
@BuiltValue()
abstract class BranchRateOverrideRevokeRequest implements Built<BranchRateOverrideRevokeRequest, BranchRateOverrideRevokeRequestBuilder> {
  @BuiltValueField(wireName: r'reason')
  String? get reason;

  BranchRateOverrideRevokeRequest._();

  factory BranchRateOverrideRevokeRequest([void updates(BranchRateOverrideRevokeRequestBuilder b)]) = _$BranchRateOverrideRevokeRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BranchRateOverrideRevokeRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BranchRateOverrideRevokeRequest> get serializer => _$BranchRateOverrideRevokeRequestSerializer();
}

class _$BranchRateOverrideRevokeRequestSerializer implements PrimitiveSerializer<BranchRateOverrideRevokeRequest> {
  @override
  final Iterable<Type> types = const [BranchRateOverrideRevokeRequest, _$BranchRateOverrideRevokeRequest];

  @override
  final String wireName = r'BranchRateOverrideRevokeRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BranchRateOverrideRevokeRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BranchRateOverrideRevokeRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BranchRateOverrideRevokeRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BranchRateOverrideRevokeRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BranchRateOverrideRevokeRequestBuilder();
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


