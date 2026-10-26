//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/survey_template_version_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'survey_template_version_list_response.g.dart';

/// SurveyTemplateVersionListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class SurveyTemplateVersionListResponse implements Built<SurveyTemplateVersionListResponse, SurveyTemplateVersionListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<SurveyTemplateVersionResponse>? get items;

  SurveyTemplateVersionListResponse._();

  factory SurveyTemplateVersionListResponse([void updates(SurveyTemplateVersionListResponseBuilder b)]) = _$SurveyTemplateVersionListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SurveyTemplateVersionListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SurveyTemplateVersionListResponse> get serializer => _$SurveyTemplateVersionListResponseSerializer();
}

class _$SurveyTemplateVersionListResponseSerializer implements PrimitiveSerializer<SurveyTemplateVersionListResponse> {
  @override
  final Iterable<Type> types = const [SurveyTemplateVersionListResponse, _$SurveyTemplateVersionListResponse];

  @override
  final String wireName = r'SurveyTemplateVersionListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SurveyTemplateVersionListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyTemplateVersionResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SurveyTemplateVersionListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SurveyTemplateVersionListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SurveyTemplateVersionResponse)]),
          ) as BuiltList<SurveyTemplateVersionResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SurveyTemplateVersionListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SurveyTemplateVersionListResponseBuilder();
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


