//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'localized_text_dto.g.dart';

/// LocalizedTextDto
///
/// Properties:
/// * [thai] 
/// * [english] 
@BuiltValue()
abstract class LocalizedTextDto implements Built<LocalizedTextDto, LocalizedTextDtoBuilder> {
  @BuiltValueField(wireName: r'thai')
  String? get thai;

  @BuiltValueField(wireName: r'english')
  String? get english;

  LocalizedTextDto._();

  factory LocalizedTextDto([void updates(LocalizedTextDtoBuilder b)]) = _$LocalizedTextDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(LocalizedTextDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<LocalizedTextDto> get serializer => _$LocalizedTextDtoSerializer();
}

class _$LocalizedTextDtoSerializer implements PrimitiveSerializer<LocalizedTextDto> {
  @override
  final Iterable<Type> types = const [LocalizedTextDto, _$LocalizedTextDto];

  @override
  final String wireName = r'LocalizedTextDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    LocalizedTextDto object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.thai != null) {
      yield r'thai';
      yield serializers.serialize(
        object.thai,
        specifiedType: const FullType.nullable(String),
      );
    }
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
    LocalizedTextDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required LocalizedTextDtoBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'thai':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
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
  LocalizedTextDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = LocalizedTextDtoBuilder();
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


