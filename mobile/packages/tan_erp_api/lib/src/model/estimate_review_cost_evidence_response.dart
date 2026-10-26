//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/estimate_conversion_snapshot_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_review_cost_evidence_response.g.dart';

/// EstimateReviewCostEvidenceResponse
///
/// Properties:
/// * [workItemCode] 
/// * [workItemDescription] 
/// * [type] 
/// * [description] 
/// * [quantity] 
/// * [unitCode] 
/// * [unitCost] 
/// * [currency] 
/// * [totalCost] 
/// * [costOrigin] 
/// * [itemCode] 
/// * [costRecordVersion] 
/// * [costSourceCode] 
/// * [sourceReference] 
/// * [evidenceFileId] 
/// * [costRecordReason] 
/// * [provisionalReasonCode] 
/// * [provisionalNote] 
/// * [isProvisional] 
/// * [conversion] 
@BuiltValue()
abstract class EstimateReviewCostEvidenceResponse implements Built<EstimateReviewCostEvidenceResponse, EstimateReviewCostEvidenceResponseBuilder> {
  @BuiltValueField(wireName: r'workItemCode')
  String? get workItemCode;

  @BuiltValueField(wireName: r'workItemDescription')
  String? get workItemDescription;

  @BuiltValueField(wireName: r'type')
  String? get type;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  @BuiltValueField(wireName: r'unitCost')
  double? get unitCost;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'totalCost')
  double? get totalCost;

  @BuiltValueField(wireName: r'costOrigin')
  String? get costOrigin;

  @BuiltValueField(wireName: r'itemCode')
  String? get itemCode;

  @BuiltValueField(wireName: r'costRecordVersion')
  int? get costRecordVersion;

  @BuiltValueField(wireName: r'costSourceCode')
  String? get costSourceCode;

  @BuiltValueField(wireName: r'sourceReference')
  String? get sourceReference;

  @BuiltValueField(wireName: r'evidenceFileId')
  String? get evidenceFileId;

  @BuiltValueField(wireName: r'costRecordReason')
  String? get costRecordReason;

  @BuiltValueField(wireName: r'provisionalReasonCode')
  String? get provisionalReasonCode;

  @BuiltValueField(wireName: r'provisionalNote')
  String? get provisionalNote;

  @BuiltValueField(wireName: r'isProvisional')
  bool? get isProvisional;

  @BuiltValueField(wireName: r'conversion')
  EstimateConversionSnapshotResponse? get conversion;

  EstimateReviewCostEvidenceResponse._();

  factory EstimateReviewCostEvidenceResponse([void updates(EstimateReviewCostEvidenceResponseBuilder b)]) = _$EstimateReviewCostEvidenceResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateReviewCostEvidenceResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateReviewCostEvidenceResponse> get serializer => _$EstimateReviewCostEvidenceResponseSerializer();
}

class _$EstimateReviewCostEvidenceResponseSerializer implements PrimitiveSerializer<EstimateReviewCostEvidenceResponse> {
  @override
  final Iterable<Type> types = const [EstimateReviewCostEvidenceResponse, _$EstimateReviewCostEvidenceResponse];

  @override
  final String wireName = r'EstimateReviewCostEvidenceResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateReviewCostEvidenceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.workItemCode != null) {
      yield r'workItemCode';
      yield serializers.serialize(
        object.workItemCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.workItemDescription != null) {
      yield r'workItemDescription';
      yield serializers.serialize(
        object.workItemDescription,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.type != null) {
      yield r'type';
      yield serializers.serialize(
        object.type,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.unitCode != null) {
      yield r'unitCode';
      yield serializers.serialize(
        object.unitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitCost != null) {
      yield r'unitCost';
      yield serializers.serialize(
        object.unitCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.totalCost != null) {
      yield r'totalCost';
      yield serializers.serialize(
        object.totalCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.costOrigin != null) {
      yield r'costOrigin';
      yield serializers.serialize(
        object.costOrigin,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.itemCode != null) {
      yield r'itemCode';
      yield serializers.serialize(
        object.itemCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costRecordVersion != null) {
      yield r'costRecordVersion';
      yield serializers.serialize(
        object.costRecordVersion,
        specifiedType: const FullType.nullable(int),
      );
    }
    if (object.costSourceCode != null) {
      yield r'costSourceCode';
      yield serializers.serialize(
        object.costSourceCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sourceReference != null) {
      yield r'sourceReference';
      yield serializers.serialize(
        object.sourceReference,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.evidenceFileId != null) {
      yield r'evidenceFileId';
      yield serializers.serialize(
        object.evidenceFileId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costRecordReason != null) {
      yield r'costRecordReason';
      yield serializers.serialize(
        object.costRecordReason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.provisionalReasonCode != null) {
      yield r'provisionalReasonCode';
      yield serializers.serialize(
        object.provisionalReasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.provisionalNote != null) {
      yield r'provisionalNote';
      yield serializers.serialize(
        object.provisionalNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isProvisional != null) {
      yield r'isProvisional';
      yield serializers.serialize(
        object.isProvisional,
        specifiedType: const FullType(bool),
      );
    }
    if (object.conversion != null) {
      yield r'conversion';
      yield serializers.serialize(
        object.conversion,
        specifiedType: const FullType(EstimateConversionSnapshotResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateReviewCostEvidenceResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateReviewCostEvidenceResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'workItemCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workItemCode = valueDes;
          break;
        case r'workItemDescription':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.workItemDescription = valueDes;
          break;
        case r'type':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.type = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        case r'unitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitCode = valueDes;
          break;
        case r'unitCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitCost = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        case r'totalCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalCost = valueDes;
          break;
        case r'costOrigin':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costOrigin = valueDes;
          break;
        case r'itemCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemCode = valueDes;
          break;
        case r'costRecordVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.costRecordVersion = valueDes;
          break;
        case r'costSourceCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costSourceCode = valueDes;
          break;
        case r'sourceReference':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceReference = valueDes;
          break;
        case r'evidenceFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.evidenceFileId = valueDes;
          break;
        case r'costRecordReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costRecordReason = valueDes;
          break;
        case r'provisionalReasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.provisionalReasonCode = valueDes;
          break;
        case r'provisionalNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.provisionalNote = valueDes;
          break;
        case r'isProvisional':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isProvisional = valueDes;
          break;
        case r'conversion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateConversionSnapshotResponse),
          ) as EstimateConversionSnapshotResponse?;
          if (valueDes == null) continue;
          result.conversion.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateReviewCostEvidenceResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateReviewCostEvidenceResponseBuilder();
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


