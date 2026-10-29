//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/site_summary_response.dart';
import 'package:tan_erp_api/src/model/site_survey_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'assigned_survey_sync_item_response.g.dart';

/// AssignedSurveySyncItemResponse
///
/// Properties:
/// * [opportunityId] 
/// * [opportunityCode] 
/// * [opportunityTitle] 
/// * [opportunityStage] 
/// * [opportunityRowVersion] 
/// * [customerDisplayName] 
/// * [site] 
/// * [survey] 
@BuiltValue()
abstract class AssignedSurveySyncItemResponse implements Built<AssignedSurveySyncItemResponse, AssignedSurveySyncItemResponseBuilder> {
  @BuiltValueField(wireName: r'opportunityId')
  String? get opportunityId;

  @BuiltValueField(wireName: r'opportunityCode')
  String? get opportunityCode;

  @BuiltValueField(wireName: r'opportunityTitle')
  String? get opportunityTitle;

  @BuiltValueField(wireName: r'opportunityStage')
  String? get opportunityStage;

  @BuiltValueField(wireName: r'opportunityRowVersion')
  String? get opportunityRowVersion;

  @BuiltValueField(wireName: r'customerDisplayName')
  String? get customerDisplayName;

  @BuiltValueField(wireName: r'site')
  SiteSummaryResponse? get site;

  @BuiltValueField(wireName: r'survey')
  SiteSurveyResponse? get survey;

  AssignedSurveySyncItemResponse._();

  factory AssignedSurveySyncItemResponse([void updates(AssignedSurveySyncItemResponseBuilder b)]) = _$AssignedSurveySyncItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AssignedSurveySyncItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AssignedSurveySyncItemResponse> get serializer => _$AssignedSurveySyncItemResponseSerializer();
}

class _$AssignedSurveySyncItemResponseSerializer implements PrimitiveSerializer<AssignedSurveySyncItemResponse> {
  @override
  final Iterable<Type> types = const [AssignedSurveySyncItemResponse, _$AssignedSurveySyncItemResponse];

  @override
  final String wireName = r'AssignedSurveySyncItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AssignedSurveySyncItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.opportunityId != null) {
      yield r'opportunityId';
      yield serializers.serialize(
        object.opportunityId,
        specifiedType: const FullType(String),
      );
    }
    if (object.opportunityCode != null) {
      yield r'opportunityCode';
      yield serializers.serialize(
        object.opportunityCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.opportunityTitle != null) {
      yield r'opportunityTitle';
      yield serializers.serialize(
        object.opportunityTitle,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.opportunityStage != null) {
      yield r'opportunityStage';
      yield serializers.serialize(
        object.opportunityStage,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.opportunityRowVersion != null) {
      yield r'opportunityRowVersion';
      yield serializers.serialize(
        object.opportunityRowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.customerDisplayName != null) {
      yield r'customerDisplayName';
      yield serializers.serialize(
        object.customerDisplayName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.site != null) {
      yield r'site';
      yield serializers.serialize(
        object.site,
        specifiedType: const FullType(SiteSummaryResponse),
      );
    }
    if (object.survey != null) {
      yield r'survey';
      yield serializers.serialize(
        object.survey,
        specifiedType: const FullType(SiteSurveyResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AssignedSurveySyncItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AssignedSurveySyncItemResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'opportunityId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityId = valueDes;
          break;
        case r'opportunityCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityCode = valueDes;
          break;
        case r'opportunityTitle':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityTitle = valueDes;
          break;
        case r'opportunityStage':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityStage = valueDes;
          break;
        case r'opportunityRowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityRowVersion = valueDes;
          break;
        case r'customerDisplayName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerDisplayName = valueDes;
          break;
        case r'site':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(SiteSummaryResponse),
          ) as SiteSummaryResponse?;
          if (valueDes == null) continue;
          result.site.replace(valueDes);
          break;
        case r'survey':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(SiteSurveyResponse),
          ) as SiteSurveyResponse?;
          if (valueDes == null) continue;
          result.survey.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AssignedSurveySyncItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AssignedSurveySyncItemResponseBuilder();
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


