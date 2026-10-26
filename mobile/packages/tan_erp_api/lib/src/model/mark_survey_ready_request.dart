//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'mark_survey_ready_request.g.dart';

/// MarkSurveyReadyRequest
///
/// Properties:
/// * [expectedRevisionVersion] 
/// * [expectedOpportunityVersion] 
@BuiltValue()
abstract class MarkSurveyReadyRequest implements Built<MarkSurveyReadyRequest, MarkSurveyReadyRequestBuilder> {
  @BuiltValueField(wireName: r'expectedRevisionVersion')
  String? get expectedRevisionVersion;

  @BuiltValueField(wireName: r'expectedOpportunityVersion')
  String? get expectedOpportunityVersion;

  MarkSurveyReadyRequest._();

  factory MarkSurveyReadyRequest([void updates(MarkSurveyReadyRequestBuilder b)]) = _$MarkSurveyReadyRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(MarkSurveyReadyRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<MarkSurveyReadyRequest> get serializer => _$MarkSurveyReadyRequestSerializer();
}

class _$MarkSurveyReadyRequestSerializer implements PrimitiveSerializer<MarkSurveyReadyRequest> {
  @override
  final Iterable<Type> types = const [MarkSurveyReadyRequest, _$MarkSurveyReadyRequest];

  @override
  final String wireName = r'MarkSurveyReadyRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    MarkSurveyReadyRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.expectedRevisionVersion != null) {
      yield r'expectedRevisionVersion';
      yield serializers.serialize(
        object.expectedRevisionVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.expectedOpportunityVersion != null) {
      yield r'expectedOpportunityVersion';
      yield serializers.serialize(
        object.expectedOpportunityVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    MarkSurveyReadyRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required MarkSurveyReadyRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'expectedRevisionVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedRevisionVersion = valueDes;
          break;
        case r'expectedOpportunityVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.expectedOpportunityVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  MarkSurveyReadyRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = MarkSurveyReadyRequestBuilder();
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


