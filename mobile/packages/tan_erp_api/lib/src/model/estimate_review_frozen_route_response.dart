//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/estimate_review_approval_trigger_response.dart';
import 'package:tan_erp_api/src/model/estimate_review_queue_reviewer_response.dart';
import 'package:tan_erp_api/src/model/estimate_review_approval_thresholds_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_review_frozen_route_response.g.dart';

/// EstimateReviewFrozenRouteResponse
///
/// Properties:
/// * [policyCode] 
/// * [policyVersion] 
/// * [routeHash] 
/// * [scopeType] 
/// * [scopeId] 
/// * [permissionKey] 
/// * [reviewer] 
/// * [triggers] 
/// * [thresholds] 
@BuiltValue()
abstract class EstimateReviewFrozenRouteResponse implements Built<EstimateReviewFrozenRouteResponse, EstimateReviewFrozenRouteResponseBuilder> {
  @BuiltValueField(wireName: r'policyCode')
  String? get policyCode;

  @BuiltValueField(wireName: r'policyVersion')
  int? get policyVersion;

  @BuiltValueField(wireName: r'routeHash')
  String? get routeHash;

  @BuiltValueField(wireName: r'scopeType')
  String? get scopeType;

  @BuiltValueField(wireName: r'scopeId')
  String? get scopeId;

  @BuiltValueField(wireName: r'permissionKey')
  String? get permissionKey;

  @BuiltValueField(wireName: r'reviewer')
  EstimateReviewQueueReviewerResponse? get reviewer;

  @BuiltValueField(wireName: r'triggers')
  BuiltList<EstimateReviewApprovalTriggerResponse>? get triggers;

  @BuiltValueField(wireName: r'thresholds')
  EstimateReviewApprovalThresholdsResponse? get thresholds;

  EstimateReviewFrozenRouteResponse._();

  factory EstimateReviewFrozenRouteResponse([void updates(EstimateReviewFrozenRouteResponseBuilder b)]) = _$EstimateReviewFrozenRouteResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateReviewFrozenRouteResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateReviewFrozenRouteResponse> get serializer => _$EstimateReviewFrozenRouteResponseSerializer();
}

class _$EstimateReviewFrozenRouteResponseSerializer implements PrimitiveSerializer<EstimateReviewFrozenRouteResponse> {
  @override
  final Iterable<Type> types = const [EstimateReviewFrozenRouteResponse, _$EstimateReviewFrozenRouteResponse];

  @override
  final String wireName = r'EstimateReviewFrozenRouteResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateReviewFrozenRouteResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.policyCode != null) {
      yield r'policyCode';
      yield serializers.serialize(
        object.policyCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.policyVersion != null) {
      yield r'policyVersion';
      yield serializers.serialize(
        object.policyVersion,
        specifiedType: const FullType(int),
      );
    }
    if (object.routeHash != null) {
      yield r'routeHash';
      yield serializers.serialize(
        object.routeHash,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.scopeType != null) {
      yield r'scopeType';
      yield serializers.serialize(
        object.scopeType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.scopeId != null) {
      yield r'scopeId';
      yield serializers.serialize(
        object.scopeId,
        specifiedType: const FullType(String),
      );
    }
    if (object.permissionKey != null) {
      yield r'permissionKey';
      yield serializers.serialize(
        object.permissionKey,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reviewer != null) {
      yield r'reviewer';
      yield serializers.serialize(
        object.reviewer,
        specifiedType: const FullType(EstimateReviewQueueReviewerResponse),
      );
    }
    if (object.triggers != null) {
      yield r'triggers';
      yield serializers.serialize(
        object.triggers,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReviewApprovalTriggerResponse)]),
      );
    }
    if (object.thresholds != null) {
      yield r'thresholds';
      yield serializers.serialize(
        object.thresholds,
        specifiedType: const FullType(EstimateReviewApprovalThresholdsResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateReviewFrozenRouteResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateReviewFrozenRouteResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'policyCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.policyCode = valueDes;
          break;
        case r'policyVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.policyVersion = valueDes;
          break;
        case r'routeHash':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.routeHash = valueDes;
          break;
        case r'scopeType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.scopeType = valueDes;
          break;
        case r'scopeId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.scopeId = valueDes;
          break;
        case r'permissionKey':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.permissionKey = valueDes;
          break;
        case r'reviewer':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateReviewQueueReviewerResponse),
          ) as EstimateReviewQueueReviewerResponse?;
          if (valueDes == null) continue;
          result.reviewer.replace(valueDes);
          break;
        case r'triggers':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReviewApprovalTriggerResponse)]),
          ) as BuiltList<EstimateReviewApprovalTriggerResponse>?;
          if (valueDes == null) continue;
          result.triggers.replace(valueDes);
          break;
        case r'thresholds':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateReviewApprovalThresholdsResponse),
          ) as EstimateReviewApprovalThresholdsResponse?;
          if (valueDes == null) continue;
          result.thresholds.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateReviewFrozenRouteResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateReviewFrozenRouteResponseBuilder();
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


