//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'unregister_push_device_request.g.dart';

/// UnregisterPushDeviceRequest
///
/// Properties:
/// * [token] 
@BuiltValue()
abstract class UnregisterPushDeviceRequest implements Built<UnregisterPushDeviceRequest, UnregisterPushDeviceRequestBuilder> {
  @BuiltValueField(wireName: r'token')
  String? get token;

  UnregisterPushDeviceRequest._();

  factory UnregisterPushDeviceRequest([void updates(UnregisterPushDeviceRequestBuilder b)]) = _$UnregisterPushDeviceRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UnregisterPushDeviceRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UnregisterPushDeviceRequest> get serializer => _$UnregisterPushDeviceRequestSerializer();
}

class _$UnregisterPushDeviceRequestSerializer implements PrimitiveSerializer<UnregisterPushDeviceRequest> {
  @override
  final Iterable<Type> types = const [UnregisterPushDeviceRequest, _$UnregisterPushDeviceRequest];

  @override
  final String wireName = r'UnregisterPushDeviceRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UnregisterPushDeviceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.token != null) {
      yield r'token';
      yield serializers.serialize(
        object.token,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UnregisterPushDeviceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UnregisterPushDeviceRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'token':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.token = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UnregisterPushDeviceRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UnregisterPushDeviceRequestBuilder();
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


