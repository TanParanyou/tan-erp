//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'effective_add_on_response.g.dart';

/// EffectiveAddOnResponse
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [perLine] 
@BuiltValue()
abstract class EffectiveAddOnResponse implements Built<EffectiveAddOnResponse, EffectiveAddOnResponseBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'perLine')
  bool? get perLine;

  EffectiveAddOnResponse._();

  factory EffectiveAddOnResponse([void updates(EffectiveAddOnResponseBuilder b)]) = _$EffectiveAddOnResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EffectiveAddOnResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EffectiveAddOnResponse> get serializer => _$EffectiveAddOnResponseSerializer();
}

class _$EffectiveAddOnResponseSerializer implements PrimitiveSerializer<EffectiveAddOnResponse> {
  @override
  final Iterable<Type> types = const [EffectiveAddOnResponse, _$EffectiveAddOnResponse];

  @override
  final String wireName = r'EffectiveAddOnResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EffectiveAddOnResponse object, {
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
    if (object.perLine != null) {
      yield r'perLine';
      yield serializers.serialize(
        object.perLine,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EffectiveAddOnResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EffectiveAddOnResponseBuilder result,
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
        case r'perLine':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.perLine = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EffectiveAddOnResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EffectiveAddOnResponseBuilder();
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


