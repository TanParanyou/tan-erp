//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/estimate_readiness_reason_response.dart';
import 'package:tan_erp_api/src/model/estimate_section_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_revision_response.g.dart';

/// EstimateRevisionResponse
///
/// Properties:
/// * [id] 
/// * [estimateId] 
/// * [revisionNo] 
/// * [status] 
/// * [currency] 
/// * [calculationVersion] 
/// * [calculationOutdated] 
/// * [calculationPolicyVersion] 
/// * [taxPolicyVersion] 
/// * [netCost] 
/// * [sellingBeforeDiscount] 
/// * [discountType] 
/// * [discountValue] 
/// * [discountReasonCode] 
/// * [discountAmount] 
/// * [netBeforeTax] 
/// * [taxAmount] 
/// * [grandTotal] 
/// * [marginAmount] 
/// * [marginRate] 
/// * [markupRate] 
/// * [calculationSnapshotJson] 
/// * [rowVersion] 
/// * [createdAtUtc] 
/// * [updatedAtUtc] 
/// * [sections] 
/// * [readiness] 
/// * [readinessReasons] 
@BuiltValue()
abstract class EstimateRevisionResponse implements Built<EstimateRevisionResponse, EstimateRevisionResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'estimateId')
  String? get estimateId;

  @BuiltValueField(wireName: r'revisionNo')
  int? get revisionNo;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'calculationVersion')
  int? get calculationVersion;

  @BuiltValueField(wireName: r'calculationOutdated')
  bool? get calculationOutdated;

  @BuiltValueField(wireName: r'calculationPolicyVersion')
  String? get calculationPolicyVersion;

  @BuiltValueField(wireName: r'taxPolicyVersion')
  String? get taxPolicyVersion;

  @BuiltValueField(wireName: r'netCost')
  double? get netCost;

  @BuiltValueField(wireName: r'sellingBeforeDiscount')
  double? get sellingBeforeDiscount;

  @BuiltValueField(wireName: r'discountType')
  String? get discountType;

  @BuiltValueField(wireName: r'discountValue')
  double? get discountValue;

  @BuiltValueField(wireName: r'discountReasonCode')
  String? get discountReasonCode;

  @BuiltValueField(wireName: r'discountAmount')
  double? get discountAmount;

  @BuiltValueField(wireName: r'netBeforeTax')
  double? get netBeforeTax;

  @BuiltValueField(wireName: r'taxAmount')
  double? get taxAmount;

  @BuiltValueField(wireName: r'grandTotal')
  double? get grandTotal;

  @BuiltValueField(wireName: r'marginAmount')
  double? get marginAmount;

  @BuiltValueField(wireName: r'marginRate')
  double? get marginRate;

  @BuiltValueField(wireName: r'markupRate')
  double? get markupRate;

  @BuiltValueField(wireName: r'calculationSnapshotJson')
  String? get calculationSnapshotJson;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime? get updatedAtUtc;

  @BuiltValueField(wireName: r'sections')
  BuiltList<EstimateSectionResponse>? get sections;

  @BuiltValueField(wireName: r'readiness')
  String? get readiness;

  @BuiltValueField(wireName: r'readinessReasons')
  BuiltList<EstimateReadinessReasonResponse>? get readinessReasons;

  EstimateRevisionResponse._();

  factory EstimateRevisionResponse([void updates(EstimateRevisionResponseBuilder b)]) = _$EstimateRevisionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateRevisionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateRevisionResponse> get serializer => _$EstimateRevisionResponseSerializer();
}

class _$EstimateRevisionResponseSerializer implements PrimitiveSerializer<EstimateRevisionResponse> {
  @override
  final Iterable<Type> types = const [EstimateRevisionResponse, _$EstimateRevisionResponse];

  @override
  final String wireName = r'EstimateRevisionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateRevisionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
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
    if (object.revisionNo != null) {
      yield r'revisionNo';
      yield serializers.serialize(
        object.revisionNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
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
    if (object.calculationOutdated != null) {
      yield r'calculationOutdated';
      yield serializers.serialize(
        object.calculationOutdated,
        specifiedType: const FullType(bool),
      );
    }
    if (object.calculationPolicyVersion != null) {
      yield r'calculationPolicyVersion';
      yield serializers.serialize(
        object.calculationPolicyVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxPolicyVersion != null) {
      yield r'taxPolicyVersion';
      yield serializers.serialize(
        object.taxPolicyVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.netCost != null) {
      yield r'netCost';
      yield serializers.serialize(
        object.netCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.sellingBeforeDiscount != null) {
      yield r'sellingBeforeDiscount';
      yield serializers.serialize(
        object.sellingBeforeDiscount,
        specifiedType: const FullType(double),
      );
    }
    if (object.discountType != null) {
      yield r'discountType';
      yield serializers.serialize(
        object.discountType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.discountValue != null) {
      yield r'discountValue';
      yield serializers.serialize(
        object.discountValue,
        specifiedType: const FullType(double),
      );
    }
    if (object.discountReasonCode != null) {
      yield r'discountReasonCode';
      yield serializers.serialize(
        object.discountReasonCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.discountAmount != null) {
      yield r'discountAmount';
      yield serializers.serialize(
        object.discountAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.netBeforeTax != null) {
      yield r'netBeforeTax';
      yield serializers.serialize(
        object.netBeforeTax,
        specifiedType: const FullType(double),
      );
    }
    if (object.taxAmount != null) {
      yield r'taxAmount';
      yield serializers.serialize(
        object.taxAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.grandTotal != null) {
      yield r'grandTotal';
      yield serializers.serialize(
        object.grandTotal,
        specifiedType: const FullType(double),
      );
    }
    if (object.marginAmount != null) {
      yield r'marginAmount';
      yield serializers.serialize(
        object.marginAmount,
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
    if (object.markupRate != null) {
      yield r'markupRate';
      yield serializers.serialize(
        object.markupRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.calculationSnapshotJson != null) {
      yield r'calculationSnapshotJson';
      yield serializers.serialize(
        object.calculationSnapshotJson,
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
    if (object.updatedAtUtc != null) {
      yield r'updatedAtUtc';
      yield serializers.serialize(
        object.updatedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.sections != null) {
      yield r'sections';
      yield serializers.serialize(
        object.sections,
        specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateSectionResponse)]),
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
        specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReadinessReasonResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateRevisionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateRevisionResponseBuilder result,
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
        case r'estimateId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateId = valueDes;
          break;
        case r'revisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.revisionNo = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        case r'calculationVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.calculationVersion = valueDes;
          break;
        case r'calculationOutdated':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.calculationOutdated = valueDes;
          break;
        case r'calculationPolicyVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.calculationPolicyVersion = valueDes;
          break;
        case r'taxPolicyVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.taxPolicyVersion = valueDes;
          break;
        case r'netCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.netCost = valueDes;
          break;
        case r'sellingBeforeDiscount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.sellingBeforeDiscount = valueDes;
          break;
        case r'discountType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.discountType = valueDes;
          break;
        case r'discountValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.discountValue = valueDes;
          break;
        case r'discountReasonCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.discountReasonCode = valueDes;
          break;
        case r'discountAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.discountAmount = valueDes;
          break;
        case r'netBeforeTax':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.netBeforeTax = valueDes;
          break;
        case r'taxAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.taxAmount = valueDes;
          break;
        case r'grandTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.grandTotal = valueDes;
          break;
        case r'marginAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.marginAmount = valueDes;
          break;
        case r'marginRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.marginRate = valueDes;
          break;
        case r'markupRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.markupRate = valueDes;
          break;
        case r'calculationSnapshotJson':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.calculationSnapshotJson = valueDes;
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
        case r'updatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.updatedAtUtc = valueDes;
          break;
        case r'sections':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateSectionResponse)]),
          ) as BuiltList<EstimateSectionResponse>?;
          if (valueDes == null) continue;
          result.sections.replace(valueDes);
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
            specifiedType: const FullType.nullable(BuiltList, [FullType(EstimateReadinessReasonResponse)]),
          ) as BuiltList<EstimateReadinessReasonResponse>?;
          if (valueDes == null) continue;
          result.readinessReasons.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateRevisionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateRevisionResponseBuilder();
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


