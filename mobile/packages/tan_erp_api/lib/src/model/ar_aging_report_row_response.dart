//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'ar_aging_report_row_response.g.dart';

/// ArAgingReportRowResponse
///
/// Properties:
/// * [billingDocumentId] 
/// * [number] 
/// * [projectCode] 
/// * [customerCode] 
/// * [customerName] 
/// * [branchCode] 
/// * [dueDate] 
/// * [daysOverdue] 
/// * [agingBucket] 
/// * [grossTotal] 
/// * [paidAmount] 
/// * [outstandingAmount] 
@BuiltValue()
abstract class ArAgingReportRowResponse implements Built<ArAgingReportRowResponse, ArAgingReportRowResponseBuilder> {
  @BuiltValueField(wireName: r'billingDocumentId')
  String? get billingDocumentId;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'projectCode')
  String? get projectCode;

  @BuiltValueField(wireName: r'customerCode')
  String? get customerCode;

  @BuiltValueField(wireName: r'customerName')
  String? get customerName;

  @BuiltValueField(wireName: r'branchCode')
  String? get branchCode;

  @BuiltValueField(wireName: r'dueDate')
  Date? get dueDate;

  @BuiltValueField(wireName: r'daysOverdue')
  int? get daysOverdue;

  @BuiltValueField(wireName: r'agingBucket')
  String? get agingBucket;

  @BuiltValueField(wireName: r'grossTotal')
  double? get grossTotal;

  @BuiltValueField(wireName: r'paidAmount')
  double? get paidAmount;

  @BuiltValueField(wireName: r'outstandingAmount')
  double? get outstandingAmount;

  ArAgingReportRowResponse._();

  factory ArAgingReportRowResponse([void updates(ArAgingReportRowResponseBuilder b)]) = _$ArAgingReportRowResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ArAgingReportRowResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ArAgingReportRowResponse> get serializer => _$ArAgingReportRowResponseSerializer();
}

class _$ArAgingReportRowResponseSerializer implements PrimitiveSerializer<ArAgingReportRowResponse> {
  @override
  final Iterable<Type> types = const [ArAgingReportRowResponse, _$ArAgingReportRowResponse];

  @override
  final String wireName = r'ArAgingReportRowResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ArAgingReportRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.billingDocumentId != null) {
      yield r'billingDocumentId';
      yield serializers.serialize(
        object.billingDocumentId,
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
    if (object.projectCode != null) {
      yield r'projectCode';
      yield serializers.serialize(
        object.projectCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.customerCode != null) {
      yield r'customerCode';
      yield serializers.serialize(
        object.customerCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.customerName != null) {
      yield r'customerName';
      yield serializers.serialize(
        object.customerName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.branchCode != null) {
      yield r'branchCode';
      yield serializers.serialize(
        object.branchCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.dueDate != null) {
      yield r'dueDate';
      yield serializers.serialize(
        object.dueDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.daysOverdue != null) {
      yield r'daysOverdue';
      yield serializers.serialize(
        object.daysOverdue,
        specifiedType: const FullType(int),
      );
    }
    if (object.agingBucket != null) {
      yield r'agingBucket';
      yield serializers.serialize(
        object.agingBucket,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.grossTotal != null) {
      yield r'grossTotal';
      yield serializers.serialize(
        object.grossTotal,
        specifiedType: const FullType(double),
      );
    }
    if (object.paidAmount != null) {
      yield r'paidAmount';
      yield serializers.serialize(
        object.paidAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.outstandingAmount != null) {
      yield r'outstandingAmount';
      yield serializers.serialize(
        object.outstandingAmount,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ArAgingReportRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ArAgingReportRowResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'billingDocumentId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.billingDocumentId = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'projectCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectCode = valueDes;
          break;
        case r'customerCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerCode = valueDes;
          break;
        case r'customerName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.customerName = valueDes;
          break;
        case r'branchCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.branchCode = valueDes;
          break;
        case r'dueDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.dueDate = valueDes;
          break;
        case r'daysOverdue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.daysOverdue = valueDes;
          break;
        case r'agingBucket':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.agingBucket = valueDes;
          break;
        case r'grossTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.grossTotal = valueDes;
          break;
        case r'paidAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.paidAmount = valueDes;
          break;
        case r'outstandingAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.outstandingAmount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ArAgingReportRowResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ArAgingReportRowResponseBuilder();
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


