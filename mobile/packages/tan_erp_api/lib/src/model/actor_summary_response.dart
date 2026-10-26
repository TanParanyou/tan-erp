//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'actor_summary_response.g.dart';

/// ActorSummaryResponse
///
/// Properties:
/// * [id] 
/// * [displayName] 
@BuiltValue()
abstract class ActorSummaryResponse implements Built<ActorSummaryResponse, ActorSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'displayName')
  String? get displayName;

  ActorSummaryResponse._();

  factory ActorSummaryResponse([void updates(ActorSummaryResponseBuilder b)]) = _$ActorSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ActorSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ActorSummaryResponse> get serializer => _$ActorSummaryResponseSerializer();
}

class _$ActorSummaryResponseSerializer implements PrimitiveSerializer<ActorSummaryResponse> {
  @override
  final Iterable<Type> types = const [ActorSummaryResponse, _$ActorSummaryResponse];

  @override
  final String wireName = r'ActorSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ActorSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.displayName != null) {
      yield r'displayName';
      yield serializers.serialize(
        object.displayName,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ActorSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ActorSummaryResponseBuilder result,
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
        case r'displayName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayName = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ActorSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ActorSummaryResponseBuilder();
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


