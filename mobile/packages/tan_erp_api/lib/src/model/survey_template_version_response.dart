//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'survey_template_version_response.g.dart';

/// SurveyTemplateVersionResponse
///
/// Properties:
/// * [code] 
/// * [requiredChecklistItems] 
/// * [minimumEvidenceCount] 
/// * [minimumAreaEvidenceCount] 
/// * [snapshotHashVersion] 
/// * [effectiveFromUtc] 
/// * [effectiveToUtc] 
/// * [isCurrent] 
@BuiltValue()
abstract class SurveyTemplateVersionResponse implements Built<SurveyTemplateVersionResponse, SurveyTemplateVersionResponseBuilder> {
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

  @BuiltValueField(wireName: r'effectiveToUtc')
  DateTime? get effectiveToUtc;

  @BuiltValueField(wireName: r'isCurrent')
  bool? get isCurrent;

  SurveyTemplateVersionResponse._();

  factory SurveyTemplateVersionResponse([void updates(SurveyTemplateVersionResponseBuilder b)]) = _$SurveyTemplateVersionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SurveyTemplateVersionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SurveyTemplateVersionResponse> get serializer => _$SurveyTemplateVersionResponseSerializer();
}

class _$SurveyTemplateVersionResponseSerializer implements PrimitiveSerializer<SurveyTemplateVersionResponse> {
  @override
  final Iterable<Type> types = const [SurveyTemplateVersionResponse, _$SurveyTemplateVersionResponse];

  @override
  final String wireName = r'SurveyTemplateVersionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SurveyTemplateVersionResponse object, {
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
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.effectiveToUtc != null) {
      yield r'effectiveToUtc';
      yield serializers.serialize(
        object.effectiveToUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.isCurrent != null) {
      yield r'isCurrent';
      yield serializers.serialize(
        object.isCurrent,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SurveyTemplateVersionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SurveyTemplateVersionResponseBuilder result,
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
        case r'effectiveToUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.effectiveToUtc = valueDes;
          break;
        case r'isCurrent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isCurrent = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SurveyTemplateVersionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SurveyTemplateVersionResponseBuilder();
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


