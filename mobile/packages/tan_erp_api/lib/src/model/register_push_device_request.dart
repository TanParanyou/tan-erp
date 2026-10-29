//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'register_push_device_request.g.dart';

/// RegisterPushDeviceRequest
///
/// Properties:
/// * [token] 
/// * [platform] 
@BuiltValue()
abstract class RegisterPushDeviceRequest implements Built<RegisterPushDeviceRequest, RegisterPushDeviceRequestBuilder> {
  @BuiltValueField(wireName: r'token')
  String? get token;

  @BuiltValueField(wireName: r'platform')
  String? get platform;

  RegisterPushDeviceRequest._();

  factory RegisterPushDeviceRequest([void updates(RegisterPushDeviceRequestBuilder b)]) = _$RegisterPushDeviceRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RegisterPushDeviceRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RegisterPushDeviceRequest> get serializer => _$RegisterPushDeviceRequestSerializer();
}

class _$RegisterPushDeviceRequestSerializer implements PrimitiveSerializer<RegisterPushDeviceRequest> {
  @override
  final Iterable<Type> types = const [RegisterPushDeviceRequest, _$RegisterPushDeviceRequest];

  @override
  final String wireName = r'RegisterPushDeviceRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RegisterPushDeviceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.token != null) {
      yield r'token';
      yield serializers.serialize(
        object.token,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.platform != null) {
      yield r'platform';
      yield serializers.serialize(
        object.platform,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RegisterPushDeviceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RegisterPushDeviceRequestBuilder result,
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
        case r'platform':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.platform = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  RegisterPushDeviceRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RegisterPushDeviceRequestBuilder();
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


