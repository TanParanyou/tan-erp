//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_survey_checklist_request.g.dart';

/// UpdateSurveyChecklistRequest
///
/// Properties:
/// * [itemCode] 
/// * [result] 
/// * [note] 
@BuiltValue()
abstract class UpdateSurveyChecklistRequest implements Built<UpdateSurveyChecklistRequest, UpdateSurveyChecklistRequestBuilder> {
  @BuiltValueField(wireName: r'itemCode')
  String? get itemCode;

  @BuiltValueField(wireName: r'result')
  String? get result;

  @BuiltValueField(wireName: r'note')
  String? get note;

  UpdateSurveyChecklistRequest._();

  factory UpdateSurveyChecklistRequest([void updates(UpdateSurveyChecklistRequestBuilder b)]) = _$UpdateSurveyChecklistRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateSurveyChecklistRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateSurveyChecklistRequest> get serializer => _$UpdateSurveyChecklistRequestSerializer();
}

class _$UpdateSurveyChecklistRequestSerializer implements PrimitiveSerializer<UpdateSurveyChecklistRequest> {
  @override
  final Iterable<Type> types = const [UpdateSurveyChecklistRequest, _$UpdateSurveyChecklistRequest];

  @override
  final String wireName = r'UpdateSurveyChecklistRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateSurveyChecklistRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
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
    UpdateSurveyChecklistRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateSurveyChecklistRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
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
  UpdateSurveyChecklistRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateSurveyChecklistRequestBuilder();
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


