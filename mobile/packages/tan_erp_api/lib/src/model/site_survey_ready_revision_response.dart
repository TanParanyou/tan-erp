//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'site_survey_ready_revision_response.g.dart';

/// SiteSurveyReadyRevisionResponse
///
/// Properties:
/// * [id] 
/// * [revisionNumber] 
/// * [snapshotHash] 
@BuiltValue()
abstract class SiteSurveyReadyRevisionResponse implements Built<SiteSurveyReadyRevisionResponse, SiteSurveyReadyRevisionResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'revisionNumber')
  int? get revisionNumber;

  @BuiltValueField(wireName: r'snapshotHash')
  String? get snapshotHash;

  SiteSurveyReadyRevisionResponse._();

  factory SiteSurveyReadyRevisionResponse([void updates(SiteSurveyReadyRevisionResponseBuilder b)]) = _$SiteSurveyReadyRevisionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SiteSurveyReadyRevisionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SiteSurveyReadyRevisionResponse> get serializer => _$SiteSurveyReadyRevisionResponseSerializer();
}

class _$SiteSurveyReadyRevisionResponseSerializer implements PrimitiveSerializer<SiteSurveyReadyRevisionResponse> {
  @override
  final Iterable<Type> types = const [SiteSurveyReadyRevisionResponse, _$SiteSurveyReadyRevisionResponse];

  @override
  final String wireName = r'SiteSurveyReadyRevisionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SiteSurveyReadyRevisionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.revisionNumber != null) {
      yield r'revisionNumber';
      yield serializers.serialize(
        object.revisionNumber,
        specifiedType: const FullType(int),
      );
    }
    if (object.snapshotHash != null) {
      yield r'snapshotHash';
      yield serializers.serialize(
        object.snapshotHash,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SiteSurveyReadyRevisionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SiteSurveyReadyRevisionResponseBuilder result,
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
        case r'revisionNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.revisionNumber = valueDes;
          break;
        case r'snapshotHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.snapshotHash = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SiteSurveyReadyRevisionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SiteSurveyReadyRevisionResponseBuilder();
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


