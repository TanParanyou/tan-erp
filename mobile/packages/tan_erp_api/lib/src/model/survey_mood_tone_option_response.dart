//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/survey_localized_label.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'survey_mood_tone_option_response.g.dart';

/// SurveyMoodToneOptionResponse
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [description] 
/// * [colorPalette] 
/// * [sortOrder] 
@BuiltValue()
abstract class SurveyMoodToneOptionResponse implements Built<SurveyMoodToneOptionResponse, SurveyMoodToneOptionResponseBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  SurveyLocalizedLabel? get name;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'colorPalette')
  BuiltList<String>? get colorPalette;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  SurveyMoodToneOptionResponse._();

  factory SurveyMoodToneOptionResponse([void updates(SurveyMoodToneOptionResponseBuilder b)]) = _$SurveyMoodToneOptionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SurveyMoodToneOptionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SurveyMoodToneOptionResponse> get serializer => _$SurveyMoodToneOptionResponseSerializer();
}

class _$SurveyMoodToneOptionResponseSerializer implements PrimitiveSerializer<SurveyMoodToneOptionResponse> {
  @override
  final Iterable<Type> types = const [SurveyMoodToneOptionResponse, _$SurveyMoodToneOptionResponse];

  @override
  final String wireName = r'SurveyMoodToneOptionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SurveyMoodToneOptionResponse object, {
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
        specifiedType: const FullType(SurveyLocalizedLabel),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.colorPalette != null) {
      yield r'colorPalette';
      yield serializers.serialize(
        object.colorPalette,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SurveyMoodToneOptionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SurveyMoodToneOptionResponseBuilder result,
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
            specifiedType: const FullType.nullable(SurveyLocalizedLabel),
          ) as SurveyLocalizedLabel?;
          if (valueDes == null) continue;
          result.name.replace(valueDes);
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'colorPalette':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.colorPalette.replace(valueDes);
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SurveyMoodToneOptionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SurveyMoodToneOptionResponseBuilder();
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


