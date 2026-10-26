//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'transition_project_request.g.dart';

/// TransitionProjectRequest
///
/// Properties:
/// * [targetStatus] 
/// * [reason] 
@BuiltValue()
abstract class TransitionProjectRequest implements Built<TransitionProjectRequest, TransitionProjectRequestBuilder> {
  @BuiltValueField(wireName: r'targetStatus')
  String? get targetStatus;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  TransitionProjectRequest._();

  factory TransitionProjectRequest([void updates(TransitionProjectRequestBuilder b)]) = _$TransitionProjectRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(TransitionProjectRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<TransitionProjectRequest> get serializer => _$TransitionProjectRequestSerializer();
}

class _$TransitionProjectRequestSerializer implements PrimitiveSerializer<TransitionProjectRequest> {
  @override
  final Iterable<Type> types = const [TransitionProjectRequest, _$TransitionProjectRequest];

  @override
  final String wireName = r'TransitionProjectRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    TransitionProjectRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.targetStatus != null) {
      yield r'targetStatus';
      yield serializers.serialize(
        object.targetStatus,
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
  }

  @override
  Object serialize(
    Serializers serializers,
    TransitionProjectRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required TransitionProjectRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'targetStatus':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.targetStatus = valueDes;
          break;
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
  TransitionProjectRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = TransitionProjectRequestBuilder();
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


