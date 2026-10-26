//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'clone_survey_revision_request.g.dart';

/// CloneSurveyRevisionRequest
///
/// Properties:
/// * [sourceRevisionId] 
/// * [reason] 
@BuiltValue()
abstract class CloneSurveyRevisionRequest implements Built<CloneSurveyRevisionRequest, CloneSurveyRevisionRequestBuilder> {
  @BuiltValueField(wireName: r'sourceRevisionId')
  String? get sourceRevisionId;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  CloneSurveyRevisionRequest._();

  factory CloneSurveyRevisionRequest([void updates(CloneSurveyRevisionRequestBuilder b)]) = _$CloneSurveyRevisionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CloneSurveyRevisionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CloneSurveyRevisionRequest> get serializer => _$CloneSurveyRevisionRequestSerializer();
}

class _$CloneSurveyRevisionRequestSerializer implements PrimitiveSerializer<CloneSurveyRevisionRequest> {
  @override
  final Iterable<Type> types = const [CloneSurveyRevisionRequest, _$CloneSurveyRevisionRequest];

  @override
  final String wireName = r'CloneSurveyRevisionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CloneSurveyRevisionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.sourceRevisionId != null) {
      yield r'sourceRevisionId';
      yield serializers.serialize(
        object.sourceRevisionId,
        specifiedType: const FullType(String),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CloneSurveyRevisionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CloneSurveyRevisionRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'sourceRevisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceRevisionId = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CloneSurveyRevisionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CloneSurveyRevisionRequestBuilder();
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


