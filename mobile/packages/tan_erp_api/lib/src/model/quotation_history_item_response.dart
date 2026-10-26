//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/quotation_document_summary_response.dart';
import 'package:tan_erp_api/src/model/quotation_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_history_item_response.g.dart';

/// QuotationHistoryItemResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [status] 
/// * [totalAmount] 
/// * [estimateRevisionId] 
/// * [estimateRevisionNo] 
/// * [issuedAtUtc] 
/// * [acceptedAtUtc] 
/// * [supersedesQuotationId] 
/// * [supersedesNumber] 
/// * [supersededByQuotationId] 
/// * [supersededByNumber] 
/// * [amendmentReason] 
/// * [voidedAtUtc] 
/// * [voidedBy] 
/// * [voidReason] 
/// * [rowVersion] 
/// * [document] 
@BuiltValue()
abstract class QuotationHistoryItemResponse implements Built<QuotationHistoryItemResponse, QuotationHistoryItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'totalAmount')
  double? get totalAmount;

  @BuiltValueField(wireName: r'estimateRevisionId')
  String? get estimateRevisionId;

  @BuiltValueField(wireName: r'estimateRevisionNo')
  int? get estimateRevisionNo;

  @BuiltValueField(wireName: r'issuedAtUtc')
  DateTime? get issuedAtUtc;

  @BuiltValueField(wireName: r'acceptedAtUtc')
  DateTime? get acceptedAtUtc;

  @BuiltValueField(wireName: r'supersedesQuotationId')
  String? get supersedesQuotationId;

  @BuiltValueField(wireName: r'supersedesNumber')
  String? get supersedesNumber;

  @BuiltValueField(wireName: r'supersededByQuotationId')
  String? get supersededByQuotationId;

  @BuiltValueField(wireName: r'supersededByNumber')
  String? get supersededByNumber;

  @BuiltValueField(wireName: r'amendmentReason')
  String? get amendmentReason;

  @BuiltValueField(wireName: r'voidedAtUtc')
  DateTime? get voidedAtUtc;

  @BuiltValueField(wireName: r'voidedBy')
  QuotationPersonResponse? get voidedBy;

  @BuiltValueField(wireName: r'voidReason')
  String? get voidReason;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'document')
  QuotationDocumentSummaryResponse? get document;

  QuotationHistoryItemResponse._();

  factory QuotationHistoryItemResponse([void updates(QuotationHistoryItemResponseBuilder b)]) = _$QuotationHistoryItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationHistoryItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationHistoryItemResponse> get serializer => _$QuotationHistoryItemResponseSerializer();
}

class _$QuotationHistoryItemResponseSerializer implements PrimitiveSerializer<QuotationHistoryItemResponse> {
  @override
  final Iterable<Type> types = const [QuotationHistoryItemResponse, _$QuotationHistoryItemResponse];

  @override
  final String wireName = r'QuotationHistoryItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationHistoryItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
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
    if (object.totalAmount != null) {
      yield r'totalAmount';
      yield serializers.serialize(
        object.totalAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.estimateRevisionId != null) {
      yield r'estimateRevisionId';
      yield serializers.serialize(
        object.estimateRevisionId,
        specifiedType: const FullType(String),
      );
    }
    if (object.estimateRevisionNo != null) {
      yield r'estimateRevisionNo';
      yield serializers.serialize(
        object.estimateRevisionNo,
        specifiedType: const FullType(int),
      );
    }
    if (object.issuedAtUtc != null) {
      yield r'issuedAtUtc';
      yield serializers.serialize(
        object.issuedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.acceptedAtUtc != null) {
      yield r'acceptedAtUtc';
      yield serializers.serialize(
        object.acceptedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.supersedesQuotationId != null) {
      yield r'supersedesQuotationId';
      yield serializers.serialize(
        object.supersedesQuotationId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.supersedesNumber != null) {
      yield r'supersedesNumber';
      yield serializers.serialize(
        object.supersedesNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.supersededByQuotationId != null) {
      yield r'supersededByQuotationId';
      yield serializers.serialize(
        object.supersededByQuotationId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.supersededByNumber != null) {
      yield r'supersededByNumber';
      yield serializers.serialize(
        object.supersededByNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.amendmentReason != null) {
      yield r'amendmentReason';
      yield serializers.serialize(
        object.amendmentReason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.voidedAtUtc != null) {
      yield r'voidedAtUtc';
      yield serializers.serialize(
        object.voidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.voidedBy != null) {
      yield r'voidedBy';
      yield serializers.serialize(
        object.voidedBy,
        specifiedType: const FullType(QuotationPersonResponse),
      );
    }
    if (object.voidReason != null) {
      yield r'voidReason';
      yield serializers.serialize(
        object.voidReason,
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
    if (object.document != null) {
      yield r'document';
      yield serializers.serialize(
        object.document,
        specifiedType: const FullType(QuotationDocumentSummaryResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationHistoryItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationHistoryItemResponseBuilder result,
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
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'totalAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalAmount = valueDes;
          break;
        case r'estimateRevisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.estimateRevisionId = valueDes;
          break;
        case r'estimateRevisionNo':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.estimateRevisionNo = valueDes;
          break;
        case r'issuedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.issuedAtUtc = valueDes;
          break;
        case r'acceptedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.acceptedAtUtc = valueDes;
          break;
        case r'supersedesQuotationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supersedesQuotationId = valueDes;
          break;
        case r'supersedesNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supersedesNumber = valueDes;
          break;
        case r'supersededByQuotationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supersededByQuotationId = valueDes;
          break;
        case r'supersededByNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.supersededByNumber = valueDes;
          break;
        case r'amendmentReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.amendmentReason = valueDes;
          break;
        case r'voidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.voidedAtUtc = valueDes;
          break;
        case r'voidedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QuotationPersonResponse),
          ) as QuotationPersonResponse?;
          if (valueDes == null) continue;
          result.voidedBy.replace(valueDes);
          break;
        case r'voidReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.voidReason = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'document':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QuotationDocumentSummaryResponse),
          ) as QuotationDocumentSummaryResponse?;
          if (valueDes == null) continue;
          result.document.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuotationHistoryItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationHistoryItemResponseBuilder();
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


