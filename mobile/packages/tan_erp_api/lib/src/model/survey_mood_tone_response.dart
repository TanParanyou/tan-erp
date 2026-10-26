//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/survey_localized_name_contract.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'survey_mood_tone_response.g.dart';

/// SurveyMoodToneResponse
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [name] 
/// * [description] 
/// * [colorPalette] 
/// * [sortOrder] 
/// * [isActive] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class SurveyMoodToneResponse implements Built<SurveyMoodToneResponse, SurveyMoodToneResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

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

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  SurveyMoodToneResponse._();

  factory SurveyMoodToneResponse([void updates(SurveyMoodToneResponseBuilder b)]) = _$SurveyMoodToneResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SurveyMoodToneResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SurveyMoodToneResponse> get serializer => _$SurveyMoodToneResponseSerializer();
}

class _$SurveyMoodToneResponseSerializer implements PrimitiveSerializer<SurveyMoodToneResponse> {
  @override
  final Iterable<Type> types = const [SurveyMoodToneResponse, _$SurveyMoodToneResponse];

  @override
  final String wireName = r'SurveyMoodToneResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SurveyMoodToneResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
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
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SurveyMoodToneResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SurveyMoodToneResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
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
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SurveyMoodToneResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SurveyMoodToneResponseBuilder();
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


