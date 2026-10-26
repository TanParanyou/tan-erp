//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'publish_survey_template_version_request.g.dart';

/// PublishSurveyTemplateVersionRequest
///
/// Properties:
/// * [code] 
/// * [requiredChecklistItems] 
/// * [minimumEvidenceCount] 
/// * [minimumAreaEvidenceCount] 
/// * [snapshotHashVersion] 
/// * [effectiveFromUtc] 
@BuiltValue()
abstract class PublishSurveyTemplateVersionRequest implements Built<PublishSurveyTemplateVersionRequest, PublishSurveyTemplateVersionRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'requiredChecklistItems')
  BuiltList<String>? get requiredChecklistItems;

  @BuiltValueField(wireName: r'minimumEvidenceCount')
  int? get minimumEvidenceCount;

  @BuiltValueField(wireName: r'minimumAreaEvidenceCount')
  int? get minimumAreaEvidenceCount;

  @BuiltValueField(wireName: r'snapshotHashVersion')
  String? get snapshotHashVersion;

  @BuiltValueField(wireName: r'effectiveFromUtc')
  DateTime? get effectiveFromUtc;

  PublishSurveyTemplateVersionRequest._();

  factory PublishSurveyTemplateVersionRequest([void updates(PublishSurveyTemplateVersionRequestBuilder b)]) = _$PublishSurveyTemplateVersionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PublishSurveyTemplateVersionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PublishSurveyTemplateVersionRequest> get serializer => _$PublishSurveyTemplateVersionRequestSerializer();
}

class _$PublishSurveyTemplateVersionRequestSerializer implements PrimitiveSerializer<PublishSurveyTemplateVersionRequest> {
  @override
  final Iterable<Type> types = const [PublishSurveyTemplateVersionRequest, _$PublishSurveyTemplateVersionRequest];

  @override
  final String wireName = r'PublishSurveyTemplateVersionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PublishSurveyTemplateVersionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.requiredChecklistItems != null) {
      yield r'requiredChecklistItems';
      yield serializers.serialize(
        object.requiredChecklistItems,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.minimumEvidenceCount != null) {
      yield r'minimumEvidenceCount';
      yield serializers.serialize(
        object.minimumEvidenceCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.minimumAreaEvidenceCount != null) {
      yield r'minimumAreaEvidenceCount';
      yield serializers.serialize(
        object.minimumAreaEvidenceCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.snapshotHashVersion != null) {
      yield r'snapshotHashVersion';
      yield serializers.serialize(
        object.snapshotHashVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.effectiveFromUtc != null) {
      yield r'effectiveFromUtc';
      yield serializers.serialize(
        object.effectiveFromUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PublishSurveyTemplateVersionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PublishSurveyTemplateVersionRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'requiredChecklistItems':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.requiredChecklistItems.replace(valueDes);
          break;
        case r'minimumEvidenceCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.minimumEvidenceCount = valueDes;
          break;
        case r'minimumAreaEvidenceCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.minimumAreaEvidenceCount = valueDes;
          break;
        case r'snapshotHashVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.snapshotHashVersion = valueDes;
          break;
        case r'effectiveFromUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.effectiveFromUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PublishSurveyTemplateVersionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PublishSurveyTemplateVersionRequestBuilder();
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


