//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/cost_review_person_response.dart';
import 'package:tan_erp_api/src/model/cost_review_reference_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'cost_review_queue_item_response.g.dart';

/// CostReviewQueueItemResponse
///
/// Properties:
/// * [id] 
/// * [itemId] 
/// * [item] 
/// * [scope] 
/// * [unitId] 
/// * [unit] 
/// * [costSourceId] 
/// * [costSource] 
/// * [amount] 
/// * [currency] 
/// * [minimumQuantity] 
/// * [maximumQuantity] 
/// * [effectiveFromUtc] 
/// * [effectiveToUtc] 
/// * [status] 
/// * [sourceReference] 
/// * [reason] 
/// * [evidenceFileId] 
/// * [maker] 
/// * [lastEditor] 
/// * [rowVersion] 
/// * [updatedAtUtc] 
@BuiltValue()
abstract class CostReviewQueueItemResponse implements Built<CostReviewQueueItemResponse, CostReviewQueueItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'item')
  CostReviewReferenceResponse? get item;

  @BuiltValueField(wireName: r'scope')
  String? get scope;

  @BuiltValueField(wireName: r'unitId')
  String? get unitId;

  @BuiltValueField(wireName: r'unit')
  CostReviewReferenceResponse? get unit;

  @BuiltValueField(wireName: r'costSourceId')
  String? get costSourceId;

  @BuiltValueField(wireName: r'costSource')
  CostReviewReferenceResponse? get costSource;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'minimumQuantity')
  double? get minimumQuantity;

  @BuiltValueField(wireName: r'maximumQuantity')
  double? get maximumQuantity;

  @BuiltValueField(wireName: r'effectiveFromUtc')
  DateTime? get effectiveFromUtc;

  @BuiltValueField(wireName: r'effectiveToUtc')
  DateTime? get effectiveToUtc;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'sourceReference')
  String? get sourceReference;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'evidenceFileId')
  String? get evidenceFileId;

  @BuiltValueField(wireName: r'maker')
  CostReviewPersonResponse? get maker;

  @BuiltValueField(wireName: r'lastEditor')
  CostReviewPersonResponse? get lastEditor;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime? get updatedAtUtc;

  CostReviewQueueItemResponse._();

  factory CostReviewQueueItemResponse([void updates(CostReviewQueueItemResponseBuilder b)]) = _$CostReviewQueueItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CostReviewQueueItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CostReviewQueueItemResponse> get serializer => _$CostReviewQueueItemResponseSerializer();
}

class _$CostReviewQueueItemResponseSerializer implements PrimitiveSerializer<CostReviewQueueItemResponse> {
  @override
  final Iterable<Type> types = const [CostReviewQueueItemResponse, _$CostReviewQueueItemResponse];

  @override
  final String wireName = r'CostReviewQueueItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CostReviewQueueItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
      );
    }
    if (object.item != null) {
      yield r'item';
      yield serializers.serialize(
        object.item,
        specifiedType: const FullType(CostReviewReferenceResponse),
      );
    }
    if (object.scope != null) {
      yield r'scope';
      yield serializers.serialize(
        object.scope,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitId != null) {
      yield r'unitId';
      yield serializers.serialize(
        object.unitId,
        specifiedType: const FullType(String),
      );
    }
    if (object.unit != null) {
      yield r'unit';
      yield serializers.serialize(
        object.unit,
        specifiedType: const FullType(CostReviewReferenceResponse),
      );
    }
    if (object.costSourceId != null) {
      yield r'costSourceId';
      yield serializers.serialize(
        object.costSourceId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costSource != null) {
      yield r'costSource';
      yield serializers.serialize(
        object.costSource,
        specifiedType: const FullType(CostReviewReferenceResponse),
      );
    }
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
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
    if (object.minimumQuantity != null) {
      yield r'minimumQuantity';
      yield serializers.serialize(
        object.minimumQuantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.maximumQuantity != null) {
      yield r'maximumQuantity';
      yield serializers.serialize(
        object.maximumQuantity,
        specifiedType: const FullType.nullable(double),
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
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
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
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
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
    if (object.maker != null) {
      yield r'maker';
      yield serializers.serialize(
        object.maker,
        specifiedType: const FullType(CostReviewPersonResponse),
      );
    }
    if (object.lastEditor != null) {
      yield r'lastEditor';
      yield serializers.serialize(
        object.lastEditor,
        specifiedType: const FullType(CostReviewPersonResponse),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.updatedAtUtc != null) {
      yield r'updatedAtUtc';
      yield serializers.serialize(
        object.updatedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CostReviewQueueItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CostReviewQueueItemResponseBuilder result,
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
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'item':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CostReviewReferenceResponse),
          ) as CostReviewReferenceResponse?;
          if (valueDes == null) continue;
          result.item.replace(valueDes);
          break;
        case r'scope':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.scope = valueDes;
          break;
        case r'unitId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitId = valueDes;
          break;
        case r'unit':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CostReviewReferenceResponse),
          ) as CostReviewReferenceResponse?;
          if (valueDes == null) continue;
          result.unit.replace(valueDes);
          break;
        case r'costSourceId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costSourceId = valueDes;
          break;
        case r'costSource':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CostReviewReferenceResponse),
          ) as CostReviewReferenceResponse?;
          if (valueDes == null) continue;
          result.costSource.replace(valueDes);
          break;
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        case r'minimumQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.minimumQuantity = valueDes;
          break;
        case r'maximumQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.maximumQuantity = valueDes;
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
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'sourceReference':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.sourceReference = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'evidenceFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.evidenceFileId = valueDes;
          break;
        case r'maker':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CostReviewPersonResponse),
          ) as CostReviewPersonResponse?;
          if (valueDes == null) continue;
          result.maker.replace(valueDes);
          break;
        case r'lastEditor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(CostReviewPersonResponse),
          ) as CostReviewPersonResponse?;
          if (valueDes == null) continue;
          result.lastEditor.replace(valueDes);
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'updatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.updatedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CostReviewQueueItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CostReviewQueueItemResponseBuilder();
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


