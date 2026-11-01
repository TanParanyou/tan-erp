//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'refresh_session_request.g.dart';

/// RefreshSessionRequest
///
/// Properties:
/// * [refreshToken] 
/// * [clientPlatform] 
@BuiltValue()
abstract class RefreshSessionRequest implements Built<RefreshSessionRequest, RefreshSessionRequestBuilder> {
  @BuiltValueField(wireName: r'refreshToken')
  String? get refreshToken;

  @BuiltValueField(wireName: r'clientPlatform')
  String? get clientPlatform;

  RefreshSessionRequest._();

  factory RefreshSessionRequest([void updates(RefreshSessionRequestBuilder b)]) = _$RefreshSessionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RefreshSessionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RefreshSessionRequest> get serializer => _$RefreshSessionRequestSerializer();
}

class _$RefreshSessionRequestSerializer implements PrimitiveSerializer<RefreshSessionRequest> {
  @override
  final Iterable<Type> types = const [RefreshSessionRequest, _$RefreshSessionRequest];

  @override
  final String wireName = r'RefreshSessionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RefreshSessionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.refreshToken != null) {
      yield r'refreshToken';
      yield serializers.serialize(
        object.refreshToken,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.clientPlatform != null) {
      yield r'clientPlatform';
      yield serializers.serialize(
        object.clientPlatform,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RefreshSessionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RefreshSessionRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'refreshToken':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.refreshToken = valueDes;
          break;
        case r'clientPlatform':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.clientPlatform = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  RefreshSessionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RefreshSessionRequestBuilder();
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


