//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'localized_text_response.g.dart';

/// LocalizedTextResponse
///
/// Properties:
/// * [thai] 
/// * [english] 
@BuiltValue()
abstract class LocalizedTextResponse implements Built<LocalizedTextResponse, LocalizedTextResponseBuilder> {
  @BuiltValueField(wireName: r'thai')
  String? get thai;

  @BuiltValueField(wireName: r'english')
  String? get english;

  LocalizedTextResponse._();

  factory LocalizedTextResponse([void updates(LocalizedTextResponseBuilder b)]) = _$LocalizedTextResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(LocalizedTextResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<LocalizedTextResponse> get serializer => _$LocalizedTextResponseSerializer();
}

class _$LocalizedTextResponseSerializer implements PrimitiveSerializer<LocalizedTextResponse> {
  @override
  final Iterable<Type> types = const [LocalizedTextResponse, _$LocalizedTextResponse];

  @override
  final String wireName = r'LocalizedTextResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    LocalizedTextResponse object, {
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
    LocalizedTextResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required LocalizedTextResponseBuilder result,
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
  LocalizedTextResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = LocalizedTextResponseBuilder();
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


