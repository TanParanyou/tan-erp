//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_dto.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'cost_record_response.g.dart';

/// CostRecordResponse
///
/// Properties:
/// * [id] 
/// * [organizationId] 
/// * [itemId] 
/// * [scope] 
/// * [branchId] 
/// * [unitId] 
/// * [unitName] 
/// * [currency] 
/// * [amount] 
/// * [minimumQuantity] 
/// * [maximumQuantity] 
/// * [effectiveFromUtc] 
/// * [effectiveToUtc] 
/// * [status] 
/// * [version] 
/// * [costSourceId] 
/// * [costSourceName] 
/// * [sourceReference] 
/// * [reason] 
/// * [evidenceFileId] 
/// * [createdByUserId] 
/// * [lastFinancialEditorId] 
/// * [approvedByUserId] 
/// * [publishedByUserId] 
/// * [rowVersion] 
/// * [createdAtUtc] 
/// * [updatedAtUtc] 
@BuiltValue()
abstract class CostRecordResponse implements Built<CostRecordResponse, CostRecordResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'organizationId')
  String? get organizationId;

  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'scope')
  String? get scope;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'unitId')
  String? get unitId;

  @BuiltValueField(wireName: r'unitName')
  LocalizedTextDto? get unitName;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

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

  @BuiltValueField(wireName: r'version')
  int? get version;

  @BuiltValueField(wireName: r'costSourceId')
  String? get costSourceId;

  @BuiltValueField(wireName: r'costSourceName')
  LocalizedTextDto? get costSourceName;

  @BuiltValueField(wireName: r'sourceReference')
  String? get sourceReference;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'evidenceFileId')
  String? get evidenceFileId;

  @BuiltValueField(wireName: r'createdByUserId')
  String? get createdByUserId;

  @BuiltValueField(wireName: r'lastFinancialEditorId')
  String? get lastFinancialEditorId;

  @BuiltValueField(wireName: r'approvedByUserId')
  String? get approvedByUserId;

  @BuiltValueField(wireName: r'publishedByUserId')
  String? get publishedByUserId;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime? get updatedAtUtc;

  CostRecordResponse._();

  factory CostRecordResponse([void updates(CostRecordResponseBuilder b)]) = _$CostRecordResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CostRecordResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CostRecordResponse> get serializer => _$CostRecordResponseSerializer();
}

class _$CostRecordResponseSerializer implements PrimitiveSerializer<CostRecordResponse> {
  @override
  final Iterable<Type> types = const [CostRecordResponse, _$CostRecordResponse];

  @override
  final String wireName = r'CostRecordResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CostRecordResponse object, {
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
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
        specifiedType: const FullType(String),
      );
    }
    if (object.scope != null) {
      yield r'scope';
      yield serializers.serialize(
        object.scope,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.branchId != null) {
      yield r'branchId';
      yield serializers.serialize(
        object.branchId,
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
    if (object.unitName != null) {
      yield r'unitName';
      yield serializers.serialize(
        object.unitName,
        specifiedType: const FullType(LocalizedTextDto),
      );
    }
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
        specifiedType: const FullType(double),
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
    if (object.version != null) {
      yield r'version';
      yield serializers.serialize(
        object.version,
        specifiedType: const FullType(int),
      );
    }
    if (object.costSourceId != null) {
      yield r'costSourceId';
      yield serializers.serialize(
        object.costSourceId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.costSourceName != null) {
      yield r'costSourceName';
      yield serializers.serialize(
        object.costSourceName,
        specifiedType: const FullType(LocalizedTextDto),
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
    if (object.createdByUserId != null) {
      yield r'createdByUserId';
      yield serializers.serialize(
        object.createdByUserId,
        specifiedType: const FullType(String),
      );
    }
    if (object.lastFinancialEditorId != null) {
      yield r'lastFinancialEditorId';
      yield serializers.serialize(
        object.lastFinancialEditorId,
        specifiedType: const FullType(String),
      );
    }
    if (object.approvedByUserId != null) {
      yield r'approvedByUserId';
      yield serializers.serialize(
        object.approvedByUserId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.publishedByUserId != null) {
      yield r'publishedByUserId';
      yield serializers.serialize(
        object.publishedByUserId,
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
  }

  @override
  Object serialize(
    Serializers serializers,
    CostRecordResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CostRecordResponseBuilder result,
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
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'scope':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.scope = valueDes;
          break;
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
          break;
        case r'unitId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitId = valueDes;
          break;
        case r'unitName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextDto),
          ) as LocalizedTextDto?;
          if (valueDes == null) continue;
          result.unitName.replace(valueDes);
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
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
        case r'version':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.version = valueDes;
          break;
        case r'costSourceId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.costSourceId = valueDes;
          break;
        case r'costSourceName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(LocalizedTextDto),
          ) as LocalizedTextDto?;
          if (valueDes == null) continue;
          result.costSourceName.replace(valueDes);
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
        case r'createdByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.createdByUserId = valueDes;
          break;
        case r'lastFinancialEditorId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.lastFinancialEditorId = valueDes;
          break;
        case r'approvedByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.approvedByUserId = valueDes;
          break;
        case r'publishedByUserId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.publishedByUserId = valueDes;
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CostRecordResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CostRecordResponseBuilder();
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


