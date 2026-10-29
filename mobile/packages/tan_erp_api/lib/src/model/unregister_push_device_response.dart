//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'unregister_push_device_response.g.dart';

/// UnregisterPushDeviceResponse
///
/// Properties:
/// * [removed] 
@BuiltValue()
abstract class UnregisterPushDeviceResponse implements Built<UnregisterPushDeviceResponse, UnregisterPushDeviceResponseBuilder> {
  @BuiltValueField(wireName: r'removed')
  bool? get removed;

  UnregisterPushDeviceResponse._();

  factory UnregisterPushDeviceResponse([void updates(UnregisterPushDeviceResponseBuilder b)]) = _$UnregisterPushDeviceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UnregisterPushDeviceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UnregisterPushDeviceResponse> get serializer => _$UnregisterPushDeviceResponseSerializer();
}

class _$UnregisterPushDeviceResponseSerializer implements PrimitiveSerializer<UnregisterPushDeviceResponse> {
  @override
  final Iterable<Type> types = const [UnregisterPushDeviceResponse, _$UnregisterPushDeviceResponse];

  @override
  final String wireName = r'UnregisterPushDeviceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UnregisterPushDeviceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.removed != null) {
      yield r'removed';
      yield serializers.serialize(
        object.removed,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UnregisterPushDeviceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UnregisterPushDeviceResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'removed':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.removed = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UnregisterPushDeviceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UnregisterPushDeviceResponseBuilder();
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


