//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'share_request.g.dart';

/// ShareRequest
///
/// Properties:
/// * [sourceVersion] 
/// * [channel] 
/// * [recipient] 
/// * [locale] 
@BuiltValue()
abstract class ShareRequest implements Built<ShareRequest, ShareRequestBuilder> {
  @BuiltValueField(wireName: r'sourceVersion')
  int? get sourceVersion;

  @BuiltValueField(wireName: r'channel')
  String? get channel;

  @BuiltValueField(wireName: r'recipient')
  String? get recipient;

  @BuiltValueField(wireName: r'locale')
  String? get locale;

  ShareRequest._();

  factory ShareRequest([void updates(ShareRequestBuilder b)]) = _$ShareRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ShareRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ShareRequest> get serializer => _$ShareRequestSerializer();
}

class _$ShareRequestSerializer implements PrimitiveSerializer<ShareRequest> {
  @override
  final Iterable<Type> types = const [ShareRequest, _$ShareRequest];

  @override
  final String wireName = r'ShareRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ShareRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.sourceVersion != null) {
      yield r'sourceVersion';
      yield serializers.serialize(
        object.sourceVersion,
        specifiedType: const FullType(int),
      );
    }
    if (object.channel != null) {
      yield r'channel';
      yield serializers.serialize(
        object.channel,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.recipient != null) {
      yield r'recipient';
      yield serializers.serialize(
        object.recipient,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.locale != null) {
      yield r'locale';
      yield serializers.serialize(
        object.locale,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ShareRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ShareRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'sourceVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sourceVersion = valueDes;
          break;
        case r'channel':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.channel = valueDes;
          break;
        case r'recipient':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.recipient = valueDes;
          break;
        case r'locale':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.locale = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ShareRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ShareRequestBuilder();
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


