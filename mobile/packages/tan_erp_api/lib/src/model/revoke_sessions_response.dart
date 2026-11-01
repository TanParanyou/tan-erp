//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'revoke_sessions_response.g.dart';

/// RevokeSessionsResponse
///
/// Properties:
/// * [revokedCount] 
@BuiltValue()
abstract class RevokeSessionsResponse implements Built<RevokeSessionsResponse, RevokeSessionsResponseBuilder> {
  @BuiltValueField(wireName: r'revokedCount')
  int? get revokedCount;

  RevokeSessionsResponse._();

  factory RevokeSessionsResponse([void updates(RevokeSessionsResponseBuilder b)]) = _$RevokeSessionsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RevokeSessionsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RevokeSessionsResponse> get serializer => _$RevokeSessionsResponseSerializer();
}

class _$RevokeSessionsResponseSerializer implements PrimitiveSerializer<RevokeSessionsResponse> {
  @override
  final Iterable<Type> types = const [RevokeSessionsResponse, _$RevokeSessionsResponse];

  @override
  final String wireName = r'RevokeSessionsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RevokeSessionsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.revokedCount != null) {
      yield r'revokedCount';
      yield serializers.serialize(
        object.revokedCount,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RevokeSessionsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RevokeSessionsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'revokedCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.revokedCount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  RevokeSessionsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RevokeSessionsResponseBuilder();
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


