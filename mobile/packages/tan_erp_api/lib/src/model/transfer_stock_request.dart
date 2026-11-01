//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/stock_line_request.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'transfer_stock_request.g.dart';

/// TransferStockRequest
///
/// Properties:
/// * [fromWarehouseId] 
/// * [toWarehouseId] 
/// * [reason] 
/// * [lines] 
/// * [fromLocationId] 
/// * [toLocationId] 
/// * [occurredAtUtc] 
@BuiltValue()
abstract class TransferStockRequest implements Built<TransferStockRequest, TransferStockRequestBuilder> {
  @BuiltValueField(wireName: r'fromWarehouseId')
  String? get fromWarehouseId;

  @BuiltValueField(wireName: r'toWarehouseId')
  String? get toWarehouseId;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'lines')
  BuiltList<StockLineRequest>? get lines;

  @BuiltValueField(wireName: r'fromLocationId')
  String? get fromLocationId;

  @BuiltValueField(wireName: r'toLocationId')
  String? get toLocationId;

  @BuiltValueField(wireName: r'occurredAtUtc')
  DateTime? get occurredAtUtc;

  TransferStockRequest._();

  factory TransferStockRequest([void updates(TransferStockRequestBuilder b)]) = _$TransferStockRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(TransferStockRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<TransferStockRequest> get serializer => _$TransferStockRequestSerializer();
}

class _$TransferStockRequestSerializer implements PrimitiveSerializer<TransferStockRequest> {
  @override
  final Iterable<Type> types = const [TransferStockRequest, _$TransferStockRequest];

  @override
  final String wireName = r'TransferStockRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    TransferStockRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.fromWarehouseId != null) {
      yield r'fromWarehouseId';
      yield serializers.serialize(
        object.fromWarehouseId,
        specifiedType: const FullType(String),
      );
    }
    if (object.toWarehouseId != null) {
      yield r'toWarehouseId';
      yield serializers.serialize(
        object.toWarehouseId,
        specifiedType: const FullType(String),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(StockLineRequest)]),
      );
    }
    if (object.fromLocationId != null) {
      yield r'fromLocationId';
      yield serializers.serialize(
        object.fromLocationId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.toLocationId != null) {
      yield r'toLocationId';
      yield serializers.serialize(
        object.toLocationId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.occurredAtUtc != null) {
      yield r'occurredAtUtc';
      yield serializers.serialize(
        object.occurredAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    TransferStockRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required TransferStockRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'fromWarehouseId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fromWarehouseId = valueDes;
          break;
        case r'toWarehouseId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.toWarehouseId = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(StockLineRequest)]),
          ) as BuiltList<StockLineRequest>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        case r'fromLocationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fromLocationId = valueDes;
          break;
        case r'toLocationId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.toLocationId = valueDes;
          break;
        case r'occurredAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.occurredAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  TransferStockRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = TransferStockRequestBuilder();
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


