//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/estimate_review_revision_diff_response.dart';
import 'package:tan_erp_api/src/model/estimate_review_price_override_response.dart';
import 'package:tan_erp_api/src/model/estimate_review_frozen_route_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/estimate_review_cost_evidence_response.dart';
import 'package:tan_erp_api/src/model/estimate_review_queue_opportunity_response.dart';
import 'package:tan_erp_api/src/model/estimate_review_queue_branch_response.dart';
import 'package:tan_erp_api/src/model/estimate_review_readiness_reason_response.dart';
import 'package:tan_erp_api/src/model/estimate_review_queue_customer_response.dart';
import 'package:tan_erp_api/src/model/estimate_review_queue_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_review_queue_item_response.g.dart';

/// EstimateReviewQueueItemResponse
///
/// Properties:
/// * [approvalRequestId] 
/// * [estimateId] 
/// * [estimateNumber] 
/// * [estimateRowVersion] 
/// * [revisionId] 
/// * [revisionNo] 
/// * [revisionStatus] 
/// * [calculationVersion] 
/// * [currency] 
/// * [grandTotal] 
/// * [marginRate] 
/// * [opportunityId] 
/// * [opportunity] 
/// * [customer] 
/// * [branch] 
/// * [requestedBy] 
/// * [requestedAtUtc] 
/// * [submissionNote] 
/// * [calculationInputHash] 
/// * [calculationSnapshotHash] 
/// * [frozenRoute] 
/// * [readiness] 
/// * [readinessReasons] 
/// * [priceOverrides] 
/// * [costEvidence] 
/// * [revisionDiff] 
/// * [provisionalCostCount] 
@BuiltValue()
abstract class EstimateReviewQueueItemResponse implements Built<EstimateReviewQueueItemResponse, EstimateReviewQueueItemResponseBuilder> {
  @BuiltValueField(wireName: r'approvalRequestId')
  String? get approvalRequestId;

  @BuiltValueField(wireName: r'estimateId')
  String? get estimateId;

  @BuiltValueField(wireName: r'estimateNumber')
  String? get estimateNumber;

  @BuiltValueField(wireName: r'estimateRowVersion')
  String? get estimateRowVersion;

  @BuiltValueField(wireName: r'revisionId')
  String? get revisionId;

  @BuiltValueField(wireName: r'revisionNo')
  int? get revisionNo;

  @BuiltValueField(wireName: r'revisionStatus')
  String? get revisionStatus;

  @BuiltValueField(wireName: r'calculationVersion')
  int? get calculationVersion;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'grandTotal')
  double? get grandTotal;

  @BuiltValueField(wireName: r'marginRate')
  double? get marginRate;

  @BuiltValueField(wireName: r'opportunityId')
  String? get opportunityId;

  @BuiltValueField(wireName: r'opportunity')
  EstimateReviewQueueOpportunityResponse? get opportunity;

  @BuiltValueField(wireName: r'customer')
  EstimateReviewQueueCustomerResponse? get customer;

  @BuiltValueField(wireName: r'branch')
  EstimateReviewQueueBranchResponse? get branch;

  @BuiltValueField(wireName: r'requestedBy')
  EstimateReviewQueuePersonResponse? get requestedBy;

  @BuiltValueField(wireName: r'requestedAtUtc')
  DateTime? get requestedAtUtc;

  @BuiltValueField(wireName: r'submissionNote')
  String? get submissionNote;

  @BuiltValueField(wireName: r'calculationInputHash')
  String? get calculationInputHash;

  @BuiltValueField(wireName: r'calculationSnapshotHash')
  String? get calculationSnapshotHash;

  @BuiltValueField(wireName: r'frozenRoute')
  EstimateReviewFrozenRouteResponse? get frozenRoute;

  @BuiltValueField(wireName: r'readiness')
  String? get readiness;

  @BuiltValueField(wireName: r'readinessReasons')
  BuiltList<EstimateReviewReadinessReasonResponse>? get readinessReasons;

  @BuiltValueField(wireName: r'priceOverrides')
  BuiltList<EstimateReviewPriceOverrideResponse>? get priceOverrides;

  @BuiltValueField(wireName: r'costEvidence')
  BuiltList<EstimateReviewCostEvidenceResponse>? get costEvidence;

  @BuiltValueField(wireName: r'revisionDiff')
  EstimateReviewRevisionDiffResponse? get revisionDiff;

  @BuiltValueField(wireName: r'provisionalCostCount')
  int? get provisionalCostCount;

  EstimateReviewQueueItemResponse._();

  factory EstimateReviewQueueItemResponse([void updates(EstimateReviewQueueItemResponseBuilder b)]) = _$EstimateReviewQueueItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateReviewQueueItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateReviewQueueItemResponse> get serializer => _$EstimateReviewQueueItemResponseSerializer();
}

class _$EstimateReviewQueueItemResponseSerializer implements PrimitiveSerializer<EstimateReviewQueueItemResponse> {
  @override
  final Iterable<Type> types = const [EstimateReviewQueueItemResponse, _$EstimateReviewQueueItemResponse];

  @override
  final String wireName = r'EstimateReviewQueueItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateReviewQueueItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.approvalRequestId != null) {
      yield r'approvalRequestId';
      yield serializers.serialize(
        object.approvalRequestId,
        specifiedType: const FullType(String),
      );
    }
    if (object.estimateId != null) {
      yield r'estimateId';
      yield serializers.serialize(
        object.estimateId,
        specifiedType: const FullType(String),
      );
    }
    if (object.estimateNumber != null) {
      yield r'estimateNumber';
      yield serializers.serialize(
        object.estimateNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.estimateRowVersion != null) {
      yield r'estimateRowVersion';
      yield serializers.serialize(
        object.estimateRowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.revisionId != null) {
      yield r'revisionId';
      yield serializers.serialize(
        object.revisionId,
        specifiedType: const FullType(String),
      );
    }
    if (object.revisionNo != null) {
      yield r'revisionNo';
      yield serializers.serialize(
        object.revisionNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.revisionStatus != null) {
      yield r'revisionStatus';
      yield serializers.serialize(
        object.revisionStatus,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.calculationVersion != null) {
      yield r'calculationVersion';
      yield serializers.serialize(
        object.calculationVersion,
        specifiedType: const FullType(int),
      );
    }
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.grandTotal != null) {
      yield r'grandTotal';
      yield serializers.serialize(
        object.grandTotal,
        specifiedType: const FullType(double),
      );
    }
    if (object.marginRate != null) {
      yield r'marginRate';
      yield serializers.serialize(
        object.marginRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.opportunityId != null) {
      yield r'opportunityId';
      yield serializers.serialize(
        object.opportunityId,
        specifiedType: const FullType(String),
      );
    }
    if (object.opportunity != null) {
      yield r'opportunity';
      yield serializers.serialize(
        object.opportunity,
        specifiedType: const FullType(EstimateReviewQueueOpportunityResponse),
      );
    }
    if (object.customer != null) {
      yield r'customer';
      yield serializers.serialize(
        object.customer,
        specifiedType: const FullType(EstimateReviewQueueCustomerResponse),
      );
    }
    if (object.branch != null) {
      yield r'branch';
      yield serializers.serialize(
        object.branch,
        specifiedType: const FullType(EstimateReviewQueueBranchResponse),
      );
    }
    if (object.requestedBy != null) {
      yield r'requestedBy';
      yield serializers.serialize(
        object.requestedBy,
        specifiedType: const FullType(EstimateReviewQueuePersonResponse),
      );
    }
    if (object.requestedAtUtc != null) {
      yield r'requestedAtUtc';
      yield serializers.serialize(
        object.requestedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.submissionNote != null) {
      yield r'submissionNote';
      yield serializers.serialize(
        object.submissionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.calculationInputHash != null) {
      yield r'calculationInputHash';
      yield serializers.serialize(
        object.calculationInputHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.calculationSnapshotHash != null) {
      yield r'calculationSnapshotHash';
      yield serializers.serialize(
        object.calculationSnapshotHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.frozenRoute != null) {
      yield r'frozenRoute';
      yield serializers.serialize(
        object.frozenRoute,
        specifiedType: const FullType(EstimateReviewFrozenRouteResponse),
      );
    }
    if (object.readiness != null) {
      yield r'readiness';
      yield serializers.serialize(
        object.readiness,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.readinessReasons != null) {
      yield r'readinessReasons';
      yield serializers.serialize(
        object.readinessReasons,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReviewReadinessReasonResponse)]),
      );
    }
    if (object.priceOverrides != null) {
      yield r'priceOverrides';
      yield serializers.serialize(
        object.priceOverrides,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReviewPriceOverrideResponse)]),
      );
    }
    if (object.costEvidence != null) {
      yield r'costEvidence';
      yield serializers.serialize(
        object.costEvidence,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReviewCostEvidenceResponse)]),
      );
    }
    if (object.revisionDiff != null) {
      yield r'revisionDiff';
      yield serializers.serialize(
        object.revisionDiff,
        specifiedType: const FullType(EstimateReviewRevisionDiffResponse),
      );
    }
    if (object.provisionalCostCount != null) {
      yield r'provisionalCostCount';
      yield serializers.serialize(
        object.provisionalCostCount,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateReviewQueueItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateReviewQueueItemResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'approvalRequestId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.approvalRequestId = valueDes;
          break;
        case r'estimateId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateId = valueDes;
          break;
        case r'estimateNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateNumber = valueDes;
          break;
        case r'estimateRowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateRowVersion = valueDes;
          break;
        case r'revisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.revisionId = valueDes;
          break;
        case r'revisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.revisionNo = valueDes;
          break;
        case r'revisionStatus':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.revisionStatus = valueDes;
          break;
        case r'calculationVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.calculationVersion = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        case r'grandTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.grandTotal = valueDes;
          break;
        case r'marginRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.marginRate = valueDes;
          break;
        case r'opportunityId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.opportunityId = valueDes;
          break;
        case r'opportunity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateReviewQueueOpportunityResponse),
          ) as EstimateReviewQueueOpportunityResponse?;
          if (valueDes == null) continue;
          result.opportunity.replace(valueDes);
          break;
        case r'customer':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateReviewQueueCustomerResponse),
          ) as EstimateReviewQueueCustomerResponse?;
          if (valueDes == null) continue;
          result.customer.replace(valueDes);
          break;
        case r'branch':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateReviewQueueBranchResponse),
          ) as EstimateReviewQueueBranchResponse?;
          if (valueDes == null) continue;
          result.branch.replace(valueDes);
          break;
        case r'requestedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateReviewQueuePersonResponse),
          ) as EstimateReviewQueuePersonResponse?;
          if (valueDes == null) continue;
          result.requestedBy.replace(valueDes);
          break;
        case r'requestedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.requestedAtUtc = valueDes;
          break;
        case r'submissionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.submissionNote = valueDes;
          break;
        case r'calculationInputHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.calculationInputHash = valueDes;
          break;
        case r'calculationSnapshotHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.calculationSnapshotHash = valueDes;
          break;
        case r'frozenRoute':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateReviewFrozenRouteResponse),
          ) as EstimateReviewFrozenRouteResponse?;
          if (valueDes == null) continue;
          result.frozenRoute.replace(valueDes);
          break;
        case r'readiness':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.readiness = valueDes;
          break;
        case r'readinessReasons':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReviewReadinessReasonResponse)]),
          ) as BuiltList<EstimateReviewReadinessReasonResponse>?;
          if (valueDes == null) continue;
          result.readinessReasons.replace(valueDes);
          break;
        case r'priceOverrides':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReviewPriceOverrideResponse)]),
          ) as BuiltList<EstimateReviewPriceOverrideResponse>?;
          if (valueDes == null) continue;
          result.priceOverrides.replace(valueDes);
          break;
        case r'costEvidence':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReviewCostEvidenceResponse)]),
          ) as BuiltList<EstimateReviewCostEvidenceResponse>?;
          if (valueDes == null) continue;
          result.costEvidence.replace(valueDes);
          break;
        case r'revisionDiff':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateReviewRevisionDiffResponse),
          ) as EstimateReviewRevisionDiffResponse?;
          if (valueDes == null) continue;
          result.revisionDiff.replace(valueDes);
          break;
        case r'provisionalCostCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.provisionalCostCount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateReviewQueueItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateReviewQueueItemResponseBuilder();
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


