//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'polish_survey_narrative_request.g.dart';

/// PolishSurveyNarrativeRequest
///
/// Properties:
/// * [text] 
/// * [language] 
@BuiltValue()
abstract class PolishSurveyNarrativeRequest implements Built<PolishSurveyNarrativeRequest, PolishSurveyNarrativeRequestBuilder> {
  @BuiltValueField(wireName: r'text')
  String? get text;

  @BuiltValueField(wireName: r'language')
  String? get language;

  PolishSurveyNarrativeRequest._();

  factory PolishSurveyNarrativeRequest([void updates(PolishSurveyNarrativeRequestBuilder b)]) = _$PolishSurveyNarrativeRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PolishSurveyNarrativeRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PolishSurveyNarrativeRequest> get serializer => _$PolishSurveyNarrativeRequestSerializer();
}

class _$PolishSurveyNarrativeRequestSerializer implements PrimitiveSerializer<PolishSurveyNarrativeRequest> {
  @override
  final Iterable<Type> types = const [PolishSurveyNarrativeRequest, _$PolishSurveyNarrativeRequest];

  @override
  final String wireName = r'PolishSurveyNarrativeRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PolishSurveyNarrativeRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.text != null) {
      yield r'text';
      yield serializers.serialize(
        object.text,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.language != null) {
      yield r'language';
      yield serializers.serialize(
        object.language,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PolishSurveyNarrativeRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PolishSurveyNarrativeRequestBuilder result,
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
        case r'language':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.language = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PolishSurveyNarrativeRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PolishSurveyNarrativeRequestBuilder();
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


