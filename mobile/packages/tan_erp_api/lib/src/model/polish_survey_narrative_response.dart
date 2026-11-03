//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'polish_survey_narrative_response.g.dart';

/// PolishSurveyNarrativeResponse
///
/// Properties:
/// * [text] 
/// * [provider] 
@BuiltValue()
abstract class PolishSurveyNarrativeResponse implements Built<PolishSurveyNarrativeResponse, PolishSurveyNarrativeResponseBuilder> {
  @BuiltValueField(wireName: r'text')
  String? get text;

  @BuiltValueField(wireName: r'provider')
  String? get provider;

  PolishSurveyNarrativeResponse._();

  factory PolishSurveyNarrativeResponse([void updates(PolishSurveyNarrativeResponseBuilder b)]) = _$PolishSurveyNarrativeResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PolishSurveyNarrativeResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PolishSurveyNarrativeResponse> get serializer => _$PolishSurveyNarrativeResponseSerializer();
}

class _$PolishSurveyNarrativeResponseSerializer implements PrimitiveSerializer<PolishSurveyNarrativeResponse> {
  @override
  final Iterable<Type> types = const [PolishSurveyNarrativeResponse, _$PolishSurveyNarrativeResponse];

  @override
  final String wireName = r'PolishSurveyNarrativeResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PolishSurveyNarrativeResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.text != null) {
      yield r'text';
      yield serializers.serialize(
        object.text,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.provider != null) {
      yield r'provider';
      yield serializers.serialize(
        object.provider,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PolishSurveyNarrativeResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PolishSurveyNarrativeResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'text':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.text = valueDes;
          break;
        case r'provider':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.provider = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PolishSurveyNarrativeResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PolishSurveyNarrativeResponseBuilder();
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


