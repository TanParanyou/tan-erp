//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'finance_reconciliation_row_response.g.dart';

/// FinanceReconciliationRowResponse
///
/// Properties:
/// * [issue] 
/// * [kind] 
/// * [resourceNumber] 
/// * [messageId] 
/// * [erpAmount] 
/// * [externalAmount] 
@BuiltValue()
abstract class FinanceReconciliationRowResponse implements Built<FinanceReconciliationRowResponse, FinanceReconciliationRowResponseBuilder> {
  @BuiltValueField(wireName: r'issue')
  String? get issue;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'resourceNumber')
  String? get resourceNumber;

  @BuiltValueField(wireName: r'messageId')
  String? get messageId;

  @BuiltValueField(wireName: r'erpAmount')
  double? get erpAmount;

  @BuiltValueField(wireName: r'externalAmount')
  double? get externalAmount;

  FinanceReconciliationRowResponse._();

  factory FinanceReconciliationRowResponse([void updates(FinanceReconciliationRowResponseBuilder b)]) = _$FinanceReconciliationRowResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(FinanceReconciliationRowResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<FinanceReconciliationRowResponse> get serializer => _$FinanceReconciliationRowResponseSerializer();
}

class _$FinanceReconciliationRowResponseSerializer implements PrimitiveSerializer<FinanceReconciliationRowResponse> {
  @override
  final Iterable<Type> types = const [FinanceReconciliationRowResponse, _$FinanceReconciliationRowResponse];

  @override
  final String wireName = r'FinanceReconciliationRowResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    FinanceReconciliationRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.issue != null) {
      yield r'issue';
      yield serializers.serialize(
        object.issue,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.resourceNumber != null) {
      yield r'resourceNumber';
      yield serializers.serialize(
        object.resourceNumber,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.messageId != null) {
      yield r'messageId';
      yield serializers.serialize(
        object.messageId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.erpAmount != null) {
      yield r'erpAmount';
      yield serializers.serialize(
        object.erpAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.externalAmount != null) {
      yield r'externalAmount';
      yield serializers.serialize(
        object.externalAmount,
        specifiedType: const FullType.nullable(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    FinanceReconciliationRowResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required FinanceReconciliationRowResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'issue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.issue = valueDes;
          break;
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'resourceNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.resourceNumber = valueDes;
          break;
        case r'messageId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.messageId = valueDes;
          break;
        case r'erpAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.erpAmount = valueDes;
          break;
        case r'externalAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.externalAmount = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  FinanceReconciliationRowResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = FinanceReconciliationRowResponseBuilder();
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


