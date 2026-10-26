//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_response.dart';
import 'package:tan_erp_api/src/model/estimate_conversion_snapshot_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_cost_component_response.g.dart';

/// EstimateCostComponentResponse
///
/// Properties:
/// * [id] 
/// * [estimateWorkItemId] 
/// * [type] 
/// * [description] 
/// * [quantity] 
/// * [unitCode] 
/// * [unitCost] 
/// * [currency] 
/// * [totalCost] 
/// * [sortOrder] 
/// * [itemId] 
/// * [costRecordId] 
/// * [costRecordVersion] 
/// * [itemCodeSnapshot] 
/// * [itemNameSnapshot] 
/// * [unitSnapshot] 
/// * [unitCostSnapshot] 
/// * [currencySnapshot] 
/// * [costScopeSnapshot] 
/// * [costEffectiveFromUtc] 
/// * [costPolicyVersion] 
/// * [resolvedAtUtc] 
/// * [costOrigin] 
/// * [costSourceIdSnapshot] 
/// * [costSourceCodeSnapshot] 
/// * [costSourceReferenceSnapshot] 
/// * [costEvidenceFileIdSnapshot] 
/// * [costRecordReasonSnapshot] 
/// * [provisionalReasonCode] 
/// * [provisionalNote] 
/// * [isProvisional] 
/// * [conversionSnapshot] 
@BuiltValue()
abstract class EstimateCostComponentResponse implements Built<EstimateCostComponentResponse, EstimateCostComponentResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'estimateWorkItemId')
  String? get estimateWorkItemId;

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

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'costRecordId')
  String? get costRecordId;

  @BuiltValueField(wireName: r'costRecordVersion')
  int? get costRecordVersion;

  @BuiltValueField(wireName: r'itemCodeSnapshot')
  String? get itemCodeSnapshot;

  @BuiltValueField(wireName: r'itemNameSnapshot')
  LocalizedTextResponse? get itemNameSnapshot;

  @BuiltValueField(wireName: r'unitSnapshot')
  String? get unitSnapshot;

  @BuiltValueField(wireName: r'unitCostSnapshot')
  double? get unitCostSnapshot;

  @BuiltValueField(wireName: r'currencySnapshot')
  String? get currencySnapshot;

  @BuiltValueField(wireName: r'costScopeSnapshot')
  String? get costScopeSnapshot;

  @BuiltValueField(wireName: r'costEffectiveFromUtc')
  DateTime? get costEffectiveFromUtc;

  @BuiltValueField(wireName: r'costPolicyVersion')
  String? get costPolicyVersion;

  @BuiltValueField(wireName: r'resolvedAtUtc')
  DateTime? get resolvedAtUtc;

  @BuiltValueField(wireName: r'costOrigin')
  String? get costOrigin;

  @BuiltValueField(wireName: r'costSourceIdSnapshot')
  String? get costSourceIdSnapshot;

  @BuiltValueField(wireName: r'costSourceCodeSnapshot')
  String? get costSourceCodeSnapshot;

  @BuiltValueField(wireName: r'costSourceReferenceSnapshot')
  String? get costSourceReferenceSnapshot;

  @BuiltValueField(wireName: r'costEvidenceFileIdSnapshot')
  String? get costEvidenceFileIdSnapshot;

  @BuiltValueField(wireName: r'costRecordReasonSnapshot')
  String? get costRecordReasonSnapshot;

  @BuiltValueField(wireName: r'provisionalReasonCode')
  String? get provisionalReasonCode;

  @BuiltValueField(wireName: r'provisionalNote')
  String? get provisionalNote;

  @BuiltValueField(wireName: r'isProvisional')
  bool? get isProvisional;

  @BuiltValueField(wireName: r'conversionSnapshot')
  EstimateConversionSnapshotResponse? get conversionSnapshot;

  EstimateCostComponentResponse._();

  factory EstimateCostComponentResponse([void updates(EstimateCostComponentResponseBuilder b)]) = _$EstimateCostComponentResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateCostComponentResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateCostComponentResponse> get serializer => _$EstimateCostComponentResponseSerializer();
}

class _$EstimateCostComponentResponseSerializer implements PrimitiveSerializer<EstimateCostComponentResponse> {
  @override
  final Iterable<Type> types = const [EstimateCostComponentResponse, _$EstimateCostComponentResponse];

  @override
  final String wireName = r'EstimateCostComponentResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateCostComponentResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.estimateWorkItemId != null) {
      yield r'estimateWorkItemId';
      yield serializers.serialize(
        object.estimateWorkItemId,
        specifiedType: const FullType(String),
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
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
      );
    }
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costRecordId != null) {
      yield r'costRecordId';
      yield serializers.serialize(
        object.costRecordId,
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
    if (object.itemCodeSnapshot != null) {
      yield r'itemCodeSnapshot';
      yield serializers.serialize(
        object.itemCodeSnapshot,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.itemNameSnapshot != null) {
      yield r'itemNameSnapshot';
      yield serializers.serialize(
        object.itemNameSnapshot,
        specifiedType: const FullType(LocalizedTextResponse),
      );
    }
    if (object.unitSnapshot != null) {
      yield r'unitSnapshot';
      yield serializers.serialize(
        object.unitSnapshot,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitCostSnapshot != null) {
      yield r'unitCostSnapshot';
      yield serializers.serialize(
        object.unitCostSnapshot,
        specifiedType: const FullType.nullable(double),
      );
    }
    if (object.currencySnapshot != null) {
      yield r'currencySnapshot';
      yield serializers.serialize(
        object.currencySnapshot,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costScopeSnapshot != null) {
      yield r'costScopeSnapshot';
      yield serializers.serialize(
        object.costScopeSnapshot,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costEffectiveFromUtc != null) {
      yield r'costEffectiveFromUtc';
      yield serializers.serialize(
        object.costEffectiveFromUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.costPolicyVersion != null) {
      yield r'costPolicyVersion';
      yield serializers.serialize(
        object.costPolicyVersion,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.resolvedAtUtc != null) {
      yield r'resolvedAtUtc';
      yield serializers.serialize(
        object.resolvedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.costOrigin != null) {
      yield r'costOrigin';
      yield serializers.serialize(
        object.costOrigin,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costSourceIdSnapshot != null) {
      yield r'costSourceIdSnapshot';
      yield serializers.serialize(
        object.costSourceIdSnapshot,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costSourceCodeSnapshot != null) {
      yield r'costSourceCodeSnapshot';
      yield serializers.serialize(
        object.costSourceCodeSnapshot,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costSourceReferenceSnapshot != null) {
      yield r'costSourceReferenceSnapshot';
      yield serializers.serialize(
        object.costSourceReferenceSnapshot,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costEvidenceFileIdSnapshot != null) {
      yield r'costEvidenceFileIdSnapshot';
      yield serializers.serialize(
        object.costEvidenceFileIdSnapshot,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costRecordReasonSnapshot != null) {
      yield r'costRecordReasonSnapshot';
      yield serializers.serialize(
        object.costRecordReasonSnapshot,
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
    if (object.conversionSnapshot != null) {
      yield r'conversionSnapshot';
      yield serializers.serialize(
        object.conversionSnapshot,
        specifiedType: const FullType(EstimateConversionSnapshotResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateCostComponentResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateCostComponentResponseBuilder result,
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
        case r'estimateWorkItemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateWorkItemId = valueDes;
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
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'costRecordId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costRecordId = valueDes;
          break;
        case r'costRecordVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.costRecordVersion = valueDes;
          break;
        case r'itemCodeSnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemCodeSnapshot = valueDes;
          break;
        case r'itemNameSnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextResponse),
          ) as LocalizedTextResponse?;
          if (valueDes == null) continue;
          result.itemNameSnapshot.replace(valueDes);
          break;
        case r'unitSnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitSnapshot = valueDes;
          break;
        case r'unitCostSnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitCostSnapshot = valueDes;
          break;
        case r'currencySnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currencySnapshot = valueDes;
          break;
        case r'costScopeSnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costScopeSnapshot = valueDes;
          break;
        case r'costEffectiveFromUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.costEffectiveFromUtc = valueDes;
          break;
        case r'costPolicyVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costPolicyVersion = valueDes;
          break;
        case r'resolvedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.resolvedAtUtc = valueDes;
          break;
        case r'costOrigin':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costOrigin = valueDes;
          break;
        case r'costSourceIdSnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costSourceIdSnapshot = valueDes;
          break;
        case r'costSourceCodeSnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costSourceCodeSnapshot = valueDes;
          break;
        case r'costSourceReferenceSnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costSourceReferenceSnapshot = valueDes;
          break;
        case r'costEvidenceFileIdSnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costEvidenceFileIdSnapshot = valueDes;
          break;
        case r'costRecordReasonSnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costRecordReasonSnapshot = valueDes;
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
        case r'conversionSnapshot':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(EstimateConversionSnapshotResponse),
          ) as EstimateConversionSnapshotResponse?;
          if (valueDes == null) continue;
          result.conversionSnapshot.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateCostComponentResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateCostComponentResponseBuilder();
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


