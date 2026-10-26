//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/production_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_transaction_response.g.dart';

/// WorkOrderTransactionResponse
///
/// Properties:
/// * [id] 
/// * [kind] 
/// * [stockDocumentId] 
/// * [stockDocumentNumber] 
/// * [quantity] 
/// * [value] 
/// * [actor] 
/// * [createdAtUtc] 
@BuiltValue()
abstract class WorkOrderTransactionResponse implements Built<WorkOrderTransactionResponse, WorkOrderTransactionResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'stockDocumentId')
  String? get stockDocumentId;

  @BuiltValueField(wireName: r'stockDocumentNumber')
  String? get stockDocumentNumber;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'value')
  double? get value;

  @BuiltValueField(wireName: r'actor')
  ProductionPersonResponse? get actor;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  WorkOrderTransactionResponse._();

  factory WorkOrderTransactionResponse([void updates(WorkOrderTransactionResponseBuilder b)]) = _$WorkOrderTransactionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderTransactionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderTransactionResponse> get serializer => _$WorkOrderTransactionResponseSerializer();
}

class _$WorkOrderTransactionResponseSerializer implements PrimitiveSerializer<WorkOrderTransactionResponse> {
  @override
  final Iterable<Type> types = const [WorkOrderTransactionResponse, _$WorkOrderTransactionResponse];

  @override
  final String wireName = r'WorkOrderTransactionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderTransactionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.stockDocumentId != null) {
      yield r'stockDocumentId';
      yield serializers.serialize(
        object.stockDocumentId,
        specifiedType: const FullType(String),
      );
    }
    if (object.stockDocumentNumber != null) {
      yield r'stockDocumentNumber';
      yield serializers.serialize(
        object.stockDocumentNumber,
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
    if (object.value != null) {
      yield r'value';
      yield serializers.serialize(
        object.value,
        specifiedType: const FullType(double),
      );
    }
    if (object.actor != null) {
      yield r'actor';
      yield serializers.serialize(
        object.actor,
        specifiedType: const FullType(ProductionPersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkOrderTransactionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderTransactionResponseBuilder result,
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
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'stockDocumentId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.stockDocumentId = valueDes;
          break;
        case r'stockDocumentNumber':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.stockDocumentNumber = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        case r'value':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.value = valueDes;
          break;
        case r'actor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProductionPersonResponse),
          ) as ProductionPersonResponse?;
          if (valueDes == null) continue;
          result.actor.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkOrderTransactionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderTransactionResponseBuilder();
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


