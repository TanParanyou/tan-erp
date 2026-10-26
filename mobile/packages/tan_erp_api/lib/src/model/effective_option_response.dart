//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'effective_option_response.g.dart';

/// EffectiveOptionResponse
///
/// Properties:
/// * [code] 
/// * [name] 
@BuiltValue()
abstract class EffectiveOptionResponse implements Built<EffectiveOptionResponse, EffectiveOptionResponseBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  EffectiveOptionResponse._();

  factory EffectiveOptionResponse([void updates(EffectiveOptionResponseBuilder b)]) = _$EffectiveOptionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EffectiveOptionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EffectiveOptionResponse> get serializer => _$EffectiveOptionResponseSerializer();
}

class _$EffectiveOptionResponseSerializer implements PrimitiveSerializer<EffectiveOptionResponse> {
  @override
  final Iterable<Type> types = const [EffectiveOptionResponse, _$EffectiveOptionResponse];

  @override
  final String wireName = r'EffectiveOptionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EffectiveOptionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EffectiveOptionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EffectiveOptionResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EffectiveOptionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EffectiveOptionResponseBuilder();
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


