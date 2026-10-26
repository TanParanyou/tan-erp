//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'void_survey_revision_request.g.dart';

/// VoidSurveyRevisionRequest
///
/// Properties:
/// * [expectedRevisionVersion] 
/// * [reason] 
@BuiltValue()
abstract class VoidSurveyRevisionRequest implements Built<VoidSurveyRevisionRequest, VoidSurveyRevisionRequestBuilder> {
  @BuiltValueField(wireName: r'expectedRevisionVersion')
  String? get expectedRevisionVersion;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  VoidSurveyRevisionRequest._();

  factory VoidSurveyRevisionRequest([void updates(VoidSurveyRevisionRequestBuilder b)]) = _$VoidSurveyRevisionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(VoidSurveyRevisionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<VoidSurveyRevisionRequest> get serializer => _$VoidSurveyRevisionRequestSerializer();
}

class _$VoidSurveyRevisionRequestSerializer implements PrimitiveSerializer<VoidSurveyRevisionRequest> {
  @override
  final Iterable<Type> types = const [VoidSurveyRevisionRequest, _$VoidSurveyRevisionRequest];

  @override
  final String wireName = r'VoidSurveyRevisionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    VoidSurveyRevisionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.expectedRevisionVersion != null) {
      yield r'expectedRevisionVersion';
      yield serializers.serialize(
        object.expectedRevisionVersion,
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
    VoidSurveyRevisionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required VoidSurveyRevisionRequestBuilder result,
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
  VoidSurveyRevisionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = VoidSurveyRevisionRequestBuilder();
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


