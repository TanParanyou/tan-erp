//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/survey_localized_name_contract.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_survey_mood_tone_request.g.dart';

/// UpdateSurveyMoodToneRequest
///
/// Properties:
/// * [name] 
/// * [description] 
/// * [colorPalette] 
/// * [sortOrder] 
/// * [isActive] 
@BuiltValue()
abstract class UpdateSurveyMoodToneRequest implements Built<UpdateSurveyMoodToneRequest, UpdateSurveyMoodToneRequestBuilder> {
  @BuiltValueField(wireName: r'name')
  SurveyLocalizedNameContract? get name;

  @BuiltValueField(wireName: r'description')
  SurveyLocalizedNameContract? get description;

  @BuiltValueField(wireName: r'colorPalette')
  BuiltList<String>? get colorPalette;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  @BuiltValueField(wireName: r'isActive')
  bool? get isActive;

  UpdateSurveyMoodToneRequest._();

  factory UpdateSurveyMoodToneRequest([void updates(UpdateSurveyMoodToneRequestBuilder b)]) = _$UpdateSurveyMoodToneRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateSurveyMoodToneRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateSurveyMoodToneRequest> get serializer => _$UpdateSurveyMoodToneRequestSerializer();
}

class _$UpdateSurveyMoodToneRequestSerializer implements PrimitiveSerializer<UpdateSurveyMoodToneRequest> {
  @override
  final Iterable<Type> types = const [UpdateSurveyMoodToneRequest, _$UpdateSurveyMoodToneRequest];

  @override
  final String wireName = r'UpdateSurveyMoodToneRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateSurveyMoodToneRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType(SurveyLocalizedNameContract),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType(SurveyLocalizedNameContract),
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
    if (object.isActive != null) {
      yield r'isActive';
      yield serializers.serialize(
        object.isActive,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateSurveyMoodToneRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateSurveyMoodToneRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(SurveyLocalizedNameContract),
          ) as SurveyLocalizedNameContract?;
          if (valueDes == null) continue;
          result.name.replace(valueDes);
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(SurveyLocalizedNameContract),
          ) as SurveyLocalizedNameContract?;
          if (valueDes == null) continue;
          result.description.replace(valueDes);
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
        case r'isActive':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isActive = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateSurveyMoodToneRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateSurveyMoodToneRequestBuilder();
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


