//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'localized_text_input.g.dart';

/// LocalizedTextInput
///
/// Properties:
/// * [thai] 
/// * [english] 
@BuiltValue()
abstract class LocalizedTextInput implements Built<LocalizedTextInput, LocalizedTextInputBuilder> {
  @BuiltValueField(wireName: r'thai')
  String get thai;

  @BuiltValueField(wireName: r'english')
  String? get english;

  LocalizedTextInput._();

  factory LocalizedTextInput([void updates(LocalizedTextInputBuilder b)]) = _$LocalizedTextInput;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(LocalizedTextInputBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<LocalizedTextInput> get serializer => _$LocalizedTextInputSerializer();
}

class _$LocalizedTextInputSerializer implements PrimitiveSerializer<LocalizedTextInput> {
  @override
  final Iterable<Type> types = const [LocalizedTextInput, _$LocalizedTextInput];

  @override
  final String wireName = r'LocalizedTextInput';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    LocalizedTextInput object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'thai';
    yield serializers.serialize(
      object.thai,
      specifiedType: const FullType(String),
    );
    if (object.english != null) {
      yield r'english';
      yield serializers.serialize(
        object.english,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    LocalizedTextInput object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required LocalizedTextInputBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'thai':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.thai = valueDes;
          break;
        case r'english':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.english = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  LocalizedTextInput deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = LocalizedTextInputBuilder();
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


