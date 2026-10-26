//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/survey_photo_tag_option_response.dart';
import 'package:tan_erp_api/src/model/survey_measurement_type_option_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/survey_category_option_response.dart';
import 'package:tan_erp_api/src/model/survey_mood_tone_option_response.dart';
import 'package:tan_erp_api/src/model/survey_design_style_option_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'survey_reference_data_response.g.dart';

/// SurveyReferenceDataResponse
///
/// Properties:
/// * [categories] 
/// * [photoTags] 
/// * [measurementTypes] 
/// * [designStyles] 
/// * [moodTones] 
@BuiltValue()
abstract class SurveyReferenceDataResponse implements Built<SurveyReferenceDataResponse, SurveyReferenceDataResponseBuilder> {
  @BuiltValueField(wireName: r'categories')
  BuiltList<SurveyCategoryOptionResponse>? get categories;

  @BuiltValueField(wireName: r'photoTags')
  BuiltList<SurveyPhotoTagOptionResponse>? get photoTags;

  @BuiltValueField(wireName: r'measurementTypes')
  BuiltList<SurveyMeasurementTypeOptionResponse>? get measurementTypes;

  @BuiltValueField(wireName: r'designStyles')
  BuiltList<SurveyDesignStyleOptionResponse>? get designStyles;

  @BuiltValueField(wireName: r'moodTones')
  BuiltList<SurveyMoodToneOptionResponse>? get moodTones;

  SurveyReferenceDataResponse._();

  factory SurveyReferenceDataResponse([void updates(SurveyReferenceDataResponseBuilder b)]) = _$SurveyReferenceDataResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SurveyReferenceDataResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SurveyReferenceDataResponse> get serializer => _$SurveyReferenceDataResponseSerializer();
}

class _$SurveyReferenceDataResponseSerializer implements PrimitiveSerializer<SurveyReferenceDataResponse> {
  @override
  final Iterable<Type> types = const [SurveyReferenceDataResponse, _$SurveyReferenceDataResponse];

  @override
  final String wireName = r'SurveyReferenceDataResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SurveyReferenceDataResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.categories != null) {
      yield r'categories';
      yield serializers.serialize(
        object.categories,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyCategoryOptionResponse)]),
      );
    }
    if (object.photoTags != null) {
      yield r'photoTags';
      yield serializers.serialize(
        object.photoTags,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyPhotoTagOptionResponse)]),
      );
    }
    if (object.measurementTypes != null) {
      yield r'measurementTypes';
      yield serializers.serialize(
        object.measurementTypes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyMeasurementTypeOptionResponse)]),
      );
    }
    if (object.designStyles != null) {
      yield r'designStyles';
      yield serializers.serialize(
        object.designStyles,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyDesignStyleOptionResponse)]),
      );
    }
    if (object.moodTones != null) {
      yield r'moodTones';
      yield serializers.serialize(
        object.moodTones,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyMoodToneOptionResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SurveyReferenceDataResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SurveyReferenceDataResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'categories':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyCategoryOptionResponse)]),
          ) as BuiltList<SurveyCategoryOptionResponse>?;
          if (valueDes == null) continue;
          result.categories.replace(valueDes);
          break;
        case r'photoTags':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyPhotoTagOptionResponse)]),
          ) as BuiltList<SurveyPhotoTagOptionResponse>?;
          if (valueDes == null) continue;
          result.photoTags.replace(valueDes);
          break;
        case r'measurementTypes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyMeasurementTypeOptionResponse)]),
          ) as BuiltList<SurveyMeasurementTypeOptionResponse>?;
          if (valueDes == null) continue;
          result.measurementTypes.replace(valueDes);
          break;
        case r'designStyles':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyDesignStyleOptionResponse)]),
          ) as BuiltList<SurveyDesignStyleOptionResponse>?;
          if (valueDes == null) continue;
          result.designStyles.replace(valueDes);
          break;
        case r'moodTones':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyMoodToneOptionResponse)]),
          ) as BuiltList<SurveyMoodToneOptionResponse>?;
          if (valueDes == null) continue;
          result.moodTones.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SurveyReferenceDataResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SurveyReferenceDataResponseBuilder();
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


