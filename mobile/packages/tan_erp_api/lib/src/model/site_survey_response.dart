//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/site_summary_response.dart';
import 'package:tan_erp_api/src/model/site_survey_ready_revision_response.dart';
import 'package:tan_erp_api/src/model/site_survey_revision_response.dart';
import 'package:tan_erp_api/src/model/surveyor_summary_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'site_survey_response.g.dart';

/// SiteSurveyResponse
///
/// Properties:
/// * [id] 
/// * [organizationId] 
/// * [branchId] 
/// * [opportunityId] 
/// * [siteId] 
/// * [surveyNumber] 
/// * [assignedSurveyorId] 
/// * [scheduledStartUtc] 
/// * [scheduledEndUtc] 
/// * [status] 
/// * [rowVersion] 
/// * [createdAtUtc] 
/// * [createdByUserId] 
/// * [currentRevision] 
/// * [assignedSurveyor] 
/// * [site] 
/// * [latestReadyRevision] 
@BuiltValue()
abstract class SiteSurveyResponse implements Built<SiteSurveyResponse, SiteSurveyResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'organizationId')
  String? get organizationId;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'opportunityId')
  String? get opportunityId;

  @BuiltValueField(wireName: r'siteId')
  String? get siteId;

  @BuiltValueField(wireName: r'surveyNumber')
  String? get surveyNumber;

  @BuiltValueField(wireName: r'assignedSurveyorId')
  String? get assignedSurveyorId;

  @BuiltValueField(wireName: r'scheduledStartUtc')
  DateTime? get scheduledStartUtc;

  @BuiltValueField(wireName: r'scheduledEndUtc')
  DateTime? get scheduledEndUtc;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'createdByUserId')
  String? get createdByUserId;

  @BuiltValueField(wireName: r'currentRevision')
  SiteSurveyRevisionResponse? get currentRevision;

  @BuiltValueField(wireName: r'assignedSurveyor')
  SurveyorSummaryResponse? get assignedSurveyor;

  @BuiltValueField(wireName: r'site')
  SiteSummaryResponse? get site;

  @BuiltValueField(wireName: r'latestReadyRevision')
  SiteSurveyReadyRevisionResponse? get latestReadyRevision;

  SiteSurveyResponse._();

  factory SiteSurveyResponse([void updates(SiteSurveyResponseBuilder b)]) = _$SiteSurveyResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SiteSurveyResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SiteSurveyResponse> get serializer => _$SiteSurveyResponseSerializer();
}

class _$SiteSurveyResponseSerializer implements PrimitiveSerializer<SiteSurveyResponse> {
  @override
  final Iterable<Type> types = const [SiteSurveyResponse, _$SiteSurveyResponse];

  @override
  final String wireName = r'SiteSurveyResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SiteSurveyResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.organizationId != null) {
      yield r'organizationId';
      yield serializers.serialize(
        object.organizationId,
        specifiedType: const FullType(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
        specifiedType: const FullType(String),
      );
    }
    if (object.opportunityId != null) {
      yield r'opportunityId';
      yield serializers.serialize(
        object.opportunityId,
        specifiedType: const FullType(String),
      );
    }
    if (object.siteId != null) {
      yield r'siteId';
      yield serializers.serialize(
        object.siteId,
        specifiedType: const FullType(String),
      );
    }
    if (object.surveyNumber != null) {
      yield r'surveyNumber';
      yield serializers.serialize(
        object.surveyNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.assignedSurveyorId != null) {
      yield r'assignedSurveyorId';
      yield serializers.serialize(
        object.assignedSurveyorId,
        specifiedType: const FullType(String),
      );
    }
    if (object.scheduledStartUtc != null) {
      yield r'scheduledStartUtc';
      yield serializers.serialize(
        object.scheduledStartUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.scheduledEndUtc != null) {
      yield r'scheduledEndUtc';
      yield serializers.serialize(
        object.scheduledEndUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
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
    if (object.currentRevision != null) {
      yield r'currentRevision';
      yield serializers.serialize(
        object.currentRevision,
        specifiedType: const FullType(SiteSurveyRevisionResponse),
      );
    }
    if (object.assignedSurveyor != null) {
      yield r'assignedSurveyor';
      yield serializers.serialize(
        object.assignedSurveyor,
        specifiedType: const FullType(SurveyorSummaryResponse),
      );
    }
    if (object.site != null) {
      yield r'site';
      yield serializers.serialize(
        object.site,
        specifiedType: const FullType(SiteSummaryResponse),
      );
    }
    if (object.latestReadyRevision != null) {
      yield r'latestReadyRevision';
      yield serializers.serialize(
        object.latestReadyRevision,
        specifiedType: const FullType(SiteSurveyReadyRevisionResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SiteSurveyResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SiteSurveyResponseBuilder result,
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
        case r'organizationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.organizationId = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'opportunityId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityId = valueDes;
          break;
        case r'siteId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.siteId = valueDes;
          break;
        case r'surveyNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.surveyNumber = valueDes;
          break;
        case r'assignedSurveyorId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.assignedSurveyorId = valueDes;
          break;
        case r'scheduledStartUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.scheduledStartUtc = valueDes;
          break;
        case r'scheduledEndUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.scheduledEndUtc = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
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
        case r'currentRevision':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(SiteSurveyRevisionResponse),
          ) as SiteSurveyRevisionResponse?;
          if (valueDes == null) continue;
          result.currentRevision.replace(valueDes);
          break;
        case r'assignedSurveyor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(SurveyorSummaryResponse),
          ) as SurveyorSummaryResponse?;
          if (valueDes == null) continue;
          result.assignedSurveyor.replace(valueDes);
          break;
        case r'site':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(SiteSummaryResponse),
          ) as SiteSummaryResponse?;
          if (valueDes == null) continue;
          result.site.replace(valueDes);
          break;
        case r'latestReadyRevision':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(SiteSurveyReadyRevisionResponse),
          ) as SiteSurveyReadyRevisionResponse?;
          if (valueDes == null) continue;
          result.latestReadyRevision.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SiteSurveyResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SiteSurveyResponseBuilder();
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


