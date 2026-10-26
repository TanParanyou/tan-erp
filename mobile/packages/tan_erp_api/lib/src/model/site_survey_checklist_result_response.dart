//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'site_survey_checklist_result_response.g.dart';

/// SiteSurveyChecklistResultResponse
///
/// Properties:
/// * [id] 
/// * [itemCode] 
/// * [result] 
/// * [note] 
@BuiltValue()
abstract class SiteSurveyChecklistResultResponse implements Built<SiteSurveyChecklistResultResponse, SiteSurveyChecklistResultResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'itemCode')
  String? get itemCode;

  @BuiltValueField(wireName: r'result')
  String? get result;

  @BuiltValueField(wireName: r'note')
  String? get note;

  SiteSurveyChecklistResultResponse._();

  factory SiteSurveyChecklistResultResponse([void updates(SiteSurveyChecklistResultResponseBuilder b)]) = _$SiteSurveyChecklistResultResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SiteSurveyChecklistResultResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SiteSurveyChecklistResultResponse> get serializer => _$SiteSurveyChecklistResultResponseSerializer();
}

class _$SiteSurveyChecklistResultResponseSerializer implements PrimitiveSerializer<SiteSurveyChecklistResultResponse> {
  @override
  final Iterable<Type> types = const [SiteSurveyChecklistResultResponse, _$SiteSurveyChecklistResultResponse];

  @override
  final String wireName = r'SiteSurveyChecklistResultResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SiteSurveyChecklistResultResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.itemCode != null) {
      yield r'itemCode';
      yield serializers.serialize(
        object.itemCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.result != null) {
      yield r'result';
      yield serializers.serialize(
        object.result,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SiteSurveyChecklistResultResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SiteSurveyChecklistResultResponseBuilder result,
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
        case r'itemCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemCode = valueDes;
          break;
        case r'result':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.result = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SiteSurveyChecklistResultResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SiteSurveyChecklistResultResponseBuilder();
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


