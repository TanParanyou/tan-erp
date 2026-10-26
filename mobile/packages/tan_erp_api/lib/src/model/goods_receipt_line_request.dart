//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'goods_receipt_line_request.g.dart';

/// GoodsReceiptLineRequest
///
/// Properties:
/// * [purchaseOrderLineId] 
/// * [quantity] 
@BuiltValue()
abstract class GoodsReceiptLineRequest implements Built<GoodsReceiptLineRequest, GoodsReceiptLineRequestBuilder> {
  @BuiltValueField(wireName: r'purchaseOrderLineId')
  String? get purchaseOrderLineId;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  GoodsReceiptLineRequest._();

  factory GoodsReceiptLineRequest([void updates(GoodsReceiptLineRequestBuilder b)]) = _$GoodsReceiptLineRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(GoodsReceiptLineRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<GoodsReceiptLineRequest> get serializer => _$GoodsReceiptLineRequestSerializer();
}

class _$GoodsReceiptLineRequestSerializer implements PrimitiveSerializer<GoodsReceiptLineRequest> {
  @override
  final Iterable<Type> types = const [GoodsReceiptLineRequest, _$GoodsReceiptLineRequest];

  @override
  final String wireName = r'GoodsReceiptLineRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    GoodsReceiptLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.purchaseOrderLineId != null) {
      yield r'purchaseOrderLineId';
      yield serializers.serialize(
        object.purchaseOrderLineId,
        specifiedType: const FullType(String),
      );
    }
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    GoodsReceiptLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required GoodsReceiptLineRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'purchaseOrderLineId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.purchaseOrderLineId = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  GoodsReceiptLineRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = GoodsReceiptLineRequestBuilder();
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


