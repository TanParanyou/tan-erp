//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'push_device_response.g.dart';

/// PushDeviceResponse
///
/// Properties:
/// * [token] 
/// * [platform] 
/// * [registeredAtUtc] 
@BuiltValue()
abstract class PushDeviceResponse implements Built<PushDeviceResponse, PushDeviceResponseBuilder> {
  @BuiltValueField(wireName: r'token')
  String? get token;

  @BuiltValueField(wireName: r'platform')
  String? get platform;

  @BuiltValueField(wireName: r'registeredAtUtc')
  DateTime? get registeredAtUtc;

  PushDeviceResponse._();

  factory PushDeviceResponse([void updates(PushDeviceResponseBuilder b)]) = _$PushDeviceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PushDeviceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PushDeviceResponse> get serializer => _$PushDeviceResponseSerializer();
}

class _$PushDeviceResponseSerializer implements PrimitiveSerializer<PushDeviceResponse> {
  @override
  final Iterable<Type> types = const [PushDeviceResponse, _$PushDeviceResponse];

  @override
  final String wireName = r'PushDeviceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PushDeviceResponse object, {
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
    if (object.registeredAtUtc != null) {
      yield r'registeredAtUtc';
      yield serializers.serialize(
        object.registeredAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PushDeviceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PushDeviceResponseBuilder result,
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
        case r'registeredAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.registeredAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PushDeviceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PushDeviceResponseBuilder();
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


