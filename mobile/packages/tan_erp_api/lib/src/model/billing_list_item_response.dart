//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'billing_list_item_response.g.dart';

/// BillingListItemResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [projectCode] 
/// * [kind] 
/// * [amount] 
/// * [paidAmount] 
/// * [status] 
/// * [dueDate] 
/// * [issuedAtUtc] 
@BuiltValue()
abstract class BillingListItemResponse implements Built<BillingListItemResponse, BillingListItemResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'projectCode')
  String? get projectCode;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'amount')
  double? get amount;

  @BuiltValueField(wireName: r'paidAmount')
  double? get paidAmount;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'dueDate')
  Date? get dueDate;

  @BuiltValueField(wireName: r'issuedAtUtc')
  DateTime? get issuedAtUtc;

  BillingListItemResponse._();

  factory BillingListItemResponse([void updates(BillingListItemResponseBuilder b)]) = _$BillingListItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BillingListItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BillingListItemResponse> get serializer => _$BillingListItemResponseSerializer();
}

class _$BillingListItemResponseSerializer implements PrimitiveSerializer<BillingListItemResponse> {
  @override
  final Iterable<Type> types = const [BillingListItemResponse, _$BillingListItemResponse];

  @override
  final String wireName = r'BillingListItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BillingListItemResponse object, {
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
    if (object.projectCode != null) {
      yield r'projectCode';
      yield serializers.serialize(
        object.projectCode,
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
    if (object.amount != null) {
      yield r'amount';
      yield serializers.serialize(
        object.amount,
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
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
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
    if (object.issuedAtUtc != null) {
      yield r'issuedAtUtc';
      yield serializers.serialize(
        object.issuedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BillingListItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BillingListItemResponseBuilder result,
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
        case r'projectCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectCode = valueDes;
          break;
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'amount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.amount = valueDes;
          break;
        case r'paidAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.paidAmount = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'dueDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.dueDate = valueDes;
          break;
        case r'issuedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.issuedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BillingListItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BillingListItemResponseBuilder();
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


