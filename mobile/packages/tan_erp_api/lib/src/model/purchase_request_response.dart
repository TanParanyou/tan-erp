//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/purchase_order_project_response.dart';
import 'package:tan_erp_api/src/model/procurement_person_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/purchase_request_line_response.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:tan_erp_api/src/model/purchase_request_converted_order_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_request_response.g.dart';

/// PurchaseRequestResponse
///
/// Properties:
/// * [id] 
/// * [branchId] 
/// * [number] 
/// * [status] 
/// * [currency] 
/// * [totalEstimatedAmount] 
/// * [requiredByDate] 
/// * [note] 
/// * [project] 
/// * [createdBy] 
/// * [createdAtUtc] 
/// * [submittedAtUtc] 
/// * [decidedBy] 
/// * [decidedAtUtc] 
/// * [decisionNote] 
/// * [cancelReason] 
/// * [convertedPurchaseOrder] 
/// * [convertedAtUtc] 
/// * [rowVersion] 
/// * [lines] 
@BuiltValue()
abstract class PurchaseRequestResponse implements Built<PurchaseRequestResponse, PurchaseRequestResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'branchId')
  String? get branchId;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'totalEstimatedAmount')
  double? get totalEstimatedAmount;

  @BuiltValueField(wireName: r'requiredByDate')
  Date? get requiredByDate;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'project')
  PurchaseOrderProjectResponse? get project;

  @BuiltValueField(wireName: r'createdBy')
  ProcurementPersonResponse? get createdBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'submittedAtUtc')
  DateTime? get submittedAtUtc;

  @BuiltValueField(wireName: r'decidedBy')
  ProcurementPersonResponse? get decidedBy;

  @BuiltValueField(wireName: r'decidedAtUtc')
  DateTime? get decidedAtUtc;

  @BuiltValueField(wireName: r'decisionNote')
  String? get decisionNote;

  @BuiltValueField(wireName: r'cancelReason')
  String? get cancelReason;

  @BuiltValueField(wireName: r'convertedPurchaseOrder')
  PurchaseRequestConvertedOrderResponse? get convertedPurchaseOrder;

  @BuiltValueField(wireName: r'convertedAtUtc')
  DateTime? get convertedAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'lines')
  BuiltList<PurchaseRequestLineResponse>? get lines;

  PurchaseRequestResponse._();

  factory PurchaseRequestResponse([void updates(PurchaseRequestResponseBuilder b)]) = _$PurchaseRequestResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseRequestResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseRequestResponse> get serializer => _$PurchaseRequestResponseSerializer();
}

class _$PurchaseRequestResponseSerializer implements PrimitiveSerializer<PurchaseRequestResponse> {
  @override
  final Iterable<Type> types = const [PurchaseRequestResponse, _$PurchaseRequestResponse];

  @override
  final String wireName = r'PurchaseRequestResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
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
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.totalEstimatedAmount != null) {
      yield r'totalEstimatedAmount';
      yield serializers.serialize(
        object.totalEstimatedAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.requiredByDate != null) {
      yield r'requiredByDate';
      yield serializers.serialize(
        object.requiredByDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.project != null) {
      yield r'project';
      yield serializers.serialize(
        object.project,
        specifiedType: const FullType(PurchaseOrderProjectResponse),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(ProcurementPersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.submittedAtUtc != null) {
      yield r'submittedAtUtc';
      yield serializers.serialize(
        object.submittedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.decidedBy != null) {
      yield r'decidedBy';
      yield serializers.serialize(
        object.decidedBy,
        specifiedType: const FullType(ProcurementPersonResponse),
      );
    }
    if (object.decidedAtUtc != null) {
      yield r'decidedAtUtc';
      yield serializers.serialize(
        object.decidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.decisionNote != null) {
      yield r'decisionNote';
      yield serializers.serialize(
        object.decisionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.cancelReason != null) {
      yield r'cancelReason';
      yield serializers.serialize(
        object.cancelReason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.convertedPurchaseOrder != null) {
      yield r'convertedPurchaseOrder';
      yield serializers.serialize(
        object.convertedPurchaseOrder,
        specifiedType: const FullType(PurchaseRequestConvertedOrderResponse),
      );
    }
    if (object.convertedAtUtc != null) {
      yield r'convertedAtUtc';
      yield serializers.serialize(
        object.convertedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseRequestLineResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PurchaseRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseRequestResponseBuilder result,
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
        case r'branchId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchId = valueDes;
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
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        case r'totalEstimatedAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalEstimatedAmount = valueDes;
          break;
        case r'requiredByDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.requiredByDate = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'project':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(PurchaseOrderProjectResponse),
          ) as PurchaseOrderProjectResponse?;
          if (valueDes == null) continue;
          result.project.replace(valueDes);
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProcurementPersonResponse),
          ) as ProcurementPersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'submittedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.submittedAtUtc = valueDes;
          break;
        case r'decidedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProcurementPersonResponse),
          ) as ProcurementPersonResponse?;
          if (valueDes == null) continue;
          result.decidedBy.replace(valueDes);
          break;
        case r'decidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.decidedAtUtc = valueDes;
          break;
        case r'decisionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.decisionNote = valueDes;
          break;
        case r'cancelReason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.cancelReason = valueDes;
          break;
        case r'convertedPurchaseOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(PurchaseRequestConvertedOrderResponse),
          ) as PurchaseRequestConvertedOrderResponse?;
          if (valueDes == null) continue;
          result.convertedPurchaseOrder.replace(valueDes);
          break;
        case r'convertedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.convertedAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseRequestLineResponse)]),
          ) as BuiltList<PurchaseRequestLineResponse>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PurchaseRequestResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseRequestResponseBuilder();
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


