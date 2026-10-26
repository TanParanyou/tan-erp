//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'deactivate_branch_request.g.dart';

/// DeactivateBranchRequest
///
/// Properties:
/// * [reason] 
@BuiltValue()
abstract class DeactivateBranchRequest implements Built<DeactivateBranchRequest, DeactivateBranchRequestBuilder> {
  @BuiltValueField(wireName: r'reason')
  String? get reason;

  DeactivateBranchRequest._();

  factory DeactivateBranchRequest([void updates(DeactivateBranchRequestBuilder b)]) = _$DeactivateBranchRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(DeactivateBranchRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<DeactivateBranchRequest> get serializer => _$DeactivateBranchRequestSerializer();
}

class _$DeactivateBranchRequestSerializer implements PrimitiveSerializer<DeactivateBranchRequest> {
  @override
  final Iterable<Type> types = const [DeactivateBranchRequest, _$DeactivateBranchRequest];

  @override
  final String wireName = r'DeactivateBranchRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    DeactivateBranchRequest object, {
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
    DeactivateBranchRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required DeactivateBranchRequestBuilder result,
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
  DeactivateBranchRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = DeactivateBranchRequestBuilder();
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


