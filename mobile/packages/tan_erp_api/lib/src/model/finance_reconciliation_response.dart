//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/finance_reconciliation_row_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'finance_reconciliation_response.g.dart';

/// FinanceReconciliationResponse
///
/// Properties:
/// * [erpBilled] 
/// * [erpPaid] 
/// * [confirmedBilled] 
/// * [confirmedPaid] 
/// * [pendingCount] 
/// * [failedCount] 
/// * [deadCount] 
/// * [rows] 
@BuiltValue()
abstract class FinanceReconciliationResponse implements Built<FinanceReconciliationResponse, FinanceReconciliationResponseBuilder> {
  @BuiltValueField(wireName: r'erpBilled')
  double? get erpBilled;

  @BuiltValueField(wireName: r'erpPaid')
  double? get erpPaid;

  @BuiltValueField(wireName: r'confirmedBilled')
  double? get confirmedBilled;

  @BuiltValueField(wireName: r'confirmedPaid')
  double? get confirmedPaid;

  @BuiltValueField(wireName: r'pendingCount')
  int? get pendingCount;

  @BuiltValueField(wireName: r'failedCount')
  int? get failedCount;

  @BuiltValueField(wireName: r'deadCount')
  int? get deadCount;

  @BuiltValueField(wireName: r'rows')
  BuiltList<FinanceReconciliationRowResponse>? get rows;

  FinanceReconciliationResponse._();

  factory FinanceReconciliationResponse([void updates(FinanceReconciliationResponseBuilder b)]) = _$FinanceReconciliationResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(FinanceReconciliationResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<FinanceReconciliationResponse> get serializer => _$FinanceReconciliationResponseSerializer();
}

class _$FinanceReconciliationResponseSerializer implements PrimitiveSerializer<FinanceReconciliationResponse> {
  @override
  final Iterable<Type> types = const [FinanceReconciliationResponse, _$FinanceReconciliationResponse];

  @override
  final String wireName = r'FinanceReconciliationResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    FinanceReconciliationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.erpBilled != null) {
      yield r'erpBilled';
      yield serializers.serialize(
        object.erpBilled,
        specifiedType: const FullType(double),
      );
    }
    if (object.erpPaid != null) {
      yield r'erpPaid';
      yield serializers.serialize(
        object.erpPaid,
        specifiedType: const FullType(double),
      );
    }
    if (object.confirmedBilled != null) {
      yield r'confirmedBilled';
      yield serializers.serialize(
        object.confirmedBilled,
        specifiedType: const FullType(double),
      );
    }
    if (object.confirmedPaid != null) {
      yield r'confirmedPaid';
      yield serializers.serialize(
        object.confirmedPaid,
        specifiedType: const FullType(double),
      );
    }
    if (object.pendingCount != null) {
      yield r'pendingCount';
      yield serializers.serialize(
        object.pendingCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.failedCount != null) {
      yield r'failedCount';
      yield serializers.serialize(
        object.failedCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.deadCount != null) {
      yield r'deadCount';
      yield serializers.serialize(
        object.deadCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.rows != null) {
      yield r'rows';
      yield serializers.serialize(
        object.rows,
        specifiedType: const FullType.nullable(BuiltList, [FullType(FinanceReconciliationRowResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    FinanceReconciliationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required FinanceReconciliationResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'erpBilled':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.erpBilled = valueDes;
          break;
        case r'erpPaid':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.erpPaid = valueDes;
          break;
        case r'confirmedBilled':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.confirmedBilled = valueDes;
          break;
        case r'confirmedPaid':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.confirmedPaid = valueDes;
          break;
        case r'pendingCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.pendingCount = valueDes;
          break;
        case r'failedCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.failedCount = valueDes;
          break;
        case r'deadCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.deadCount = valueDes;
          break;
        case r'rows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(FinanceReconciliationRowResponse)]),
          ) as BuiltList<FinanceReconciliationRowResponse>?;
          if (valueDes == null) continue;
          result.rows.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  FinanceReconciliationResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = FinanceReconciliationResponseBuilder();
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


