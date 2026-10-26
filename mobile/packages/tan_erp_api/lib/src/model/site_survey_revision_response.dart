//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/site_survey_evidence_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/site_survey_area_response.dart';
import 'package:tan_erp_api/src/model/site_survey_checklist_result_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'site_survey_revision_response.g.dart';

/// SiteSurveyRevisionResponse
///
/// Properties:
/// * [id] 
/// * [siteSurveyId] 
/// * [revisionNumber] 
/// * [surveyTemplateVersion] 
/// * [visitedAtUtc] 
/// * [scopeSummary] 
/// * [assumptions] 
/// * [constraints] 
/// * [missingDetails] 
/// * [readiness] 
/// * [status] 
/// * [readyAtUtc] 
/// * [readyByUserId] 
/// * [snapshotHash] 
/// * [rowVersion] 
/// * [createdAtUtc] 
/// * [createdByUserId] 
/// * [areas] 
/// * [checklist] 
/// * [evidence] 
@BuiltValue()
abstract class SiteSurveyRevisionResponse implements Built<SiteSurveyRevisionResponse, SiteSurveyRevisionResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'siteSurveyId')
  String? get siteSurveyId;

  @BuiltValueField(wireName: r'revisionNumber')
  int? get revisionNumber;

  @BuiltValueField(wireName: r'surveyTemplateVersion')
  String? get surveyTemplateVersion;

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

  @BuiltValueField(wireName: r'readiness')
  String? get readiness;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'readyAtUtc')
  DateTime? get readyAtUtc;

  @BuiltValueField(wireName: r'readyByUserId')
  String? get readyByUserId;

  @BuiltValueField(wireName: r'snapshotHash')
  String? get snapshotHash;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'createdByUserId')
  String? get createdByUserId;

  @BuiltValueField(wireName: r'areas')
  BuiltList<SiteSurveyAreaResponse>? get areas;

  @BuiltValueField(wireName: r'checklist')
  BuiltList<SiteSurveyChecklistResultResponse>? get checklist;

  @BuiltValueField(wireName: r'evidence')
  BuiltList<SiteSurveyEvidenceResponse>? get evidence;

  SiteSurveyRevisionResponse._();

  factory SiteSurveyRevisionResponse([void updates(SiteSurveyRevisionResponseBuilder b)]) = _$SiteSurveyRevisionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SiteSurveyRevisionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SiteSurveyRevisionResponse> get serializer => _$SiteSurveyRevisionResponseSerializer();
}

class _$SiteSurveyRevisionResponseSerializer implements PrimitiveSerializer<SiteSurveyRevisionResponse> {
  @override
  final Iterable<Type> types = const [SiteSurveyRevisionResponse, _$SiteSurveyRevisionResponse];

  @override
  final String wireName = r'SiteSurveyRevisionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SiteSurveyRevisionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.siteSurveyId != null) {
      yield r'siteSurveyId';
      yield serializers.serialize(
        object.siteSurveyId,
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
    if (object.surveyTemplateVersion != null) {
      yield r'surveyTemplateVersion';
      yield serializers.serialize(
        object.surveyTemplateVersion,
        specifiedType: const FullType.nullable(String),
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
    if (object.readiness != null) {
      yield r'readiness';
      yield serializers.serialize(
        object.readiness,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.readyAtUtc != null) {
      yield r'readyAtUtc';
      yield serializers.serialize(
        object.readyAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.readyByUserId != null) {
      yield r'readyByUserId';
      yield serializers.serialize(
        object.readyByUserId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.snapshotHash != null) {
      yield r'snapshotHash';
      yield serializers.serialize(
        object.snapshotHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.createdByUserId != null) {
      yield r'createdByUserId';
      yield serializers.serialize(
        object.createdByUserId,
        specifiedType: const FullType(String),
      );
    }
    if (object.areas != null) {
      yield r'areas';
      yield serializers.serialize(
        object.areas,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SiteSurveyAreaResponse)]),
      );
    }
    if (object.checklist != null) {
      yield r'checklist';
      yield serializers.serialize(
        object.checklist,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SiteSurveyChecklistResultResponse)]),
      );
    }
    if (object.evidence != null) {
      yield r'evidence';
      yield serializers.serialize(
        object.evidence,
        specifiedType: const FullType.nullable(BuiltList, [FullType(SiteSurveyEvidenceResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SiteSurveyRevisionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SiteSurveyRevisionResponseBuilder result,
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
        case r'siteSurveyId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.siteSurveyId = valueDes;
          break;
        case r'revisionNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.revisionNumber = valueDes;
          break;
        case r'surveyTemplateVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.surveyTemplateVersion = valueDes;
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
        case r'readiness':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.readiness = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'readyAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.readyAtUtc = valueDes;
          break;
        case r'readyByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.readyByUserId = valueDes;
          break;
        case r'snapshotHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.snapshotHash = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'createdByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.createdByUserId = valueDes;
          break;
        case r'areas':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SiteSurveyAreaResponse)]),
          ) as BuiltList<SiteSurveyAreaResponse>?;
          if (valueDes == null) continue;
          result.areas.replace(valueDes);
          break;
        case r'checklist':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SiteSurveyChecklistResultResponse)]),
          ) as BuiltList<SiteSurveyChecklistResultResponse>?;
          if (valueDes == null) continue;
          result.checklist.replace(valueDes);
          break;
        case r'evidence':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(SiteSurveyEvidenceResponse)]),
          ) as BuiltList<SiteSurveyEvidenceResponse>?;
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
  SiteSurveyRevisionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SiteSurveyRevisionResponseBuilder();
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


