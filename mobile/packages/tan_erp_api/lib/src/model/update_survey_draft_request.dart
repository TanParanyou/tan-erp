//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/update_survey_evidence_request.dart';
import 'package:tan_erp_api/src/model/update_survey_area_request.dart';
import 'package:tan_erp_api/src/model/update_survey_checklist_request.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_survey_draft_request.g.dart';

/// UpdateSurveyDraftRequest
///
/// Properties:
/// * [expectedRevisionVersion] 
/// * [visitedAtUtc] 
/// * [scopeSummary] 
/// * [assumptions] 
/// * [constraints] 
/// * [missingDetails] 
/// * [areas] 
/// * [checklist] 
/// * [evidence] 
@BuiltValue()
abstract class UpdateSurveyDraftRequest implements Built<UpdateSurveyDraftRequest, UpdateSurveyDraftRequestBuilder> {
  @BuiltValueField(wireName: r'expectedRevisionVersion')
  String? get expectedRevisionVersion;

  @BuiltValueField(wireName: r'visitedAtUtc')
  DateTime? get visitedAtUtc;

  @BuiltValueField(wireName: r'scopeSummary')
  String? get scopeSummary;

  @BuiltValueField(wireName: r'assumptions')
  BuiltList<String>? get assumptions;

  @BuiltValueField(wireName: r'constraints')
  BuiltList<String>? get constraints;

  @BuiltValueField(wireName: r'missingDetails')
  BuiltList<String>? get missingDetails;

  @BuiltValueField(wireName: r'areas')
  BuiltList<UpdateSurveyAreaRequest>? get areas;

  @BuiltValueField(wireName: r'checklist')
  BuiltList<UpdateSurveyChecklistRequest>? get checklist;

  @BuiltValueField(wireName: r'evidence')
  BuiltList<UpdateSurveyEvidenceRequest>? get evidence;

  UpdateSurveyDraftRequest._();

  factory UpdateSurveyDraftRequest([void updates(UpdateSurveyDraftRequestBuilder b)]) = _$UpdateSurveyDraftRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateSurveyDraftRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateSurveyDraftRequest> get serializer => _$UpdateSurveyDraftRequestSerializer();
}

class _$UpdateSurveyDraftRequestSerializer implements PrimitiveSerializer<UpdateSurveyDraftRequest> {
  @override
  final Iterable<Type> types = const [UpdateSurveyDraftRequest, _$UpdateSurveyDraftRequest];

  @override
  final String wireName = r'UpdateSurveyDraftRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateSurveyDraftRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.expectedRevisionVersion != null) {
      yield r'expectedRevisionVersion';
      yield serializers.serialize(
        object.expectedRevisionVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.visitedAtUtc != null) {
      yield r'visitedAtUtc';
      yield serializers.serialize(
        object.visitedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.scopeSummary != null) {
      yield r'scopeSummary';
      yield serializers.serialize(
        object.scopeSummary,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.assumptions != null) {
      yield r'assumptions';
      yield serializers.serialize(
        object.assumptions,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.constraints != null) {
      yield r'constraints';
      yield serializers.serialize(
        object.constraints,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.missingDetails != null) {
      yield r'missingDetails';
      yield serializers.serialize(
        object.missingDetails,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.areas != null) {
      yield r'areas';
      yield serializers.serialize(
        object.areas,
        specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateSurveyAreaRequest)]),
      );
    }
    if (object.checklist != null) {
      yield r'checklist';
      yield serializers.serialize(
        object.checklist,
        specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateSurveyChecklistRequest)]),
      );
    }
    if (object.evidence != null) {
      yield r'evidence';
      yield serializers.serialize(
        object.evidence,
        specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateSurveyEvidenceRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateSurveyDraftRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateSurveyDraftRequestBuilder result,
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
        case r'visitedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.visitedAtUtc = valueDes;
          break;
        case r'scopeSummary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.scopeSummary = valueDes;
          break;
        case r'assumptions':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.assumptions.replace(valueDes);
          break;
        case r'constraints':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.constraints.replace(valueDes);
          break;
        case r'missingDetails':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.missingDetails.replace(valueDes);
          break;
        case r'areas':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateSurveyAreaRequest)]),
          ) as BuiltList<UpdateSurveyAreaRequest>?;
          if (valueDes == null) continue;
          result.areas.replace(valueDes);
          break;
        case r'checklist':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateSurveyChecklistRequest)]),
          ) as BuiltList<UpdateSurveyChecklistRequest>?;
          if (valueDes == null) continue;
          result.checklist.replace(valueDes);
          break;
        case r'evidence':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateSurveyEvidenceRequest)]),
          ) as BuiltList<UpdateSurveyEvidenceRequest>?;
          if (valueDes == null) continue;
          result.evidence.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateSurveyDraftRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateSurveyDraftRequestBuilder();
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


